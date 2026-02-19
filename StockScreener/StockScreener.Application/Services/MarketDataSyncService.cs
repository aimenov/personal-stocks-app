using System.Diagnostics;
using Microsoft.Extensions.Logging;
using StockScreener.Application.Interfaces;
using StockScreener.Domain.Entities;

namespace StockScreener.Application.Services;

/// <summary>
/// Service for synchronizing market data from external providers
/// Handles concurrency limits and rate limit backoff
/// </summary>
public class MarketDataSyncService : ISyncService
{
    private readonly IMarketDataProvider _marketDataProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<MarketDataSyncService> _logger;

    public MarketDataSyncService(
        IMarketDataProvider marketDataProvider,
        IUnitOfWork unitOfWork,
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<MarketDataSyncService> logger)
    {
        _marketDataProvider = marketDataProvider;
        _unitOfWork = unitOfWork;
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    /// <summary>
    /// Sync daily candles for a single ticker
    /// </summary>
    public async Task<SyncResult> SyncTickerCandlesAsync(
        string ticker,
        int daysLookback = 365,
        CancellationToken ct = default)
    {
        // Use the main UnitOfWork for single ticker sync (not concurrent)
        return await SyncTickerCandlesAsyncInternal(ticker, daysLookback, _unitOfWork, ct);
    }

    /// <summary>
    /// Internal method that performs the actual sync with a provided UnitOfWork
    /// Used by both single and batch sync methods
    /// </summary>
    private async Task<SyncResult> SyncTickerCandlesAsyncInternal(
        string ticker,
        int daysLookback,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            _logger.LogInformation("Starting candle sync for {Ticker} ({DaysLookback} days)", ticker, daysLookback);

            // Get stock
            var stocks = await unitOfWork.Stocks.GetAllAsync(ct);
            var stock = stocks.FirstOrDefault(s => s.Ticker == ticker);

            if (stock == null)
            {
                return new SyncResult(
                    ticker,
                    false,
                    0,
                    $"Stock '{ticker}' not found in database",
                    stopwatch.Elapsed);
            }

            // Download candles
            var candles = await _marketDataProvider.GetDailyCandlesAsync(ticker, daysLookback, ct);

            if (candles.Count == 0)
            {
                _logger.LogWarning("No candles downloaded for {Ticker}", ticker);
                return new SyncResult(
                    ticker,
                    false,
                    0,
                    "No candles downloaded from provider",
                    stopwatch.Elapsed);
            }

            // Get existing dates  
            var existingCandles = await unitOfWork.DailyCandlesRepository.GetAllAsync(ct);
            var existingDates = existingCandles
                .Where(c => c.StockId == stock.Id)
                .Select(c => c.Date)
                .ToHashSet();

            // Filter to only new candles
            var newCandles = candles.Where(c => !existingDates.Contains(c.Date)).ToList();

            if (newCandles.Count > 0)
            {
                // Set stock ID and add to database
                foreach (var candle in newCandles)
                    candle.StockId = stock.Id;

                await unitOfWork.DailyCandlesRepository.AddRangeAsync(newCandles, ct);
                await unitOfWork.SaveChangesAsync(ct);
            }

            var inserted = newCandles.Count;
            stopwatch.Stop();

            _logger.LogInformation(
                "Completed candle sync for {Ticker}: {InsertedCount} inserted, {Duration}ms",
                ticker,
                inserted,
                stopwatch.ElapsedMilliseconds);

            return new SyncResult(
                ticker,
                true,
                inserted,
                inserted > 0 ? $"Successfully inserted {inserted} candles" : "No new candles to insert",
                stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error syncing candles for {Ticker}", ticker);
            return new SyncResult(
                ticker,
                false,
                0,
                ex.Message,
                stopwatch.Elapsed);
        }
    }

    /// <summary>
    /// Sync daily candles for all tickers in the universe
    /// Uses semaphore to limit concurrency and factory for separate DbContexts
    /// </summary>
    public async Task<SyncBatchResult> SyncAllTickersCandlesAsync(
        int daysLookback = 365,
        int maxConcurrency = 8,
        CancellationToken ct = default)
    {
        var batchStopwatch = Stopwatch.StartNew();
        var semaphore = new SemaphoreSlim(maxConcurrency);
        var results = new List<SyncResult>();

        try
        {
            // Get all stocks from main context
            var stocks = await _unitOfWork.Stocks.GetAllAsync(ct);

            _logger.LogInformation(
                "Starting batch sync for {Count} tickers (max {MaxConcurrency} concurrent)",
                stocks.Count,
                maxConcurrency);

            // Create sync tasks with concurrency limit and separate DbContexts
            var tasks = stocks.Select(async stock =>
            {
                await semaphore.WaitAsync(ct);
                try
                {
                    // Create a new UnitOfWork with fresh DbContext for this thread
                    var scopedUnitOfWork = _unitOfWorkFactory.CreateUnitOfWork();
                    var result = await SyncTickerCandlesAsyncInternal(
                        stock.Ticker,
                        daysLookback,
                        scopedUnitOfWork,
                        ct);
                    lock (results)
                    {
                        results.Add(result);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
            batchStopwatch.Stop();

            var successCount = results.Count(r => r.Success);
            var failedCount = results.Count(r => !r.Success);
            var totalInserted = results.Sum(r => r.CandlesInserted);

            _logger.LogInformation(
                "Batch sync completed: {Success}/{Total} tickers succeeded, {Inserted} candles inserted, {Duration}ms",
                successCount,
                stocks.Count,
                totalInserted,
                batchStopwatch.ElapsedMilliseconds);

            return new SyncBatchResult(
                stocks.Count,
                successCount,
                failedCount,
                totalInserted,
                batchStopwatch.Elapsed,
                results);
        }
        catch (Exception ex)
        {
            batchStopwatch.Stop();
            _logger.LogError(ex, "Error in batch sync operation");
            throw;
        }
        finally
        {
            semaphore.Dispose();
        }
    }
}

