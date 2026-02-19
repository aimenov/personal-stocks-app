# Stock Screener Application

A high-performance stock screening application built with .NET 10 that identifies trading opportunities based on volatility patterns, drawdown analysis, and liquidity filters.

## 🎯 Goal

Build a personal stock trading screener that ranks stocks to find "golden opportunities" - those positioned at extremes with meaningful volatility patterns and sufficient liquidity for $100k order execution.

## ✨ Key Features

✅ **4-Component Scoring System**
- Volatility (price reversals in 60-day window)
- Drawdown (maximum peak-to-trough decline)
- Extreme (distance from 60-day price extremes)
- Liquidity (ensures $100k order execution capability)

✅ **Smart Filtering**
- Automatic liquidity gate (only stocks with ADV ≥ $5M qualify)
- Meaningful volatility detection
- Risk-aware drawdown analysis

✅ **Historical Analysis**
- Tracks screening runs over time
- Identifies "missed opportunities" (stocks previously ranked but no longer qualifying)
- Complete audit trail of decisions

✅ **Clean Architecture**
- Domain-driven design
- Fully testable (dependency injection throughout)
- Repository pattern for data access
- Entity Framework Core with SQLite

✅ **RESTful API**
- Minimal API endpoints for screening operations
- Swagger/OpenAPI documentation
- Error handling and validation

## 🏗️ Architecture

```
StockScreener/
├── StockScreener.Domain/              # Core business logic
│   ├── Entities/                      # Stock, ScreenRun, ScreenResult, DailyCandle
│   ├── ValueObjects/                  # StockMetrics, LiquidityProfile
│   └── Constants/                     # ScreeningConstants
│
├── StockScreener.Application/         # Use cases & business rules
│   ├── Interfaces/                    # IScreeningService, IRepository<T>, providers
│   ├── Services/                      # ScreeningEngine, ScoringCalculator, MetricsCalculator
│   └── DTOs/                          # Data transfer objects for API
│
├── StockScreener.Infrastructure/      # Data access & external services
│   ├── Persistence/                   # StockScreenerDbContext, repositories
│   ├── ExternalApis/                  # Stub data providers (Universe, MarketData, News)
│   └── DependencyInjection.cs         # Service registration
│
└── StockScreener.API/                 # Web layer
    ├── Program.cs                     # Host configuration
    └── Endpoints/ScreenerEndpoints.cs # API route handlers
```

## 🚀 Quick Start

### Prerequisites
- .NET 10 SDK
- Unix/Linux terminal or WSL

### Build & Run

```bash
# Build all projects
cd StockScreener
dotnet build

# Run the API server
cd StockScreener.API
dotnet run

# API listens on http://localhost:5128
```

### Test the API

```bash
# Run a complete screening
curl -X POST http://localhost:5128/api/screener/run

# Get top 10 stocks
curl http://localhost:5128/api/screener/top/10

# Find missed opportunities
curl "http://localhost:5128/api/screener/missed-opportunities?daysLookback=30&topN=50"

# Check health
curl http://localhost:5128/health

# View Swagger docs
# Open http://localhost:5128/swagger in browser
```

## 📊 Scoring Formula

Each stock receives a composite score from 0-100:

| Component | Weight | Range | How It Works |
|-----------|--------|-------|--------------|
| **Volatility** | 25% | 0-100 | Price reversals / 30 × 100. Higher = more directional changes |
| **Drawdown** | 25% | 0-100 | Recent max drawdown / 30% × 100. Higher = smaller declines |
| **Extreme** | 25% | 0-100 | Distance from 60d highs/lows. Higher = at reversal points |
| **Liquidity** | 25% | 0-100 | **GATE FILTER**: 100 if $2B+ cap + $50M+ ADV, down to 0 if below $5M ADV |

**Logic**: 
- If Liquidity < 40 → Stock is excluded
- Otherwise → TotalScore = (Volatility × 0.25) + (Drawdown × 0.25) + (Extreme × 0.25) + (Liquidity × 0.25)

## 📈 Example Result

```json
{
  "ticker": "MSFT",
  "companyName": "Microsoft Corp.",
  "rank": 1,
  "totalScore": 77.14,
  "scores": {
    "volatility": 66.67,
    "drawdown": 98.19,
    "extreme": 43.68,
    "liquidity": 100
  },
  "metrics": {
    "fluctuationCount": 20,
    "rangePercent": 39.19,
    "recentDrawdownPercent": 29.46,
    "distanceFromHighPercent": 1.88,
    "distanceFromLowPercent": 43.68
  },
  "explanation": "MSFT: Score 77.1/100. Volatility: 20 reversals (67/100). Drawdown: 29.5% recent (98/100). Extreme: 43.7% from edge (44/100). Liquidity: ADV$65.0M, Cap$2500.0B (100/100)."
}
```

## 🗄️ Database

**SQLite** with auto-migrations on startup.

**Tables:**
- `Stocks`: Universe of stocks (Ticker, CompanyName, MarketCapUSD, AverageDailyVolumeUSD)
- `DailyCandlesDb`: OHLCV data (60-day rolling window per stock)
- `ScreenRuns`: Historical screening sessions
- `ScreenResults`: Score breakdown per stock per run
- `NewsArticles`: Market/company news (for future use)

## 🔄 Data Flow

```
1. Fetch Universe (S&P 500, Nasdaq, ITS) via StubUniverseProvider
   ↓
2. Fetch 60 days of candles per stock via StubMarketDataProvider
   ↓
3. Compute metrics (volatility, drawdown, extremes) via MetricsCalculator
   ↓
4. Calculate 4-component score via ScoringCalculator
   ↓
5. Filter by liquidity (LiquidityScore ≥ 40)
   ↓
6. Rank by total score (highest first)
   ↓
7. Persist ScreenRun + ScreenResults to database
```

## 🧪 Current Data Providers (Stubs)

The application ships with **realistic stub data providers** that generate deterministic, seeded data perfect for development and testing:

- **StubUniverseProvider**: Returns S&P 500 + Nasdaq + ITS stocks
- **StubMarketDataProvider**: Generates 60 daily candles with mean-reversion pricing
- **StubNewsProvider**: Returns sample market news

## 🔌 API Endpoints

### POST /api/screener/run
Execute complete screening. Returns ranked list of qualified stocks.

### GET /api/screener/top/{count}
Get top N stocks from latest screening run.
- **Path parameter**: `count` (required, > 0)

### GET /api/screener/missed-opportunities
Find stocks that dropped out of top N across screening runs.
- **Query params**: 
  - `daysLookback` (optional, default: 30)
  - `topN` (optional, default: 50)

### GET /health
Health check endpoint.

### GET /swagger
Interactive API documentation (development only).

## 🛠️ Development

### Project Structure Rationale

**Domain Layer** (no external deps)
- Pure business logic
- Entities, value objects, interfaces
- Can be referenced by any layer

**Application Layer** (depends on Domain)
- Use case orchestration
- Service interfaces
- DTOs
- No database/HTTP knowledge

**Infrastructure Layer** (depends on Application)
- Data persistence (EF Core)
- External API clients
- Configuration
- Repository implementations

**API Layer** (depends on all)
- HTTP endpoints
- Request/response handling
- Dependency injection setup

### Adding a New Feature

1. **Domain**: Define entity/value object if needed
2. **Application**: Add interface & service implementation
3. **Infrastructure**: Implement concrete provider if needed
4. **API**: Add endpoint handler

Example: To add sector filtering:
```csharp
// Domain: Add Sector property to Stock entity
public string Sector { get; set; }

// Application: Add sector filter to interface
Task<List<ScreenResultDto>> GetBySeqorAsync(string sector, CancellationToken ct);

// Infrastructure: Implement in repository
// API: Add GET /api/screener/sector/{sector} endpoint
```

## 📝 Next Steps (Not in MVP)

1. **Real Data Integration**
   - Replace stub providers with Yahoo Finance / Alpha Vantage APIs
   - Add caching for fundamentals (rarely changes)

2. **Enhanced Analysis**
   - Add technical indicators (RSI, MACD, Bollinger Bands)
   - Support multiple scoring profiles (aggressive/conservative)
   - Sector-specific thresholds

3. **User Interface**
   - React/Vue dashboard
   - Real-time notifications
   - Portfolio integration

4. **Scaling**
   - PostgreSQL/Azure SQL instead of SQLite
   - Redis caching
   - Background job processor (Hangfire)
   - Cloud deployment (Azure App Service)

## 📚 Documentation

See [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) for comprehensive API reference.

## 📦 Technology Stack

- **.NET 10.0** - Latest .NET runtime
- **C# 13** - Latest language features
- **ASP.NET Core** - Web framework
- **Entity Framework Core 10** - ORM
- **SQLite** - Embedded database
- **Swashbuckle** - Swagger/OpenAPI

## 📄 License

This is a personal project for stock analysis and education.

---

**Status**: MVP Complete ✅
- Core screening logic implemented
- API fully functional
- Database with migrations
- Ready for real data provider integration
