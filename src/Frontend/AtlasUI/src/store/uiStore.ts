// UI Store - using vanilla JS singleton (no external dep) for IDE compatibility
// When zustand is installed it works perfectly; this fallback also works.

interface UIState {
  sidebarCollapsed: boolean
  commandPaletteOpen: boolean
  activeTeamId: string | null
  notificationCount: number
  theme: 'light' | 'dark' | 'system'
}

type Listener = () => void

function createStore(initial: UIState) {
  let state = { ...initial }
  const listeners = new Set<Listener>()

  function getState() { return state }

  function setState(partial: Partial<UIState>) {
    state = { ...state, ...partial }
    listeners.forEach(l => l())
  }

  function subscribe(listener: Listener) {
    listeners.add(listener)
    return () => listeners.delete(listener)
  }

  return { getState, setState, subscribe }
}

function getPersistedState(): Partial<UIState> {
  try {
    const raw = localStorage.getItem('atlas-ui-store')
    if (raw) return JSON.parse(raw)
  } catch {}
  return {}
}

const persisted = getPersistedState()

const store = createStore({
  sidebarCollapsed: persisted.sidebarCollapsed ?? false,
  commandPaletteOpen: false,
  activeTeamId: persisted.activeTeamId ?? null,
  notificationCount: 0,
  theme: (persisted.theme as UIState['theme']) ?? 'dark',
})

// Persist on change
store.subscribe(() => {
  const s = store.getState()
  try {
    localStorage.setItem('atlas-ui-store', JSON.stringify({
      sidebarCollapsed: s.sidebarCollapsed,
      theme: s.theme,
      activeTeamId: s.activeTeamId,
    }))
  } catch {}
})

// React hook
import { useState, useEffect } from 'react'

export function useUIStore<T>(selector: (s: UIState & {
  toggleSidebar: () => void
  setSidebarCollapsed: (v: boolean) => void
  openCommandPalette: () => void
  closeCommandPalette: () => void
  setActiveTeamId: (id: string | null) => void
  incrementNotifications: (n?: number) => void
  clearNotifications: () => void
  setTheme: (t: UIState['theme']) => void
}) => T): T {
  const actions = {
    toggleSidebar: () => store.setState({ sidebarCollapsed: !store.getState().sidebarCollapsed }),
    setSidebarCollapsed: (v: boolean) => store.setState({ sidebarCollapsed: v }),
    openCommandPalette: () => store.setState({ commandPaletteOpen: true }),
    closeCommandPalette: () => store.setState({ commandPaletteOpen: false }),
    setActiveTeamId: (id: string | null) => store.setState({ activeTeamId: id }),
    incrementNotifications: (n = 1) => store.setState({ notificationCount: store.getState().notificationCount + n }),
    clearNotifications: () => store.setState({ notificationCount: 0 }),
    setTheme: (t: UIState['theme']) => store.setState({ theme: t }),
  }

  const [value, setValue] = useState(() => selector({ ...store.getState(), ...actions }))

  useEffect(() => {
    const unsub = store.subscribe(() => {
      setValue(selector({ ...store.getState(), ...actions }))
    })
    return () => { unsub() }
  }, [])

  return value
}

export type { UIState }
