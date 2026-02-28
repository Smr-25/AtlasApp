import { useState } from 'react'
import { useQuery, useMutation } from '@tanstack/react-query'
import { motion } from 'framer-motion'
import {
  Shield, AlertTriangle, CheckCircle2, Lock,
  Scan, Globe, Activity, Key
} from 'lucide-react'
import AppShell from '@/components/layout/AppShell'
import { StatCard, StatsGrid } from '@/components/dashboard/StatCard'
import FocusTimerWidget from '@/components/dashboard/FocusTimerWidget'
import CloudflareControl from '@/components/secops/CloudflareControl'
import QuickScan from '@/components/secops/QuickScan'
import HashGen from '@/components/secops/HashGen'
import {
  secopsinsights, secopsutilities, secopsagents, secopsscripts
} from '@/lib/apiClient'
import { queryKeys } from '@/lib/queryKeys'
import { toast } from 'sonner'
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid,
  Tooltip, ResponsiveContainer
} from 'recharts'

const THREAT_DATA = Array.from({ length: 14 }, (_, i) => ({
  day: i + 1,
  threats: Math.floor(Math.random() * 50 + 5),
  blocked: Math.floor(Math.random() * 45 + 5),
}))

export default function SecOpsDashboard() {
  const [sslDomain, setSslDomain] = useState('')
  const [sslResult, setSslResult] = useState<any>(null)
  const [ipTarget, setIpTarget] = useState('')
  const [ipResult, setIpResult] = useState<any>(null)

  const { data: securityScore, isLoading: scoreLoading } = useQuery({
    queryKey: queryKeys.secopsinsights.securityScore(),
    queryFn: () => secopsinsights.securityScore(),
    staleTime: 300000,
    retry: false,
  })

  const { data: streak, isLoading: streakLoading } = useQuery({
    queryKey: queryKeys.secopsinsights.zeroIncidentStreak(),
    queryFn: () => secopsinsights.zeroIncidentStreak(),
    staleTime: 300000,
    retry: false,
  })

  const { data: threatsBlocked, isLoading: threatsLoading } = useQuery({
    queryKey: queryKeys.secopsinsights.threatsBlocked(),
    queryFn: () => secopsinsights.threatsBlocked(),
    staleTime: 300000,
    retry: false,
  })

  const { data: vpnStatus } = useQuery({
    queryKey: ['secops', 'vpn-status'],
    queryFn: () => secopsagents.vpnStatus(),
    staleTime: 30000,
    retry: false,
  })

  const sslMutation = useMutation({
    mutationFn: (domain: string) => secopsutilities.sslCheck(domain),
    onSuccess: (data) => setSslResult(data),
    onError: () => toast.error('SSL check failed'),
  })

  const ipDnsMutation = useMutation({
    mutationFn: (target: string) => secopsutilities.ipDns(target),
    onSuccess: (data) => setIpResult(data),
    onError: () => toast.error('Lookup failed'),
  })

  const panicMutation = useMutation({
    mutationFn: (reason: string) => secopsscripts.panicButton(reason),
    onSuccess: (data) => toast.success('🚨 Panic mode activated', { description: data?.output }),
    onError: () => toast.error('Panic button failed'),
  })

  const scanLeakedMutation = useMutation({
    mutationFn: () => secopsagents.scanLeakedKeys('.'),
    onSuccess: (data) => {
      const count = Array.isArray(data) ? data.length : 0
      toast.info(`Scanned. Found ${count} potential issues.`)
    },
  })

  const score = securityScore?.score ?? securityScore?.Score ?? 0

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
              <Shield className="w-6 h-6 text-red-500" />
              SecOps Dashboard
            </h1>
            <p className="text-sm text-muted-foreground mt-0.5">
              Protect. Detect. Respond. — Security posture at a glance
            </p>
          </div>

          {/* Panic Button */}
          <motion.button
            whileHover={{ scale: 1.02 }}
            whileTap={{ scale: 0.95 }}
            onClick={() => {
              const reason = prompt('Panic reason (optional):') ?? ''
              panicMutation.mutate(reason)
            }}
            className="flex items-center gap-2 px-4 py-2 rounded-xl bg-red-600 hover:bg-red-700 text-white text-sm font-semibold transition-colors shadow-lg shadow-red-500/25"
          >
            <AlertTriangle className="w-4 h-4" />
            🚨 Panic Button
          </motion.button>
        </motion.div>

        {/* Stats */}
        <StatsGrid>
          <StatCard
            title="Security Score"
            value={scoreLoading ? '…' : `${score}/100`}
            subtitle={score >= 80 ? 'Excellent' : score >= 60 ? 'Good' : 'Needs attention'}
            icon={<Shield className="w-5 h-5" />}
            trend={{ value: 5 }}
            gradient="from-red-600 to-orange-500"
            loading={scoreLoading}
          />
          <StatCard
            title="Threats Blocked"
            value={threatsBlocked?.count ?? threatsBlocked?.Count ?? '—'}
            subtitle="last 7 days"
            icon={<AlertTriangle className="w-5 h-5" />}
            trend={{ value: -12, label: 'fewer threats' }}
            gradient="from-amber-600 to-yellow-500"
            loading={threatsLoading}
          />
          <StatCard
            title="Incident Streak"
            value={streak?.days ?? streak?.Days ?? '—'}
            subtitle="zero incident days"
            icon={<CheckCircle2 className="w-5 h-5" />}
            trend={{ value: 0 }}
            gradient="from-emerald-600 to-green-500"
            loading={streakLoading}
          />
          <StatCard
            title="VPN Status"
            value={vpnStatus?.connected ? 'Connected' : vpnStatus === undefined ? '—' : 'Disconnected'}
            subtitle={vpnStatus?.provider ?? ''}
            icon={<Lock className="w-5 h-5" />}
            gradient="from-blue-600 to-cyan-500"
          />
        </StatsGrid>

        {/* Main Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-5">
          {/* Left */}
          <div className="space-y-5">
            <CloudflareControl />
            <FocusTimerWidget />
          </div>

          {/* Middle */}
          <div className="space-y-5">
            <QuickScan />

            {/* Threat Timeline */}
            <div className="rounded-2xl border border-border bg-card p-5">
              <div className="flex items-center gap-2 mb-4">
                <Activity className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">Threat Timeline</h3>
                <span className="ml-auto text-[10px] text-muted-foreground">Last 14 days</span>
              </div>
              <ResponsiveContainer width="100%" height={150}>
                <LineChart data={THREAT_DATA}>
                  <CartesianGrid strokeDasharray="3 3" stroke="hsl(var(--border))" />
                  <XAxis dataKey="day" tick={{ fontSize: 10 }} stroke="hsl(var(--muted-foreground))" />
                  <YAxis tick={{ fontSize: 10 }} stroke="hsl(var(--muted-foreground))" />
                  <Tooltip
                    contentStyle={{
                      background: 'hsl(var(--popover))',
                      border: '1px solid hsl(var(--border))',
                      borderRadius: '8px',
                      fontSize: 11,
                    }}
                  />
                  <Line type="monotone" dataKey="threats" stroke="#EF4444" strokeWidth={2} dot={false} />
                  <Line type="monotone" dataKey="blocked" stroke="#10B981" strokeWidth={2} dot={false} />
                </LineChart>
              </ResponsiveContainer>
              <div className="flex gap-4 mt-2">
                <div className="flex items-center gap-1.5 text-[10px] text-muted-foreground">
                  <div className="w-2.5 h-0.5 rounded-full bg-red-500" />
                  Detected
                </div>
                <div className="flex items-center gap-1.5 text-[10px] text-muted-foreground">
                  <div className="w-2.5 h-0.5 rounded-full bg-emerald-500" />
                  Blocked
                </div>
              </div>
            </div>
          </div>

          {/* Right */}
          <div className="space-y-5">
            <HashGen />

            {/* SSL Checker */}
            <div className="rounded-2xl border border-border bg-card p-5 space-y-3">
              <div className="flex items-center gap-2">
                <Key className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">SSL Checker</h3>
              </div>
              <div className="flex gap-2">
                <input
                  className="flex-1 h-9 text-xs rounded-xl bg-muted border border-border px-3 focus:outline-none focus:ring-1 focus:ring-primary/50"
                  placeholder="example.com"
                  value={sslDomain}
                  onChange={(e) => setSslDomain(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && sslDomain && sslMutation.mutate(sslDomain)}
                />
                <motion.button
                  whileTap={{ scale: 0.95 }}
                  onClick={() => sslDomain && sslMutation.mutate(sslDomain)}
                  className="px-3 h-9 rounded-xl bg-primary text-primary-foreground text-xs font-medium hover:bg-primary/90"
                >
                  Check
                </motion.button>
              </div>
              {sslResult && (
                <div className="space-y-1 text-xs">
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Valid:</span>
                    <span className={sslResult.isValid ? 'text-emerald-500' : 'text-red-500'}>
                      {sslResult.isValid ? '✓ Yes' : '✗ No'}
                    </span>
                  </div>
                  {sslResult.daysRemaining !== undefined && (
                    <div className="flex justify-between">
                      <span className="text-muted-foreground">Expires in:</span>
                      <span className={sslResult.daysRemaining < 30 ? 'text-amber-500' : 'text-foreground'}>
                        {sslResult.daysRemaining} days
                      </span>
                    </div>
                  )}
                  {sslResult.issuer && (
                    <div className="flex justify-between">
                      <span className="text-muted-foreground">Issuer:</span>
                      <span className="text-foreground truncate max-w-[150px]">{sslResult.issuer}</span>
                    </div>
                  )}
                </div>
              )}
            </div>

            {/* IP/DNS Lookup */}
            <div className="rounded-2xl border border-border bg-card p-5 space-y-3">
              <div className="flex items-center gap-2">
                <Globe className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">IP / DNS Lookup</h3>
              </div>
              <div className="flex gap-2">
                <input
                  className="flex-1 h-9 text-xs rounded-xl bg-muted border border-border px-3 focus:outline-none focus:ring-1 focus:ring-primary/50"
                  placeholder="IP or domain"
                  value={ipTarget}
                  onChange={(e) => setIpTarget(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && ipTarget && ipDnsMutation.mutate(ipTarget)}
                />
                <motion.button
                  whileTap={{ scale: 0.95 }}
                  onClick={() => ipTarget && ipDnsMutation.mutate(ipTarget)}
                  className="px-3 h-9 rounded-xl bg-primary text-primary-foreground text-xs font-medium hover:bg-primary/90"
                >
                  Lookup
                </motion.button>
              </div>
              {ipResult && (
                <pre className="text-[10px] text-muted-foreground bg-muted p-2 rounded-lg overflow-auto max-h-28">
                  {JSON.stringify(ipResult, null, 2)}
                </pre>
              )}
            </div>

            {/* AI Agent: Scan Leaked Keys */}
            <motion.button
              whileHover={{ scale: 1.01 }}
              whileTap={{ scale: 0.98 }}
              onClick={() => scanLeakedMutation.mutate()}
              disabled={scanLeakedMutation.isPending}
              className="w-full h-10 rounded-xl bg-amber-500/10 border border-amber-500/30 text-amber-600 text-sm font-medium hover:bg-amber-500/20 transition-colors flex items-center justify-center gap-2"
            >
              <Scan className="w-4 h-4" />
              {scanLeakedMutation.isPending ? 'Scanning...' : '🤖 Scan for Leaked API Keys'}
            </motion.button>
          </div>
        </div>
      </div>
    </AppShell>
  )
}

