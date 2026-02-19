# Stock Screener UI - Architecture Proposal

## 📐 UI Architecture

### Routes Structure
```
/                           # Dashboard (top stocks)
  └─ layout.tsx            # Root layout with navigation, theme toggle
  └─ page.tsx              # Dashboard page (main entry)
  
/stocks                     # Stock details
  └─ [ticker]/page.tsx     # Individual stock detail page
  
/opportunities              # Missed opportunities
  └─ page.tsx              # Historical analysis page
  
/news                       # News aggregation
  └─ page.tsx              # Combined market + company news
  
/settings                   # User preferences
  └─ page.tsx              # Theme, columns, filters
  
/api                        # Frontend API routes (optional, for now just proxy to .NET)
  └─ stocks/               # Proxy routes if needed later
```

### Component Structure
```
components/
├── layout/
│   ├── Header.tsx          # Nav bar + logo + theme toggle
│   ├── Sidebar.tsx         # Optional navigation sidebar
│   └── Footer.tsx          # Footer with links
│
├── dashboard/
│   ├── StockTable.tsx      # Main stocks table/grid
│   ├── StockRow.tsx        # Single stock row
│   ├── ScorePopup.tsx      # Hover tooltip showing score breakdown
│   ├── SearchBar.tsx       # Search + filters
│   └── Pagination.tsx      # Page navigation
│
├── stock-details/
│   ├── StockHeader.tsx     # Logo, ticker, name, price badge
│   ├── PriceChart.tsx      # 60-day chart using Recharts
│   ├── MetricsCard.tsx     # Individual metric display
│   ├── MetricsGrid.tsx     # Grid of all metrics
│   ├── NewsSection.tsx     # Company news list
│   └── ActionButtons.tsx   # Watch, Ignore, Traded buttons
│
├── common/
│   ├── Card.tsx            # Base card component
│   ├── Badge.tsx           # Status badges
│   ├── Icon.tsx            # Icon wrapper
│   ├── LoadingSpinner.tsx  # Loading state
│   ├── ErrorBoundary.tsx   # Error handling
│   └── AnimatedButton.tsx  # Reusable button with hover animation
│
├── opportunities/
│   ├── OpportunityList.tsx # List of missed opportunities
│   ├── OpportunityCard.tsx # Single opportunity
│   └── PerformanceChart.tsx # Forward return chart
│
└── news/
    ├── NewsCard.tsx        # Single news item
    ├── NewsFeed.tsx        # News list with filters
    └── NewsFilter.tsx      # Filter by category/stock
```

### Design System

#### Colors (Dark Mode Primary)
```
Primary:
  - bg-gray-950           # Nearly black background
  - bg-gray-900           # Card backgrounds
  - bg-gray-800           # Hover states, borders

Accent:
  - emerald-500           # Positive/buy signal (green)
  - amber-500             # Neutral/watch signal (orange)
  - red-500               # Negative/risk signal (red)
  - blue-500              # Information/selection (blue)

Text:
  - text-white            # Primary text
  - text-gray-300         # Secondary text
  - text-gray-500         # Tertiary text (muted)

Charts:
  - emerald-400           # Score bars (green)
  - amber-400             # Neutral metrics (amber)
  - blue-400              # Volatility (blue)
  - violet-400            # Extreme score (purple)
```

#### Typography
```
Font Family: 'Inter' or 'Geist' (from Next.js default)
Font Weights: 400 (normal), 500 (medium), 600 (semibold), 700 (bold)

Heading 1 (h1): text-4xl font-bold              # Dashboard title
Heading 2 (h2): text-2xl font-semibold          # Page titles
Heading 3 (h3): text-xl font-semibold           # Section titles
Body (p):       text-base font-normal           # Default text
Small:          text-sm font-normal             # Secondary text
Tiny:           text-xs font-normal             # Muted text
Mono:           font-mono                       # Ticker symbols, numbers
```

#### Spacing Scale
```
xs:  0.25rem   (4px)
sm:  0.5rem    (8px)
md:  1rem      (16px)
lg:  1.5rem    (24px)
xl:  2rem      (32px)
2xl: 3rem      (48px)
```

#### Components Style
```
Cards:
  - bg-gray-900
  - border border-gray-800
  - rounded-lg
  - shadow-lg hover:shadow-xl transition-shadow
  - p-4 or p-6

Buttons:
  - Primary:     bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg
  - Secondary:   bg-gray-800 hover:bg-gray-700 text-white rounded-lg
  - Ghost:       bg-transparent hover:bg-gray-800 text-white rounded-lg
  - All:         transition-all duration-200 ease-out

Inputs:
  - bg-gray-800
  - border-gray-700
  - text-white placeholder-gray-500
  - rounded-lg
  - px-4 py-2

Badges:
  - Positive:    bg-emerald-900/30 text-emerald-300
  - Neutral:     bg-amber-900/30 text-amber-300
  - Negative:    bg-red-900/30 text-red-300
  - Info:        bg-blue-900/30 text-blue-300
```

---

## 🔌 API Endpoints (Backend Integration)

### Endpoints to Consume (Existing)

#### 1. Run Screening
```
POST /api/screener/run
Response:
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
  "explanation": "MSFT: Score 77.1/100..."
}[]
```

#### 2. Get Top Results
```
GET /api/screener/top/{count}
Parameters: count (integer, 1-100)
Response: Same as above, filtered to top N
```

#### 3. Get Missed Opportunities
```
GET /api/screener/missed-opportunities?daysLookback=30&topN=50
Parameters: daysLookback, topN
Response: Array of ScreenResultDto (same schema)
```

### Endpoints to Add (Minor Backend Enhancement)

#### 4. Get Stock Details (NEW - Optional)
```
GET /api/screener/stocks/{ticker}
Response:
{
  "ticker": "MSFT",
  "companyName": "Microsoft Corp.",
  "exchange": "NASDAQ",
  "marketCapUSD": 2500000000000,
  "averageDailyVolumeUSD": 65000000,
  "dailyCandlesLast60": [
    {
      "date": "2026-01-22",
      "open": 403.50,
      "high": 407.80,
      "low": 401.20,
      "close": 405.10,
      "volume": 50000000
    }
  ],
  "latestScreenResult": { /* ScreenResultDto */ }
}
```

#### 5. Get News Feed (NEW - Optional)
```
GET /api/screener/news?ticker=MSFT&category=company
Response:
[
  {
    "id": "uuid",
    "ticker": "MSFT",
    "title": "Microsoft Q4 Earnings Beat",
    "source": "Bloomberg",
    "url": "https://...",
    "publishedAt": "2026-01-22T10:30:00Z",
    "category": "Company"
  }
]
```

#### 6. Get Stock Logo URL (NEW - Optional Helper)
```
GET /api/screener/logos/{ticker}
Response:
{
  "ticker": "MSFT",
  "logoUrl": "https://logo.clearbit.com/microsoft.com"
}
```

### CORS Configuration Needed
```csharp
// In Program.cs - Add CORS middleware
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs", builder =>
    {
        builder
            .WithOrigins("http://localhost:3000", "http://localhost:3001")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// In pipeline - use CORS
app.UseCors("AllowNextJs");
```

---

## 🎨 Key Design Decisions

### 1. **Pagination Strategy**
- Load initial 50 stocks
- Virtual scrolling or "Load More" button for performance
- React Query caching for instant re-fetches

### 2. **Logo Caching**
- Use Clearbit: `https://logo.clearbit.com/{domain}.com`
- Cache URLs in state/localStorage
- Fallback to generic stock icon if missing
- Example: MSFT → `https://logo.clearbit.com/microsoft.com`

### 3. **Chart Data**
- Fetch dailyCandlesLast60 from stock details endpoint
- Use Recharts for clean, minimal visualization
- Show: Open, High, Low, Close, Volume (with tabs)

### 4. **Hover Popups**
- Use Radix UI Popover (via shadcn/ui)
- Show score breakdown on table row hover
- Positioned to follow cursor or stick to row
- Animation: fade-in + scale (0.95 → 1.0)

### 5. **Animations**
- Button hover: scale(1.05) + shadow boost
- Row hover: bg color shift + popup fade in
- Page transitions: cross-fade or slide
- Use Framer Motion for complex sequences

### 6. **Mobile Responsive**
- Desktop: Table view (full metrics visible)
- Tablet: Simplified columns (logo, ticker, score, single top metric)
- Mobile: Card view (logo, ticker, score, explanation)
- Use Tailwind breakpoints: sm, md, lg, xl, 2xl

### 7. **Performance**
- React Query for server state + caching
- Virtualized lists for 1000+ items (if needed later)
- Image optimization (Next.js Image component)
- Code splitting by route
- Debounced search (300ms)

### 8. **Search & Filter**
- Search by ticker or company name (client-side for top 50, server if pagination)
- Filter by score range: sliders for min/max score
- Filter by metric: volatility > X, liquidity tier
- Sort by: score, volatility, drawdown, liquidity, name

---

## 📦 Tech Stack Implementation

| Feature | Library | Why |
|---------|---------|-----|
| Framework | Next.js 14 | Server components, API routes, optimal DX |
| Language | TypeScript | Type safety, IDE support |
| Styling | Tailwind CSS | Utility-first, fast, great dark mode |
| Components | shadcn/ui | Headless, unstyled, fully customizable |
| Charts | Recharts | React, simple, works great in Next.js |
| Animations | Framer Motion | Smooth, performant, React-native feel |
| Data Fetching | React Query | Caching, background updates, offline |
| HTTP | Axios | Type-safe, interceptors for API base URL |
| Icons | lucide-react | Minimal, clean, works with Tailwind |
| Notifications | Sonner | Toast notifications, non-intrusive |
| Modals/Popups | Radix UI | Accessible, composable (via shadcn) |

---

## 🚀 Phased Implementation Plan

### Phase 1: Project Setup (5 min)
- Create Next.js 14 app with TS, Tailwind, shadcn/ui
- Configure API client (Axios with env var for backend URL)
- Set up React Query provider

### Phase 2: Dashboard (30 min)
- Build StockTable component
- Fetch top 50 stocks
- Add sorting, filtering, search
- Premium styling + dark mode

### Phase 3: Interactions (20 min)
- Add hover popups (score breakdown)
- Button animations
- Loading/error states
- Page transitions

### Phase 4: Stock Details (30 min)
- Create [ticker] page
- Fetch stock candle data
- Render price chart
- Show metrics cards + news

### Phase 5: Additional Pages (20 min)
- Missed opportunities
- News feed
- Settings (theme toggle)

### Phase 6: Polish (20 min)
- Responsive mobile/tablet views
- Accessibility (aria labels)
- Performance optimization
- Dark/light mode toggle

**Total**: ~2-2.5 hours for full implementation

---

## 🎯 Success Criteria

✅ Dashboard loads top 50 stocks in < 2 seconds
✅ Hover popup appears instantly (no lag)
✅ Search filters as you type (debounced)
✅ Stock detail page shows chart in < 1 second
✅ Mobile view is fully functional
✅ Dark mode is smooth, no flash on load
✅ All animations are 60fps (no jank)
✅ Responsive from 375px (mobile) to 1920px+ (desktop)

---

## Next Step
Ready to implement. Will start with:
1. Creating Next.js project
2. Setting up design system (tailwind config)
3. Building dashboard with top stocks table
4. Adding premium styling + animations
