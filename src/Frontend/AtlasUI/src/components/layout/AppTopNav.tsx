import { useState, useEffect, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import { motion, AnimatePresence } from 'framer-motion'
import {
  Bell,
  Sun,
  Moon,
  Search,
  Settings,
  LogOut,
  User,
  ChevronDown,
  Command,
} from 'lucide-react'
import { useAuth } from '@/context/AuthContext'
import { useTheme } from '@/context/ThemeContext'
import { useUIStore } from '@/store/uiStore'
import { greeting } from '@/lib/apiClient'

const AppTopNav = () => {
  const { user, logout } = useAuth()
  const { theme, toggleTheme } = useTheme()
  const notificationCount = useUIStore(s => s.notificationCount)
  const clearNotifications = useUIStore(s => s.clearNotifications)
  const openCommandPalette = useUIStore(s => s.openCommandPalette)
  const navigate = useNavigate()
  const [dropdownOpen, setDropdownOpen] = useState(false)
  const [greetingText, setGreetingText] = useState<string>('')
  const dropdownRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (user?.fullName) {
      const offset = new Date().getTimezoneOffset()
      greeting.get(user.fullName, offset, 'en')
        .then((msg) => setGreetingText(typeof msg === 'string' ? msg : ''))
        .catch(() => setGreetingText(`Hello, ${user.fullName}!`))
    }
  }, [user?.fullName])

  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
        setDropdownOpen(false)
      }
    }
    document.addEventListener('mousedown', handler)
    return () => document.removeEventListener('mousedown', handler)
  }, [])

  // ⌘K shortcut
  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
        e.preventDefault()
        openCommandPalette()
      }
    }
    document.addEventListener('keydown', handler)
    return () => document.removeEventListener('keydown', handler)
  }, [openCommandPalette])

  return (
    <motion.header
      initial={{ opacity: 0, y: -8 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
      className="h-16 border-b border-border bg-card/80 backdrop-blur-sm flex items-center justify-between px-6 shrink-0 sticky top-0 z-20"
    >
      {/* Left - Greeting */}
      <div className="hidden md:flex flex-col justify-center">
        {greetingText ? (
          <p className="text-sm font-medium text-foreground">{greetingText}</p>
        ) : (
          <p className="text-sm font-medium text-foreground">ATLAS Dashboard</p>
        )}
        <p className="text-xs text-muted-foreground">
          {new Date().toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric' })}
        </p>
      </div>

      {/* Center - Search / Command Palette trigger */}
      <div className="flex-1 max-w-sm mx-6">
        <motion.button
          whileHover={{ scale: 1.01 }}
          onClick={openCommandPalette}
          className="w-full h-9 flex items-center gap-2 px-3 rounded-xl bg-muted/60 border border-border text-xs text-muted-foreground hover:bg-muted hover:border-primary/30 transition-all"
        >
          <Search className="w-3.5 h-3.5" />
          <span className="flex-1 text-left">Search or run commands...</span>
          <kbd className="hidden sm:inline-flex items-center gap-0.5 text-[10px] bg-background border border-border rounded px-1.5 py-0.5">
            <Command className="w-2.5 h-2.5" />K
          </kbd>
        </motion.button>
      </div>

      {/* Right - Actions */}
      <div className="flex items-center gap-1">
        <motion.button
          whileTap={{ rotate: 180 }}
          onClick={toggleTheme}
          className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-primary transition-colors"
          title="Toggle theme"
        >
          {theme === 'light' ? <Moon className="w-4 h-4" /> : <Sun className="w-4 h-4" />}
        </motion.button>

        <button
          onClick={() => navigate('/settings')}
          className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-foreground transition-colors"
          title="Settings"
        >
          <Settings className="w-4 h-4" />
        </button>

        <button
          onClick={() => { clearNotifications(); navigate('/notifications') }}
          className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-foreground transition-colors relative"
          title="Notifications"
        >
          <Bell className="w-4 h-4" />
          <AnimatePresence>
            {notificationCount > 0 && (
              <motion.span
                initial={{ scale: 0 }}
                animate={{ scale: 1 }}
                exit={{ scale: 0 }}
                className="absolute -top-0.5 -right-0.5 min-w-[16px] h-4 bg-red-500 text-white text-[9px] font-bold rounded-full flex items-center justify-center px-1"
              >
                {notificationCount > 99 ? '99+' : notificationCount}
              </motion.span>
            )}
          </AnimatePresence>
        </button>

        {/* User dropdown */}
        <div className="relative ml-1" ref={dropdownRef}>
          <motion.button
            whileHover={{ scale: 1.05 }}
            onClick={() => setDropdownOpen((o) => !o)}
            className="flex items-center gap-1.5 pl-1 pr-2 py-1 rounded-xl hover:bg-muted transition-colors"
          >
            <div className="w-7 h-7 rounded-full bg-gradient-to-br from-primary to-primary/60 flex items-center justify-center">
              <span className="text-[11px] font-bold text-primary-foreground">
                {user?.fullName?.[0]?.toUpperCase() ?? 'U'}
              </span>
            </div>
            <ChevronDown className={`w-3 h-3 text-muted-foreground transition-transform ${dropdownOpen ? 'rotate-180' : ''}`} />
          </motion.button>

          <AnimatePresence>
            {dropdownOpen && (
              <motion.div
                initial={{ opacity: 0, y: 8, scale: 0.95 }}
                animate={{ opacity: 1, y: 0, scale: 1 }}
                exit={{ opacity: 0, y: 8, scale: 0.95 }}
                transition={{ duration: 0.15 }}
                className="absolute right-0 top-full mt-2 w-52 bg-popover border border-border rounded-xl shadow-xl z-50 overflow-hidden"
              >
                <div className="px-3 py-2.5 border-b border-border">
                  <p className="text-sm font-semibold text-foreground truncate">{user?.fullName}</p>
                  <p className="text-xs text-muted-foreground truncate">{user?.email}</p>
                </div>
                <div className="p-1">
                  <button
                    onClick={() => { navigate('/profile'); setDropdownOpen(false) }}
                    className="w-full flex items-center gap-2 px-3 py-2 text-sm rounded-lg hover:bg-muted text-foreground transition-colors"
                  >
                    <User className="w-4 h-4" />
                    Profile
                  </button>
                  <button
                    onClick={() => { navigate('/subscription'); setDropdownOpen(false) }}
                    className="w-full flex items-center gap-2 px-3 py-2 text-sm rounded-lg hover:bg-muted text-foreground transition-colors"
                  >
                    <Settings className="w-4 h-4" />
                    Subscription
                  </button>
                  <div className="my-1 border-t border-border" />
                  <button
                    onClick={() => { logout(); setDropdownOpen(false) }}
                    className="w-full flex items-center gap-2 px-3 py-2 text-sm rounded-lg hover:bg-red-500/10 text-red-500 transition-colors"
                  >
                    <LogOut className="w-4 h-4" />
                    Sign out
                  </button>
                </div>
              </motion.div>
            )}
          </AnimatePresence>
        </div>
      </div>
    </motion.header>
  )
}

export default AppTopNav

