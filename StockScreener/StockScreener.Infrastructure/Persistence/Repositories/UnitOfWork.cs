using StockScreener.Application.Interfaces;
using StockScreener.Domain.Entities;

namespace StockScreener.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly StockScreenerDbContext _context;

    public IRepository<Stock> Stocks { get; }
    public IRepository<ScreenRun> ScreenRuns { get; }
    public IRepository<ScreenResult> ScreenResults { get; }
    public IRepository<NewsArticle> News { get; }
    public IRepository<DailyCandle> DailyCandlesRepository { get; }

    public UnitOfWork(StockScreenerDbContext context)
    {
        _context = context;
        Stocks = new Repository<Stock>(context);
        ScreenRuns = new Repository<ScreenRun>(context);
        ScreenResults = new Repository<ScreenResult>(context);
        News = new Repository<NewsArticle>(context);
        DailyCandlesRepository = new Repository<DailyCandle>(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
