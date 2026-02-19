# 🚀 Stock Screener - Complete Full-Stack MVP

A high-performance stock screening application built with **.NET 10 + Next.js 14**.

## ⚡ Quick Start

```bash
# Option 1: Automated (starts both servers)
./start.sh

# Option 2: Manual - Backend
cd StockScreener
dotnet run --project StockScreener.API/StockScreener.API.csproj

# Option 3: Manual - Frontend (in new terminal)
cd stock-screener-ui
npm run dev
```

Then open: **http://localhost:3000**

---


### 🎯 Dashboard
- **Top 50 Ranked Stocks** - Automatically scored and ranked
- **Sortable Columns** - Click any header to sort
- **Real-time Search** - Filter by ticker or company name instantly
- **Score Breakdown** - Hover over scores to see detailed analysis
- **Smooth Animations** - Premium transitions and interactions

### 📈 Smart Scoring
- **4-Component Algorithm**
  - Volatility (25%) - Price fluctuation frequency
  - Drawdown (25%) - Recent decline magnitude
  - Extremeness (25%) - Unusual price movements
  - Liquidity (25%) - Market depth and volume

### 🏗️ Architecture
- **Clean Architecture** - Domain → Application → Infrastructure → API
- **Responsive Design** - Works on desktop, tablet, mobile
- **Type-Safe** - Full TypeScript + C# with strict modes
- **Database** - SQLite with auto-migration

---

## 🛠️ Tech Stack

### Backend
```
.NET 10.0 + C# 13
├── ASP.NET Core (Minimal APIs)
├── Entity Framework Core 10.0.2
├── SQLite
└── Clean Architecture Pattern
```

### Frontend
```
Next.js 14 + TypeScript
├── React 18.3.1
├── Tailwind CSS 3.4.1
├── React Query 5.35.1 (server state)
├── Framer Motion 10.18.0 (animations)
└── Axios 1.7.2 (HTTP client)
```

### Database
```
SQLite (embedded)
├── 5 tables (Stocks, Candles, Runs, Results, News)
├── 6 test stocks (AAPL, MSFT, GOOGL, NVDA, TSLA, ITA)
└── 60 days of OHLCV data per stock
```

---

## 📚 Documentation

| Document | Purpose |
|----------|---------|
| **[QUICK_START.md](QUICK_START.md)** | Setup & run instructions |
| **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)** | Complete feature overview |
| **[INDEX.md](INDEX.md)** | Documentation index by role |
| **[MANIFEST.md](MANIFEST.md)** | Complete file manifest |
| **[StockScreener/API_DOCUMENTATION.md](StockScreener/API_DOCUMENTATION.md)** | API endpoint reference |
| **[StockScreener/PROJECT_COMPLETION.md](StockScreener/PROJECT_COMPLETION.md)** | Implementation details |
| **[stock-screener-ui/UI_ARCHITECTURE.md](stock-screener-ui/UI_ARCHITECTURE.md)** | Design system & components |

## 🚀 Running the Application

### Prerequisites
```bash
# Check requirements
node --version          # v18+ required
npm --version           # v9+ required
dotnet --version        # .NET 10.0 required
```

### After Startup
```
✓ Backend:        http://localhost:5128
✓ Frontend:       http://localhost:3000
✓ API Docs:       http://localhost:5128/swagger
✓ Health Check:   http://localhost:5128/health
```
---

## 🔌 API Endpoints

### 1. Get Top Stocks
```http
GET /api/screener/top/50
```
Returns top N ranked stocks with scores and metrics.

### 2. Run Screening
```http
POST /api/screener/run
```
Executes a new screening cycle.

### 3. Missed Opportunities
```http
GET /api/screener/missed-opportunities
```
Returns past stocks that outperformed.

---

## 📝 License

Personal project for stock analysis.

---
