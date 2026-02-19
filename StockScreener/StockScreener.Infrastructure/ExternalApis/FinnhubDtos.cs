using System.Text.Json.Serialization;

namespace StockScreener.Infrastructure.ExternalApis;

/// <summary>
/// Finnhub Stock Candles API response DTO
/// </summary>
public class FinnhubCandleResponse
{
    [JsonPropertyName("s")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("c")]
    public List<decimal> Close { get; set; } = new();

    [JsonPropertyName("o")]
    public List<decimal> Open { get; set; } = new();

    [JsonPropertyName("h")]
    public List<decimal> High { get; set; } = new();

    [JsonPropertyName("l")]
    public List<decimal> Low { get; set; } = new();

    [JsonPropertyName("v")]
    public List<long> Volume { get; set; } = new();

    [JsonPropertyName("t")]
    public List<long> Timestamp { get; set; } = new();
}

/// <summary>
/// Represents a single candle from Finnhub response
/// </summary>
public record CandleData(
    DateOnly Date,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    long Volume
);
