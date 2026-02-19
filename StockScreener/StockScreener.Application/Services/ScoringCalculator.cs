namespace StockScreener.Application.Services;

/// <summary>
/// Core scoring calculator implementing the 4-component ranking formula.
/// 
/// Score = (Volatility × 0.25) + (Drawdown × 0.25) + (Extreme × 0.25) + (Liquidity × 0.25)
/// 
/// Each component is 0-100. Final score is 0-100.
/// Liquidity Score = 0 means stock is excluded from results.
/// </summary>
public class ScoringCalculator
{
    /// <summary>
    /// Calculate the 4-component scoring breakdown for a stock.
    /// </summary>
    public ScoreBreakdown CalculateScore(
        int fluctuationCount,
        decimal priceRangePercent,
        decimal recentDrawdownPercent,
        decimal distanceFromHighPercent,
        decimal distanceFromLowPercent,
        decimal marketCapUSD,
        decimal avgDailyVolumeUSD)
    {
        var volatilityScore = CalculateVolatilityScore(fluctuationCount);
        var drawdownScore = CalculateDrawdownScore(recentDrawdownPercent);
        var extremeScore = CalculateExtremeScore(distanceFromHighPercent, distanceFromLowPercent);
        var liquidityScore = CalculateLiquidityScore(marketCapUSD, avgDailyVolumeUSD);

        // Total score: weighted average (each component = 25%)
        var totalScore = liquidityScore > 0
            ? (volatilityScore * 0.25m) + (drawdownScore * 0.25m) + 
              (extremeScore * 0.25m) + (liquidityScore * 0.25m)
            : 0m;

        return new ScoreBreakdown
        {
            VolatilityScore = volatilityScore,
            DrawdownScore = drawdownScore,
            ExtremeScore = extremeScore,
            LiquidityScore = liquidityScore,
            TotalScore = Math.Min(100m, Math.Max(0m, totalScore)),
            IsQualified = liquidityScore >= 40m  // Must pass minimum liquidity
        };
    }

    /// <summary>
    /// VOLATILITY: Measures frequency of up/down reversals.
    /// Expected max ~30-40 reversals in 60 days.
    /// Score = (reversals / 30) * 100, capped at 100.
    /// </summary>
    private decimal CalculateVolatilityScore(int fluctuationCount)
    {
        const int maxExpectedFluctuations = 30;
        var score = (fluctuationCount / (decimal)maxExpectedFluctuations) * 100m;
        return Math.Min(100m, Math.Max(0m, score));
    }

    /// <summary>
    /// DRAWDOWN: Measures recent decline from peak.
    /// Expected max ~30% drawdown in 60 days.
    /// Score = (drawdown% / 30%) * 100, capped at 100.
    /// </summary>
    private decimal CalculateDrawdownScore(decimal recentDrawdownPercent)
    {
        const decimal maxExpectedDrawdown = 30m;  // 30%
        var score = (recentDrawdownPercent / maxExpectedDrawdown) * 100m;
        return Math.Min(100m, Math.Max(0m, score));
    }

    /// <summary>
    /// EXTREME: Measures if stock is near 60-day highs or lows.
    /// Higher score = stock is at extreme (high volatility opportunity).
    /// Takes the MAX of (distance from high, distance from low).
    /// If at 60d high → 100% (far from low), if at 60d low → 100% (far from high).
    /// If at 60d mean → 0% (not extreme).
    /// </summary>
    private decimal CalculateExtremeScore(decimal distanceFromHighPercent, decimal distanceFromLowPercent)
    {
        // Convert percentages (0-100 range) to raw score
        // If stock is 80% below the high (20% of range from high), that's extreme = 80 score
        // If stock is 80% above the low (80% of range from low), that's extreme = 80 score
        var score = Math.Max(distanceFromHighPercent, distanceFromLowPercent);
        return Math.Min(100m, Math.Max(0m, score));
    }

    /// <summary>
    /// LIQUIDITY: Ensures stock is tradeable for $100k orders.
    /// Tiered scoring based on market cap and ADV.
    /// </summary>
    private decimal CalculateLiquidityScore(decimal marketCapUSD, decimal avgDailyVolumeUSD)
    {
        // Tier 1: Mega-cap + High ADV
        if (marketCapUSD >= 2_000_000_000m && avgDailyVolumeUSD >= 50_000_000m)
            return 100m;

        // Tier 2: Large-cap + Good ADV
        if (marketCapUSD >= 500_000_000m && avgDailyVolumeUSD >= 10_000_000m)
            return 80m;

        // Tier 3: Good ADV (cap-agnostic)
        if (avgDailyVolumeUSD >= 10_000_000m)
            return 60m;

        // Tier 4: Minimum ADV for filtering
        if (avgDailyVolumeUSD >= 5_000_000m)
            return 40m;

        // Below minimum
        return 0m;
    }
}

/// <summary>
/// Result of scoring calculations.
/// </summary>
public class ScoreBreakdown
{
    public decimal VolatilityScore { get; set; }
    public decimal DrawdownScore { get; set; }
    public decimal ExtremeScore { get; set; }
    public decimal LiquidityScore { get; set; }
    public decimal TotalScore { get; set; }
    public bool IsQualified { get; set; }  // LiquidityScore >= 40
}
