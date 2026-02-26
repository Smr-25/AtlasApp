import { useEffect, useState } from 'react'
import api, { ApiError } from '@/lib/apiClient'
import { useToast } from '@/hooks/use-toast'
import IntegrationCard from '@/components/integrations/IntegrationCard'
import ConnectModal from '@/components/integrations/ConnectModal'

export default function Integrations() {
  const [items, setItems] = useState<any[]>([])
  const [loading, setLoading] = useState(false)
  const [connectFor, setConnectFor] = useState<any | null>(null)
  const { toast } = useToast()

  const load = async () => {
    setLoading(true)
    try {
      const res = await api.integrations.list()
      setItems(res || [])
    } catch (e) {
      if (e instanceof ApiError) toast({ title: 'Failed to load integrations', description: e.errors?.join(', ') || e.message })
      else toast({ title: 'Failed to load integrations', description: 'Unknown error' })
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { load() }, [])

  const handleReconnect = async (id: string) => {
    try {
      await api.integrations.reconnect(id)
      toast({ title: 'Reconnected', description: 'Integration reconnected' })
      await load()
    } catch (e) {
      if (e instanceof ApiError) toast({ title: 'Reconnect failed', description: e.errors?.join(', ') || e.message })
      else toast({ title: 'Reconnect failed', description: 'Unknown error' })
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to disconnect this integration?')) return
    try {
      await api.integrations.delete(id)
      toast({ title: 'Disconnected', description: 'Integration disconnected' })
      await load()
    } catch (e) {
      if (e instanceof ApiError) toast({ title: 'Disconnect failed', description: e.errors?.join(', ') || e.message })
      else toast({ title: 'Disconnect failed', description: 'Unknown error' })
    }
  }

  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-2xl font-semibold">Integrations</h2>
        <div>
          <button onClick={() => setConnectFor({ provider: 'GitHub' })} className="btn btn-primary">Connect GitHub</button>
        </div>
      </div>

      {loading && <div>Loading...</div>}

      {!loading && items.length === 0 && (
        <div className="text-muted">No integrations yet.</div>
      )}

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
        {items.map((it: any) => (
          <IntegrationCard key={it.Id} integration={it} onReconnect={() => handleReconnect(it.Id)} onDelete={() => handleDelete(it.Id)} onConnect={() => setConnectFor(it)} />
        ))}
      </div>

      {connectFor && <ConnectModal provider={connectFor} onClose={() => setConnectFor(null)} onConnected={() => { setConnectFor(null); load() }} />}
    </div>
  )
}
