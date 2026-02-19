'use client'

import { Stock } from '@/lib/types'
import { motion } from 'framer-motion'

interface ScorePopupProps {
  stock: Stock
}

export default function ScorePopup({ stock }: ScorePopupProps) {
  const scoreComponents = [
    { label: 'Volatility', value: stock.scores.volatility, barColor: 'bg-blue-600' },
    { label: 'Drawdown', value: stock.scores.drawdown, barColor: 'bg-amber-600' },
    { label: 'Extreme', value: stock.scores.extreme, barColor: 'bg-violet-600' },
    { label: 'Liquidity', value: stock.scores.liquidity, barColor: 'bg-emerald-600' },
  ]

  return (
    <motion.div
      initial={{ opacity: 0, scale: 0.95 }}
      animate={{ opacity: 1, scale: 1 }}
      exit={{ opacity: 0, scale: 0.95 }}
      transition={{ duration: 0.15 }}
      className="absolute left-0 top-full mt-2 w-64 bg-slate-900 border border-slate-700 rounded-lg shadow-2xl p-4 z-50"
    >
      <h3 className="text-sm font-semibold text-white mb-3">Score Breakdown</h3>

      <div className="space-y-2 mb-4">
        {scoreComponents.map((comp) => (
          <div key={comp.label} className="flex justify-between items-center">
            <span className="text-xs text-slate-400">{comp.label}</span>
            <div className="flex items-center gap-2">
              <div className="w-12 h-2 bg-slate-800 rounded-full overflow-hidden">
                <div
                  className={`h-full ${comp.barColor}`}
                  style={{ width: `${(comp.value / 100) * 100}%` }}
                />
              </div>
              <span className="text-xs font-bold w-8 text-right text-slate-200">
                {comp.value.toFixed(0)}
              </span>
            </div>
          </div>
        ))}
      </div>

      <div className="border-t border-slate-700 pt-3">
        <p className="text-xs text-slate-300 leading-relaxed line-clamp-3">
          {typeof stock.explanation === 'string' ? stock.explanation : 'No explanation available'}
        </p>
      </div>
    </motion.div>
  )
}
