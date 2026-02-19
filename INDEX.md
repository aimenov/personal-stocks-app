# 📚 Stock Screener - Complete Project Documentation

Welcome! This document indexes all resources for the Stock Screener MVP.

## 🚀 Getting Started (Pick One)

### Option 1: Automated Setup (Fastest)
```bash
./start.sh
```
This starts both backend (port 5128) and frontend (port 3000) in the background.

### Option 2: Manual Setup (Recommended for Development)
See [QUICK_START.md](QUICK_START.md) for detailed terminal setup instructions.

### Option 3: Visual Code  
Check the "Task" menu in VS Code for pre-configured build tasks.

---

## 📖 Documentation Structure

### 🎯 For Getting Started
1. **[QUICK_START.md](QUICK_START.md)** ← START HERE
   - Step-by-step setup instructions
   - Two-terminal approach (recommended)
   - Troubleshooting guide
   - How to test the application

2. **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)**
   - Complete feature list
   - Architecture overview
   - Tech stack details
   - Performance characteristics

### 🔧 Technical Reference

#### Backend Documentation
- **[StockScreener/README.md](StockScreener/README.md)**
  - Backend project structure
  - How to run and test
  - API overview

- **[StockScreener/API_DOCUMENTATION.md](StockScreener/API_DOCUMENTATION.md)**
  - Complete endpoint reference
  - Request/response examples
  - Error handling
  - CURL examples

- **[StockScreener/PROJECT_COMPLETION.md](StockScreener/PROJECT_COMPLETION.md)**
  - Implementation details
  - Design decisions
  - Known limitations
  - Future enhancements

- **[StockScreener/QUICK_REFERENCE.md](StockScreener/QUICK_REFERENCE.md)**
  - Key files and classes
  - Code snippets
  - Common tasks

#### Frontend Documentation
- **[stock-screener-ui/README.md](stock-screener-ui/README.md)**
  - Frontend project setup
  - Dependencies explained
  - Component structure
  - Development workflow

- **[stock-screener-ui/UI_ARCHITECTURE.md](stock-screener-ui/UI_ARCHITECTURE.md)**
  - Design system
  - Component hierarchy
  - Color scheme
  - Animation system
  - API integration plan

---

## 🏗️ Project Structure

```
/workspaces/dotnet-codespaces/
│
├── 📚 Documentation (You Are Here)
│   ├── README.md                    ← Project overview
│   ├── QUICK_START.md              ← Setup & run instructions
│   ├── IMPLEMENTATION_SUMMARY.md    ← Complete feature summary
│   ├── INDEX.md                     ← This file
│   ├── setup.sh                     ← Automated setup
│   └── start.sh                     ← Automated startup
│
├── StockScreener/                   ← Backend (.NET)
│   ├── StockScreener.slnx           ← Solution file
│   ├── StockScreener.API/
│   │   ├── Program.cs               ← Entry point, DI, CORS setup
│   │   ├── ScreenerEndpoints.cs     ← REST API routes
│   │   └── appsettings.json         ← Configuration
│   │
│   ├── StockScreener.Application/
│   │   ├── ScreeningEngine.cs       ← Main orchestrator
│   │   ├── ScoringCalculator.cs     ← Scoring algorithm
│   │   ├── MetricsCalculator.cs     ← Price metrics
│   │   ├── Interfaces/              ← Service contracts
│   │   └── DTOs/                    ← API data contracts
│   │
│   ├── StockScreener.Domain/
│   │   ├── Entities/                ← Business objects
│   │   ├── ValueObjects/            ← Immutable values
│   │   └── Constants/               ← Domain constants
│   │
│   ├── StockScreener.Infrastructure/
│   │   ├── Persistence/
│   │   │   └── StockScreenerDbContext.cs ← EF Core mapping
│   │   ├── Repository.cs            ← Generic data access
│   │   ├── UnitOfWork.cs            ← Transaction handling
│   │   ├── ExternalApis/            ← Stub data providers
│   │   └── Migrations/              ← Database schema
│   │
│   ├── README.md                    ← Backend overview
│   ├── API_DOCUMENTATION.md         ← Endpoint reference
│   ├── PROJECT_COMPLETION.md        ← Implementation notes
│   └── QUICK_REFERENCE.md           ← Code snippets
│
└── stock-screener-ui/               ← Frontend (Next.js)
    ├── app/
    │   ├── layout.tsx               ← Root layout + providers
    │   ├── page.tsx                 ← Dashboard page
    │   ├── globals.css              ← Global styles
    │   ├── opportunities/           ← Opportunities page
    │   ├── news/                    ← News page
    │   └── stocks/[ticker]/         ← Stock detail page
    │
    ├── components/
    │   ├── layout/
    │   │   └── Header.tsx           ← Navigation
    │   └── dashboard/
    │       ├── StockTable.tsx       ← Main table
    │       ├── StockRow.tsx         ← Table row
    │       └── ScorePopup.tsx       ← Hover tooltip
    │
    ├── lib/
    │   ├── api.ts                   ← API client
    │   ├── queryClient.ts           ← React Query setup
    │   └── types.ts                 ← TypeScript types
    │
    ├── public/                      ← Static assets
    ├── README.md                    ← Frontend overview
    ├── UI_ARCHITECTURE.md           ← Design decisions
    ├── package.json                 ← Dependencies
    ├── tsconfig.json                ← TypeScript config
    ├── tailwind.config.ts           ← Tailwind theme
    ├── next.config.js               ← Next.js config
    ├── .env.local                   ← Environment variables
    └── .npmrc                       ← npm config
```

---

## 🎯 Quick Navigation by Role

### 🔨 Backend Developer
1. Start: [StockScreener/README.md](StockScreener/README.md)
2. Reference: [API_DOCUMENTATION.md](StockScreener/API_DOCUMENTATION.md)
3. Key Files:
   - [ScreeningEngine.cs](StockScreener/StockScreener.Application/Services/ScreeningEngine.cs) - Main logic
   - [Program.cs](StockScreener/StockScreener.API/Program.cs) - Configuration
   - [StockScreenerDbContext.cs](StockScreener/StockScreener.Infrastructure/Persistence/StockScreenerDbContext.cs) - Database

### 💻 Frontend Developer
1. Start: [stock-screener-ui/README.md](stock-screener-ui/README.md)
2. Reference: [UI_ARCHITECTURE.md](stock-screener-ui/UI_ARCHITECTURE.md)
3. Key Files:
   - [page.tsx](stock-screener-ui/app/page.tsx) - Dashboard
   - [StockTable.tsx](stock-screener-ui/components/dashboard/StockTable.tsx) - Main component
   - [api.ts](stock-screener-ui/lib/api.ts) - API integration

### 🎨 UI/UX Designer
1. Read: [UI_ARCHITECTURE.md](stock-screener-ui/UI_ARCHITECTURE.md)
2. Design System:
   - Colors: See tailwind.config.ts
   - Components: StockTable, ScorePopup, Header
   - Animations: Framer Motion transitions

### 📊 Analyst/Trader
1. Read: [QUICK_START.md](QUICK_START.md)
2. Run: `./start.sh`
3. Explore: http://localhost:3000
4. Test: http://localhost:5128/swagger

---

## 💡 Common Tasks

### Running the Application
```bash
# Option 1: Automated
./start.sh

# Option 2: Manual - Backend
cd StockScreener
dotnet run --project StockScreener.API/StockScreener.API.csproj

# Option 2: Manual - Frontend (new terminal)
cd stock-screener-ui
npm run dev
```

### Building Components
- Backend Endpoint: See [PROJECT_COMPLETION.md](StockScreener/PROJECT_COMPLETION.md) → "Adding New Endpoints"
- Frontend Page: See [UI_ARCHITECTURE.md](stock-screener-ui/UI_ARCHITECTURE.md) → "Adding New Features"

### Debugging
- Backend: Check [QUICK_START.md](QUICK_START.md) → "Troubleshooting"
- Frontend: Check browser console, React Query DevTools
- Database: Check StockScreener/stockscreener.db file

### Testing
- API: Visit http://localhost:5128/swagger
- Frontend: http://localhost:3000
- Health: http://localhost:5128/health

---

## 🔑 Key Concepts

### Scoring Algorithm
```
Score = (Volatility × 25% + Drawdown × 25% + 
         Extremeness × 25% + Liquidity × 25%)
        if IsQualified() else 0
```
See: [QUICK_REFERENCE.md](StockScreener/QUICK_REFERENCE.md) → "Scoring"

### Component Hierarchy
```
Dashboard (page.tsx)
├── Header (navigation)
└── StockTable (data display)
    └── StockRow (animated row)
        └── ScorePopup (hover tooltip)
```

### API Flow
```
Frontend (React Query)
    ↓ (Axios)
REST API (Minimal API)
    ↓
Application (Services)
    ↓
Domain (Business Logic)
    ↓
Infrastructure (EF Core)
    ↓
SQLite Database
```

---

## 📊 Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| **Backend** | .NET | 10.0 |
| Language | C# | 13 |
| API Framework | ASP.NET Core | 10.0 |
| Database | SQLite | Latest |
| ORM | Entity Framework Core | 10.0.2 |
| **Frontend** | Next.js | 14 |
| Language | TypeScript | Latest |
| Styling | Tailwind CSS | 3.4 |
| State | React Query | 5.35 |
| Animations | Framer Motion | 10.18 |
| HTTP | Axios | 1.7 |
| **Dev Tools** | Package Manager | npm/yarn |
| | Build Tool | Next.js built-in |
| | Linter | ESLint |
| | Type Checker | TypeScript |

---

## ✅ Verification Checklist

After setup, verify everything works:

- [ ] Backend builds: `cd StockScreener && dotnet build`
- [ ] Backend runs: http://localhost:5128/health returns OK
- [ ] API responds: http://localhost:5128/api/screener/top/50 returns JSON
- [ ] Frontend builds: `cd stock-screener-ui && npm run build`
- [ ] Frontend runs: http://localhost:3000 loads dashboard
- [ ] Database exists: StockScreener/stockscreener.db file created
- [ ] Table displays: Dashboard shows top 50 stocks
- [ ] Search works: Type "AAPL" in search box
- [ ] Sort works: Click column headers
- [ ] Hover works: Move mouse over a stock row
- [ ] Animation smooth: No jank or lag

---

## 🎓 Learning Path

1. **Day 1: Overview**
   - Read: [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)
   - Watch: Run `./start.sh` and explore http://localhost:3000

2. **Day 2: Backend**
   - Read: [API_DOCUMENTATION.md](StockScreener/API_DOCUMENTATION.md)
   - Explore: http://localhost:5128/swagger
   - Code: Review [ScreeningEngine.cs](StockScreener/StockScreener.Application/Services/ScreeningEngine.cs)

3. **Day 3: Frontend**
   - Read: [UI_ARCHITECTURE.md](stock-screener-ui/UI_ARCHITECTURE.md)
   - Code: Review [StockTable.tsx](stock-screener-ui/components/dashboard/StockTable.tsx)
   - Modify: Try adding a new column to the table

4. **Day 4: Full Stack**
   - Add new API endpoint
   - Add new component to display it
   - Test the integration

5. **Day 5+: Extend**
   - Implement stock detail page with charts
   - Add missed opportunities page
   - Create news feed page

---

## 🚀 Deployment (Future)

When ready to deploy:

**Backend:**
- Consider: Azure App Service, AWS Lambda, or Docker
- Database: Migrate from SQLite to PostgreSQL/SQL Server
- See: [PROJECT_COMPLETION.md](StockScreener/PROJECT_COMPLETION.md) → "Deployment"

**Frontend:**
- Consider: Vercel (recommended), Netlify, or Docker
- See: [stock-screener-ui/README.md](stock-screener-ui/README.md) → "Deployment"

---

## 📞 Support

**Issues?** See [QUICK_START.md](QUICK_START.md) → "Troubleshooting"

**Questions?** Review relevant documentation file above.

**Want to extend?** Follow patterns in existing code and refer to architecture documents.

---

## 📝 File Legend

| Icon | Meaning |
|------|---------|
| 📚 | Documentation |
| 🔧 | Configuration |
| 📄 | Code files |
| 📁 | Folders |
| ✅ | Complete & tested |
| 🔄 | Placeholder/stub |
| ⏳ | Planned feature |

---

## 🎉 You're All Set!

Everything is ready to go. Choose your starting point above and dive in!

**Next Step:** [QUICK_START.md](QUICK_START.md)
