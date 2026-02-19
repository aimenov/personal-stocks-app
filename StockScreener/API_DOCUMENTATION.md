# Stock Screener API - MVP Documentation

## Overview
The Stock Screener API is a .NET 10 application that identifies trading opportunities based on volatility patterns, drawdown analysis, and liquidity filters. It ranks stocks using a 4-component scoring system.

## Architecture

### Technology Stack
- **.NET 10.0** with C# 13
- **ASP.NET Core** Minimal API
- **Entity Framework Core 10** with SQLite
- **Clean Architecture**: Domain → Application → Infrastructure → API

### Project Structure
```
StockScreener/
├── StockScreener.Domain/           # Entities, value objects, constants
├── StockScreener.Application/      # Interfaces, DTOs, business logic
├── StockScreener.Infrastructure/   # EF Core, data providers, repositories
└── StockScreener.API/              # Minimal API endpoints
```

## Running the Application

### Prerequisites
- .NET 10 SDK installed
- Unix/Linux terminal (or Windows with WSL)

### Start the API
```bash
cd StockScreener/StockScreener.API
dotnet run
```

The API will:
1. Auto-migrate the SQLite database on startup
2. Listen on `http://localhost:5128` (or configured port)
3. Swagger documentation available at `/swagger`

## API Endpoints

### 1. POST /api/screener/run
**Execute a complete screening run**

- Fetches universe of stocks (S&P 500, Nasdaq, ITS)
- Analyzes 60 days of price data
- Calculates 4-component scores for each stock
- Filters by liquidity requirements
- Ranks and persists results

**Request:**
```bash
curl -X POST http://localhost:5128/api/screener/run
```

**Response:**
```json
[
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
    "explanation": "MSFT: Score 77.1/100. Volatility: 20 reversals (67/100)..."
  }
]
```

### 2. GET /api/screener/top/{count}
**Get top N stocks from latest screening run**

**Request:**
```bash
curl http://localhost:5128/api/screener/top/10
```

**Response:** Same format as RunScreener (ranked list)

**Parameters:**
- `count` (required, path): Number of top results to return (must be > 0)

### 3. GET /api/screener/missed-opportunities
**Find stocks that dropped out of top N**

Compares current screening run with historical runs to identify previously ranked stocks no longer in top N.

**Request:**
```bash
curl "http://localhost:5128/api/screener/missed-opportunities?daysLookback=30&topN=50"
```

**Parameters:**
- `daysLookback` (optional, query): Number of days to look back (default: 30, must be > 0)
- `topN` (optional, query): Size of top N list to compare (default: 50, must be > 0)

**Response:** Same format as RunScreener (list of ScreenResultDto)

## Scoring Formula

The system ranks stocks using a weighted 4-component score (0-100):

### 1. **Volatility Score** (0-100)
- Measures price reversals over 60 days
- Formula: (reversals / 30) × 100
- Capped at 100
- Higher = more directional changes (opportunity for momentum trading)

### 2. **Drawdown Score** (0-100)
- Measures maximum peak-to-trough decline in 60-day window
- Formula: (drawdown% / 30%) × 100
- Capped at 100
- Higher = smaller recent drawdowns (less risky to enter)

### 3. **Extreme Score** (0-100)
- Measures how far current price is from 60-day extremes
- Formula: MAX(distance from high%, distance from low%)
- Range: 0-100
- Higher = price at extremes (potential reversal points)

### 4. **Liquidity Score** (0-100) - **GATE FILTER**
- Ensures sufficient capital is available for $100k order execution
- Tiered scoring:
  - **100**: Market Cap ≥ $2B AND ADV ≥ $50M/day
  - **80**: Market Cap ≥ $500M AND ADV ≥ $10M/day
  - **60**: ADV ≥ $10M/day
  - **40**: ADV ≥ $5M/day
  - **0**: Below thresholds (EXCLUDED from results)

### **Total Score**
```
If LiquidityScore < 40:
  TotalScore = 0 (stock excluded)
Else:
  TotalScore = (Volatility × 0.25) + (Drawdown × 0.25) + (Extreme × 0.25) + (Liquidity × 0.25)
```

## Database Schema

### Core Tables
- **Stocks**: Universe of stocks being tracked
  - Ticker (unique), CompanyName, Exchange, MarketCapUSD, AverageDailyVolumeUSD
  
- **DailyCandlesDb**: OHLCV data (60-day rolling window)
  - StockId (FK), Date (unique per stock), Open, High, Low, Close, Volume
  
- **ScreenRuns**: Historical screening sessions
  - RunDate, TotalStocksScanned, StocksWithValidScore
  
- **ScreenResults**: Score breakdown per stock per run
  - ScreenRunId (FK), StockId (FK), Rank, TotalScore, 4 component scores, MetricsExplanation

- **NewsArticles**: Market and company news (for future enhancement)

## Data Providers

### Current Implementation (Stubs)
The application includes realistic stub data providers for development/testing:

1. **StubUniverseProvider**: Returns hardcoded S&P 500, Nasdaq, and ITS stocks
2. **StubMarketDataProvider**: Generates 60 daily candles with mean-reversion pricing
3. **StubNewsProvider**: Returns sample market and company news

### Future Enhancement
Replace stubs with real APIs:
- Yahoo Finance for price data
- Alpha Vantage or similar for fundamentals
- NewsAPI for news aggregation

## Error Handling

The API returns appropriate HTTP status codes:
- **200**: Success (with results array)
- **400**: Bad request (invalid parameters or screening failure)
- **500**: Server error

Example error response:
```json
{
  "error": "Count must be greater than 0"
}
```

## Development Features

### Swagger/OpenAPI Documentation
Access interactive API documentation:
```
http://localhost:5128/swagger
```

### Health Check
```bash
curl http://localhost:5128/health
```

Response:
```json
{
  "status": "OK",
  "timestamp": "2026-01-22T15:21:00.3072075Z"
}
```

## Performance Notes

- First run takes ~2-3 seconds (fetches universe, analyzes 60 days of data)
- Results are persisted, enabling historical analysis
- GetTopResults is near-instantaneous (queries database)
- MissedOpportunities query scans multiple screening runs (slower with many runs)

## Testing

### Manual Testing
```bash
# Run complete screening
curl -X POST http://localhost:5128/api/screener/run | jq .

# Get top 5 stocks
curl http://localhost:5128/api/screener/top/5 | jq '.[] | {ticker, rank, totalScore}'

# Check missed opportunities
curl "http://localhost:5128/api/screener/missed-opportunities?daysLookback=7&topN=3" | jq .
```

### Integration with External Systems
The ScreeningEngine implements `IScreeningService`:
```csharp
Task<List<ScreenResultDto>> RunScreenerAsync(CancellationToken ct);
Task<List<ScreenResultDto>> GetTopResultsAsync(int count, CancellationToken ct);
Task<List<ScreenResultDto>> GetMissedOpportunitiesAsync(int daysLookback, int topN, CancellationToken ct);
```

Can be injected into any .NET service/scheduler for automated screening runs.

## Future Enhancements

1. **Real Data Integration**
   - Connect to Yahoo Finance, Alpha Vantage, or Polygon APIs
   - Replace stub providers with live market data

2. **Advanced Filtering**
   - Add sector/industry filters
   - Support custom scoring weights
   - Add technical indicators (RSI, MACD, Bollinger Bands)

3. **Persistence & Reporting**
   - Historical performance tracking
   - PDF reports of screening runs
   - Alert system for qualified stocks

4. **User Interface**
   - React/Vue dashboard
   - Real-time stock notifications
   - Portfolio integration

5. **Scalability**
   - Move to cloud SQL (PostgreSQL, Azure SQL)
   - Add caching layer (Redis)
   - Implement background job processing

## Architecture Highlights

### Clean Architecture Benefits
- **Testability**: Domain/Application layers have zero external dependencies
- **Maintainability**: Clear separation of concerns
- **Extensibility**: Add new data providers without changing scoring logic
- **Reusability**: IScreeningService can be used in any .NET project

### Key Design Patterns
- **Repository Pattern**: Abstract data access
- **Dependency Injection**: Loose coupling throughout
- **Value Objects**: Immutable StockMetrics and LiquidityProfile
- **Unit of Work**: Transactional consistency across repositories
- **DTO Pattern**: API contracts separate from domain models

---

**MVP Status**: ✅ Complete
- All core screening logic implemented
- API endpoints fully functional
- Database migrations ready
- Stub data providers for testing
- Clean Architecture enforced

**Ready for**: Testing, deployment, and real data provider integration
