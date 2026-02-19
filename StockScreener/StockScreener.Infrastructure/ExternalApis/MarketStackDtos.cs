using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StockScreener.Infrastructure.ExternalApis;

/// <summary>
/// MarketStack end-of-day (EOD) historical data response
/// Example: https://api.marketstack.com/v1/eod?symbols=MSFT&date_from=2024-01-01&date_to=2024-12-31
/// </summary>
public class MarketStackEodResponse
{
    [JsonPropertyName("data")]
    public List<MarketStackEodCandle> Data { get; set; } = new();

    [JsonPropertyName("pagination")]
    public MarketStackPagination Pagination { get; set; } = new();

    [JsonPropertyName("error")]
    public MarketStackError? Error { get; set; }
}

public class MarketStackEodCandle
{
    /// <summary>Exchange symbol (e.g., "MSFT")</summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>Date in format YYYY-MM-DDTHH:MM:SS+0000</summary>
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    /// <summary>Opening price</summary>
    [JsonPropertyName("open")]
    public decimal? Open { get; set; }

    /// <summary>Highest price</summary>
    [JsonPropertyName("high")]
    public decimal? High { get; set; }

    /// <summary>Lowest price</summary>
    [JsonPropertyName("low")]
    public decimal? Low { get; set; }

    /// <summary>Closing price</summary>
    [JsonPropertyName("close")]
    public decimal? Close { get; set; }

    /// <summary>Trading volume (as decimal, will be cast to long)</summary>
    [JsonPropertyName("volume")]
    public decimal? Volume { get; set; }

    /// <summary>Adjusted close (for stock splits, dividends)</summary>
    [JsonPropertyName("adj_close")]
    public decimal? AdjustedClose { get; set; }

    /// <summary>Adjusted volume</summary>
    [JsonPropertyName("adj_volume")]
    public decimal? AdjustedVolume { get; set; }
}

public class MarketStackPagination
{
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    [JsonPropertyName("count")]
    public int? Count { get; set; }

    [JsonPropertyName("total")]
    public int? Total { get; set; }
}

public class MarketStackError
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
