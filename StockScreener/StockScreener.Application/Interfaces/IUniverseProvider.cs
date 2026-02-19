using StockScreener.Domain.Entities;

namespace StockScreener.Application.Interfaces;

public interface IUniverseProvider
{
    Task<List<Stock>> GetSP500TickersAsync(CancellationToken ct = default);
    Task<List<Stock>> GetNasdaqTickersAsync(CancellationToken ct = default);
    Task<List<Stock>> GetITSTickersAsync(CancellationToken ct = default);
}
