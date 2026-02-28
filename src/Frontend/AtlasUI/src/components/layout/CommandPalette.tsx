import React, { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Command } from 'cmdk'
import { motion, AnimatePresence } from 'framer-motion'
import {
  LayoutDashboard,
  FolderOpen,
  Link2,
  Timer,
  Code2,
  Users,
  CreditCard,
  User,
  Settings,
  LogOut,
  Terminal,
  Shield,
  Palette,
  TrendingUp,
  Crown,
  Search,
} from 'lucide-react'
import { useAuth } from '@/context/AuthContext'
import { UserRole } from '@/context/AuthContext'

interface CommandItem {
  id: string
  label: string
  description?: string
  icon: React.ElementType
  action: () => void
  section: string
  roles?: UserRole[]
}

interface Props {
  open: boolean
  onClose: () => void
}

export default function CommandPalette({ open, onClose }: Props) {
  const navigate = useNavigate()
  const { user, logout } = useAuth()
  const [search, setSearch] = useState('')
  const role = user?.role as UserRole

  const nav = (path: string) => { navigate(path); onClose() }

  const allCommands: CommandItem[] = [
    // Navigation
    { id: 'dashboard', label: 'Go to Dashboard', icon: LayoutDashboard, action: () => nav('/dashboard'), section: 'Navigation' },
    { id: 'workspaces', label: 'Workspaces', icon: FolderOpen, action: () => nav('/workspaces'), section: 'Navigation' },
    { id: 'integrations', label: 'Integrations', icon: Link2, action: () => nav('/integrations'), section: 'Navigation' },
    { id: 'focus', label: 'Focus Timer', icon: Timer, action: () => nav('/focus'), section: 'Navigation' },
    { id: 'snippets', label: 'Snippets', icon: Code2, action: () => nav('/snippets'), section: 'Navigation' },
    { id: 'teams', label: 'Teams', icon: Users, action: () => nav('/teams'), section: 'Navigation' },
    { id: 'subscription', label: 'Subscription & Billing', icon: CreditCard, action: () => nav('/subscription'), section: 'Navigation' },
    { id: 'profile', label: 'Profile Settings', icon: User, action: () => nav('/profile'), section: 'Navigation' },

    // Role-specific
    { id: 'dev-dashboard', label: 'Developer Dashboard', icon: Terminal, action: () => nav('/developer'), section: 'Developer', roles: ['developer'] },
    { id: 'designer-dashboard', label: 'Designer Dashboard', icon: Palette, action: () => nav('/designer'), section: 'Designer', roles: ['designer'] },
    { id: 'secops-dashboard', label: 'SecOps Dashboard', icon: Shield, action: () => nav('/secops'), section: 'SecOps', roles: ['cybersecurity'] },
    { id: 'marketer-dashboard', label: 'Marketer Dashboard', icon: TrendingUp, action: () => nav('/marketer'), section: 'Marketing', roles: ['marketer'] },
    { id: 'leader-dashboard', label: 'Leader Dashboard', icon: Crown, action: () => nav('/leader'), section: 'Leader', roles: ['team-leader'] },

    // Actions
    { id: 'settings', label: 'Settings', icon: Settings, action: () => nav('/settings'), section: 'Actions' },
    { id: 'logout', label: 'Sign Out', description: 'Log out of your account', icon: LogOut, action: () => { logout(); onClose() }, section: 'Actions' },
  ]

  const filtered = search
    ? allCommands.filter((cmd) => {
        if (cmd.roles && !cmd.roles.includes(role)) return false
        return cmd.label.toLowerCase().includes(search.toLowerCase())
      })
    : allCommands.filter((cmd) => !cmd.roles || cmd.roles.includes(role))

  // Group by section
  const sections = [...new Set(filtered.map((c) => c.section))]

  useEffect(() => {
    if (!open) setSearch('')
  }, [open])

  return (
    <AnimatePresence>
      {open && (
        <>
          {/* Backdrop */}
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={onClose}
            className="fixed inset-0 bg-black/50 backdrop-blur-sm z-50"
          />

          {/* Panel */}
          <motion.div
            initial={{ opacity: 0, scale: 0.95, y: -20 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.95, y: -20 }}
            transition={{ type: 'spring', stiffness: 400, damping: 30 }}
            className="fixed top-[20%] left-1/2 -translate-x-1/2 z-50 w-full max-w-xl"
          >
            <Command
              className="bg-popover border border-border rounded-2xl shadow-2xl overflow-hidden"
              shouldFilter={false}
            >
              <div className="flex items-center gap-3 px-4 border-b border-border">
                <Search className="w-4 h-4 text-muted-foreground shrink-0" />
                <Command.Input
                  value={search}
                  onValueChange={setSearch}
                  placeholder="Search commands..."
                  className="flex-1 h-14 bg-transparent text-sm text-foreground placeholder:text-muted-foreground outline-none"
                  autoFocus
                />
                <kbd
                  onClick={onClose}
                  className="text-[10px] bg-muted border border-border rounded px-1.5 py-0.5 text-muted-foreground cursor-pointer hover:bg-muted/80"
                >
                  ESC
                </kbd>
              </div>

              <Command.List className="max-h-[380px] overflow-y-auto p-2">
                <Command.Empty className="py-8 text-center text-sm text-muted-foreground">
                  No results found.
                </Command.Empty>

                {sections.map((section) => (
                  <Command.Group key={section} heading={section} className="mb-2">
                    {filtered
                      .filter((c) => c.section === section)
                      .map((cmd) => (
                        <Command.Item
                          key={cmd.id}
                          value={cmd.id}
                          onSelect={cmd.action}
                          className="flex items-center gap-3 px-3 py-2.5 rounded-xl cursor-pointer text-sm text-foreground hover:bg-muted aria-selected:bg-muted transition-colors"
                        >
                          <div className="w-7 h-7 rounded-lg bg-muted flex items-center justify-center shrink-0">
                            <cmd.icon className="w-3.5 h-3.5 text-muted-foreground" />
                          </div>
                          <div className="flex-1 min-w-0">
                            <p className="font-medium truncate">{cmd.label}</p>
                            {cmd.description && (
                              <p className="text-xs text-muted-foreground truncate">{cmd.description}</p>
                            )}
                          </div>
                        </Command.Item>
                      ))}
                  </Command.Group>
                ))}
              </Command.List>
            </Command>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  )
}

