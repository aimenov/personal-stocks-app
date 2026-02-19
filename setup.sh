#!/bin/bash

echo "╔════════════════════════════════════════════════════════════════╗"
echo "║     Stock Screener - Full Stack Setup & Start Guide            ║"
echo "╚════════════════════════════════════════════════════════════════╝"
echo ""

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}STEP 1: Backend Setup${NC}"
echo "────────────────────────────────────────────────────────────────"
echo "Building .NET backend with CORS enabled..."
echo ""

cd /workspaces/dotnet-codespaces/StockScreener

if dotnet build; then
    echo -e "${GREEN}✓ Backend built successfully${NC}"
else
    echo -e "${RED}✗ Backend build failed${NC}"
    exit 1
fi

echo ""
echo -e "${BLUE}STEP 2: Frontend Dependencies${NC}"
echo "────────────────────────────────────────────────────────────────"
echo "Installing Node.js packages..."
echo ""

cd /workspaces/dotnet-codespaces/stock-screener-ui

if npm install --prefer-offline --no-audit; then
    echo -e "${GREEN}✓ Dependencies installed${NC}"
else
    echo -e "${RED}✗ npm install failed${NC}"
    exit 1
fi

echo ""
echo -e "${BLUE}STEP 3: Ready to Launch${NC}"
echo "────────────────────────────────────────────────────────────────"
echo ""
echo -e "${GREEN}Setup Complete!${NC}"
echo ""
echo "To start the application, open TWO terminals:"
echo ""
echo -e "${YELLOW}Terminal 1 - Backend API (port 5128):${NC}"
echo "  cd /workspaces/dotnet-codespaces/StockScreener"
echo "  dotnet run --project StockScreener.API/StockScreener.API.csproj"
echo ""
echo -e "${YELLOW}Terminal 2 - Frontend UI (port 3000):${NC}"
echo "  cd /workspaces/dotnet-codespaces/stock-screener-ui"
echo "  npm run dev"
echo ""
echo -e "${GREEN}Then visit: http://localhost:3000${NC}"
echo ""
echo "CORS Configuration: ✓ Enabled for localhost:3000 and localhost:3001"
echo "Database: ✓ Auto-migrates on backend startup"
echo "Test Data: ✓ Generates realistic stub data"
echo ""
echo "Dashboard will display top 50 ranked stocks with:"
echo "  • Sortable columns (rank, score, metrics)"
echo "  • Real-time search and filtering"
echo "  • Smooth hover animations"
echo "  • Score breakdown tooltips"
echo ""
echo -e "${BLUE}API Endpoints:${NC}"
echo "  • POST   /api/screener/run                    - Run screening"
echo "  • GET    /api/screener/top/{count}            - Top ranked stocks"
echo "  • GET    /api/screener/missed-opportunities   - Historical opps"
echo "  • GET    /health                              - Health check"
echo "  • GET    /swagger                             - API docs"
echo ""
