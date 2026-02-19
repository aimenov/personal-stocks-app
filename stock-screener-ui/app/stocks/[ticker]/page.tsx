'use client'

import { Suspense } from 'react'
import { ArrowLeft } from 'lucide-react'
import Link from 'next/link'
import Header from '@/components/layout/Header'

interface StockDetailPageProps {
  params: { ticker: string }
}

export default function StockDetailPage({ params }: StockDetailPageProps) {
  const ticker = params.ticker.toUpperCase()

  return (
    <div className="min-h-screen bg-slate-950">
      <Header />
      
      <main className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <Link href="/" className="flex items-center gap-2 text-emerald-500 hover:text-emerald-400 mb-8 transition-colors">
          <ArrowLeft className="w-4 h-4" />
          Back to Dashboard
        </Link>

        <div className="rounded-lg bg-slate-900 border border-slate-800 p-8 text-center">
          <h1 className="text-4xl font-bold mb-2">{ticker}</h1>
          <p className="text-slate-400 mb-8">Stock details page - Coming soon</p>
          <p className="text-sm text-slate-500">
            Detailed metrics, charts, and company news will appear here.
          </p>
        </div>
      </main>
    </div>
  )
}
