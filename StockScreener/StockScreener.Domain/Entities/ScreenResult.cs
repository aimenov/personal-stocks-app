namespace StockScreener.Domain.Entities;

/// <summary>
/// Represents the results of screening a single stock in a particular ScreenRun.
/// Owned by ScreenRun entity.
/// </summary>
public class ScreenResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ScreenRunId { get; set; }
    public Guid StockId { get; set; }
    public int Rank { get; set; }
    public decimal TotalScore { get; set; }          // 0-100
    public decimal VolatilityScore { get; set; }
    public decimal DrawdownScore { get; set; }
    public decimal ExtremeScore { get; set; }
    public decimal LiquidityScore { get; set; }
    public string MetricsExplanation { get; set; } = null!;   // JSON breakdown
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Stock Stock { get; set; } = null!;
    public ScreenRun ScreenRun { get; set; } = null!;
}
