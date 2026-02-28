import { ReactNode } from 'react'
import AppSidebar from './AppSidebar'
import AppTopNav from './AppTopNav'
import CommandPalette from '@/components/layout/CommandPalette'
import { useUIStore } from '@/store/uiStore'

interface AppShellProps {
  children: ReactNode
}

const AppShell = ({ children }: AppShellProps) => {
  const commandPaletteOpen = useUIStore(s => s.commandPaletteOpen)
  const closeCommandPalette = useUIStore(s => s.closeCommandPalette)

  return (
    <div className="flex h-screen bg-background overflow-hidden">
      <AppSidebar />

      <div className="flex flex-col flex-1 min-w-0 overflow-hidden">
        <AppTopNav />

        <main className="flex-1 overflow-y-auto p-6">
          {children}
        </main>
      </div>

      <CommandPalette open={commandPaletteOpen} onClose={closeCommandPalette} />
    </div>
  )
}

export default AppShell

