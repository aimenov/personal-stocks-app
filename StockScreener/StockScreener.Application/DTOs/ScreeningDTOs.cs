namespace StockScreener.Application.DTOs;

public class StockDto
{
    public string Ticker { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public decimal? MarketCapUSD { get; set; }
    public decimal AverageDailyVolumeUSD { get; set; }
}

public class ScreenComponentsDto
{
    public decimal Volatility { get; set; }
    public decimal Drawdown { get; set; }
    public decimal Extreme { get; set; }
    public decimal Liquidity { get; set; }
}

public class MetricsBreakdownDto
{
    public int FluctuationCount { get; set; }
    public decimal RangePercent { get; set; }
    public decimal RecentDrawdownPercent { get; set; }
    public decimal DistanceFromHighPercent { get; set; }
    public decimal DistanceFromLowPercent { get; set; }
}

public class ScreenResultDto
{
    public string Ticker { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public int Rank { get; set; }
    public decimal TotalScore { get; set; }
    public ScreenComponentsDto Scores { get; set; } = new();
    public MetricsBreakdownDto Metrics { get; set; } = new();
    public string Explanation { get; set; } = null!;
}

public class NewsDto
{
    public string? Ticker { get; set; }
    public string Title { get; set; } = null!;
    public string Source { get; set; } = null!;
    public string Url { get; set; } = null!;
    public DateTime PublishedAt { get; set; }
    public string Category { get; set; } = null!;
}
