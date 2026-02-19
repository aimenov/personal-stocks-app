'use client'

import { useQuery } from '@tanstack/react-query'
import { screenerAPI } from '@/lib/api'
import { Stock } from '@/lib/types'
import Header from '@/components/layout/Header'
import StockTable from '@/components/dashboard/StockTable'
import { Loader } from 'lucide-react'

export default function Dashboard() {
  const {
    data: stocks = [],
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ['topStocks'],
    queryFn: async () => {
      const res = await screenerAPI.getTopStocks(50)
      return res.data as Stock[]
    },
  })

  return (
    <div className="min-h-screen bg-slate-950">
      <Header />
      
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="mb-8">
          <h1 className="text-4xl font-bold mb-2">Stock Screener</h1>
          <p className="text-slate-400">
            Top {stocks.length} ranked opportunities based on volatility, drawdown, and liquidity
          </p>
        </div>

        {isLoading ? (
          <div className="flex items-center justify-center py-16">
            <div className="flex flex-col items-center gap-4">
              <Loader className="w-8 h-8 animate-spin text-emerald-500" />
              <p className="text-slate-400">Loading stocks...</p>
            </div>
          </div>
        ) : error ? (
          <div className="rounded-lg bg-red-900/20 border border-red-800 p-6 text-red-300">
            <p className="font-semibold mb-2">Failed to load stocks</p>
            <p className="text-sm mb-4">
              {error instanceof Error ? error.message : 'Unknown error'}
            </p>
            <button
              onClick={() => refetch()}
              className="btn btn-primary"
            >
              Try Again
            </button>
          </div>
        ) : stocks.length > 0 ? (
          <StockTable stocks={stocks} />
        ) : (
          <div className="rounded-lg bg-slate-900 border border-slate-800 p-12 text-center">
            <p className="text-slate-400 mb-4">No stocks found</p>
            <button
              onClick={() => refetch()}
              className="btn btn-primary"
            >
              Refresh
            </button>
          </div>
        )}
      </main>
    </div>
  )
}
