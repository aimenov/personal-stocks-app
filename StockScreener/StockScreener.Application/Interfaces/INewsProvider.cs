using StockScreener.Domain.Entities;

namespace StockScreener.Application.Interfaces;

public interface INewsProvider
{
    Task<List<NewsArticle>> GetMarketNewsAsync(int count, CancellationToken ct = default);
    Task<List<NewsArticle>> GetCompanyNewsAsync(string ticker, int count, CancellationToken ct = default);
}
