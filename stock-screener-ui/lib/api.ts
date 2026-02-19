import axios from 'axios'

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5128'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

export const screenerAPI = {
  // Get top N stocks
  getTopStocks: (count: number) =>
    api.get(`/api/screener/top/${count}`),

  // Run full screening
  runScreening: () =>
    api.post('/api/screener/run', {}),

  // Get missed opportunities
  getMissedOpportunities: (daysLookback = 30, topN = 50) =>
    api.get('/api/screener/missed-opportunities', {
      params: { daysLookback, topN },
    }),
}

export default api
