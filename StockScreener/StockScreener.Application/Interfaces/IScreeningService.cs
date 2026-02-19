using StockScreener.Application.DTOs;

namespace StockScreener.Application.Interfaces;

public interface IScreeningService
{
    Task<List<ScreenResultDto>> RunScreenerAsync(CancellationToken ct = default);
    Task<List<ScreenResultDto>> GetTopResultsAsync(int count, CancellationToken ct = default);
    Task<List<ScreenResultDto>> GetMissedOpportunitiesAsync(int daysLookback, int topN, CancellationToken ct = default);
}
