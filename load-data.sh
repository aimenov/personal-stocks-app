#!/bin/bash

# Script to clean all stock data and reload 10 stocks with 1 year of data from MarketStack

set -e

echo "=========================================="
echo "Stock Screener Data Reload Script"
echo "=========================================="
echo ""

API_URL="http://localhost:5128"
DB_FILE="/workspaces/dotnet-codespaces/StockScreener/stockscreener.db"

# Check if API is running
echo "1️⃣  Checking API health..."
if ! curl -s "$API_URL/health" > /dev/null 2>&1; then
    echo "❌ API is not running at $API_URL"
    echo "Please start the API first with: cd StockScreener && dotnet run --project StockScreener.API/StockScreener.API.csproj"
    exit 1
fi
echo "✅ API is healthy"
echo ""

# Delete the database file completely to start fresh
echo "2️⃣  Deleting database to start fresh..."
if [ -f "$DB_FILE" ]; then
    rm "$DB_FILE"
    echo "✅ Old database deleted"
fi
echo ""

# Restart API to recreate the database
echo "3️⃣  Restarting API to recreate database schema..."
pkill -f "dotnet run.*StockScreener.API" 2>/dev/null || true
sleep 3
cd /workspaces/dotnet-codespaces/StockScreener && dotnet run --project StockScreener.API/StockScreener.API.csproj > /tmp/api.log 2>&1 &
sleep 10

# Verify API is back up
if ! curl -s "$API_URL/health" > /dev/null 2>&1; then
    echo "❌ API failed to restart"
    exit 1
fi
echo "✅ API restarted with fresh database"
echo ""

# Load 1 year of data for all 10 stocks
echo "4️⃣  Loading 1 year of daily candles for all 10 stocks..."
echo "   Stocks: AAPL, MSFT, GOOGL, NVDA, META, TSLA, AMZN, BRK.B, JNJ, V"
echo ""
echo "⏳ This may take 5-10 minutes depending on API rate limits..."
echo ""

SYNC_RESPONSE=$(curl -s -X POST "$API_URL/api/sync/candles/1y")

echo "$SYNC_RESPONSE" | jq .

TOTAL=$(echo "$SYNC_RESPONSE" | jq -r '.totalTickers // 0')
SUCCESS=$(echo "$SYNC_RESPONSE" | jq -r '.successCount // 0')
FAILED=$(echo "$SYNC_RESPONSE" | jq -r '.failedCount // 0')
TOTAL_CANDLES=$(echo "$SYNC_RESPONSE" | jq -r '.totalCandlesInserted // 0')

echo ""
echo "=========================================="
echo "✅ Data Reload Complete!"
echo "=========================================="
echo "Total Stocks: $TOTAL"
echo "Successfully Synced: $SUCCESS"
echo "Failed: $FAILED"
echo "Total Candles Loaded: $TOTAL_CANDLES"
echo ""

if [ "$FAILED" -gt 0 ]; then
    echo "⚠️  Some stocks failed to sync. Check details above."
    echo "This may be due to API rate limits. Try again in a few minutes."
fi

echo "✨ Ready to use! Visit http://localhost:3000 to see the stock screener."
