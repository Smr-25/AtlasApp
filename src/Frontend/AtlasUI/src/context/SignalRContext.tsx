import {
  createContext,
  useContext,
  useEffect,
  useRef,
  ReactNode,
} from 'react'
import { signalRManager, setTokenAccessor } from '@/lib/signalr'
import { getTokens } from '@/lib/apiClient'
import { useAuth } from '@/context/AuthContext'
import { useUIStore } from '@/store/uiStore'
import { toast } from 'sonner'

// Wire up token accessor once
setTokenAccessor(getTokens)

interface SignalRContextType {
  joinTeam: (teamId: string) => Promise<void>
  leaveTeam: (teamId: string) => Promise<void>
  isConnected: boolean
}

const SignalRContext = createContext<SignalRContextType>({
  joinTeam: async () => {},
  leaveTeam: async () => {},
  isConnected: false,
})

export const useSignalR = () => useContext(SignalRContext)

export const SignalRProvider = ({ children }: { children: ReactNode }) => {
  const { isAuthenticated } = useAuth()
  const incrementNotifications = useUIStore((s) => s.incrementNotifications)
  const connectedRef = useRef(false)

  useEffect(() => {
    if (!isAuthenticated) {
      if (connectedRef.current) {
        signalRManager.disconnect()
        connectedRef.current = false
      }
      return
    }

    // Connect
    signalRManager.connect().then(() => {
      connectedRef.current = true
    })

    // Register global event listeners
    const unsubAlert = signalRManager.on('ReceiveAlert', (data) => {
      incrementNotifications()
      toast.warning(data.alertType, {
        description: typeof data.payload === 'object'
          ? JSON.stringify(data.payload).slice(0, 100)
          : String(data.payload),
      })
    })

    const unsubJob = signalRManager.on('JobCompleted', (data) => {
      toast.success(`Job completed: ${data.jobType}`, {
        description: typeof data.payload === 'object'
          ? (data.payload as any)?.message || ''
          : String(data.payload),
      })
    })

    const unsubFeed = signalRManager.on('FeedUpdated', (_data) => {
      // Query invalidation will be handled in individual components
    })

    return () => {
      unsubAlert()
      unsubJob()
      unsubFeed()
    }
  }, [isAuthenticated])

  return (
    <SignalRContext.Provider
      value={{
        joinTeam: (teamId) => signalRManager.joinTeam(teamId),
        leaveTeam: (teamId) => signalRManager.leaveTeam(teamId),
        isConnected: signalRManager.isConnected,
      }}
    >
      {children}
    </SignalRContext.Provider>
  )
}
