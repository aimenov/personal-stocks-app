namespace StockScreener.Application.Interfaces;

/// <summary>
/// Service for synchronizing market data with external providers
/// </summary>
public interface ISyncService
{
    /// <summary>
    /// Sync daily candles for a single ticker
    /// </summary>
    Task<SyncResult> SyncTickerCandlesAsync(
        string ticker,
        int daysLookback = 365,
        CancellationToken ct = default);

    /// <summary>
    /// Sync daily candles for all tickers in the universe
    /// </summary>
    Task<SyncBatchResult> SyncAllTickersCandlesAsync(
        int daysLookback = 365,
        int maxConcurrency = 8,
        CancellationToken ct = default);
}

/// <summary>
/// Result of syncing a single ticker
/// </summary>
public record SyncResult(
    string Ticker,
    bool Success,
    int CandlesInserted,
    string? ErrorMessage = null,
    TimeSpan? ElapsedTime = null
);

/// <summary>
/// Result of batch syncing all tickers
/// </summary>
public record SyncBatchResult(
    int TotalTickers,
    int SuccessCount,
    int FailedCount,
    int TotalCandlesInserted,
    TimeSpan ElapsedTime,
    List<SyncResult> Details
);
