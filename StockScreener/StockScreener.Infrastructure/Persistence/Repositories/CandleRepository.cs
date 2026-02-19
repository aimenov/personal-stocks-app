using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockScreener.Domain.Entities;

namespace StockScreener.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for managing DailyCandle entities with upsert logic
/// </summary>
public class CandleRepository
{
    private readonly StockScreenerDbContext _context;
    private readonly ILogger<CandleRepository> _logger;

    public CandleRepository(
        StockScreenerDbContext context,
        ILogger<CandleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all candles for a stock within a date range
    /// </summary>
    public async Task<List<DailyCandle>> GetCandlesAsync(
        Guid stockId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken ct = default)
    {
        var query = _context.DailyCandlesDb
            .Where(c => c.StockId == stockId);

        if (fromDate.HasValue)
            query = query.Where(c => c.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(c => c.Date <= toDate.Value);

        return await query
            .OrderBy(c => c.Date)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Upsert candles: insert missing ones, ignore existing ones
    /// Returns the count of newly inserted candles
    /// </summary>
    public async Task<int> UpsertCandlesAsync(
        Guid stockId,
        List<DailyCandle> candles,
        CancellationToken ct = default)
    {
        if (candles.Count == 0)
            return 0;

        // Set the StockId for all candles
        foreach (var candle in candles)
            candle.StockId = stockId;

        // Get existing dates for this stock
        var existingDates = await _context.DailyCandlesDb
            .Where(c => c.StockId == stockId)
            .Select(c => c.Date)
            .ToHashSetAsync(ct);

        // Filter to only new candles
        var newCandles = candles
            .Where(c => !existingDates.Contains(c.Date))
            .ToList();

        if (newCandles.Count == 0)
        {
            _logger.LogInformation(
                "No new candles to insert for stock {StockId}. {SkippedCount} candles already exist.",
                stockId,
                candles.Count);
            return 0;
        }

        // Insert new candles
        await _context.DailyCandlesDb.AddRangeAsync(newCandles, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Inserted {NewCount} candles for stock {StockId}. {SkippedCount} candles already existed.",
            newCandles.Count,
            stockId,
            existingDates.Count);

        return newCandles.Count;
    }

    /// <summary>
    /// Get date range of existing candles for a stock
    /// </summary>
    public async Task<(DateOnly? FromDate, DateOnly? ToDate)> GetCandleDateRangeAsync(
        Guid stockId,
        CancellationToken ct = default)
    {
        var candles = await _context.DailyCandlesDb
            .Where(c => c.StockId == stockId)
            .OrderBy(c => c.Date)
            .Select(c => c.Date)
            .ToListAsync(ct);

        if (candles.Count == 0)
            return (null, null);

        return (candles.First(), candles.Last());
    }

    /// <summary>
    /// Delete all candles for a stock (for maintenance/testing)
    /// </summary>
    public async Task<int> DeleteCandlesAsync(
        Guid stockId,
        CancellationToken ct = default)
    {
        var deleted = await _context.DailyCandlesDb
            .Where(c => c.StockId == stockId)
            .ExecuteDeleteAsync(ct);

        _logger.LogWarning("Deleted {Count} candles for stock {StockId}", deleted, stockId);
        return deleted;
    }
}
