using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockScreener.Application.Interfaces;
using StockScreener.Application.Services;
using StockScreener.Infrastructure.ExternalApis;
using StockScreener.Infrastructure.Persistence;
using StockScreener.Infrastructure.Persistence.Repositories;

namespace StockScreener.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<StockScreenerDbContext>(options =>
            options.UseSqlite(connectionString)
        );

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();        
        // Application services
        services.AddScoped<IScreeningService, ScreeningEngine>();
        
        // External data providers
        services.AddScoped<IUniverseProvider, StubUniverseProvider>();
        // FinnhubMarketDataProvider is registered in Program.cs with HttpClient
        services.AddScoped<INewsProvider, StubNewsProvider>();

        return services;
    }
}