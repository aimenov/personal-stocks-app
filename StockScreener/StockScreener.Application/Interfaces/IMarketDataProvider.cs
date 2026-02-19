using StockScreener.Domain.Entities;

namespace StockScreener.Application.Interfaces;

public interface IMarketDataProvider
{
    Task<List<DailyCandle>> GetDailyCandlesAsync(string ticker, int days, CancellationToken ct = default);
    Task<(decimal marketCap, decimal avgDailyVolUSD)> GetFundamentalsAsync(string ticker, CancellationToken ct = default);
}
