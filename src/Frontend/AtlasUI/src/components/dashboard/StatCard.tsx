import { ReactNode } from 'react'
import { motion } from 'framer-motion'
import { TrendingUp, TrendingDown, Minus } from 'lucide-react'

interface StatCardProps {
  title: string
  value: string | number
  subtitle?: string
  icon: ReactNode
  trend?: { value: number; label?: string }
  gradient?: string
  loading?: boolean
}

export function StatCard({ title, value, subtitle, icon, trend, gradient, loading }: StatCardProps) {
  const trendColor = trend
    ? trend.value > 0
      ? 'text-emerald-500'
      : trend.value < 0
      ? 'text-red-500'
      : 'text-muted-foreground'
    : ''
  const TrendIcon = trend?.value === 0 ? Minus : trend && trend.value > 0 ? TrendingUp : TrendingDown

  return (
    <motion.div
      whileHover={{ y: -2, boxShadow: '0 8px 30px -6px rgba(0,0,0,0.15)' }}
      className="relative overflow-hidden rounded-2xl border border-border bg-card p-5 transition-all"
    >
      {gradient && (
        <div className={`absolute inset-0 bg-gradient-to-br ${gradient} opacity-5 pointer-events-none`} />
      )}
      <div className="flex items-start justify-between mb-3">
        <div className="w-10 h-10 rounded-xl bg-muted flex items-center justify-center text-primary">
          {icon}
        </div>
        {trend !== undefined && !loading && (
          <div className={`flex items-center gap-1 text-xs font-medium ${trendColor}`}>
            <TrendIcon className="w-3 h-3" />
            {Math.abs(trend.value)}%
          </div>
        )}
      </div>
      {loading ? (
        <div className="space-y-2">
          <div className="h-7 w-24 rounded bg-muted animate-pulse" />
          <div className="h-3 w-32 rounded bg-muted animate-pulse" />
        </div>
      ) : (
        <>
          <p className="text-2xl font-bold text-foreground leading-none mb-1">{value}</p>
          <p className="text-xs font-medium text-foreground/80">{title}</p>
          {subtitle && <p className="text-[11px] text-muted-foreground mt-0.5">{subtitle}</p>}
          {trend?.label && <p className="text-[10px] text-muted-foreground mt-1">{trend.label}</p>}
        </>
      )}
    </motion.div>
  )
}

interface StatsGridProps {
  children: ReactNode
}

export function StatsGrid({ children }: StatsGridProps) {
  return (
    <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
      {children}
    </div>
  )
}

