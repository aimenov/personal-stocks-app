export interface Stock {
  ticker: string
  companyName: string
  rank: number
  totalScore: number
  scores: {
    volatility: number
    drawdown: number
    extreme: number
    liquidity: number
  }
  metrics: {
    fluctuationCount: number
    rangePercent: number
    recentDrawdownPercent: number
    distanceFromHighPercent: number
    distanceFromLowPercent: number
  }
  explanation: string
}

export interface DailyCandle {
  date: string
  open: number
  high: number
  low: number
  close: number
  volume: number
}

export interface ScoreBreakdown {
  volatility: number
  drawdown: number
  extreme: number
  liquidity: number
  total: number
}
