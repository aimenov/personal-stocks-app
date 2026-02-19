# 🚀 Stock Screener - Quick Start Guide

## ✅ Setup Complete!

Your full-stack stock screener is ready to run. Both the .NET backend and Next.js frontend are configured and ready.

## Starting the Application

### Option 1: Using VS Code Terminals (Recommended)

Open TWO new terminals in VS Code:

**Terminal 1 - Start Backend API:**
```bash
cd /workspaces/dotnet-codespaces/StockScreener
dotnet run --project StockScreener.API/StockScreener.API.csproj
```

You should see:
```
info: Microsoft.EntityFrameworkCore.Database.Command
      Executed DbCommand (12ms) [Parameters=[], CommandType='Text']
      SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Stocks';
...
info: Microsoft.Hosting.Lifetime
      Now listening on: http://localhost:5128
      Application started. Press Ctrl+C to stop.
```

**Terminal 2 - Start Frontend UI:**
```bash
cd /workspaces/dotnet-codespaces/stock-screener-ui
npm run dev
```

You should see:
```
  ▲ Next.js 14.1.0
  - ready started server on [::1]:3000, url: http://localhost:3000
  ✓ compiled client and server successfully
```

### Option 2: Using "watch" Tasks (Faster Iteration)

If you prefer auto-reload during development:

**Backend:**
```bash
cd /workspaces/dotnet-codespaces/StockScreener
dotnet watch run --project StockScreener.API/StockScreener.API.csproj
```

**Frontend:**
```bash
cd /workspaces/dotnet-codespaces/stock-screener-ui
npm run dev  # Already has hot-reload
```

## Accessing the Application

Once both servers are running, open your browser:

### 🎯 **Main Dashboard**
**http://localhost:3000**
- View top 50 ranked stocks
- Sort by rank, score, or metrics
- Search for stocks by ticker or company name
- Hover over scores to see detailed breakdown

### 📊 **API Documentation**
**http://localhost:5128/swagger**
- Interactive API explorer
- Test endpoints directly
- See request/response schemas

### 🏥 **Health Check**
**http://localhost:5128/health**
- Verify backend is running
- Returns: `{"status":"OK","timestamp":"2024-..."}`

## What's Happening Behind the Scenes

### Backend ✓
- **Database**: SQLite (`stockscreener.db`) auto-created and migrated
- **Stub Data**: 6 stocks (AAPL, MSFT, GOOGL, NVDA, TSLA, ITA) with 60 days of price history
- **Scoring Engine**: Evaluates stocks on Volatility, Drawdown, Extremeness, and Liquidity
- **CORS Enabled**: Allows requests from localhost:3000 and localhost:3001
- **API Endpoints**: 
  - `POST /api/screener/run` - Run a new screening
  - `GET /api/screener/top/50` - Get top 50 stocks
  - `GET /api/screener/missed-opportunities` - Historical opportunities

### Frontend ✓
- **Framework**: Next.js 14 (App Router, Server/Client Components)
- **State Management**: React Query (automatic caching, refetching)
- **Styling**: Tailwind CSS (dark mode, responsive)
- **Animations**: Framer Motion (smooth hover effects)
- **API Client**: Axios with full TypeScript typing

## Testing the Dashboard

1. **On Load**: Dashboard fetches top 50 stocks from backend
2. **Search**: Type in the search box (top right of table) to filter by:
   - Stock ticker: "AAPL" or "msft"
   - Company name: "Apple" or "Microsoft"
3. **Sort**: Click any column header to sort (ascending/descending)
   - Rank, Score, Volatility, Drawdown, Range, Liquidity
4. **Score Breakdown**: Hover over a stock's score to see:
   - Individual component scores (4 metrics)
   - Color-coded breakdown
   - Detailed explanation

## Common Issues & Solutions

### "Cannot GET /" or Connection Refused
- ✅ Ensure both servers are running in separate terminals
- ✅ Check ports aren't blocked: 3000 (frontend) and 5128 (backend)

### CORS Error in Browser Console
- ✅ Verify backend has CORS enabled in `Program.cs`
- ✅ Check `NEXT_PUBLIC_API_URL` in `.env.local` is correct

### No Data Displayed in Dashboard
- ✅ Check backend console for errors during startup
- ✅ Visit http://localhost:5128/api/screener/top/50 directly (should return JSON)
- ✅ Check browser Network tab for failed API requests

### NPM Packages Not Found
```bash
# Clear cache and reinstall
cd /workspaces/dotnet-codespaces/stock-screener-ui
rm -rf node_modules
npm install
npm run dev
```

### Backend Won't Start
```bash
# Clean and rebuild
cd /workspaces/dotnet-codespaces/StockScreener
dotnet clean
dotnet build
dotnet run --project StockScreener.API/StockScreener.API.csproj
```

## Project Structure

```
/workspaces/dotnet-codespaces/
├── StockScreener/               # Backend (.NET 10)
│   ├── StockScreener.Domain/    # Entities, value objects
│   ├── StockScreener.Application/  # Services, business logic
│   ├── StockScreener.Infrastructure/   # EF Core, data access
│   └── StockScreener.API/       # REST API endpoints
│
└── stock-screener-ui/           # Frontend (Next.js 14)
    ├── app/                     # Pages (dashboard, stock details, etc.)
    ├── components/              # Reusable React components
    ├── lib/                     # Utilities (API client, types)
    └── public/                  # Static assets
```

## Next Development Steps

### Planned Features (Not Yet Implemented)
- ⏳ Stock detail page with 60-day price chart
- ⏳ Missed opportunities page
- ⏳ News feed integration
- ⏳ Portfolio tracking
- ⏳ Custom alerts
- ⏳ User authentication
- ⏳ Backtesting engine

### To Add a New Feature
1. Update backend API endpoint in `StockScreener.API/Endpoints/`
2. Add TypeScript types in `stock-screener-ui/lib/types.ts`
3. Create API method in `stock-screener-ui/lib/api.ts`
4. Create React component in `stock-screener-ui/components/`
5. Use React Query hook in your page

## Performance Tips

- **Caching**: React Query caches data for 5 minutes by default
- **Search**: Client-side filtering (no extra API calls)
- **Sorting**: Client-side sorting (full table loaded once)
- **Future**: Add pagination or virtual scrolling for 1000+ stocks

## Need Help?

Check these documentation files:
- [Stock Screener Architecture](stock-screener-ui/UI_ARCHITECTURE.md) - UI design decisions
- [API Documentation](StockScreener/API_DOCUMENTATION.md) - Backend endpoints
- [Project Completion](StockScreener/PROJECT_COMPLETION.md) - Implementation details
- [Quick Reference](StockScreener/QUICK_REFERENCE.md) - Code snippets

## Troubleshooting Checklist

- [ ] Backend running on port 5128?
- [ ] Frontend running on port 3000?
- [ ] Can you ping http://localhost:5128/health?
- [ ] Can you see stocks in http://localhost:5128/api/screener/top/50?
- [ ] No CORS errors in browser console?
- [ ] Both npm install and dotnet build succeeded?

---

**Happy analyzing! 📈**

Once you're familiar with the setup, check out the Architecture docs to understand design decisions and next steps.
