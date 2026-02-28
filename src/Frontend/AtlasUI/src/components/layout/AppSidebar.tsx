import React, { useState } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import { motion, AnimatePresence } from 'framer-motion'
import {
  LayoutDashboard,
  FolderOpen,
  Link2,
  Timer,
  Code2,
  Key,
  Radio,
  Trophy,
  Radar,
  BookOpen,
  Users,
  Info,
  CreditCard,
  User,
  ChevronLeft,
  ChevronRight,
  Crown,
  Shield,
  Palette,
  TrendingUp,
  Terminal,
  GitPullRequest,
  Container,
  Wifi,
  Hash,
  Scan,
  Figma,
  BarChart3,
  Megaphone,
  Target,
  Zap,
} from 'lucide-react'
import { useAuth, UserRole } from '@/context/AuthContext'
import { useUIStore } from '@/store/uiStore'
import { useRole } from '@/hooks/useRole'

interface NavItem {
  icon: React.ElementType
  label: string
  path: string
  badge?: string
  highlight?: boolean
}

type NavSection = {
  title: string
  items: NavItem[]
}

const COMMON_SECTIONS: NavSection[] = [
  {
    title: 'Workspace',
    items: [
      { icon: FolderOpen, label: 'Workspaces', path: '/workspaces' },
      { icon: Link2, label: 'Integrations', path: '/integrations' },
      { icon: Timer, label: 'Focus Timer', path: '/focus' },
      { icon: Code2, label: 'Snippets', path: '/snippets' },
      { icon: Key, label: 'Hotkeys', path: '/hotkeys' },
    ],
  },
  {
    title: 'Team',
    items: [
      { icon: Radio, label: 'OmniFeed', path: '/omnifeed' },
      { icon: Trophy, label: 'Squad Arena', path: '/squad-arena' },
      { icon: Radar, label: 'Squad Radar', path: '/squad-radar' },
      { icon: BookOpen, label: 'Resource Hub', path: '/resource-hub' },
      { icon: Users, label: 'Teams', path: '/teams' },
      { icon: Info, label: 'Team Info', path: '/team-info' },
    ],
  },
  {
    title: 'Account',
    items: [
      { icon: CreditCard, label: 'Subscription', path: '/subscription' },
      { icon: User, label: 'Profile', path: '/profile' },
    ],
  },
]

const ROLE_SECTIONS: Record<UserRole, NavSection[]> = {
  developer: [
    {
      title: 'Developer',
      items: [
        { icon: LayoutDashboard, label: 'Dashboard', path: '/developer' },
        { icon: GitPullRequest, label: 'GitHub PRs', path: '/developer/github' },
        { icon: Container, label: 'Docker', path: '/developer/docker' },
        { icon: Wifi, label: 'HTTP Client', path: '/developer/postman' },
        { icon: Terminal, label: 'Dev Tools', path: '/developer/tools' },
        { icon: Zap, label: 'AI Agents', path: '/developer/agents', highlight: true },
      ],
    },
  ],
  designer: [
    {
      title: 'Designer',
      items: [
        { icon: LayoutDashboard, label: 'Dashboard', path: '/designer' },
        { icon: Figma, label: 'Figma', path: '/designer/figma' },
        { icon: Palette, label: 'Color Tools', path: '/designer/colors' },
        { icon: FolderOpen, label: 'Asset Export', path: '/designer/assets' },
        { icon: Zap, label: 'Design Utilities', path: '/designer/utilities', highlight: true },
      ],
    },
  ],
  cybersecurity: [
    {
      title: 'SecOps',
      items: [
        { icon: LayoutDashboard, label: 'Dashboard', path: '/secops' },
        { icon: Shield, label: 'Quick Scan', path: '/secops/scan' },
        { icon: Hash, label: 'Hash Generator', path: '/secops/hash' },
        { icon: Scan, label: 'Port Scanner', path: '/secops/ports' },
        { icon: Zap, label: 'AI Agents', path: '/secops/agents', highlight: true },
      ],
    },
  ],
  marketer: [
    {
      title: 'Marketing',
      items: [
        { icon: LayoutDashboard, label: 'Dashboard', path: '/marketer' },
        { icon: BarChart3, label: 'Analytics', path: '/marketer/analytics' },
        { icon: Megaphone, label: 'Campaigns', path: '/marketer/campaigns' },
        { icon: Target, label: 'SEO Tools', path: '/marketer/seo' },
        { icon: Zap, label: 'AI Agents', path: '/marketer/agents', highlight: true },
      ],
    },
  ],
  'team-leader': [
    {
      title: 'Leader',
      items: [
        { icon: LayoutDashboard, label: 'Dashboard', path: '/leader' },
        { icon: TrendingUp, label: 'Insights', path: '/leader/insights' },
        { icon: Users, label: 'Team Radar', path: '/leader/radar' },
        { icon: Trophy, label: 'Squad Arena', path: '/leader/arena' },
        { icon: Zap, label: 'AI Agents', path: '/leader/agents', highlight: true },
      ],
    },
  ],
}

const ROLE_COLORS: Record<UserRole, string> = {
  developer: 'from-blue-600 to-cyan-500',
  designer: 'from-purple-600 to-pink-500',
  cybersecurity: 'from-red-600 to-orange-500',
  marketer: 'from-green-600 to-emerald-500',
  'team-leader': 'from-primary to-primary/70',
}

const ROLE_LABELS: Record<UserRole, string> = {
  developer: 'Developer',
  designer: 'Designer',
  cybersecurity: 'SecOps',
  marketer: 'Marketer',
  'team-leader': 'Team Leader',
}

const ROLE_ICONS: Record<UserRole, React.ElementType> = {
  developer: Terminal,
  designer: Palette,
  cybersecurity: Shield,
  marketer: TrendingUp,
  'team-leader': Crown,
}

const AppSidebar = () => {
  const { user } = useAuth()
  const sidebarCollapsed = useUIStore(s => s.sidebarCollapsed)
  const toggleSidebar = useUIStore(s => s.toggleSidebar)
  const navigate = useNavigate()
  const location = useLocation()
  const [hovered, setHovered] = useState<string | null>(null)

  // const role = (user?.role as UserRole) ?? 'developer'
  const roleHook = useRole()
  const role = (roleHook.primary as unknown as UserRole) ?? (user?.role as UserRole) ?? 'developer'
  const roleSections = ROLE_SECTIONS[role] ?? []
  const allSections = [...roleSections, ...COMMON_SECTIONS]
  const gradientClass = ROLE_COLORS[role]
  const RoleIcon = ROLE_ICONS[role]

  const isActive = (path: string) => location.pathname === path || location.pathname.startsWith(path + '/')

  return (
    <motion.aside
      animate={{ width: sidebarCollapsed ? 64 : 240 }}
      transition={{ type: 'spring', stiffness: 300, damping: 30 }}
      className="shrink-0 border-r border-border bg-card flex flex-col h-full overflow-hidden relative"
    >
      {/* Logo */}
      <div className={`h-16 flex items-center shrink-0 border-b border-border px-4 ${sidebarCollapsed ? 'justify-center' : 'gap-3'}`}>
        <motion.div
          whileHover={{ scale: 1.05 }}
          className={`w-8 h-8 rounded-lg bg-gradient-to-br ${gradientClass} flex items-center justify-center shadow-lg shrink-0 cursor-pointer`}
          onClick={() => navigate('/dashboard')}
        >
          <RoleIcon className="w-4 h-4 text-white" />
        </motion.div>
        <AnimatePresence>
          {!sidebarCollapsed && (
            <motion.div
              initial={{ opacity: 0, x: -10 }}
              animate={{ opacity: 1, x: 0 }}
              exit={{ opacity: 0, x: -10 }}
              className="overflow-hidden"
            >
              <h1 className="text-sm font-bold text-foreground leading-tight whitespace-nowrap">ATLAS</h1>
              <p className="text-[10px] text-muted-foreground leading-tight whitespace-nowrap">{ROLE_LABELS[role]}</p>
            </motion.div>
          )}
        </AnimatePresence>
      </div>

      {/* Nav */}
      <div className="flex-1 overflow-y-auto py-3 px-2 space-y-4 scrollbar-thin scrollbar-thumb-border">
        {allSections.map((section) => (
          <div key={section.title}>
            <AnimatePresence>
              {!sidebarCollapsed && (
                <motion.p
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  exit={{ opacity: 0 }}
                  className="text-[9px] font-semibold text-muted-foreground/60 tracking-widest uppercase mb-1 px-2"
                >
                  {section.title}
                </motion.p>
              )}
            </AnimatePresence>
            <nav className="space-y-0.5">
              {section.items.map((item, idx) => {
                const active = isActive(item.path)
                return (
                  <motion.button
                    key={item.path}
                    initial={{ opacity: 0, x: -15 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ delay: idx * 0.03 }}
                    whileHover={{ x: sidebarCollapsed ? 0 : 2 }}
                    onClick={() => navigate(item.path)}
                    onMouseEnter={() => setHovered(item.path)}
                    onMouseLeave={() => setHovered(null)}
                    title={sidebarCollapsed ? item.label : undefined}
                    className={`w-full flex items-center gap-2.5 px-2.5 py-2 rounded-lg text-sm transition-colors relative ${
                      active
                        ? 'bg-primary/15 text-primary font-medium'
                        : item.highlight
                        ? 'text-primary/80 hover:bg-primary/10 hover:text-primary'
                        : 'text-muted-foreground hover:bg-muted hover:text-foreground'
                    } ${sidebarCollapsed ? 'justify-center' : ''}`}
                  >
                    {active && (
                      <motion.div
                        layoutId="active-indicator"
                        className="absolute left-0 top-1/2 -translate-y-1/2 w-0.5 h-5 rounded-r-full bg-primary"
                      />
                    )}
                    <item.icon className="w-4 h-4 shrink-0" />
                    <AnimatePresence>
                      {!sidebarCollapsed && (
                        <motion.span
                          initial={{ opacity: 0, width: 0 }}
                          animate={{ opacity: 1, width: 'auto' }}
                          exit={{ opacity: 0, width: 0 }}
                          className="flex-1 text-left overflow-hidden whitespace-nowrap"
                        >
                          {item.label}
                        </motion.span>
                      )}
                    </AnimatePresence>
                    {!sidebarCollapsed && item.badge && (
                      <span className="text-[9px] bg-muted text-muted-foreground px-1.5 py-0.5 rounded-md font-medium">
                        {item.badge}
                      </span>
                    )}
                    {/* Tooltip on collapsed */}
                    {sidebarCollapsed && hovered === item.path && (
                      <motion.div
                        initial={{ opacity: 0, x: 5 }}
                        animate={{ opacity: 1, x: 0 }}
                        className="absolute left-full ml-2 top-1/2 -translate-y-1/2 z-50 bg-popover border border-border text-popover-foreground text-xs rounded-lg px-2.5 py-1.5 whitespace-nowrap shadow-lg pointer-events-none"
                      >
                        {item.label}
                      </motion.div>
                    )}
                  </motion.button>
                )
              })}
            </nav>
          </div>
        ))}
      </div>

      {/* Upgrade + collapse */}
      <div className="p-3 border-t border-border space-y-2 shrink-0">
        {!sidebarCollapsed && (
          <motion.button
            whileHover={{ scale: 1.02 }}
            whileTap={{ scale: 0.98 }}
            onClick={() => navigate('/subscription')}
            className={`w-full flex items-center justify-center gap-2 h-9 rounded-xl bg-gradient-to-r ${gradientClass} text-white text-xs font-semibold shadow-lg hover:opacity-90 transition-opacity`}
          >
            <Crown className="w-3.5 h-3.5" />
            Upgrade to PRO
          </motion.button>
        )}
        <div className="flex items-center gap-2">
          {!sidebarCollapsed && (
            <button
              onClick={() => navigate('/profile')}
              className="flex-1 flex items-center gap-2 px-2 py-1.5 rounded-lg hover:bg-muted transition-colors"
            >
              <div className={`w-6 h-6 rounded-full bg-gradient-to-br ${gradientClass} flex items-center justify-center shrink-0`}>
                <span className="text-[10px] font-bold text-white">
                  {user?.fullName?.[0]?.toUpperCase() ?? 'U'}
                </span>
              </div>
              <div className="text-left overflow-hidden">
                <p className="text-xs font-medium text-foreground truncate">{user?.fullName ?? 'User'}</p>
                <p className="text-[9px] text-muted-foreground truncate">{user?.email ?? ''}</p>
              </div>
            </button>
          )}
          <motion.button
            whileHover={{ scale: 1.1 }}
            whileTap={{ scale: 0.9 }}
            onClick={toggleSidebar}
            className="w-7 h-7 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-foreground transition-colors shrink-0"
          >
            {sidebarCollapsed ? <ChevronRight className="w-4 h-4" /> : <ChevronLeft className="w-4 h-4" />}
          </motion.button>
        </div>
      </div>
    </motion.aside>
  )
}

export default React.memo(AppSidebar)
