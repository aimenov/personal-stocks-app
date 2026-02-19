using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StockScreener.Application.Interfaces;
using StockScreener.Domain.Entities;

namespace StockScreener.Infrastructure.ExternalApis;

/// <summary>
/// MarketStack provider for fetching daily OHLCV (Open, High, Low, Close, Volume) candle data
/// API: https://marketstack.com/documentation
/// 
/// MarketStack Advantages:
/// - No rate limit on free tier (unlimited requests)
/// - Historical data back to 1998
/// - Includes adjusted close for splits/dividends
/// - Simple REST API
/// 
/// MarketStack Limitations:
/// - Free tier requires HTTP (not HTTPS)
/// - 100 API requests per month on free tier (but this is for EOD data, not strict rate limit)
/// - Pagination limited to 100 results per request
/// </summary>
public class MarketStackMarketDataProvider : IMarketDataProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MarketStackMarketDataProvider> _logger;
    private readonly string _apiKey;
    private const string BaseUrl = "http://api.marketstack.com/v1";
    private const string EodEndpoint = "/eod";

    public MarketStackMarketDataProvider(HttpClient httpClient, ILogger<MarketStackMarketDataProvider> logger, string apiKey)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

        // Set reasonable timeout for MarketStack API
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// Fetches daily OHLCV candles from MarketStack API for the specified ticker
    /// </summary>
    /// <param name="ticker">Stock ticker symbol (e.g., "MSFT")</param>
    /// <param name="days">Number of days of historical data to fetch (default 365)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of daily candles</returns>
    public async Task<List<DailyCandle>> GetDailyCandlesAsync(string ticker, int days = 365, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker cannot be null or empty", nameof(ticker));

        if (days <= 0)
            throw new ArgumentException("Days must be greater than 0", nameof(days));

        try
        {
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var fromDate = toDate.AddDays(-days);

            _logger.LogInformation("Fetching {Days} days of candles for {Ticker} from MarketStack (from {FromDate} to {ToDate})", 
                days, ticker, fromDate, toDate);

            var url = $"{BaseUrl}{EodEndpoint}?symbols={ticker}&date_from={fromDate:yyyy-MM-dd}&date_to={toDate:yyyy-MM-dd}&limit=100&access_key={_apiKey}";

            var response = await _httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<MarketStackEodResponse>(json);

            if (result?.Error != null)
            {
                _logger.LogError("MarketStack API returned error: {Code} - {Message}", result.Error.Code, result.Error.Message);
                return new List<DailyCandle>();
            }

            if (result?.Data == null || !result.Data.Any())
            {
                _logger.LogWarning("No candle data returned from MarketStack for {Ticker}", ticker);
                return new List<DailyCandle>();
            }

            var candles = result.Data
                .Where(c => c.Close.HasValue && c.Open.HasValue && c.High.HasValue && c.Low.HasValue)
                .Select(c => new DailyCandle
                {
                    Date = DateOnly.ParseExact(c.Date.Substring(0, 10), "yyyy-MM-dd"),
                    Open = c.Open!.Value,
                    High = c.High!.Value,
                    Low = c.Low!.Value,
                    Close = c.Close!.Value,
                    Volume = (long)(c.Volume ?? 0)
                })
                .OrderBy(c => c.Date)
                .ToList();

            _logger.LogInformation("Successfully fetched {Count} candles for {Ticker} from MarketStack", candles.Count, ticker);
            return candles;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching candles for {Ticker} from MarketStack", ticker);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error for {Ticker} from MarketStack", ticker);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching candles for {Ticker} from MarketStack", ticker);
            throw;
        }
    }

    /// <summary>
    /// Gets fundamentals data (market cap and average daily volume in USD)
    /// MarketStack free tier does not provide fundamentals, so this returns empty values
    /// </summary>
    public async Task<(decimal marketCap, decimal avgDailyVolUSD)> GetFundamentalsAsync(string ticker, CancellationToken ct = default)
    {
        // MarketStack free tier does not provide fundamentals data
        // Return empty values - this would need to be sourced from another API like Alpha Vantage or IEX Cloud
        _logger.LogWarning("GetFundamentalsAsync called but MarketStack free tier does not provide fundamentals. Returning empty values.");
        await Task.CompletedTask;
        return (0, 0);
    }

    /// <summary>
    /// Fetches multiple tickers in parallel with concurrency control
    /// MarketStack free tier: 100 requests/month = ~3 requests/day
    /// We use small delays between requests to stay within limits
    /// </summary>
    public async Task<Dictionary<string, List<DailyCandle>>> GetDailyCandlesAsync(IEnumerable<string> tickers, int days = 365, int maxConcurrency = 2)
    {
        if (tickers == null || !tickers.Any())
            throw new ArgumentException("Tickers collection cannot be null or empty", nameof(tickers));

        var semaphore = new System.Threading.SemaphoreSlim(maxConcurrency, maxConcurrency);
        var tasks = new List<Task<(string Ticker, List<DailyCandle> Candles)>>();

        foreach (var ticker in tickers)
        {
            tasks.Add(FetchWithSemaphoreAsync(ticker, days, semaphore));
        }

        var results = await Task.WhenAll(tasks);

        return results.ToDictionary(r => r.Ticker, r => r.Candles);
    }

    private async Task<(string Ticker, List<DailyCandle> Candles)> FetchWithSemaphoreAsync(
        string ticker, int days, System.Threading.SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();
        try
        {
            // Add delay between requests to respect rate limits (2 sec = ~30 req/min)
            await Task.Delay(2000);
            var candles = await GetDailyCandlesAsync(ticker, days);
            return (ticker, candles);
        }
        finally
        {
            semaphore.Release();
        }
    }
}
