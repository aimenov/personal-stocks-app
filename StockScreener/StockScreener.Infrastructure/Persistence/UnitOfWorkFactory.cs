using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockScreener.Application.Interfaces;
using StockScreener.Infrastructure.Persistence.Repositories;

namespace StockScreener.Infrastructure.Persistence;

/// <summary>
/// Factory for creating new UnitOfWork instances with fresh DbContext scopes
/// Used for concurrent operations to avoid DbContext threading issues
/// </summary>
public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWorkFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IUnitOfWork CreateUnitOfWork()
    {
        // Create a new scope with a fresh DbContext
        var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StockScreenerDbContext>();
        
        // Return new UnitOfWork with the scoped context
        return new UnitOfWork(dbContext);
    }
}
