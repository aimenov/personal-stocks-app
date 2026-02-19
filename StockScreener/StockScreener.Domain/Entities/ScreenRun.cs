namespace StockScreener.Domain.Entities;

/// <summary>
/// Represents a single screener run (scan of all stocks).
/// Aggregate Root for screen results.
/// </summary>
public class ScreenRun
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime RunDate { get; set; }
    public int TotalStocksScanned { get; set; }
    public int StocksWithValidScore { get; set; }  // After liquidity filter
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<ScreenResult> Results { get; set; } = new List<ScreenResult>();
}
