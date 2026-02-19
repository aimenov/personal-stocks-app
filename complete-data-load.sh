#!/bin/bash

# Complete workflow to reset database, initialize 10 stocks, load data, and run screener

set -e

echo "=========================================="
echo "Stock Screener - Complete Data Load"
echo "=========================================="
echo ""

API_URL="http://localhost:5128"

# Check if API is running
echo "1️⃣  Checking API health..."
if ! curl -s "$API_URL/health" > /dev/null 2>&1; then
    echo "❌ API is not running at $API_URL"
    echo "Please start the API first with:"
    echo "  cd StockScreener && dotnet run --project StockScreener.API/StockScreener.API.csproj"
    exit 1
fi
echo "✅ API is healthy"
echo ""

# Step 1: Reset database
echo "2️⃣  Resetting database..."
RESET_RESPONSE=$(curl -s -X POST "$API_URL/api/sync/database/reset")
CLEARED=$(echo "$RESET_RESPONSE" | jq -r '.cleared | values | length')
echo "✅ Cleared $CLEARED record types from database"
echo ""

# Step 2: Initialize 10 stocks
echo "3️⃣  Initializing 10 quality stocks..."
INIT_RESPONSE=$(curl -s -X POST "$API_URL/api/sync/database/init-stocks")
STOCK_COUNT=$(echo "$INIT_RESPONSE" | jq -r '.stocks | length')
echo "✅ Initialized $STOCK_COUNT stocks:"
echo "$INIT_RESPONSE" | jq -r '.stocks[] | "   • \(.ticker) - \(.companyName)"'
echo ""

# Step 3: Load 1 year of candles
echo "4️⃣  Loading 1 year of market data from MarketStack..."
echo "   (This may take a few minutes due to API rate limits...)"
echo ""

SYNC_RESPONSE=$(curl -s -X POST "$API_URL/api/sync/candles/1y")

TOTAL=$(echo "$SYNC_RESPONSE" | jq -r '.totalTickers')
SUCCESS=$(echo "$SYNC_RESPONSE" | jq -r '.successCount')
FAILED=$(echo "$SYNC_RESPONSE" | jq -r '.failedCount')
TOTAL_CANDLES=$(echo "$SYNC_RESPONSE" | jq -r '.totalCandlesInserted')

echo "   Total: $TOTAL tickers"
echo "   Success: $SUCCESS"
echo "   Failed: $FAILED"
echo "✅ Loaded $TOTAL_CANDLES candles"
echo ""

# Show successful syncs
SUCCESSFUL=$(echo "$SYNC_RESPONSE" | jq -r '.details[] | select(.success == true) | .ticker' | sort)
if [ ! -z "$SUCCESSFUL" ]; then
    echo "   Stocks with data:"
    echo "$SUCCESSFUL" | while read ticker; do
        echo "   ✅ $ticker"
    done
    echo ""
fi

# Show failed syncs
FAILED_TICKERS=$(echo "$SYNC_RESPONSE" | jq -r '.details[] | select(.success == false) | .ticker' | sort)
if [ ! -z "$FAILED_TICKERS" ]; then
    echo "   Stocks without data (rate limited):"
    echo "$FAILED_TICKERS" | while read ticker; do
        echo "   ⚠️  $ticker"
    done
    echo ""
    echo "   Note: Retry later with: curl -X POST $API_URL/api/sync/candles/1y"
    echo ""
fi

# Step 4: Run screener
echo "5️⃣  Running stock screener to generate rankings..."
SCREENER_RESPONSE=$(curl -s -X POST "$API_URL/api/screener/run")
RESULT_COUNT=$(echo "$SCREENER_RESPONSE" | jq 'length')
echo "✅ Generated rankings for $RESULT_COUNT stocks"
echo ""

# Show top 5
echo "   Top 5 stocks:"
echo "$SCREENER_RESPONSE" | jq -r '.[:5] | .[] | "   \(.rank). \(.ticker) - \(.totalScore | round) pts"'
echo ""

echo "=========================================="
echo "✨ Data Load Complete!"
echo "=========================================="
echo ""
echo "📊 Dashboard is ready at: http://localhost:3000"
echo ""
