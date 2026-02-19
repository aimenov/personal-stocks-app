namespace StockScreener.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
    Task<int> CountAsync(CancellationToken ct = default);
}

public interface IUnitOfWork : IDisposable
{
    IRepository<Domain.Entities.Stock> Stocks { get; }
    IRepository<Domain.Entities.ScreenRun> ScreenRuns { get; }
    IRepository<Domain.Entities.ScreenResult> ScreenResults { get; }
    IRepository<Domain.Entities.NewsArticle> News { get; }
    IRepository<Domain.Entities.DailyCandle> DailyCandlesRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
