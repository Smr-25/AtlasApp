// Centralized TanStack Query key factory for ATLAS
// Usage: queryKeys.user.profile → ['user', 'profile']
//        queryKeys.omnifeed.list(teamId) → ['omnifeed', teamId]

export const queryKeys = {
  user: {
    all: ['user'] as const,
    profile: () => ['user', 'profile'] as const,
    account: () => ['user', 'account'] as const,
  },
  profiles: {
    me: () => ['profiles', 'me'] as const,
  },
  workspaces: {
    all: ['workspaces'] as const,
    list: () => ['workspaces', 'list'] as const,
    detail: (id: string) => ['workspaces', id] as const,
  },
  integrations: {
    all: ['integrations'] as const,
    list: () => ['integrations', 'list'] as const,
    pending: () => ['integrations', 'pending'] as const,
    detail: (id: string) => ['integrations', id] as const,
  },
  teams: {
    all: ['teams'] as const,
    my: () => ['teams', 'my'] as const,
    detail: (teamId: string) => ['teams', teamId] as const,
    radar: (teamId: string) => ['teams', teamId, 'radar'] as const,
    productivity: (teamId: string) => ['teams', teamId, 'productivity'] as const,
  },
  teaminfo: {
    detail: (teamId: string) => ['teaminfo', teamId] as const,
  },
  focus: {
    active: () => ['focus', 'active'] as const,
    stats: () => ['focus', 'stats'] as const,
    history: (days?: number) => ['focus', 'history', days ?? 7] as const,
  },
  hotkeys: {
    list: () => ['hotkeys', 'list'] as const,
  },
  omnifeed: {
    list: (teamId: string, source?: string, page?: number) =>
      ['omnifeed', teamId, source ?? 'all', page ?? 1] as const,
  },
  squadarena: {
    leaderboard: (teamId: string) => ['squadarena', 'leaderboard', teamId] as const,
    bounties: (teamId: string) => ['squadarena', 'bounties', teamId] as const,
  },
  squadradar: {
    presence: (teamId: string) => ['squadradar', teamId] as const,
  },
  resourcehub: {
    list: (teamId: string, category?: string) =>
      ['resourcehub', teamId, category ?? 'all'] as const,
  },
  snippets: {
    list: () => ['snippets', 'list'] as const,
  },
  subscription: {
    current: () => ['subscription', 'current'] as const,
    usage: () => ['subscription', 'usage'] as const,
  },
  // Developer-specific
  devinsights: {
    timeSaved: (from?: string, to?: string) => ['devinsights', 'time-saved', from, to] as const,
    focusHeatmap: (from?: string, to?: string) => ['devinsights', 'heatmap', from, to] as const,
    peakHours: (from?: string, to?: string) => ['devinsights', 'peak-hours', from, to] as const,
    deployRate: (from?: string, to?: string) => ['devinsights', 'deploy-rate', from, to] as const,
  },
  docker: {
    list: () => ['docker', 'list'] as const,
    logs: (id: string) => ['docker', id, 'logs'] as const,
  },
  git: {
    dashboard: (integrationId: string) => ['git', 'dashboard', integrationId] as const,
  },
  sentry: {
    issues: (integrationId: string, projectSlug?: string) =>
      ['sentry', integrationId, projectSlug ?? 'all'] as const,
  },
  // Designer-specific
  designinsights: {
    assetsOptimized: () => ['designinsights', 'assets'] as const,
    colorTrends: () => ['designinsights', 'color-trends'] as const,
    designDebt: () => ['designinsights', 'design-debt'] as const,
  },
  figma: {
    comments: (integrationId: string, fileKey: string) =>
      ['figma', integrationId, fileKey, 'comments'] as const,
  },
  palettes: {
    list: () => ['palettes', 'list'] as const,
  },
  // SecOps-specific
  secopsinsights: {
    securityScore: () => ['secopsinsights', 'score'] as const,
    threatsBlocked: (from?: string, to?: string) => ['secopsinsights', 'threats', from, to] as const,
    zeroIncidentStreak: () => ['secopsinsights', 'streak'] as const,
  },
  // Marketer-specific
  marketerinsights: {
    roas: (from?: string, to?: string) => ['marketerinsights', 'roas', from, to] as const,
    leadsGenerated: (from?: string, to?: string) => ['marketerinsights', 'leads', from, to] as const,
    peakEngagement: (from?: string, to?: string) => ['marketerinsights', 'engagement', from, to] as const,
    audienceSentiment: (from?: string, to?: string) => ['marketerinsights', 'sentiment', from, to] as const,
  },
  // Leader-specific
  leaderinsights: {
    sprintVelocity: (teamId: string, from?: string, to?: string) =>
      ['leaderinsights', 'sprint-velocity', teamId, from, to] as const,
    teamMood: (teamId: string) => ['leaderinsights', 'team-mood', teamId] as const,
    topContributor: (teamId: string) => ['leaderinsights', 'top-contributor', teamId] as const,
  },
  leaderagents: {
    bottleneck: (teamId: string) => ['leaderagents', 'bottleneck', teamId] as const,
    burnoutRisk: (teamId: string) => ['leaderagents', 'burnout-risk', teamId] as const,
    unassignedBugs: (teamId: string) => ['leaderagents', 'unassigned-bugs', teamId] as const,
    milestone: (teamId: string) => ['leaderagents', 'milestone', teamId] as const,
  },
}

