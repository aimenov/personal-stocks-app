using StockScreener.Application.Interfaces;
using StockScreener.Domain.Entities;

namespace StockScreener.Infrastructure.ExternalApis;

/// <summary>
/// Stub implementation for demonstration. Replace with real API calls.
/// </summary>
public class StubUniverseProvider : IUniverseProvider
{
    public Task<List<Stock>> GetSP500TickersAsync(CancellationToken ct = default)
    {
        // 7 largest-cap NASDAQ stocks
        var stocks = new List<Stock>
        {
            new() { Ticker = "AAPL", CompanyName = "Apple Inc.", Exchange = "NASDAQ", MarketCapUSD = 3_200_000_000_000, AverageDailyVolumeUSD = 85_000_000, LastUpdated = DateTime.UtcNow },
            new() { Ticker = "MSFT", CompanyName = "Microsoft Corp.", Exchange = "NASDAQ", MarketCapUSD = 3_000_000_000_000, AverageDailyVolumeUSD = 75_000_000, LastUpdated = DateTime.UtcNow },
            new() { Ticker = "GOOGL", CompanyName = "Alphabet Inc.", Exchange = "NASDAQ", MarketCapUSD = 2_200_000_000_000, AverageDailyVolumeUSD = 65_000_000, LastUpdated = DateTime.UtcNow },
            new() { Ticker = "AMZN", CompanyName = "Amazon.com Inc.", Exchange = "NASDAQ", MarketCapUSD = 2_100_000_000_000, AverageDailyVolumeUSD = 90_000_000, LastUpdated = DateTime.UtcNow },
            new() { Ticker = "NVDA", CompanyName = "NVIDIA Corporation", Exchange = "NASDAQ", MarketCapUSD = 1_400_000_000_000, AverageDailyVolumeUSD = 100_000_000, LastUpdated = DateTime.UtcNow },
            new() { Ticker = "META", CompanyName = "Meta Platforms Inc.", Exchange = "NASDAQ", MarketCapUSD = 1_100_000_000_000, AverageDailyVolumeUSD = 80_000_000, LastUpdated = DateTime.UtcNow },
            new() { Ticker = "TSLA", CompanyName = "Tesla Inc.", Exchange = "NASDAQ", MarketCapUSD = 1_000_000_000_000, AverageDailyVolumeUSD = 120_000_000, LastUpdated = DateTime.UtcNow },
        };
        return Task.FromResult(stocks);
    }

    public Task<List<Stock>> GetNasdaqTickersAsync(CancellationToken ct = default)
    {
        // 3 large-cap NYSE stocks
        var stocks = new List<Stock>
        {
            new() { Ticker = "BRK.B", CompanyName = "Berkshire Hathaway Inc.", Exchange = "NYSE", MarketCapUSD = 800_000_000_000, AverageDailyVolumeUSD = 50_000_000, LastUpdated = DateTime.UtcNow },
            new() { Ticker = "JNJ", CompanyName = "Johnson & Johnson", Exchange = "NYSE", MarketCapUSD = 400_000_000_000, AverageDailyVolumeUSD = 40_000_000, LastUpdated = DateTime.UtcNow },
            new() { Ticker = "V", CompanyName = "Visa Inc.", Exchange = "NYSE", MarketCapUSD = 650_000_000_000, AverageDailyVolumeUSD = 60_000_000, LastUpdated = DateTime.UtcNow },
        };
        return Task.FromResult(stocks);
    }

    public Task<List<Stock>> GetITSTickersAsync(CancellationToken ct = default)
    {
        // Empty - all stocks returned from above
        var stocks = new List<Stock>();
        return Task.FromResult(stocks);
    }
}
