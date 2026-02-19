using StockScreener.Application.Interfaces;
using StockScreener.Domain.Entities;

namespace StockScreener.Infrastructure.ExternalApis;

/// <summary>
/// Stub implementation for demonstration. Replace with real market data API.
/// Generates realistic synthetic OHLCV data for testing.
/// </summary>
public class StubMarketDataProvider : IMarketDataProvider
{
    public async Task<List<DailyCandle>> GetDailyCandlesAsync(string ticker, int days, CancellationToken ct = default)
    {
        var candles = new List<DailyCandle>();
        var basePrice = GetBasePriceByTicker(ticker);
        var random = new Random(ticker.GetHashCode() ^ DateTime.UtcNow.DayOfYear);

        // Generate more realistic price data with trends and reversals
        decimal trend = 0m;
        for (int i = days; i >= 1; i--)
        {
            var date = DateTime.UtcNow.AddDays(-i);

            // Add some mean reversion - trend doesn't go too far
            trend = trend * 0.9m + (decimal)(random.NextDouble() * 0.04 - 0.02);
            trend = Math.Max(-0.03m, Math.Min(0.03m, trend));

            var changePercent = trend + (decimal)(random.NextDouble() * 0.04 - 0.02);
            var newPrice = basePrice * (1m + changePercent);

            var open = basePrice;
            var close = newPrice;
            var high = Math.Max(open, close) * (1m + (decimal)(random.NextDouble() * 0.02));
            var low = Math.Min(open, close) * (1m - (decimal)(random.NextDouble() * 0.02));
            var volume = 50_000_000 + random.Next(-15_000_000, 15_000_000);

            candles.Add(new DailyCandle
            {
                Date = DateOnly.FromDateTime(date),
                Open = Math.Round(open, 2),
                High = Math.Round(high, 2),
                Low = Math.Round(low, 2),
                Close = Math.Round(close, 2),
                Volume = Math.Max(1_000_000, volume)
            });

            basePrice = close;
        }

        return await Task.FromResult(candles);
    }

    public async Task<(decimal marketCap, decimal avgDailyVolUSD)> GetFundamentalsAsync(string ticker, CancellationToken ct = default)
    {
        var (marketCap, avgVol) = ticker.ToUpper() switch
        {
            "AAPL" => (2_800_000_000_000m, 75_000_000m),
            "MSFT" => (2_500_000_000_000m, 65_000_000m),
            "GOOGL" => (2_100_000_000_000m, 55_000_000m),
            "TSLA" => (800_000_000_000m, 120_000_000m),
            "NVDA" => (1_200_000_000_000m, 90_000_000m),
            "ITA" => (500_000_000m, 35_000_000m),
            _ => (1_000_000_000m, 20_000_000m)
        };

        return await Task.FromResult((marketCap, avgVol));
    }

    private decimal GetBasePriceByTicker(string ticker)
    {
        return ticker.ToUpper() switch
        {
            "AAPL" => 235m,
            "MSFT" => 418m,
            "GOOGL" => 140m,
            "TSLA" => 250m,
            "NVDA" => 875m,
            "ITA" => 82m,
            _ => 100m
        };
    }
}
