import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { motion } from 'framer-motion'
import {
  Crown, Users, TrendingUp, Zap, Activity,
  Timer, Target, AlertTriangle, CheckCircle2,
  Flame, GitPullRequest
} from 'lucide-react'
import AppShell from '@/components/layout/AppShell'
import { StatCard, StatsGrid } from '@/components/dashboard/StatCard'
import FocusTimerWidget from '@/components/dashboard/FocusTimerWidget'
import OmniFeed from '@/components/leader/OmniFeed'
import SquadArena, { SquadArenaComparison } from '@/components/leader/SquadArena'
import SquadRadar from '@/components/leader/SquadRadar'
import InsightsWidget from '@/components/leader/InsightsWidget'
import ProactiveAgents from '@/components/leader/ProactiveAgents'
import ResourceHub from '@/components/leader/ResourceHub'
import ScriptsPanel from '@/components/leader/ScriptsPanel'
import UtilitiesModal from '@/components/leader/UtilitiesModal'
import IntegrationsModal from '@/components/leader/IntegrationsModal'
import {
  leaderinsightsAll, leaderagentsAll, leaderscripts as leaderscriptsApi,
  teams, omnifeed as omnifeedApi
} from '@/lib/apiClient'
import { queryKeys } from '@/lib/queryKeys'
import { useUIStore } from '@/store/uiStore'
import { useSignalR } from '@/context/SignalRContext'
import { toast } from 'sonner'
import {
  RadarChart, PolarGrid, PolarAngleAxis, Radar,
  ResponsiveContainer
} from 'recharts'
import { MOCK_AGENTS, MOCK_FEED, MOCK_INTEGRATIONS, MOCK_RESOURCES, MOCK_SCRIPTS, MOCK_SQUADS } from './__leaderMocks'

const RADAR_DEMO = [
  { subject: 'Velocity', A: 85, fullMark: 100 },
  { subject: 'Quality', A: 72, fullMark: 100 },
  { subject: 'Collab', A: 90, fullMark: 100 },
  { subject: 'Delivery', A: 68, fullMark: 100 },
  { subject: 'Morale', A: 88, fullMark: 100 },
]

export default function LeaderDashboard() {
  const [openScripts, setOpenScripts] = useState(false)
  const [openUtilities, setOpenUtilities] = useState(false)
  const [openIntegrations, setOpenIntegrations] = useState(false)
  const [openResources, setOpenResources] = useState(false)
  const { activeTeamId } = useUIStore()
  const { joinTeam } = useSignalR()

  const { data: myTeams } = useQuery({
    queryKey: queryKeys.teams.my(),
    queryFn: () => teams.my(),
    staleTime: 60000,
    retry: false,
  })

  const effectiveTeamId = activeTeamId || myTeams?.[0]?.Id || myTeams?.[0]?.id || ''

  const { data: sprintVelocity, isLoading: velocityLoading } = useQuery({
    queryKey: queryKeys.leaderinsights.sprintVelocity(effectiveTeamId),
    queryFn: () => leaderinsightsAll.sprintVelocity(effectiveTeamId),
    enabled: !!effectiveTeamId,
    staleTime: 300000,
    retry: false,
  })

  const { data: teamMood, isLoading: moodLoading } = useQuery({
    queryKey: queryKeys.leaderinsights.teamMood(effectiveTeamId),
    queryFn: () => leaderinsightsAll.teamMood(effectiveTeamId),
    enabled: !!effectiveTeamId,
    staleTime: 300000,
    retry: false,
  })

  const { data: topContributor, isLoading: contributorLoading } = useQuery({
    queryKey: queryKeys.leaderinsights.topContributor(effectiveTeamId),
    queryFn: () => leaderinsightsAll.topContributor(effectiveTeamId),
    enabled: !!effectiveTeamId,
    staleTime: 300000,
    retry: false,
  })

  const { data: bottleneck } = useQuery({
    queryKey: queryKeys.leaderagents.bottleneck(effectiveTeamId),
    queryFn: () => leaderagentsAll.bottleneck(effectiveTeamId),
    enabled: !!effectiveTeamId,
    staleTime: 120000,
    retry: false,
  })

  const { data: burnoutRisk } = useQuery({
    queryKey: queryKeys.leaderagents.burnoutRisk(effectiveTeamId),
    queryFn: () => leaderagentsAll.burnoutRisk(effectiveTeamId),
    enabled: !!effectiveTeamId,
    staleTime: 120000,
    retry: false,
  })

  const { data: unassignedBugs } = useQuery({
    queryKey: queryKeys.leaderagents.unassignedBugs(effectiveTeamId),
    queryFn: () => leaderagentsAll.unassignedBugs(effectiveTeamId),
    enabled: !!effectiveTeamId,
    staleTime: 120000,
    retry: false,
  })

  const { data: feedData } = useQuery({
    queryKey: queryKeys.omnifeed.list(effectiveTeamId),
    queryFn: () => omnifeedApi.list(effectiveTeamId),
    enabled: !!effectiveTeamId,
    staleTime: 30000,
    retry: false,
  })

  const sprintStarterMutation = useMutation({
    mutationFn: () => leaderscriptsApi.sprintStarter({
      teamId: effectiveTeamId,
      sprintName: `Sprint ${new Date().toLocaleDateString('en-US', { month: 'short', day: 'numeric' })}`,
      goals: ['Complete sprint backlog', 'Improve test coverage'],
    }),
    onSuccess: (data) => toast.success('Sprint started!', { description: data?.output }),
    onError: () => toast.error('Could not start sprint'),
  })

  const standupMutation = useMutation({
    mutationFn: () => leaderscriptsApi.standupPing(effectiveTeamId, '⏰ Standup time!'),
    onSuccess: (data) => toast.success('Standup ping sent!', { description: data?.output }),
  })

  const ghostMembersMutation = useMutation({
    mutationFn: () => leaderagentsAll.ghostMembers(effectiveTeamId, 7),
    onSuccess: (data) => toast.info(`Ghost members: ${data?.pingedCount ?? 0} pinged`),
  })

  const comparisons: SquadArenaComparison[] = [
    { id: 'c1', left: { name: 'Frontend', score: 88 }, right: { name: 'Backend', score: 43 }, winner: 'left' },
    { id: 'c2', left: { name: 'Payments', score: 72 }, right: { name: 'Support', score: 70 }, winner: 'left' },
  ]

  const feedItems = Array.isArray(feedData?.items) ? feedData.items
    : Array.isArray(feedData) ? feedData : MOCK_FEED

  return (
    <AppShell>
      <div className="space-y-6">
        {/* Header */}
        <motion.div
          initial={{ opacity: 0, y: -10 }}
          animate={{ opacity: 1, y: 0 }}
          className="flex items-center justify-between flex-wrap gap-3"
        >
          <div>
            <h1 className="text-2xl font-bold text-foreground flex items-center gap-2">
              <Crown className="w-6 h-6 text-yellow-500" />
              Leader Dashboard
            </h1>
            <p className="text-sm text-muted-foreground mt-0.5">
              Lead. Align. Deliver. — Team intelligence at your command
            </p>
          </div>
          <div className="flex items-center gap-2 flex-wrap">
            <motion.button
              whileTap={{ scale: 0.95 }}
              onClick={() => sprintStarterMutation.mutate()}
              disabled={!effectiveTeamId || sprintStarterMutation.isPending}
              className="flex items-center gap-1.5 px-3 py-2 rounded-xl bg-primary text-primary-foreground text-xs font-medium hover:bg-primary/90 disabled:opacity-50"
            >
              <Flame className="w-3.5 h-3.5" />
              Start Sprint
            </motion.button>
            <motion.button
              whileTap={{ scale: 0.95 }}
              onClick={() => standupMutation.mutate()}
              disabled={!effectiveTeamId}
              className="flex items-center gap-1.5 px-3 py-2 rounded-xl bg-muted text-foreground text-xs font-medium hover:bg-muted/80 disabled:opacity-50"
            >
              <Timer className="w-3.5 h-3.5" />
              Standup Ping
            </motion.button>
            <motion.button
              whileTap={{ scale: 0.95 }}
              onClick={() => ghostMembersMutation.mutate()}
              disabled={!effectiveTeamId}
              className="flex items-center gap-1.5 px-3 py-2 rounded-xl bg-amber-500/10 text-amber-600 border border-amber-500/30 text-xs font-medium hover:bg-amber-500/20 disabled:opacity-50"
            >
              <Users className="w-3.5 h-3.5" />
              Ping Ghosts
            </motion.button>
            <motion.button
              whileTap={{ scale: 0.95 }}
              onClick={() => setOpenIntegrations(true)}
              className="flex items-center gap-1.5 px-3 py-2 rounded-xl bg-muted text-foreground text-xs font-medium hover:bg-muted/80"
            >
              <Zap className="w-3.5 h-3.5" />
              Integrations
            </motion.button>
          </div>
        </motion.div>

        {/* AI Alert Strip */}
        {(bottleneck || burnoutRisk || (Array.isArray(unassignedBugs) && unassignedBugs.length > 0)) && (
          <motion.div
            initial={{ opacity: 0, y: -5 }}
            animate={{ opacity: 1, y: 0 }}
            className="flex flex-wrap gap-2"
          >
            {bottleneck && (
              <div className="flex items-center gap-2 px-3 py-2 rounded-xl bg-amber-500/10 border border-amber-500/20 text-xs text-amber-600">
                <AlertTriangle className="w-3.5 h-3.5" />
                Bottleneck: {bottleneck.task ?? bottleneck.member ?? 'Review needed'}
              </div>
            )}
            {burnoutRisk && (
              <div className="flex items-center gap-2 px-3 py-2 rounded-xl bg-red-500/10 border border-red-500/20 text-xs text-red-500">
                <AlertTriangle className="w-3.5 h-3.5" />
                Burnout risk: {burnoutRisk.member ?? 'Team member needs attention'}
              </div>
            )}
            {Array.isArray(unassignedBugs) && unassignedBugs.length > 0 && (
              <div className="flex items-center gap-2 px-3 py-2 rounded-xl bg-purple-500/10 border border-purple-500/20 text-xs text-purple-600">
                <GitPullRequest className="w-3.5 h-3.5" />
                {unassignedBugs.length} unassigned bug{unassignedBugs.length > 1 ? 's' : ''}
              </div>
            )}
          </motion.div>
        )}

        {/* Stats */}
        <StatsGrid>
          <StatCard
            title="Sprint Velocity"
            value={sprintVelocity?.velocity ?? sprintVelocity?.Velocity ?? '—'}
            subtitle="story points / sprint"
            icon={<TrendingUp className="w-5 h-5" />}
            trend={{ value: 8 }}
            gradient="from-yellow-600 to-orange-500"
            loading={velocityLoading}
          />
          <StatCard
            title="Team Mood"
            value={`${teamMood?.index ?? teamMood?.Index ?? '—'}/10`}
            subtitle="mood index"
            icon={<Activity className="w-5 h-5" />}
            trend={{ value: 5 }}
            gradient="from-pink-600 to-rose-500"
            loading={moodLoading}
          />
          <StatCard
            title="Top Contributor"
            value={topContributor?.name ?? topContributor?.Name ?? '—'}
            subtitle={`${topContributor?.commits ?? topContributor?.Commits ?? ''} commits`}
            icon={<CheckCircle2 className="w-5 h-5" />}
            gradient="from-emerald-600 to-green-500"
            loading={contributorLoading}
          />
          <StatCard
            title="Active Teams"
            value={myTeams?.length ?? '—'}
            subtitle="teams connected"
            icon={<Users className="w-5 h-5" />}
            gradient="from-blue-600 to-cyan-500"
          />
        </StatsGrid>

        {/* Main Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-5">
          {/* Left */}
          <div className="space-y-5">
            <ProactiveAgents agents={MOCK_AGENTS} />
            <FocusTimerWidget />
            {/* Team Performance Radar */}
            <div className="rounded-2xl border border-border bg-card p-5">
              <div className="flex items-center gap-2 mb-3">
                <Target className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">Team Performance</h3>
              </div>
              <ResponsiveContainer width="100%" height={180}>
                <RadarChart data={RADAR_DEMO}>
                  <PolarGrid stroke="hsl(var(--border))" />
                  <PolarAngleAxis dataKey="subject" tick={{ fontSize: 10, fill: 'hsl(var(--muted-foreground))' }} />
                  <Radar name="Team" dataKey="A" stroke="hsl(var(--primary))" fill="hsl(var(--primary))" fillOpacity={0.2} strokeWidth={2} />
                </RadarChart>
              </ResponsiveContainer>
            </div>
          </div>

          {/* Middle */}
          <div className="space-y-5">
            <OmniFeed items={feedItems} />
            <SquadArena comparisons={comparisons} />
          </div>

          {/* Right */}
          <div className="space-y-5">
            <SquadRadar squads={MOCK_SQUADS} />
            <InsightsWidget />
          </div>
        </div>

        {/* Modals */}
        <IntegrationsModal open={openIntegrations} onClose={() => setOpenIntegrations(false)} integrations={MOCK_INTEGRATIONS} />
        <ScriptsPanel open={openScripts} onClose={() => setOpenScripts(false)} scripts={MOCK_SCRIPTS} />
        <UtilitiesModal open={openUtilities} onClose={() => setOpenUtilities(false)} />
        <ResourceHub open={openResources} onClose={() => setOpenResources(false)} resources={MOCK_RESOURCES} />
      </div>
    </AppShell>
  )
}
