namespace StockScreener.Domain.Constants;

/// <summary>
/// Screening constants and thresholds.
/// </summary>
public static class ScreeningConstants
{
    /// <summary>
    /// Number of trading days to analyze per stock.
    /// </summary>
    public const int DAYS_LOOKBACK = 60;

    /// <summary>
    /// Liquidity thresholds for stock filtering.
    /// </summary>
    public const decimal MIN_MARKET_CAP_MEGA = 2_000_000_000m;      // $2B
    public const decimal MIN_MARKET_CAP_LARGE = 500_000_000m;       // $500M
    public const decimal MIN_ADV_PRIMARY = 50_000_000m;             // $50M/day
    public const decimal MIN_ADV_SECONDARY = 10_000_000m;           // $10M/day
    public const decimal MIN_ADV_FILTER = 5_000_000m;               // $5M/day

    /// <summary>
    /// Score component weights (must sum to 1.0).
    /// </summary>
    public const decimal VOLATILITY_WEIGHT = 0.25m;
    public const decimal DRAWDOWN_WEIGHT = 0.25m;
    public const decimal EXTREME_WEIGHT = 0.25m;
    public const decimal LIQUIDITY_WEIGHT = 0.25m;

    /// <summary>
    /// Scoring component caps/ranges.
    /// </summary>
    public const int MAX_EXPECTED_FLUCTUATIONS = 30;   // reversals in 60 days
    public const decimal MAX_EXPECTED_DRAWDOWN = 30m;   // percent
}
