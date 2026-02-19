'use client'

import Link from 'next/link'
import { TrendingUp } from 'lucide-react'

export default function Header() {
  return (
    <header className="sticky top-0 z-50 border-b border-slate-800 bg-slate-950/95 backdrop-blur-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
        <div className="flex items-center justify-between">
          <Link href="/" className="flex items-center gap-2 hover:opacity-80 transition-opacity">
            <TrendingUp className="w-8 h-8 text-emerald-500" />
            <span className="text-xl font-bold">StockScreen</span>
          </Link>

          <nav className="hidden md:flex items-center gap-8">
            <Link href="/" className="text-sm font-medium hover:text-emerald-500 transition-colors">
              Dashboard
            </Link>
            <Link href="/opportunities" className="text-sm font-medium text-slate-400 hover:text-white transition-colors">
              Opportunities
            </Link>
            <Link href="/news" className="text-sm font-medium text-slate-400 hover:text-white transition-colors">
              News
            </Link>
          </nav>

          <div className="flex items-center gap-2">
            <span className="text-xs px-3 py-1 rounded-full bg-emerald-900/30 text-emerald-300 font-mono">
              Live
            </span>
          </div>
        </div>
      </div>
    </header>
  )
}
