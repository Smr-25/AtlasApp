// SignalR Manager - wraps @microsoft/signalr with graceful fallback
// if package not yet installed in node_modules

import api from './apiClient'

type SignalREventMap = {
  ReceiveAlert: (data: { alertType: string; payload: object; timestamp: string }) => void
  PresenceUpdated: (data: { payload: object; timestamp: string }) => void
  FocusStateChanged: (data: { payload: object; timestamp: string }) => void
  JobCompleted: (data: { jobType: string; payload: object; timestamp: string }) => void
  FeedUpdated: (data: { eventType: string; payload: object; timestamp: string }) => void
}

type EventName = keyof SignalREventMap
type Handler<E extends EventName> = SignalREventMap[E]

// Lazy import to avoid module-not-found errors at parse time
let _signalRModule: any = null
async function getSignalR() {
  if (_signalRModule) return _signalRModule
  try {
    _signalRModule = await import('@microsoft/signalr')
    return _signalRModule
  } catch {
    return null
  }
}

// Token accessor - injected from apiClient
export let getTokens: () => { accessToken?: string | null } = () => ({ accessToken: null })
export function setTokenAccessor(fn: () => { accessToken?: string | null; refreshToken?: string }) {
  getTokens = fn
}

const HUB_URL = `${(import.meta as any).env?.VITE_API_BASE || 'http://localhost:5075'}/hubs/atlas`

class AtlasSignalRManager {
  private connection: any = null
  private listeners: Map<string, Set<(...args: any[]) => void>> = new Map()
  private joinedTeams: Set<string> = new Set()
  private _connected = false

  async connect(): Promise<void> {
    const sr = await getSignalR()
    if (!sr) return // signalr not installed yet, silently skip

    // Don't try to connect if we don't have an access token (avoids 401 attempts)
    const token = getTokens()?.accessToken
    if (!token) {
      // console.debug('[SignalR] No access token available, skipping connect')
      return
    }

    if (this.connection && this.connection.state !== sr.HubConnectionState.Disconnected) return

    this.connection = new sr.HubConnectionBuilder()
      .withUrl(HUB_URL, {
        accessTokenFactory: () => getTokens().accessToken || '',
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(
        (import.meta as any).env?.DEV ? sr.LogLevel.Information : sr.LogLevel.Warning
      )
      .build()

    // Register buffered listeners
    this.listeners.forEach((handlers, event) => {
      handlers.forEach(handler => this.connection.on(event, handler))
    })

    this.connection.onreconnected(async () => {
      for (const teamId of this.joinedTeams) {
        try { await this.connection?.invoke('JoinTeam', teamId) } catch {}
      }
    })

    try {
      await this.connection.start()
      this._connected = true
    } catch (e: any) {
      // If server returned 401/Unauthorized during negotiate/start, clear tokens so app can fall back to login flow
      try {
        const msg = e && (e.message || String(e))
        const statusHint = (e && (e.status || e.Status || e.code)) || ''
        console.warn('[SignalR] Connection failed:', msg, statusHint)
        // crude detection for unauthorized/401
        if (msg && /401|Unauthorized|authorization/i.test(msg)) {
          try { api.clearTokens() } catch {}
        }
      } catch (inner) {}
      this._connected = false
    }
  }

  async disconnect(): Promise<void> {
    try { await this.connection?.stop() } catch {}
    this.connection = null
    this._connected = false
    this.joinedTeams.clear()
  }

  on<E extends EventName>(event: E, handler: Handler<E>): () => void {
    if (!this.listeners.has(event)) this.listeners.set(event, new Set())
    this.listeners.get(event)!.add(handler as any)
    this.connection?.on(event, handler as any)
    return () => this.off(event, handler)
  }

  off<E extends EventName>(event: E, handler: Handler<E>): void {
    this.listeners.get(event)?.delete(handler as any)
    this.connection?.off(event, handler as any)
  }

  async joinTeam(teamId: string): Promise<void> {
    this.joinedTeams.add(teamId)
    try { await this.connection?.invoke('JoinTeam', teamId) } catch {}
  }

  async leaveTeam(teamId: string): Promise<void> {
    this.joinedTeams.delete(teamId)
    try { await this.connection?.invoke('LeaveTeam', teamId) } catch {}
  }

  get isConnected(): boolean { return this._connected }
}

export const signalRManager = new AtlasSignalRManager()
export default signalRManager

