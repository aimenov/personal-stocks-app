'use client'

import { useState } from 'react'
import Link from 'next/link'
import { Stock } from '@/lib/types'
import ScorePopup from './ScorePopup'
import { motion } from 'framer-motion'

interface StockRowProps {
  stock: Stock
}

export default function StockRow({ stock }: StockRowProps) {
  const [showPopup, setShowPopup] = useState(false)

  const getScoreColor = (score: number) => {
    if (score >= 80) return 'text-emerald-400'
    if (score >= 70) return 'text-emerald-300'
    if (score >= 60) return 'text-amber-300'
    return 'text-slate-300'
  }

  const getScoreBg = (score: number) => {
    if (score >= 80) return 'bg-emerald-900/20'
    if (score >= 70) return 'bg-emerald-900/10'
    if (score >= 60) return 'bg-amber-900/10'
    return 'bg-slate-800/20'
  }

  return (
    <motion.tr
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
      className="hover:bg-slate-800/50 transition-colors cursor-pointer"
      onMouseEnter={() => setShowPopup(true)}
      onMouseLeave={() => setShowPopup(false)}
    >
      <td className="px-6 py-4">
        <span className="text-sm font-bold text-emerald-500">#{stock.rank}</span>
      </td>

      <td className="px-6 py-4">
        <Link href={`/stocks/${stock.ticker}`} className="hover:opacity-80 transition-opacity">
          <div>
            <div className="text-ticker text-white hover:text-emerald-400">{stock.ticker}</div>
            <div className="text-sm text-slate-400">{stock.companyName}</div>
          </div>
        </Link>
      </td>

      <td className="px-6 py-4 relative">
        <div className="flex items-center gap-2">
          <span className={`font-bold text-lg ${getScoreColor(stock.totalScore)}`}>
            {stock.totalScore.toFixed(1)}
          </span>
          <span className={`px-2 py-1 rounded text-xs font-semibold ${getScoreBg(stock.totalScore)} text-slate-300`}>
            /100
          </span>
        </div>
        {showPopup && <ScorePopup stock={stock} />}
      </td>

      <td className="px-6 py-4">
        <span className="text-sm font-mono text-blue-400">{stock.scores.volatility.toFixed(0)}</span>
      </td>

      <td className="px-6 py-4">
        <span className="text-sm font-mono text-amber-400">{stock.scores.drawdown.toFixed(0)}</span>
      </td>

      <td className="px-6 py-4">
        <span className="text-sm font-mono text-slate-400">{stock.metrics.rangePercent.toFixed(1)}%</span>
      </td>

      <td className="px-6 py-4">
        <span className={`text-sm font-mono ${stock.scores.liquidity >= 80 ? 'text-emerald-400' : 'text-amber-400'}`}>
          {stock.scores.liquidity.toFixed(0)}
        </span>
      </td>
    </motion.tr>
  )
}
