namespace StockScreener.Domain.Entities;

/// <summary>
/// Represents a stock in the screener universe.
/// Aggregate Root for the stock domain.
/// </summary>
public class Stock
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Ticker { get; set; } = null!;              // "AAPL"
    public string CompanyName { get; set; } = null!;
    public string Exchange { get; set; } = null!;            // "NASDAQ", "NYSE"
    public decimal? MarketCapUSD { get; set; }               // Nullable: may not have data
    public decimal AverageDailyVolumeUSD { get; set; }       // In millions
    public DateTime LastUpdated { get; set; }

    // Navigation properties
    public ICollection<DailyCandle> DailyCandlesLast60 { get; set; } = new List<DailyCandle>();
    public ICollection<ScreenResult> ScreenResults { get; set; } = new List<ScreenResult>();
}
