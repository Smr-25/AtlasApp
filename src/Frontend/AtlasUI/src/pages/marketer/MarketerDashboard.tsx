import { useState } from 'react'
import { useQuery, useMutation } from '@tanstack/react-query'
import { motion } from 'framer-motion'
import {
  TrendingUp, BarChart3, Target, Megaphone, Mail,
  Zap, DollarSign, Users, MousePointer, Eye
} from 'lucide-react'
import AppShell from '@/components/layout/AppShell'
import { StatCard, StatsGrid } from '@/components/dashboard/StatCard'
import FocusTimerWidget from '@/components/dashboard/FocusTimerWidget'
import {
  marketerinsights, marketerutilities, marketeragents, marketerscripts
} from '@/lib/apiClient'
import { queryKeys } from '@/lib/queryKeys'
import { toast } from 'sonner'
import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid,
  Tooltip, ResponsiveContainer, LineChart, Line
} from 'recharts'

const DEMO_LEADS = [
  { week: 'W1', leads: 142, conversions: 28 },
  { week: 'W2', leads: 198, conversions: 35 },
  { week: 'W3', leads: 165, conversions: 31 },
  { week: 'W4', leads: 231, conversions: 52 },
]

const DEMO_ENGAGEMENT = [
  { hour: '9am', rate: 12 }, { hour: '10am', rate: 24 }, { hour: '11am', rate: 38 },
  { hour: '12pm', rate: 31 }, { hour: '1pm', rate: 18 }, { hour: '2pm', rate: 15 },
  { hour: '3pm', rate: 22 }, { hour: '4pm', rate: 35 }, { hour: '5pm', rate: 29 },
  { hour: '6pm', rate: 42 }, { hour: '7pm', rate: 38 }, { hour: '8pm', rate: 25 },
]

export default function MarketerDashboard() {
  const [seoUrl, setSeoUrl] = useState('')
  const [seoResult, setSeoResult] = useState<any>(null)
  const [copyPrompt, setCopyPrompt] = useState('')
  const [copyResult, setCopyResult] = useState('')
  const [utmBase, setUtmBase] = useState('')
  const [utmCampaign, setUtmCampaign] = useState('')
  const [utmResult, setUtmResult] = useState('')
  const [emailsInput, setEmailsInput] = useState('')

  const { data: roasData, isLoading: roasLoading } = useQuery({
    queryKey: queryKeys.marketerinsights.roas(),
    queryFn: () => marketerinsights.totalRoas(),
    staleTime: 300000,
    retry: false,
  })

  const { data: leadsData, isLoading: leadsLoading } = useQuery({
    queryKey: queryKeys.marketerinsights.leadsGenerated(),
    queryFn: () => marketerinsights.leadsGenerated(),
    staleTime: 300000,
    retry: false,
  })

  const { data: sentimentData, isLoading: sentimentLoading } = useQuery({
    queryKey: ['marketerinsights', 'sentiment'],
    queryFn: () => marketerinsights.audienceSentiment(),
    staleTime: 300000,
    retry: false,
  })

  const { data: cartData } = useQuery({
    queryKey: ['marketer', 'cart-abandonment'],
    queryFn: () => marketeragents.cartAbandonment(),
    staleTime: 300000,
    retry: false,
  })

  const seoMutation = useMutation({
    mutationFn: (url: string) => marketerutilities.seoCheck({ url }),
    onSuccess: (data) => setSeoResult(data),
    onError: () => toast.error('SEO check failed'),
  })

  const copyMutation = useMutation({
    mutationFn: (prompt: string) => marketerutilities.copywriting({ prompt, tone: 'professional' }),
    onSuccess: (data) => setCopyResult(data?.copy ?? ''),
    onError: () => toast.error('Copywriting failed'),
  })

  const utmMutation = useMutation({
    mutationFn: ({ url, campaign }: { url: string; campaign: string }) =>
      marketerscripts.utmLink({ baseUrl: url, campaign, source: 'atlas', medium: 'saas' }),
    onSuccess: (data) => {
      const url = data?.utmUrl ?? ''
      setUtmResult(url)
      if (url) { navigator.clipboard.writeText(url); toast.success('UTM link copied!') }
    },
  })

  const emailVerifyMutation = useMutation({
    mutationFn: (emails: string[]) => marketerscripts.verifyEmails(emails),
    onSuccess: (data) => {
      const valid = Array.isArray(data) ? data.filter((e: any) => e.isValid).length : 0
      toast.success(`Verified: ${valid} valid emails`)
    },
  })

  const viralTrendsMutation = useMutation({
    mutationFn: () => marketeragents.viralTrends(['ai', 'saas', 'productivity'], 'twitter'),
    onSuccess: (data) => toast.info('Viral trends fetched', { description: JSON.stringify(data)?.slice(0, 80) }),
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
              <TrendingUp className="w-6 h-6 text-green-500" />
              Marketer Dashboard
            </h1>
            <p className="text-sm text-muted-foreground mt-0.5">
              Attract. Convert. Retain. — Marketing intelligence hub
            </p>
          </div>
        </motion.div>

        {/* Stats */}
        <StatsGrid>
          <StatCard
            title="Total ROAS"
            value={roasData?.roas ?? roasData?.Roas ?? '—'}
            subtitle="return on ad spend"
            icon={<DollarSign className="w-5 h-5" />}
            trend={{ value: 18 }}
            gradient="from-green-600 to-emerald-500"
            loading={roasLoading}
          />
          <StatCard
            title="Leads Generated"
            value={leadsData?.count ?? leadsData?.Count ?? '—'}
            subtitle="this month"
            icon={<Users className="w-5 h-5" />}
            trend={{ value: 24 }}
            gradient="from-blue-600 to-cyan-500"
            loading={leadsLoading}
          />
          <StatCard
            title="Audience Sentiment"
            value={sentimentData?.positive ?? sentimentData?.Positive ?? '—'}
            subtitle="% positive"
            icon={<Eye className="w-5 h-5" />}
            trend={{ value: 3 }}
            gradient="from-purple-600 to-violet-500"
            loading={sentimentLoading}
          />
          <StatCard
            title="Cart Abandonment"
            value={cartData?.rate ?? cartData?.Rate ?? '—'}
            subtitle="abandonment rate"
            icon={<MousePointer className="w-5 h-5" />}
            trend={{ value: -8, label: 'improved' }}
            gradient="from-orange-600 to-amber-500"
          />
        </StatsGrid>

        {/* Main Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-5">
          {/* Left */}
          <div className="space-y-5">
            {/* Leads Chart */}
            <div className="rounded-2xl border border-border bg-card p-5">
              <div className="flex items-center gap-2 mb-4">
                <BarChart3 className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">Leads & Conversions</h3>
              </div>
              <ResponsiveContainer width="100%" height={160}>
                <BarChart data={DEMO_LEADS}>
                  <CartesianGrid strokeDasharray="3 3" stroke="hsl(var(--border))" />
                  <XAxis dataKey="week" tick={{ fontSize: 10 }} stroke="hsl(var(--muted-foreground))" />
                  <YAxis tick={{ fontSize: 10 }} stroke="hsl(var(--muted-foreground))" />
                  <Tooltip
                    contentStyle={{
                      background: 'hsl(var(--popover))',
                      border: '1px solid hsl(var(--border))',
                      borderRadius: '8px',
                      fontSize: 11,
                    }}
                  />
                  <Bar dataKey="leads" fill="#3B82F6" radius={[4, 4, 0, 0]} />
                  <Bar dataKey="conversions" fill="#10B981" radius={[4, 4, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            </div>

            {/* Viral Trends agent */}
            <motion.button
              whileHover={{ scale: 1.01 }}
              whileTap={{ scale: 0.98 }}
              onClick={() => viralTrendsMutation.mutate()}
              disabled={viralTrendsMutation.isPending}
              className="w-full h-10 rounded-xl bg-purple-500/10 border border-purple-500/30 text-purple-600 text-sm font-medium hover:bg-purple-500/20 transition-colors flex items-center justify-center gap-2"
            >
              <Zap className="w-4 h-4" />
              {viralTrendsMutation.isPending ? 'Fetching...' : '🤖 Detect Viral Trends'}
            </motion.button>

            <FocusTimerWidget />
          </div>

          {/* Middle */}
          <div className="space-y-5">
            {/* Engagement Heatmap */}
            <div className="rounded-2xl border border-border bg-card p-5">
              <div className="flex items-center gap-2 mb-4">
                <Target className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">Peak Engagement Hours</h3>
              </div>
              <ResponsiveContainer width="100%" height={160}>
                <LineChart data={DEMO_ENGAGEMENT}>
                  <CartesianGrid strokeDasharray="3 3" stroke="hsl(var(--border))" />
                  <XAxis dataKey="hour" tick={{ fontSize: 9 }} stroke="hsl(var(--muted-foreground))" />
                  <YAxis tick={{ fontSize: 10 }} stroke="hsl(var(--muted-foreground))" />
                  <Tooltip
                    contentStyle={{
                      background: 'hsl(var(--popover))',
                      border: '1px solid hsl(var(--border))',
                      borderRadius: '8px',
                      fontSize: 11,
                    }}
                  />
                  <Line type="monotone" dataKey="rate" stroke="#10B981" strokeWidth={2} dot={false} />
                </LineChart>
              </ResponsiveContainer>
            </div>

            {/* UTM Link Generator */}
            <div className="rounded-2xl border border-border bg-card p-5 space-y-3">
              <div className="flex items-center gap-2">
                <Megaphone className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">UTM Link Generator</h3>
              </div>
              <input
                className="w-full h-9 text-xs rounded-xl bg-muted border border-border px-3 focus:outline-none focus:ring-1 focus:ring-primary/50"
                placeholder="https://yoursite.com/page"
                value={utmBase}
                onChange={(e) => setUtmBase(e.target.value)}
              />
              <input
                className="w-full h-9 text-xs rounded-xl bg-muted border border-border px-3 focus:outline-none focus:ring-1 focus:ring-primary/50"
                placeholder="Campaign name"
                value={utmCampaign}
                onChange={(e) => setUtmCampaign(e.target.value)}
              />
              <motion.button
                whileTap={{ scale: 0.95 }}
                onClick={() => utmBase && utmCampaign && utmMutation.mutate({ url: utmBase, campaign: utmCampaign })}
                className="w-full h-8 rounded-lg bg-primary text-primary-foreground text-xs font-medium hover:bg-primary/90"
              >
                Generate UTM Link
              </motion.button>
              {utmResult && (
                <div className="text-[10px] bg-muted p-2 rounded-lg text-muted-foreground break-all">
                  {utmResult}
                </div>
              )}
            </div>

            {/* Email Verifier */}
            <div className="rounded-2xl border border-border bg-card p-5 space-y-3">
              <div className="flex items-center gap-2">
                <Mail className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">Bulk Email Verifier</h3>
              </div>
              <textarea
                className="w-full text-xs rounded-xl bg-muted border border-border px-3 py-2 focus:outline-none focus:ring-1 focus:ring-primary/50 resize-none"
                rows={3}
                placeholder="Paste emails (one per line)"
                value={emailsInput}
                onChange={(e) => setEmailsInput(e.target.value)}
              />
              <motion.button
                whileTap={{ scale: 0.95 }}
                onClick={() => {
                  const emails = emailsInput.split('\n').map((e) => e.trim()).filter(Boolean)
                  if (emails.length) emailVerifyMutation.mutate(emails)
                }}
                className="w-full h-8 rounded-lg bg-primary text-primary-foreground text-xs font-medium hover:bg-primary/90"
              >
                {emailVerifyMutation.isPending ? 'Verifying...' : 'Verify Emails'}
              </motion.button>
            </div>
          </div>

          {/* Right */}
          <div className="space-y-5">
            {/* SEO Checker */}
            <div className="rounded-2xl border border-border bg-card p-5 space-y-3">
              <div className="flex items-center gap-2">
                <Eye className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">SEO Analyzer</h3>
              </div>
              <div className="flex gap-2">
                <input
                  className="flex-1 h-9 text-xs rounded-xl bg-muted border border-border px-3 focus:outline-none focus:ring-1 focus:ring-primary/50"
                  placeholder="https://yoursite.com"
                  value={seoUrl}
                  onChange={(e) => setSeoUrl(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && seoUrl && seoMutation.mutate(seoUrl)}
                />
                <motion.button
                  whileTap={{ scale: 0.95 }}
                  onClick={() => seoUrl && seoMutation.mutate(seoUrl)}
                  className="px-3 h-9 rounded-xl bg-primary text-primary-foreground text-xs font-medium hover:bg-primary/90"
                >
                  Analyze
                </motion.button>
              </div>
              {seoResult && (
                <div className="space-y-2">
                  <div className="flex items-center gap-2">
                    <div className="text-lg font-bold text-foreground">{seoResult.score ?? '—'}</div>
                    <div className="flex-1 h-2 rounded-full bg-muted overflow-hidden">
                      <div
                        className="h-full bg-emerald-500 rounded-full transition-all"
                        style={{ width: `${seoResult.score ?? 0}%` }}
                      />
                    </div>
                    <span className="text-xs text-muted-foreground">/ 100</span>
                  </div>
                  {seoResult.issues?.slice(0, 3).map((issue: string, i: number) => (
                    <div key={i} className="flex items-start gap-1.5 text-xs text-amber-500">
                      <span>⚠</span>
                      <span>{issue}</span>
                    </div>
                  ))}
                </div>
              )}
            </div>

            {/* AI Copywriter */}
            <div className="rounded-2xl border border-border bg-card p-5 space-y-3">
              <div className="flex items-center gap-2">
                <Zap className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">AI Copywriter</h3>
              </div>
              <textarea
                className="w-full text-xs rounded-xl bg-muted border border-border px-3 py-2 focus:outline-none focus:ring-1 focus:ring-primary/50 resize-none"
                rows={2}
                placeholder="Write a headline for a SaaS landing page..."
                value={copyPrompt}
                onChange={(e) => setCopyPrompt(e.target.value)}
              />
              <motion.button
                whileTap={{ scale: 0.95 }}
                onClick={() => copyPrompt && copyMutation.mutate(copyPrompt)}
                className="w-full h-8 rounded-lg bg-primary text-primary-foreground text-xs font-medium hover:bg-primary/90"
              >
                {copyMutation.isPending ? 'Generating...' : '✨ Generate Copy'}
              </motion.button>
              {copyResult && (
                <div className="text-xs bg-primary/5 border border-primary/20 p-3 rounded-xl text-foreground">
                  "{copyResult}"
                  <button
                    onClick={() => { navigator.clipboard.writeText(copyResult); toast.success('Copied!') }}
                    className="block mt-1 text-[10px] text-primary hover:underline"
                  >
                    Copy to clipboard
                  </button>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </AppShell>
  )
}

