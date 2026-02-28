// Minimal shared types and mock data for leader dashboard
export type IntegrationProvider = 'github' | 'jira' | 'slack' | 'notion' | 'calendar' | 'sentry' | 'pagerduty' | string

export interface Integration {
  id: string
  name: string
  provider: IntegrationProvider
  status: 'connected' | 'pending' | 'disconnected'
}

export interface ScriptItem {
  id: string
  name: string
  description?: string
  lastRun?: string
  status?: 'idle' | 'running' | 'failed' | 'ok'
}

export interface AgentStatus {
  id: string
  name: string
  status: 'idle' | 'active' | 'alert'
  lastSeen?: string
  note?: string
}

export interface FeedItem {
  id: string
  type: 'alert' | 'update' | 'comment' | 'deploy'
  title: string
  description?: string
  time: string
  severity?: 'low' | 'medium' | 'high'
}

export interface SquadSummary {
  id: string
  name: string
  health: 'green' | 'yellow' | 'red'
  focus?: string
  score?: number
}

export interface ResourceItem {
  id: string
  title: string
  type: 'doc' | 'link' | 'video'
  url?: string
  summary?: string
}

// Small mock factories used by components when backend is not present
export const MOCK_INTEGRATIONS: Integration[] = [
  { id: 'i1', name: 'GitHub - Atlas', provider: 'github', status: 'connected' },
  { id: 'i2', name: 'Slack - Team', provider: 'slack', status: 'pending' },
  { id: 'i3', name: 'Jira - Board', provider: 'jira', status: 'disconnected' },
]

export const MOCK_SCRIPTS: ScriptItem[] = [
  { id: 's1', name: 'Sprint Starter', description: 'Create sprint + seed tasks', lastRun: '2d ago', status: 'idle' },
  { id: 's2', name: 'End-of-Week Summary', description: 'Compile weekly summary', lastRun: '6d ago', status: 'ok' },
]

export const MOCK_AGENTS: AgentStatus[] = [
  { id: 'a1', name: 'Bottleneck Predictor', status: 'active', lastSeen: '5m', note: 'Ali stuck on payment flow' },
  { id: 'a2', name: 'Burnout Warner', status: 'idle', lastSeen: '1h' },
  { id: 'a3', name: 'PR Review Nag', status: 'alert', lastSeen: '2m', note: '3 PRs pending >24h' },
]

export const MOCK_FEED: FeedItem[] = [
  { id: 'f1', type: 'update', title: 'Ali opened a PR', description: 'Feature/cart', time: '10:15', severity: 'low' },
  { id: 'f2', type: 'comment', title: 'Design updated', description: 'Figma: Checkout', time: '10:20' },
  { id: 'f3', type: 'alert', title: 'Sentry: NullReference in API', description: 'Critical', time: '10:45', severity: 'high' },
  { id: 'f4', type: 'deploy', title: 'Staging deployed', description: 'v1.2.3', time: '11:00' },
  { id: 'f5', type: 'update', title: 'Aysel finished screen', description: 'Figma', time: '11:10' },
]

export const MOCK_SQUADS: SquadSummary[] = [
  { id: 'q1', name: 'Payments', health: 'yellow', focus: 'Checkout', score: 72 },
  { id: 'q2', name: 'Frontend', health: 'green', focus: 'Performance', score: 88 },
  { id: 'q3', name: 'Backend', health: 'red', focus: 'API stability', score: 43 },
]

export const MOCK_RESOURCES: ResourceItem[] = [
  { id: 'r1', title: 'PRD - Checkout', type: 'doc', url: 'https://notion.so/prd-checkout', summary: 'Product requirements for checkout' },
  { id: 'r2', title: 'Design System', type: 'link', url: 'https://figma.com/file/xyz', summary: 'Figma design system' },
  { id: 'r3', title: 'Onboarding Guide', type: 'doc', url: 'https://notion.so/onboarding', summary: 'How new hires start' },
]

