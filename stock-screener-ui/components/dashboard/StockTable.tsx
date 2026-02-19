'use client'

import { useState, useMemo } from 'react'
import { Stock } from '@/lib/types'
import StockRow from './StockRow'
import { ChevronUp, ChevronDown } from 'lucide-react'

type SortKey = 'rank' | 'totalScore' | 'volatility' | 'drawdown' | 'liquidity'
type SortDir = 'asc' | 'desc'

interface StockTableProps {
  stocks: Stock[]
}

export default function StockTable({ stocks }: StockTableProps) {
  const [sortKey, setSortKey] = useState<SortKey>('rank')
  const [sortDir, setSortDir] = useState<SortDir>('asc')
  const [search, setSearch] = useState('')

  const sortedStocks = useMemo(() => {
    let filtered = stocks.filter((stock) =>
      stock.ticker.toLowerCase().includes(search.toLowerCase()) ||
      stock.companyName.toLowerCase().includes(search.toLowerCase())
    )

    return filtered.sort((a, b) => {
      let aVal: number = 0
      let bVal: number = 0

      switch (sortKey) {
        case 'rank':
          aVal = a.rank
          bVal = b.rank
          break
        case 'totalScore':
          aVal = a.totalScore
          bVal = b.totalScore
          break
        case 'volatility':
          aVal = a.scores.volatility
          bVal = b.scores.volatility
          break
        case 'drawdown':
          aVal = a.scores.drawdown
          bVal = b.scores.drawdown
          break
        case 'liquidity':
          aVal = a.scores.liquidity
          bVal = b.scores.liquidity
          break
      }

      return sortDir === 'asc' ? aVal - bVal : bVal - aVal
    })
  }, [stocks, search, sortKey, sortDir])

  const handleSort = (key: SortKey) => {
    if (sortKey === key) {
      setSortDir(sortDir === 'asc' ? 'desc' : 'asc')
    } else {
      setSortKey(key)
      setSortDir('asc')
    }
  }

  const SortIcon = ({ active, direction }: { active: boolean; direction: SortDir }) => {
    if (!active) return <span className="text-slate-600 ml-1">⇅</span>
    return direction === 'asc' ? (
      <ChevronUp className="w-4 h-4 ml-1" />
    ) : (
      <ChevronDown className="w-4 h-4 ml-1" />
    )
  }

  return (
    <div className="space-y-4">
      {/* Search Bar */}
      <div className="flex gap-4">
        <input
          type="text"
          placeholder="Search ticker or company name..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="flex-1 px-4 py-3 rounded-lg bg-slate-900 border border-slate-800 text-white placeholder-slate-500 focus:outline-none focus:border-emerald-600 focus:ring-1 focus:ring-emerald-600"
        />
      </div>

      {/* Table */}
      <div className="rounded-lg border border-slate-800 bg-slate-900 overflow-hidden">
        <div className="overflow-x-auto">
        <table className="w-full">
          <thead>
            <tr className="border-b border-slate-800 bg-slate-800/50">
              <th className="px-6 py-4 text-left text-sm font-semibold text-slate-300">
                <button
                  onClick={() => handleSort('rank')}
                  className="flex items-center hover:text-white transition-colors"
                >
                  Rank
                  <SortIcon active={sortKey === 'rank'} direction={sortDir} />
                </button>
              </th>
              <th className="px-6 py-4 text-left text-sm font-semibold text-slate-300">Stock</th>
              <th className="px-6 py-4 text-left text-sm font-semibold text-slate-300">
                <button
                  onClick={() => handleSort('totalScore')}
                  className="flex items-center hover:text-white transition-colors"
                >
                  Score
                  <SortIcon active={sortKey === 'totalScore'} direction={sortDir} />
                </button>
              </th>
              <th className="px-6 py-4 text-left text-sm font-semibold text-slate-300">
                <button
                  onClick={() => handleSort('volatility')}
                  className="flex items-center hover:text-white transition-colors"
                >
                  Vol
                  <SortIcon active={sortKey === 'volatility'} direction={sortDir} />
                </button>
              </th>
              <th className="px-6 py-4 text-left text-sm font-semibold text-slate-300">
                <button
                  onClick={() => handleSort('drawdown')}
                  className="flex items-center hover:text-white transition-colors"
                >
                  Drawdown
                  <SortIcon active={sortKey === 'drawdown'} direction={sortDir} />
                </button>
              </th>
              <th className="px-6 py-4 text-left text-sm font-semibold text-slate-300">Range</th>
              <th className="px-6 py-4 text-left text-sm font-semibold text-slate-300">
                <button
                  onClick={() => handleSort('liquidity')}
                  className="flex items-center hover:text-white transition-colors"
                >
                  Liq
                  <SortIcon active={sortKey === 'liquidity'} direction={sortDir} />
                </button>
              </th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-800">
            {sortedStocks.map((stock) => (
              <StockRow key={stock.ticker} stock={stock} />
            ))}
          </tbody>
        </table>
        </div>
      </div>

      {sortedStocks.length === 0 && (
        <div className="text-center py-8">
          <p className="text-slate-400">No stocks match your search</p>
        </div>
      )}
    </div>
  )
}
