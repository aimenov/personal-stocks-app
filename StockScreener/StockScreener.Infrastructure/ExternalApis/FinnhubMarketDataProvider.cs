using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using StockScreener.Domain.Entities;
using StockScreener.Application.Interfaces;

namespace StockScreener.Infrastructure.ExternalApis;

/// <summary>
/// Finnhub.io market data provider for OHLCV candle data
/// Free tier: 60 API calls per minute
/// </summary>
public class FinnhubMarketDataProvider : IMarketDataProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<FinnhubMarketDataProvider> _logger;
    private readonly RetryPolicy _retryPolicy;

    private const string BaseUrl = "https://finnhub.io/api/v1";

    public FinnhubMarketDataProvider(
        HttpClient httpClient,
        ILogger<FinnhubMarketDataProvider> logger,
        RetryPolicy retryPolicy)
    {
        _httpClient = httpClient;
        _logger = logger;
        _retryPolicy = retryPolicy;
        _apiKey = "d5pjgtpr01qlfcaed6c0d5pjgtpr01qlfcaed6cg";
    }

    /// <summary>
    /// Download daily OHLCV candles for a ticker from the past N days
    /// </summary>
    public async Task<List<DailyCandle>> GetDailyCandlesAsync(
        string ticker,
        int days,
        CancellationToken ct = default)
    {
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var fromDate = toDate.AddDays(-days);

        return await GetDailyCandlesAsync(ticker, fromDate, toDate, ct);
    }

    /// <summary>
    /// Download daily OHLCV candles for a ticker within a date range
    /// </summary>
    public async Task<List<DailyCandle>> GetDailyCandlesAsync(
        string ticker,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken ct = default)
    {
        var candles = new List<DailyCandle>();

        try
        {
            var fromUnix = ((DateTimeOffset)fromDate.ToDateTime(TimeOnly.MinValue)).ToUnixTimeSeconds();
            var toUnix = ((DateTimeOffset)toDate.ToDateTime(TimeOnly.MinValue)).ToUnixTimeSeconds();

            var url = $"{BaseUrl}/stock/candle?symbol={ticker}&resolution=D&from={fromUnix}&to={toUnix}&token={_apiKey}";

            var data = await _retryPolicy.ExecuteAsync(
                async (cancellationToken) =>
                {
                    using var response = await _httpClient.GetAsync(url, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadFromJsonAsync<FinnhubCandleResponse>(
                        JsonSerializerOptions.Default,
                        cancellationToken);
                },
                $"GetDailyCandles({ticker}, {fromDate:yyyy-MM-dd}, {toDate:yyyy-MM-dd})",
                ct);

            if (data?.Status == "ok" && data.Close.Count > 0)
            {
                for (int i = 0; i < data.Close.Count; i++)
                {
                    var dateTime = UnixTimeStampToDateTime(data.Timestamp[i]);
                    candles.Add(new DailyCandle
                    {
                        Date = DateOnly.FromDateTime(dateTime),
                        Open = data.Open[i],
                        High = data.High[i],
                        Low = data.Low[i],
                        Close = data.Close[i],
                        Volume = data.Volume[i]
                    });
                }

                _logger.LogInformation(
                    "Downloaded {Count} candles for {Ticker} from {FromDate:yyyy-MM-dd} to {ToDate:yyyy-MM-dd}",
                    candles.Count,
                    ticker,
                    fromDate,
                    toDate);
            }
            else
            {
                _logger.LogWarning(
                    "No data returned from Finnhub for {Ticker}. Status: {Status}",
                    ticker,
                    data?.Status ?? "null");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error downloading candles for {Ticker}: {Message}",
                ticker,
                ex.Message);
            throw;
        }

        return candles;
    }

    /// <summary>
    /// Get fundamental data (market cap, avg daily volume)
    /// Not implemented for now - stub returns placeholder values
    /// </summary>
    public async Task<(decimal marketCap, decimal avgDailyVolUSD)> GetFundamentalsAsync(
        string ticker,
        CancellationToken ct = default)
    {
        // TODO: Implement using Finnhub profile endpoint
        // For now, return placeholder
        await Task.Delay(10, ct);
        return (marketCap: 0, avgDailyVolUSD: 0);
    }

    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime;
    }
}
