using StockScreener.Infrastructure;
using StockScreener.Infrastructure.Persistence;
using StockScreener.Infrastructure.ExternalApis;
using StockScreener.Application.Interfaces;
using StockScreener.Application.Services;
using StockScreener.API.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add infrastructure layer with EF Core and data providers
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=stockscreener.db";
builder.Services.AddInfrastructure(connectionString);

// Add market data sync services
builder.Services.AddScoped<RetryPolicy>();

// Choose between Finnhub and MarketStack providers
// Set MARKET_DATA_PROVIDER=marketstack or MARKET_DATA_PROVIDER=finnhub in environment
var dataProvider = builder.Configuration["MARKET_DATA_PROVIDER"] ?? "marketstack";

if (dataProvider.Equals("finnhub", StringComparison.OrdinalIgnoreCase))
{
    var finnhubKey = builder.Configuration["FINNHUB_API_KEY"] ?? "d5pjgtpr01qlfcaed6c0d5pjgtpr01qlfcaed6cg";
    builder.Services.AddHttpClient<IMarketDataProvider, FinnhubMarketDataProvider>(httpClient =>
    {
        httpClient.Timeout = TimeSpan.FromSeconds(30);
    });
    Console.WriteLine("✓ Using Finnhub as market data provider");
}
else
{
    var marketStackKey = builder.Configuration["MARKETSTACK_API_KEY"] ?? "afa1e619d13603016c2c574463e94e17";
    builder.Services.AddScoped<IMarketDataProvider>(sp =>
    {
        var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<MarketStackMarketDataProvider>>();
        return new MarketStackMarketDataProvider(httpClient, logger, marketStackKey);
    });
    Console.WriteLine("✓ Using MarketStack as market data provider");
}

builder.Services.AddScoped<ISyncService, MarketDataSyncService>();

// Add CORS to allow Next.js frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs", corsBuilder =>
    {
        corsBuilder
            .WithOrigins(
                "http://localhost:3000", 
                "http://localhost:3001", 
                "http://127.0.0.1:3000", 
                "http://127.0.0.1:3001",
                "https://studious-potato-qp667rrqx54399rp-3000.app.github.dev"
            )
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<StockScreenerDbContext>();
    dbContext.Database.Migrate();
}

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowNextJs");

app.MapGet("/", () => "Stock Screener API - Use /api/screener/* endpoints or visit /swagger for documentation")
    .WithName("Root");

app.MapGet("/health", () => Results.Ok(new { status = "OK", timestamp = DateTime.UtcNow }))
    .WithName("Health Check");

// Map screener endpoints
app.MapScreenerEndpoints();

// Map sync endpoints
app.MapSyncEndpoints();

app.Run();
