namespace StockScreener.Domain.ValueObjects;

/// <summary>
/// Computed metrics for a stock during screening.
/// </summary>
public class StockMetrics
{
    public int FluctuationCount { get; set; }           // Up/down reversals in 60d
    public decimal PriceRange { get; set; }             // High - Low over 60d
    public decimal RangePercent { get; set; }           // (High - Low) / Mean * 100
    public decimal RecentDrawdown { get; set; }         // % drop from recent peak
    public decimal DistanceFromHigh { get; set; }       // % below 60d high (0-100)
    public decimal DistanceFromLow { get; set; }        // % above 60d low (0-100)
    public decimal DistanceFromMean { get; set; }       // Std devs from mean
    public decimal Current60DayMean { get; set; }
    public decimal Current60DayStdDev { get; set; }
}
