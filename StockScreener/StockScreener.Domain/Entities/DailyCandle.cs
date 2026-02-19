namespace StockScreener.Domain.Entities;

/// <summary>
/// Represents a single day of OHLCV data for a stock.
/// Owned by Stock entity.
/// </summary>
public class DailyCandle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StockId { get; set; }
    public DateOnly Date { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public long Volume { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Stock Stock { get; set; } = null!;
}
