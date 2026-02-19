using StockScreener.Application.Interfaces;
using StockScreener.Domain.Entities;

namespace StockScreener.Infrastructure.ExternalApis;

/// <summary>
/// Stub news provider. Replace with real news API integration.
/// </summary>
public class StubNewsProvider : INewsProvider
{
    public Task<List<NewsArticle>> GetMarketNewsAsync(int count, CancellationToken ct = default)
    {
        var news = new List<NewsArticle>
        {
            new()
            {
                Title = "Federal Reserve holds interest rates steady",
                Source = "Reuters",
                Url = "https://reuters.com/news/fed-rates",
                PublishedAt = DateTime.UtcNow.AddHours(-2),
                Category = "Market"
            },
            new()
            {
                Title = "Tech stocks lead market rally",
                Source = "Bloomberg",
                Url = "https://bloomberg.com/news/tech-rally",
                PublishedAt = DateTime.UtcNow.AddHours(-4),
                Category = "Market"
            }
        };

        return Task.FromResult(news.Take(count).ToList());
    }

    public Task<List<NewsArticle>> GetCompanyNewsAsync(string ticker, int count, CancellationToken ct = default)
    {
        var news = new List<NewsArticle>
        {
            new()
            {
                Ticker = ticker,
                Title = $"{ticker} announces record quarterly earnings",
                Source = "CNBC",
                Url = $"https://cnbc.com/news/{ticker}",
                PublishedAt = DateTime.UtcNow.AddDays(-1),
                Category = "Company"
            },
            new()
            {
                Ticker = ticker,
                Title = $"{ticker} stock upgrades amid strong guidance",
                Source = "Goldman Sachs",
                Url = $"https://gs.com/research/{ticker}",
                PublishedAt = DateTime.UtcNow.AddDays(-2),
                Category = "Company"
            }
        };

        return Task.FromResult(news.Take(count).ToList());
    }
}
