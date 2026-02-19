# Stock Screener UI

A premium, high-tech stock screener frontend built with Next.js, TypeScript, and Tailwind CSS.

## Features

✨ **Premium Design**
- Dark mode first, minimal aesthetic
- Smooth animations and transitions
- Responsive mobile-first design
- High-tech, modern look

📊 **Core Functionality**
- Dashboard with top 50 ranked stocks
- Real-time search and filtering
- Sortable columns (by score, metrics, etc.)
- Hover popups showing score breakdowns
- Stock details pages with charts (coming soon)
- Missed opportunities tracking (coming soon)
- Market news feed (coming soon)

⚡ **Performance**
- React Query for intelligent caching
- Virtual scrolling ready
- Code splitting by route
- Optimized images (Clearbit logos)
- ~80+ Lighthouse score

## Tech Stack

- **Next.js 14** - React framework with server components
- **TypeScript** - Type-safe development
- **Tailwind CSS** - Utility-first styling
- **React Query** - Server state management
- **Framer Motion** - Smooth animations
- **Recharts** - Data visualization (stock charts)
- **Axios** - HTTP client
- **Lucide React** - Icon library

## Prerequisites

- Node.js 18+ (LTS recommended)
- npm or yarn
- Backend API running on `http://localhost:5128`

## Installation

### 1. Install Dependencies

```bash
cd stock-screener-ui
npm install
```

### 2. Configure Environment

Edit `.env.local`:

```env
# API Configuration
NEXT_PUBLIC_API_URL=http://localhost:5128
```

If your backend is on a different host/port, update this value.

### 3. Start Development Server

```bash
npm run dev
```

The app will be available at `http://localhost:3000`

### 4. Build for Production

```bash
npm run build
npm start
```

## API Integration

The frontend consumes these endpoints from your .NET backend:

### Endpoints Used

- `POST /api/screener/run` - Execute full screening
- `GET /api/screener/top/{count}` - Get top N stocks
- `GET /api/screener/missed-opportunities?daysLookback=30&topN=50` - Historical opportunities

### Required CORS Configuration

Your backend needs CORS enabled. Add this to your .NET `Program.cs`:

```csharp
// Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs", builder =>
    {
        builder
            .WithOrigins("http://localhost:3000", "http://localhost:3001")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// In middleware pipeline, before MapScreenerEndpoints
app.UseCors("AllowNextJs");
```

## Project Structure

```
stock-screener-ui/
├── app/                          # Next.js app directory
│   ├── layout.tsx               # Root layout
│   ├── page.tsx                 # Dashboard (/)
│   ├── globals.css              # Global styles
│   ├── opportunities/           # Opportunities page
│   ├── news/                    # News page
│   └── stocks/
│       └── [ticker]/page.tsx    # Stock details page
│
├── components/
│   ├── layout/
│   │   └── Header.tsx           # Navigation header
│   └── dashboard/
│       ├── StockTable.tsx       # Main table component
│       ├── StockRow.tsx         # Individual row
│       └── ScorePopup.tsx       # Hover popup
│
├── lib/
│   ├── api.ts                   # Axios API client
│   ├── queryClient.ts           # React Query setup
│   └── types.ts                 # TypeScript interfaces
│
├── package.json
├── tsconfig.json
├── tailwind.config.ts
├── postcss.config.js
└── next.config.js
```

## Key Components

### StockTable
- Displays top 50 stocks in an interactive table
- Sortable columns (click header to sort)
- Real-time search (ticker or company name)
- Animated rows with hover effects

### ScorePopup
- Shows detailed score breakdown on hover
- Animated entrance/exit
- Displays metrics explanation
- Positioned above the hovered row

### Dashboard (/)
- Entry point of the application
- Fetches data from `/api/screener/top/50`
- Handles loading and error states
- Responsive grid on mobile

## Styling System

### Colors (Dark Mode)

```
Primary Background:  slate-950 (#0f172a)
Secondary:          slate-900 (#0f1729)
Tertiary:           slate-800 (#1e293b)
Text Primary:       white
Text Secondary:     slate-400
Text Muted:         slate-500

Accents:
- Success:   emerald-500 (green)
- Warning:   amber-500 (orange)
- Danger:    red-500 (red)
- Info:      blue-500 (blue)
```

### Custom Classes

```css
.card              /* Rounded card with border */
.btn               /* Base button styles */
.btn-primary       /* Emerald button */
.btn-secondary     /* Slate button */
.btn-ghost         /* Transparent button */
.text-ticker       /* Mono font, bold ticker */
.text-muted        /* Secondary text color */
```

## Performance Tips

1. **Caching**: React Query caches for 5 minutes by default
2. **Search Debouncing**: Search input is debounced to reduce re-renders
3. **Image Optimization**: Using Next.js Image component for Clearbit logos
4. **Code Splitting**: Each route loads its own code bundle

## Customization

### Change Colors

Edit `tailwind.config.ts`:

```typescript
colors: {
  emerald: { 500: '#22c55e', /* ... */ }
  slate: { 950: '#0f172a', /* ... */ }
}
```

### Adjust Table Columns

Edit `components/dashboard/StockTable.tsx`:

```typescript
// Add/remove columns in the <thead> and <tbody>
<th>Your Custom Column</th>
```

### Add New Routes

Create new directories under `app/`:

```
app/
├── your-page/
│   └── page.tsx
```

## Environment Variables

| Variable | Purpose | Default |
|----------|---------|---------|
| `NEXT_PUBLIC_API_URL` | Backend API base URL | `http://localhost:5128` |

## Troubleshooting

### "Cannot find module" errors

```bash
# Clear cache and reinstall
rm -rf node_modules .next
npm install
npm run dev
```

### CORS errors

Ensure your backend has CORS enabled:

```csharp
app.UseCors("AllowNextJs");  // Must be called BEFORE route mapping
```

### API not connecting

1. Check backend is running: `http://localhost:5128/health`
2. Verify `NEXT_PUBLIC_API_URL` in `.env.local`
3. Check browser console for exact error message

### Slow performance

1. Check Network tab in DevTools
2. Verify React Query is caching (check Network → headers)
3. Use Chrome DevTools Lighthouse to identify bottlenecks

## Development Workflow

### Adding a New Feature

1. Create component in `components/`
2. Import in page
3. Style with Tailwind classes
4. Use `useQuery` for data fetching
5. Test in browser dev tools

### Updating API Integration

1. Edit endpoints in `lib/api.ts`
2. Add types to `lib/types.ts`
3. Update components to use new data

## Deployment

### Vercel (Recommended)

```bash
# Push to GitHub first
git push origin main

# Deploy from Vercel Dashboard
# Set environment variables in Vercel project settings
```

### Docker

```dockerfile
FROM node:18-alpine
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build
EXPOSE 3000
CMD ["npm", "start"]
```

### Build & Run Locally

```bash
npm run build
npm start
# Visit http://localhost:3000
```

## Browser Support

- Chrome/Edge 90+
- Firefox 88+
- Safari 14+
- Mobile browsers (iOS 14+, Android 12+)

## License

Personal project for stock analysis.

## Next Steps

1. ✅ Dashboard implemented
2. 🔄 Stock details page with charts
3. 🔄 Missed opportunities page
4. 🔄 News feed
5. 🔄 User preferences/settings
6. 🔄 Portfolio tracking
7. 🔄 Alert system
8. 🔄 Historical backtesting

---

**Ready to trade!** Start the backend and frontend servers to see the screener in action.
