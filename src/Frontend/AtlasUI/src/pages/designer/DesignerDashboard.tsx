import React, { useState } from 'react'
import { useQuery, useMutation } from '@tanstack/react-query'
import { motion } from 'framer-motion'
import {
  Palette, Image, Layers, Contrast,
  TrendingUp, Eye
} from 'lucide-react'
import AppShell from '@/components/layout/AppShell'
import { StatCard, StatsGrid } from '@/components/dashboard/StatCard'
import FocusTimerWidget from '@/components/dashboard/FocusTimerWidget'
import FigmaIntegrations from '@/components/designer/FigmaIntegrations'
import AssetExporter from '@/components/designer/AssetExporter'
import ColorTools from '@/components/designer/ColorTools'
import { designinsights, designutilities, palettes as palettesApi } from '@/lib/apiClient'
import { queryKeys } from '@/lib/queryKeys'
import { toast } from 'sonner'
import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer, Legend } from 'recharts'

const COLOR_PIE = [
  { name: '#3B82F6', value: 35 },
  { name: '#8B5CF6', value: 25 },
  { name: '#10B981', value: 20 },
  { name: '#F59E0B', value: 12 },
  { name: '#EF4444', value: 8 },
]

export default function DesignerDashboard(): React.ReactElement {
  const [contrastFg, setContrastFg] = useState('#FFFFFF')
  const [contrastBg, setContrastBg] = useState('#3B82F6')
  const [contrastResult, setContrastResult] = useState<any>(null)

  const { data: assetsData, isLoading: assetsLoading } = useQuery({
    queryKey: queryKeys.designinsights.assetsOptimized(),
    queryFn: () => designinsights.assetsOptimized(),
    staleTime: 300000,
    retry: false,
  })

  const { data: colorTrends, isLoading: colorLoading } = useQuery({
    queryKey: queryKeys.designinsights.colorTrends(),
    queryFn: () => designinsights.colorTrends(),
    staleTime: 300000,
    retry: false,
  })

  const { data: designDebt, isLoading: debtLoading } = useQuery({
    queryKey: queryKeys.designinsights.designDebt(),
    queryFn: () => designinsights.designDebt(),
    staleTime: 300000,
    retry: false,
  })

  const { data: paletteList } = useQuery({
    queryKey: queryKeys.palettes.list(),
    queryFn: () => palettesApi.list(),
    staleTime: 60000,
    retry: false,
  })

  const contrastMutation = useMutation({
    mutationFn: ({ fg, bg }: { fg: string; bg: string }) =>
      designutilities.checkContrast(fg, bg),
    onSuccess: (data) => setContrastResult(data),
    onError: () => toast.error('Could not check contrast'),
  })

  return (
    <AppShell>
      <div className="space-y-6">
        {/* Header */}
        <motion.div
          initial={{ opacity: 0, y: -10 }}
          animate={{ opacity: 1, y: 0 }}
          className="flex items-center justify-between"
        >
          <div>
            <h1 className="text-2xl font-bold text-foreground flex items-center gap-2">
              <Palette className="w-6 h-6 text-purple-500" />
              Designer Dashboard
            </h1>
            <p className="text-sm text-muted-foreground mt-0.5">
              Design. Create. Handoff. — Visual tools at your fingertips
            </p>
          </div>
        </motion.div>

        {/* Stats */}
        <StatsGrid>
          <StatCard
            title="Assets Optimized"
            value={assetsData?.count ?? assetsData?.Count ?? '—'}
            subtitle="this month"
            icon={<Image className="w-5 h-5" />}
            trend={{ value: 8 }}
            gradient="from-purple-600 to-pink-500"
            loading={assetsLoading}
          />
          <StatCard
            title="Color Trends"
            value={colorTrends?.topColor ?? colorTrends?.TopColor ?? '#3B82F6'}
            subtitle="most used this week"
            icon={<Palette className="w-5 h-5" />}
            gradient="from-pink-600 to-rose-500"
            loading={colorLoading}
          />
          <StatCard
            title="Design Debt"
            value={designDebt?.count ?? designDebt?.Count ?? '—'}
            subtitle="outdated components"
            icon={<Layers className="w-5 h-5" />}
            trend={{ value: -5, label: 'improved' }}
            gradient="from-orange-600 to-amber-500"
            loading={debtLoading}
          />
          <StatCard
            title="Palettes"
            value={paletteList?.length ?? '—'}
            subtitle="saved palettes"
            icon={<Eye className="w-5 h-5" />}
            gradient="from-cyan-600 to-blue-500"
          />
        </StatsGrid>

        {/* Main Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-5">
          {/* Left Column */}
          <div className="space-y-5">
            <FigmaIntegrations />
            <FocusTimerWidget />
          </div>

          {/* Middle Column */}
          <div className="space-y-5">
            <AssetExporter />

            {/* Color Usage Chart */}
            <div className="rounded-2xl border border-border bg-card p-5">
              <div className="flex items-center gap-2 mb-4">
                <TrendingUp className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">Color Usage Trends</h3>
              </div>
              <ResponsiveContainer width="100%" height={180}>
                <PieChart>
                  <Pie
                    data={COLOR_PIE}
                    cx="50%"
                    cy="50%"
                    innerRadius={50}
                    outerRadius={75}
                    paddingAngle={3}
                    dataKey="value"
                  >
                    {COLOR_PIE.map((entry, idx) => (
                      <Cell key={idx} fill={entry.name} />
                    ))}
                  </Pie>
                  <Tooltip
                    formatter={(v, n) => [`${v}%`, n]}
                    contentStyle={{
                      background: 'hsl(var(--popover))',
                      border: '1px solid hsl(var(--border))',
                      borderRadius: '8px',
                      fontSize: 11,
                    }}
                  />
                  <Legend
                    iconSize={8}
                    formatter={(value) => (
                      <span style={{ color: 'hsl(var(--muted-foreground))', fontSize: 10 }}>{value}</span>
                    )}
                  />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </div>

          {/* Right Column */}
          <div className="space-y-5">
            <ColorTools />

            {/* Contrast Checker */}
            <div className="rounded-2xl border border-border bg-card p-5 space-y-3">
              <div className="flex items-center gap-2">
                <Contrast className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">Contrast Checker</h3>
              </div>
              <div className="flex gap-2">
                <div className="flex-1">
                  <label className="text-[10px] text-muted-foreground mb-1 block">Foreground</label>
                  <div className="flex gap-1.5 items-center">
                    <input type="color" value={contrastFg} onChange={(e) => setContrastFg(e.target.value)}
                      className="w-8 h-8 rounded cursor-pointer border border-border" />
                    <input value={contrastFg} onChange={(e) => setContrastFg(e.target.value)}
                      className="flex-1 h-8 text-xs rounded-lg bg-muted border border-border px-2 focus:outline-none" />
                  </div>
                </div>
                <div className="flex-1">
                  <label className="text-[10px] text-muted-foreground mb-1 block">Background</label>
                  <div className="flex gap-1.5 items-center">
                    <input type="color" value={contrastBg} onChange={(e) => setContrastBg(e.target.value)}
                      className="w-8 h-8 rounded cursor-pointer border border-border" />
                    <input value={contrastBg} onChange={(e) => setContrastBg(e.target.value)}
                      className="flex-1 h-8 text-xs rounded-lg bg-muted border border-border px-2 focus:outline-none" />
                  </div>
                </div>
              </div>

              {/* Preview */}
              <div
                className="h-12 rounded-xl flex items-center justify-center text-sm font-medium transition-colors"
                style={{ backgroundColor: contrastBg, color: contrastFg }}
              >
                Preview Text
              </div>

              <motion.button
                whileTap={{ scale: 0.95 }}
                onClick={() => contrastMutation.mutate({ fg: contrastFg, bg: contrastBg })}
                className="w-full h-8 rounded-lg bg-primary text-primary-foreground text-xs font-medium hover:bg-primary/90"
              >
                {contrastMutation.isPending ? 'Checking...' : 'Check WCAG Contrast'}
              </motion.button>

              {contrastResult && (
                <div className="space-y-1 text-xs">
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Ratio:</span>
                    <span className="font-bold text-foreground">{contrastResult.ratio}:1</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">WCAG AA:</span>
                    <span className={contrastResult.wcagAA ? 'text-emerald-500' : 'text-red-500'}>
                      {contrastResult.wcagAA ? '✓ Pass' : '✗ Fail'}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">WCAG AAA:</span>
                    <span className={contrastResult.wcagAAA ? 'text-emerald-500' : 'text-red-500'}>
                      {contrastResult.wcagAAA ? '✓ Pass' : '✗ Fail'}
                    </span>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </AppShell>
  )
}
