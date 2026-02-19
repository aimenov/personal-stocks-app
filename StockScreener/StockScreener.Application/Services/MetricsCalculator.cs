using StockScreener.Domain.Entities;
using StockScreener.Domain.ValueObjects;

namespace StockScreener.Application.Services;

/// <summary>
/// Computes StockMetrics from daily candle data.
/// Analyzes volatility patterns, drawdowns, and price extremes.
/// </summary>
public class MetricsCalculator
{
    /// <summary>
    /// Compute all metrics for a stock from its daily candles (60 days).
    /// </summary>
    public StockMetrics ComputeMetrics(List<DailyCandle> candles)
    {
        if (!candles.Any())
            return CreateEmptyMetrics();

        // Sort by date ascending
        var sortedCandles = candles.OrderBy(c => c.Date).ToList();

        var closes = sortedCandles.Select(c => c.Close).ToList();
        var highs = sortedCandles.Select(c => c.High).ToList();
        var lows = sortedCandles.Select(c => c.Low).ToList();

        // Calculate basic statistics
        var mean = closes.Average();
        var stdDev = CalculateStdDev(closes, mean);
        var high60d = highs.Max();
        var low60d = lows.Min();
        var currentPrice = closes.Last();

        // Calculate metrics
        var fluctuationCount = CountFluctuations(closes);
        var priceRange = high60d - low60d;
        var rangePercent = mean != 0 ? (priceRange / mean) * 100m : 0m;
        var recentDrawdown = CalculateRecentDrawdown(sortedCandles);
        var distanceFromHigh = CalculateDistanceFromHigh(currentPrice, high60d);
        var distanceFromLow = CalculateDistanceFromLow(currentPrice, low60d);
        var distanceFromMean = stdDev != 0 ? Math.Abs(currentPrice - mean) / stdDev : 0m;

        return new StockMetrics
        {
            FluctuationCount = fluctuationCount,
            PriceRange = priceRange,
            RangePercent = rangePercent,
            RecentDrawdown = recentDrawdown,
            DistanceFromHigh = distanceFromHigh,
            DistanceFromLow = distanceFromLow,
            DistanceFromMean = distanceFromMean,
            Current60DayMean = mean,
            Current60DayStdDev = stdDev
        };
    }

    /// <summary>
    /// Count the number of up/down reversals (direction changes).
    /// A reversal is when price closes higher then next day closes lower, or vice versa.
    /// </summary>
    private int CountFluctuations(List<decimal> closes)
    {
        if (closes.Count < 2)
            return 0;

        int count = 0;
        decimal lastDirection = 0;  // 1 = up, -1 = down

        for (int i = 1; i < closes.Count; i++)
        {
            var currentDirection = closes[i] > closes[i - 1] ? 1 : 
                                   closes[i] < closes[i - 1] ? -1 : 0;

            if (currentDirection != 0 && lastDirection != 0 && currentDirection != lastDirection)
            {
                count++;
            }

            if (currentDirection != 0)
                lastDirection = currentDirection;
        }

        return count;
    }

    /// <summary>
    /// Calculate the maximum drawdown from the recent peak within 60 days.
    /// Looks back from the last candle and finds the deepest drop.
    /// </summary>
    private decimal CalculateRecentDrawdown(List<DailyCandle> sortedCandles)
    {
        if (sortedCandles.Count < 2)
            return 0m;

        decimal maxDrawdown = 0m;
        decimal peak = sortedCandles[0].High;

        foreach (var candle in sortedCandles)
        {
            if (candle.High > peak)
                peak = candle.High;

            var drawdown = peak != 0 ? ((peak - candle.Low) / peak) * 100m : 0m;
            if (drawdown > maxDrawdown)
                maxDrawdown = drawdown;
        }

        return maxDrawdown;
    }

    /// <summary>
    /// Calculate how far the current price is from the 60-day high.
    /// Returns 0-100: 0 = at the high, 100 = at the low.
    /// </summary>
    private decimal CalculateDistanceFromHigh(decimal currentPrice, decimal high60d)
    {
        if (high60d == 0)
            return 0m;

        var distance = ((high60d - currentPrice) / high60d) * 100m;
        return Math.Min(100m, Math.Max(0m, distance));
    }

    /// <summary>
    /// Calculate how far the current price is from the 60-day low.
    /// Returns 0-100: 0 = at the low, 100 = at the high.
    /// </summary>
    private decimal CalculateDistanceFromLow(decimal currentPrice, decimal low60d)
    {
        if (low60d == 0)
            return 0m;

        var distance = ((currentPrice - low60d) / low60d) * 100m;
        return Math.Min(100m, Math.Max(0m, distance));
    }

    private decimal CalculateStdDev(List<decimal> values, decimal mean)
    {
        if (values.Count < 2)
            return 0m;

        var variance = values.Sum(v => (v - mean) * (v - mean)) / values.Count;
        return (decimal)Math.Sqrt((double)variance);
    }

    private StockMetrics CreateEmptyMetrics()
    {
        return new StockMetrics
        {
            FluctuationCount = 0,
            PriceRange = 0m,
            RangePercent = 0m,
            RecentDrawdown = 0m,
            DistanceFromHigh = 0m,
            DistanceFromLow = 0m,
            DistanceFromMean = 0m,
            Current60DayMean = 0m,
            Current60DayStdDev = 0m
        };
    }
}
