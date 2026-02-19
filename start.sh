#!/bin/bash

# Stock Screener - Complete Startup Script
# Usage: ./start.sh

echo ""
echo "╔════════════════════════════════════════════════════════════════════╗"
echo "║           Stock Screener - Starting Both Servers                   ║"
echo "╚════════════════════════════════════════════════════════════════════╝"
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Check if we can start both servers
echo -e "${BLUE}Checking prerequisites...${NC}"
echo ""

# Check Node.js
if ! command -v node &> /dev/null; then
    echo -e "${RED}✗ Node.js not found${NC}"
    echo "Install from: https://nodejs.org/"
    exit 1
fi
NODE_VERSION=$(node -v)
echo -e "${GREEN}✓ Node.js ${NODE_VERSION}${NC}"

# Check npm
if ! command -v npm &> /dev/null; then
    echo -e "${RED}✗ npm not found${NC}"
    exit 1
fi
NPM_VERSION=$(npm -v)
echo -e "${GREEN}✓ npm ${NPM_VERSION}${NC}"

# Check .NET
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}✗ .NET SDK not found${NC}"
    echo "Install from: https://dotnet.microsoft.com/download"
    exit 1
fi
DOTNET_VERSION=$(dotnet --version)
echo -e "${GREEN}✓ .NET ${DOTNET_VERSION}${NC}"

echo ""
echo -e "${BLUE}Starting Backend API Server...${NC}"
echo "─────────────────────────────────────────────────────────────────────"
echo ""

# Start backend in background
cd /workspaces/dotnet-codespaces/StockScreener
dotnet run --project StockScreener.API/StockScreener.API.csproj &
BACKEND_PID=$!

# Wait for backend to start
echo -e "${YELLOW}Waiting for backend to initialize...${NC}"
sleep 3

# Check if backend is running
if ! kill -0 $BACKEND_PID 2>/dev/null; then
    echo -e "${RED}✗ Backend failed to start${NC}"
    exit 1
fi

echo -e "${GREEN}✓ Backend running (PID: $BACKEND_PID)${NC}"
echo -e "${GREEN}  URL: http://localhost:5128${NC}"
echo ""

echo -e "${BLUE}Starting Frontend UI Server...${NC}"
echo "─────────────────────────────────────────────────────────────────────"
echo ""

# Start frontend
cd /workspaces/dotnet-codespaces/stock-screener-ui
npm run dev &
FRONTEND_PID=$!

# Wait for frontend to start
sleep 2

echo -e "${GREEN}✓ Frontend running (PID: $FRONTEND_PID)${NC}"
echo -e "${GREEN}  URL: http://localhost:3000${NC}"
echo ""

echo "╔════════════════════════════════════════════════════════════════════╗"
echo -e "${GREEN}║                   ✓ All Systems Ready!                         ║${NC}"
echo "╚════════════════════════════════════════════════════════════════════╝"
echo ""
echo -e "${GREEN}Backend API:${NC}      http://localhost:5128"
echo -e "${GREEN}Dashboard UI:${NC}      http://localhost:3000"
echo -e "${GREEN}API Docs:${NC}          http://localhost:5128/swagger"
echo -e "${GREEN}Health Check:${NC}      http://localhost:5128/health"
echo ""
echo "To stop both servers: Press Ctrl+C"
echo ""

# Wait for both processes
wait
