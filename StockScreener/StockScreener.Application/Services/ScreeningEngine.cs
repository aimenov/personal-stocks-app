using System.Text.Json;
using StockScreener.Application.DTOs;
using StockScreener.Application.Interfaces;
using StockScreener.Domain.Constants;
using StockScreener.Domain.Entities;
using StockScreener.Domain.ValueObjects;

namespace StockScreener.Application.Services;

/// <summary>
/// Orchestrates the complete screening process:
/// 1. Fetch universe (S&P 500 + Nasdaq + ITS)
/// 2. Fetch 60 days of market data
/// 3. Compute metrics and scores
/// 4. Filter by liquidity and rank
/// 5. Save results to database
/// </summary>
public class ScreeningEngine : IScreeningService
{
    private readonly IUniverseProvider _universeProvider;
    private readonly IMarketDataProvider _marketDataProvider;
    private readonly INewsProvider _newsProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly MetricsCalculator _metricsCalculator;
    private readonly ScoringCalculator _scoringCalculator;

    public ScreeningEngine(
        IUniverseProvider universeProvider,
        IMarketDataProvider marketDataProvider,
        INewsProvider newsProvider,
        IUnitOfWork unitOfWork)
    {
        _universeProvider = universeProvider;
        _marketDataProvider = marketDataProvider;
        _newsProvider = newsProvider;
        _unitOfWork = unitOfWork;
        _metricsCalculator = new MetricsCalculator();
        _scoringCalculator = new ScoringCalculator();
    }

    public async Task<List<ScreenResultDto>> RunScreenerAsync(CancellationToken ct = default)
    {
        // Create screening run record
        var screenRun = new ScreenRun
        {
            RunDate = DateTime.UtcNow
        };

        var results = new List<ScreenResultDto>();
        var screenResults = new List<ScreenResult>();

        try
        {
            // Step 1: Fetch universe
            var stocks = await FetchUniverse(ct);
            screenRun.TotalStocksScanned = stocks.Count;

            // Get existing stocks to handle duplicates
            var existingStocks = await _unitOfWork.Stocks.GetAllAsync(ct);
            var existingTickers = existingStocks.ToDictionary(s => s.Ticker);

            // Upsert stocks: update existing, add new ones
            foreach (var stock in stocks)
            {
                if (existingTickers.TryGetValue(stock.Ticker, out var existing))
                {
                    // Update existing stock
                    existing.CompanyName = stock.CompanyName;
                    existing.Exchange = stock.Exchange;
                    existing.LastUpdated = DateTime.UtcNow;
                    _unitOfWork.Stocks.Update(existing);
                }
                else
                {
                    // Add new stock
                    await _unitOfWork.Stocks.AddAsync(stock, ct);
                }
            }
            await _unitOfWork.SaveChangesAsync(ct);

            // Refresh stocks list to get updated IDs
            stocks = await _unitOfWork.Stocks.GetAllAsync(ct);
            var stockById = stocks.ToDictionary(s => s.Ticker);

            // Step 2: Score each stock
            var rankedStocks = new List<(Stock stock, ScreenResultDto dto, decimal score)>();

            foreach (var stock in stocks)
            {
                try
                {
                    var resultDto = await ScoreStock(stock, ct);
                    if (resultDto != null)
                    {
                        rankedStocks.Add((stock, resultDto, resultDto.TotalScore));
                    }
                }
                catch (Exception ex)
                {
                    // Silently skip stocks that fail to score
                    _ = ex;
                }
            }

            // Step 3: Rank and prepare results
            rankedStocks = rankedStocks
                .OrderByDescending(r => r.score)
                .ToList();

            screenRun.StocksWithValidScore = rankedStocks.Count;

            // Step 4: Save screening run first
            await _unitOfWork.ScreenRuns.AddAsync(screenRun, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Step 5: Create and save screen results (now stocks and screenRun are in DB)
            int rank = 1;
            foreach (var (stock, resultDto, _) in rankedStocks)
            {
                resultDto.Rank = rank;

                var screenResult = new ScreenResult
                {
                    ScreenRunId = screenRun.Id,
                    StockId = stock.Id,
                    Rank = rank,
                    TotalScore = resultDto.TotalScore,
                    VolatilityScore = resultDto.Scores.Volatility,
                    DrawdownScore = resultDto.Scores.Drawdown,
                    ExtremeScore = resultDto.Scores.Extreme,
                    LiquidityScore = resultDto.Scores.Liquidity,
                    MetricsExplanation = SerializeMetrics(resultDto.Metrics)
                };

                screenResults.Add(screenResult);
                results.Add(resultDto);
                rank++;
            }

            // Save screen results
            await _unitOfWork.ScreenResults.AddRangeAsync(screenResults, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch
        {
            throw;
        }

        return results;
    }

    public async Task<List<ScreenResultDto>> GetTopResultsAsync(int count, CancellationToken ct = default)
    {
        // Get the most recent screening run
        var screenRuns = await _unitOfWork.ScreenRuns.GetAllAsync(ct);
        var latestRun = screenRuns.OrderByDescending(r => r.RunDate).FirstOrDefault();

        if (latestRun == null)
            return new List<ScreenResultDto>();

        // Get top results from that run
        var allResults = await _unitOfWork.ScreenResults.GetAllAsync(ct);
        var topResults = allResults
            .Where(r => r.ScreenRunId == latestRun.Id)
            .OrderBy(r => r.Rank)
            .Take(count)
            .ToList();

        var dtos = new List<ScreenResultDto>();
        foreach (var result in topResults)
        {
            var stock = await _unitOfWork.Stocks.GetByIdAsync(result.StockId, ct);
            if (stock != null)
            {
                dtos.Add(MapToDto(result, stock));
            }
        }

        return dtos;
    }

    public async Task<List<ScreenResultDto>> GetMissedOpportunitiesAsync(int daysLookback, int topN, CancellationToken ct = default)
    {
        // Get all screening runs in the lookback window
        var allRuns = await _unitOfWork.ScreenRuns.GetAllAsync(ct);
        var cutoffDate = DateTime.UtcNow.AddDays(-daysLookback);
        var historicalRuns = allRuns
            .Where(r => r.RunDate >= cutoffDate)
            .OrderByDescending(r => r.RunDate)
            .ToList();

        if (historicalRuns.Count < 2)
            return new List<ScreenResultDto>();

        // Get top N from the latest run
        var latestRun = historicalRuns.First();
        var allResults = await _unitOfWork.ScreenResults.GetAllAsync(ct);
        var latestTopN = allResults
            .Where(r => r.ScreenRunId == latestRun.Id)
            .OrderBy(r => r.Rank)
            .Take(topN)
            .Select(r => r.StockId)
            .ToHashSet();

        // Find stocks that were in top N of older runs but not in latest
        var missedOpportunities = new List<ScreenResult>();
        foreach (var olderRun in historicalRuns.Skip(1))
        {
            var olderTopN = allResults
                .Where(r => r.ScreenRunId == olderRun.Id)
                .OrderBy(r => r.Rank)
                .Take(topN)
                .ToList();

            var missed = olderTopN.Where(r => !latestTopN.Contains(r.StockId)).ToList();
            missedOpportunities.AddRange(missed);
        }

        // Deduplicate and convert to DTOs
        var uniqueMissed = missedOpportunities
            .GroupBy(m => m.StockId)
            .Select(g => g.First())
            .OrderByDescending(m => m.TotalScore)
            .Take(topN)
            .ToList();

        var dtos = new List<ScreenResultDto>();
        foreach (var result in uniqueMissed)
        {
            var stock = await _unitOfWork.Stocks.GetByIdAsync(result.StockId, ct);
            if (stock != null)
            {
                dtos.Add(MapToDto(result, stock));
            }
        }

        return dtos;
    }

    private async Task<List<Stock>> FetchUniverse(CancellationToken ct)
    {
        var sp500 = await _universeProvider.GetSP500TickersAsync(ct);
        var nasdaq = await _universeProvider.GetNasdaqTickersAsync(ct);
        var its = await _universeProvider.GetITSTickersAsync(ct);

        var all = new List<Stock>();
        all.AddRange(sp500);
        all.AddRange(nasdaq);
        all.AddRange(its);

        // Deduplicate by ticker
        return all
            .GroupBy(s => s.Ticker)
            .Select(g => g.First())
            .ToList();
    }

    private async Task<ScreenResultDto?> ScoreStock(Stock stock, CancellationToken ct)
    {
        // Fetch candles for this stock
        var candles = await _marketDataProvider.GetDailyCandlesAsync(
            stock.Ticker, ScreeningConstants.DAYS_LOOKBACK, ct);

        if (!candles.Any())
        {
            return null;
        }

        // Fetch fundamentals
        var (marketCap, avgDailyVolUSD) = await _marketDataProvider.GetFundamentalsAsync(stock.Ticker, ct);

        // Update stock with latest fundamentals
        stock.MarketCapUSD = marketCap > 0 ? marketCap : stock.MarketCapUSD;
        stock.AverageDailyVolumeUSD = avgDailyVolUSD > 0 ? avgDailyVolUSD : stock.AverageDailyVolumeUSD;
        stock.LastUpdated = DateTime.UtcNow;

        // Compute metrics
        var metrics = _metricsCalculator.ComputeMetrics(candles);

        // Calculate score
        var scoreBreakdown = _scoringCalculator.CalculateScore(
            metrics.FluctuationCount,
            metrics.RangePercent,
            metrics.RecentDrawdown,
            metrics.DistanceFromHigh,
            metrics.DistanceFromLow,
            stock.MarketCapUSD ?? 0m,
            stock.AverageDailyVolumeUSD
        );

        if (!scoreBreakdown.IsQualified)
            return null;  // Below liquidity threshold

        return new ScreenResultDto
        {
            Ticker = stock.Ticker,
            CompanyName = stock.CompanyName,
            TotalScore = scoreBreakdown.TotalScore,
            Scores = new ScreenComponentsDto
            {
                Volatility = scoreBreakdown.VolatilityScore,
                Drawdown = scoreBreakdown.DrawdownScore,
                Extreme = scoreBreakdown.ExtremeScore,
                Liquidity = scoreBreakdown.LiquidityScore
            },
            Metrics = new MetricsBreakdownDto
            {
                FluctuationCount = metrics.FluctuationCount,
                RangePercent = Math.Round(metrics.RangePercent, 2),
                RecentDrawdownPercent = Math.Round(metrics.RecentDrawdown, 2),
                DistanceFromHighPercent = Math.Round(metrics.DistanceFromHigh, 2),
                DistanceFromLowPercent = Math.Round(metrics.DistanceFromLow, 2)
            },
            Explanation = GenerateExplanation(stock, metrics, scoreBreakdown)
        };
    }

    private string GenerateExplanation(Stock stock, StockMetrics metrics, ScoreBreakdown scores)
    {
        return $"{stock.Ticker}: Score {scores.TotalScore:F1}/100. " +
               $"Volatility: {metrics.FluctuationCount} reversals ({scores.VolatilityScore:F0}/100). " +
               $"Drawdown: {metrics.RecentDrawdown:F1}% recent ({scores.DrawdownScore:F0}/100). " +
               $"Extreme: {Math.Max(metrics.DistanceFromHigh, metrics.DistanceFromLow):F1}% from edge ({scores.ExtremeScore:F0}/100). " +
               $"Liquidity: ADV${stock.AverageDailyVolumeUSD/1_000_000m:F1}M, Cap${stock.MarketCapUSD/1_000_000_000m:F1}B ({scores.LiquidityScore:F0}/100).";
    }

    private ScreenResultDto MapToDto(ScreenResult result, Stock stock)
    {
        var metrics = DeserializeMetrics(result.MetricsExplanation);

        return new ScreenResultDto
        {
            Ticker = stock.Ticker,
            CompanyName = stock.CompanyName,
            Rank = result.Rank,
            TotalScore = result.TotalScore,
            Scores = new ScreenComponentsDto
            {
                Volatility = result.VolatilityScore,
                Drawdown = result.DrawdownScore,
                Extreme = result.ExtremeScore,
                Liquidity = result.LiquidityScore
            },
            Metrics = metrics,
            Explanation = result.MetricsExplanation
        };
    }

    private string SerializeMetrics(MetricsBreakdownDto metrics)
    {
        return JsonSerializer.Serialize(metrics);
    }

    private MetricsBreakdownDto DeserializeMetrics(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<MetricsBreakdownDto>(json) ?? new MetricsBreakdownDto();
        }
        catch
        {
            return new MetricsBreakdownDto();
        }
    }
}
