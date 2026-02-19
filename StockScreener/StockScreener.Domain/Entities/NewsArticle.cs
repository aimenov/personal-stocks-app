namespace StockScreener.Domain.Entities;

/// <summary>
/// Represents a news article (market-wide or company-specific).
/// </summary>
public class NewsArticle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Ticker { get; set; }              // Null = market-wide news
    public string Title { get; set; } = null!;
    public string Source { get; set; } = null!;      // "Reuters", "Bloomberg", etc.
    public string Url { get; set; } = null!;
    public DateTime PublishedAt { get; set; }
    public string Category { get; set; } = null!;   // "Market" or "Company"
    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
}
