using StockScreener.Application.Interfaces;
using StockScreener.Domain.Entities;

namespace StockScreener.API.Endpoints;

/// <summary>
/// API endpoints for market data synchronization
/// </summary>
public static class SyncEndpoints
{
    public static void MapSyncEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/sync")
            .WithName("Market Data Sync");

        group.MapPost("/database/reset", ResetDatabase)
            .WithName("Reset Database")
            .WithDescription("Clear all data from database (stocks, candles, screen results)")
            .Produces<object>(StatusCodes.Status200OK);

        group.MapPost("/database/init-stocks", InitializeStocks)
            .WithName("Initialize 10 Stocks")
            .WithDescription("Populate database with 10 high-quality stocks from universe")
            .Produces<object>(StatusCodes.Status200OK);

        group.MapPost("/candles/1y", SyncAll1YearCandles)
            .WithName("Sync 1Y Candles - All Tickers")
            .WithDescription("Sync 1 year of daily OHLCV candles for all stocks in the universe")
            .Produces<SyncBatchResultDto>(StatusCodes.Status200OK);

        group.MapPost("/candles/{ticker}", SyncTickerCandles)
            .WithName("Sync Candles - Single Ticker")
            .WithDescription("Sync daily OHLCV candles for a specific ticker")
            .Produces<SyncResultDto>(StatusCodes.Status200OK);

        group.MapGet("/stock/{ticker}/candles", GetTickerCandles)
            .WithName("Get Candles from DB")
            .WithDescription("Retrieve stored daily candles for a ticker (no external call)")
            .Produces<List<CandleDto>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> ResetDatabase(
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        try
        {
            // Delete in correct order to respect foreign keys
            var screenResults = await unitOfWork.ScreenResults.GetAllAsync(ct);
            foreach (var result in screenResults)
                unitOfWork.ScreenResults.Remove(result);
            
            var screenRuns = await unitOfWork.ScreenRuns.GetAllAsync(ct);
            foreach (var run in screenRuns)
                unitOfWork.ScreenRuns.Remove(run);
            
            var candles = await unitOfWork.DailyCandlesRepository.GetAllAsync(ct);
            foreach (var candle in candles)
                unitOfWork.DailyCandlesRepository.Remove(candle);
            
            var newsArticles = await unitOfWork.News.GetAllAsync(ct);
            foreach (var article in newsArticles)
                unitOfWork.News.Remove(article);
            
            var stocks = await unitOfWork.Stocks.GetAllAsync(ct);
            foreach (var stock in stocks)
                unitOfWork.Stocks.Remove(stock);
            
            await unitOfWork.SaveChangesAsync(ct);
            
            return Results.Ok(new { message = "Database reset successfully", cleared = new { 
                stocks = stocks.Count,
                candles = candles.Count,
                screenRuns = screenRuns.Count,
                screenResults = screenResults.Count,
                newsArticles = newsArticles.Count
            }});
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> InitializeStocks(
        IUnitOfWork unitOfWork,
        IUniverseProvider universeProvider,
        CancellationToken ct)
    {
        try
        {
            // Get 10 stocks from universe provider
            var sp500 = await universeProvider.GetSP500TickersAsync(ct);
            var nasdaq = await universeProvider.GetNasdaqTickersAsync(ct);
            var its = await universeProvider.GetITSTickersAsync(ct);

            var allStocks = new List<Stock>();
            allStocks.AddRange(sp500);
            allStocks.AddRange(nasdaq);
            allStocks.AddRange(its);

            // Deduplicate by ticker
            var uniqueStocks = allStocks
                .GroupBy(s => s.Ticker)
                .Select(g => g.First())
                .ToList();

            // Add to database
            foreach (var stock in uniqueStocks)
            {
                await unitOfWork.Stocks.AddAsync(stock, ct);
            }
            await unitOfWork.SaveChangesAsync(ct);

            return Results.Ok(new { message = $"Initialized {uniqueStocks.Count} stocks", stocks = uniqueStocks.Select(s => new { s.Ticker, s.CompanyName }) });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> SyncAll1YearCandles(
        ISyncService syncService,
        CancellationToken ct)
    {
        try
        {
            var result = await syncService.SyncAllTickersCandlesAsync(daysLookback: 365, ct: ct);
            return Results.Ok(new SyncBatchResultDto(
                result.TotalTickers,
                result.SuccessCount,
                result.FailedCount,
                result.TotalCandlesInserted,
                (long)result.ElapsedTime.TotalMilliseconds,
                result.Details.Select(r => new SyncResultDto(
                    r.Ticker,
                    r.Success,
                    r.CandlesInserted,
                    r.ErrorMessage,
                    (long?)r.ElapsedTime?.TotalMilliseconds
                )).ToList()
            ));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> SyncTickerCandles(
        string ticker,
        int days = 365,
        ISyncService syncService = null!,
        CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ticker))
                return Results.BadRequest(new { error = "Ticker is required" });

            if (days <= 0)
                return Results.BadRequest(new { error = "Days must be greater than 0" });

            var result = await syncService.SyncTickerCandlesAsync(ticker, days, ct);
            return Results.Ok(new SyncResultDto(
                result.Ticker,
                result.Success,
                result.CandlesInserted,
                result.ErrorMessage,
                (long?)result.ElapsedTime?.TotalMilliseconds
            ));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetTickerCandles(
        string ticker,
        int days = 365,
        IUnitOfWork unitOfWork = null!,
        CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ticker))
                return Results.BadRequest(new { error = "Ticker is required" });

            if (days <= 0)
                return Results.BadRequest(new { error = "Days must be greater than 0" });

            // Find stock
            var stocks = await unitOfWork.Stocks.GetAllAsync(ct);
            var stock = stocks.FirstOrDefault(s => s.Ticker.ToUpper() == ticker.ToUpper());

            if (stock == null)
                return Results.NotFound(new { error = $"Stock '{ticker}' not found" });

            // Get candles from last N days
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-days));
            var candles = stock.DailyCandlesLast60
                .Where(c => c.Date >= fromDate)
                .OrderBy(c => c.Date)
                .ToList();

            return Results.Ok(candles.Select(c => new CandleDto(
                c.Date.ToString("yyyy-MM-dd"),
                (double)c.Open,
                (double)c.High,
                (double)c.Low,
                (double)c.Close,
                c.Volume
            )).ToList());
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}

// DTOs
public record SyncResultDto(
    string Ticker,
    bool Success,
    int CandlesInserted,
    string? ErrorMessage,
    long? ElapsedMs
);

public record SyncBatchResultDto(
    int TotalTickers,
    int SuccessCount,
    int FailedCount,
    int TotalCandlesInserted,
    long ElapsedMs,
    List<SyncResultDto> Details
);

public record CandleDto(
    string Date,
    double Open,
    double High,
    double Low,
    double Close,
    long Volume
);
