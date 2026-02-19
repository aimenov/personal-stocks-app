namespace StockScreener.Domain.ValueObjects;

/// <summary>
/// Liquidity profile computed from market data.
/// </summary>
public class LiquidityProfile
{
    public decimal MarketCapUSD { get; set; }
    public decimal AverageDailyVolumeUSD { get; set; }

    public bool MetsMegaCapRequirement => MarketCapUSD >= 2_000_000_000m;
    public bool MeetsADVRequirement => AverageDailyVolumeUSD >= 50_000_000m;

    public bool IsLiquidEnough =>
        MeetsADVRequirement &&
        (MetsMegaCapRequirement || AverageDailyVolumeUSD >= 10_000_000m);

    public decimal GetLiquidityScore()
    {
        if (MarketCapUSD >= 2_000_000_000m && AverageDailyVolumeUSD >= 50_000_000m)
            return 100m;

        if (MarketCapUSD >= 500_000_000m && AverageDailyVolumeUSD >= 10_000_000m)
            return 80m;

        if (AverageDailyVolumeUSD >= 10_000_000m)
            return 60m;

        if (AverageDailyVolumeUSD >= 5_000_000m)
            return 40m;

        return 0m;
    }
}
