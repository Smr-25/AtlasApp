import { useState } from 'react'
import { useQuery, useMutation } from '@tanstack/react-query'
import { motion } from 'framer-motion'
import {
  Terminal, Container, Activity,
  Clock, Code2, CheckCircle2, TrendingUp
} from 'lucide-react'
import AppShell from '@/components/layout/AppShell'
import { StatCard, StatsGrid } from '@/components/dashboard/StatCard'
import FocusTimerWidget from '@/components/dashboard/FocusTimerWidget'
import GitHubPRs from '@/components/developer/GitHubPRs'
import DockerControl from '@/components/developer/DockerControl'
import MiniPostman from '@/components/developer/MiniPostman'
import {
  devinsights, docker, proactiveagents,
  devutilities
} from '@/lib/apiClient'
import { queryKeys } from '@/lib/queryKeys'
import { toast } from 'sonner'
import {
  AreaChart, Area, XAxis, YAxis, CartesianGrid,
  Tooltip, ResponsiveContainer
} from 'recharts'

// Fallback data for when backend is not connected
const DEMO_VELOCITY = Array.from({ length: 7 }, (_, i) => ({
  day: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'][i],
  commits: Math.floor(Math.random() * 20 + 3),
  prs: Math.floor(Math.random() * 5),
}))

export default function DeveloperDashboard() {
  const [regexPattern, setRegexPattern] = useState('')
  const [regexInput, setRegexInput] = useState('')
  const [regexResult, setRegexResult] = useState<any>(null)
  const [jwtToken, setJwtToken] = useState('')
  const [decodedJwt, setDecodedJwt] = useState<any>(null)


  const { data: timeSaved, isLoading: loadingTime } = useQuery({
    queryKey: queryKeys.devinsights.timeSaved(),
    queryFn: () => devinsights.timeSaved(),
    staleTime: 300000,
    retry: false,
  })

  const { data: deployRate, isLoading: loadingDeploy } = useQuery({
    queryKey: queryKeys.devinsights.deployRate(),
    queryFn: () => devinsights.deploymentSuccessRate(),
    staleTime: 300000,
    retry: false,
  })

  const { data: peakHours, isLoading: loadingPeak } = useQuery({
    queryKey: queryKeys.devinsights.peakHours(),
    queryFn: () => devinsights.peakHours(),
    staleTime: 300000,
    retry: false,
  })

  const { data: dockerList, isLoading: dockerLoading } = useQuery({
    queryKey: queryKeys.docker.list(),
    queryFn: () => docker.list(),
    staleTime: 30000,
    retry: false,
  })

  // Proactive agents mutations

  const regexMutation = useMutation({
    mutationFn: ({ pattern, input }: { pattern: string; input: string }) =>
      devutilities.testRegex(pattern, input),
    onSuccess: (data) => setRegexResult(data),
    onError: () => toast.error('Invalid regex pattern'),
  })

  const jwtMutation = useMutation({
    mutationFn: (token: string) => devutilities.decodeJwt(token),
    onSuccess: (data) => setDecodedJwt(data),
    onError: () => toast.error('Invalid JWT token'),
  })

  // dockerList may not always be an array (server error or unexpected shape), guard with Array.isArray
  const dockerArray = Array.isArray(dockerList) ? dockerList : []
  const runningContainers = dockerArray.filter((c: any) =>
    c && (c.Status === 'running' || c.status === 'running')
  ).length ?? 0

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
              <Terminal className="w-6 h-6 text-blue-500" />
              Developer Dashboard
            </h1>
            <p className="text-sm text-muted-foreground mt-0.5">
              Code. Build. Ship. — Real-time dev insights
            </p>
          </div>
        </motion.div>

        {/* Stats */}
        <StatsGrid>
          <StatCard
            title="Time Saved"
            value={timeSaved?.hours ?? timeSaved?.HoursSaved ?? '—'}
            subtitle="hours this week"
            icon={<Clock className="w-5 h-5" />}
            trend={{ value: 12, label: 'vs last week' }}
            gradient="from-blue-600 to-cyan-500"
            loading={loadingTime}
          />
          <StatCard
            title="Deploy Success"
            value={`${deployRate?.rate ?? deployRate?.Rate ?? '—'}%`}
            subtitle="last 30 days"
            icon={<CheckCircle2 className="w-5 h-5" />}
            trend={{ value: 3 }}
            gradient="from-emerald-600 to-green-500"
            loading={loadingDeploy}
          />
          <StatCard
            title="Active Containers"
            value={dockerLoading ? '…' : runningContainers}
            subtitle="docker running"
            icon={<Container className="w-5 h-5" />}
            gradient="from-orange-600 to-amber-500"
            loading={dockerLoading}
          />
          <StatCard
            title="Peak Hours"
            value={peakHours?.peakTime ?? peakHours?.PeakTime ?? '10-12'}
            subtitle="most productive"
            icon={<TrendingUp className="w-5 h-5" />}
            gradient="from-purple-600 to-violet-500"
            loading={loadingPeak}
          />
        </StatsGrid>

        {/* Main Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-5">
          {/* Left Column */}
          <div className="space-y-5">
            {/* GitHub PRs */}
            <GitHubPRs />

            {/* Focus Timer */}
            <FocusTimerWidget />
          </div>

          {/* Middle Column */}
          <div className="space-y-5">
            {/* Docker Control */}
            <DockerControl />

            {/* Velocity Chart */}
            <div className="rounded-2xl border border-border bg-card p-5">
              <div className="flex items-center gap-2 mb-4">
                <Activity className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">Commit Velocity</h3>
              </div>
              <ResponsiveContainer width="100%" height={150}>
                <AreaChart data={DEMO_VELOCITY}>
                  <defs>
                    <linearGradient id="devGrad" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="hsl(var(--primary))" stopOpacity={0.3} />
                      <stop offset="95%" stopColor="hsl(var(--primary))" stopOpacity={0} />
                    </linearGradient>
                  </defs>
                  <CartesianGrid strokeDasharray="3 3" stroke="hsl(var(--border))" />
                  <XAxis dataKey="day" tick={{ fontSize: 10 }} stroke="hsl(var(--muted-foreground))" />
                  <YAxis tick={{ fontSize: 10 }} stroke="hsl(var(--muted-foreground))" />
                  <Tooltip
                    contentStyle={{
                      background: 'hsl(var(--popover))',
                      border: '1px solid hsl(var(--border))',
                      borderRadius: '8px',
                      fontSize: 12,
                    }}
                  />
                  <Area type="monotone" dataKey="commits" stroke="hsl(var(--primary))" fill="url(#devGrad)" strokeWidth={2} />
                </AreaChart>
              </ResponsiveContainer>
            </div>
          </div>

          {/* Right Column */}
          <div className="space-y-5">
            {/* Mini Postman */}
            <MiniPostman />

            {/* Dev Utilities */}
            <div className="rounded-2xl border border-border bg-card p-5 space-y-4">
              <div className="flex items-center gap-2">
                <Code2 className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">Dev Tools</h3>
              </div>

              {/* Regex Tester */}
              <div>
                <p className="text-xs font-medium text-muted-foreground mb-2">Regex Tester</p>
                <input
                  className="w-full h-8 text-xs rounded-lg bg-muted border border-border px-3 mb-1 focus:outline-none focus:ring-1 focus:ring-primary/50"
                  placeholder="Pattern: ^[a-z]+$"
                  value={regexPattern}
                  onChange={(e) => setRegexPattern(e.target.value)}
                />
                <input
                  className="w-full h-8 text-xs rounded-lg bg-muted border border-border px-3 mb-2 focus:outline-none focus:ring-1 focus:ring-primary/50"
                  placeholder="Test input"
                  value={regexInput}
                  onChange={(e) => setRegexInput(e.target.value)}
                />
                <motion.button
                  whileTap={{ scale: 0.95 }}
                  onClick={() => regexPattern && regexMutation.mutate({ pattern: regexPattern, input: regexInput })}
                  className="w-full h-8 rounded-lg bg-primary text-primary-foreground text-xs font-medium hover:bg-primary/90 transition-colors"
                >
                  {regexMutation.isPending ? 'Testing...' : 'Test Regex'}
                </motion.button>
                {regexResult && (
                  <div className={`mt-2 text-xs p-2 rounded-lg ${regexResult.isMatch ? 'bg-emerald-500/10 text-emerald-600' : 'bg-red-500/10 text-red-500'}`}>
                    {regexResult.isMatch ? '✓ Match found' : '✗ No match'}
                    {regexResult.groups && Object.keys(regexResult.groups).length > 0 && (
                      <pre className="mt-1 text-[10px]">{JSON.stringify(regexResult.groups, null, 2)}</pre>
                    )}
                  </div>
                )}
              </div>

              {/* JWT Decoder */}
              <div>
                <p className="text-xs font-medium text-muted-foreground mb-2">JWT Decoder</p>
                <textarea
                  className="w-full text-xs rounded-lg bg-muted border border-border px-3 py-2 mb-2 focus:outline-none focus:ring-1 focus:ring-primary/50 resize-none"
                  rows={2}
                  placeholder="Paste JWT token here..."
                  value={jwtToken}
                  onChange={(e) => setJwtToken(e.target.value)}
                />
                <motion.button
                  whileTap={{ scale: 0.95 }}
                  onClick={() => jwtToken && jwtMutation.mutate(jwtToken)}
                  className="w-full h-8 rounded-lg bg-primary text-primary-foreground text-xs font-medium hover:bg-primary/90 transition-colors"
                >
                  {jwtMutation.isPending ? 'Decoding...' : 'Decode JWT'}
                </motion.button>
                {decodedJwt && (
                  <pre className="mt-2 text-[10px] text-muted-foreground bg-muted p-2 rounded-lg overflow-auto max-h-24">
                    {JSON.stringify(decodedJwt, null, 2)}
                  </pre>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </AppShell>
  )
}
