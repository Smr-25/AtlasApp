import { useEffect, useState } from 'react'
import api, { ApiError } from '@/lib/apiClient'
import { useToast } from '@/hooks/use-toast'
import WorkspaceCard from '@/components/workspaces/WorkspaceCard'
import WorkspaceForm from '@/components/workspaces/WorkspaceForm'
import { formatApiError } from '@/lib/errorUtils'

export default function Workspaces() {
  const [workspaces, setWorkspaces] = useState<any[]>([])
  const [loading, setLoading] = useState(false)
  const [showCreate, setShowCreate] = useState(false)
  const { toast } = useToast()

  const load = async () => {
    setLoading(true)
    try {
      const res = await api.workspaces.list()
      setWorkspaces(res || [])
    } catch (e) {
      const fe = formatApiError(e, 'Failed to load workspaces')
      toast({ title: fe.title, description: fe.message })
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { load() }, [])

  const handleCreate = async (payload: { Name: string }) => {
    try {
      const res = await api.workspaces.create(payload as any)
      toast({ title: 'Workspace created', description: 'Workspace created successfully' })
      setShowCreate(false)
      await load()
      return true
    } catch (e) {
      const fe = formatApiError(e, 'Create failed')
      toast({ title: fe.title, description: fe.message })
      return false
    }
  }

  const handleSetDefault = async (id: string) => {
    try {
      await api.workspaces.setDefault(id)
      toast({ title: 'Default set', description: 'Workspace marked as default' })
      await load()
    } catch (e) {
      const fe = formatApiError(e, 'Operation failed')
      toast({ title: fe.title, description: fe.message })
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this workspace?')) return
    try {
      await api.workspaces.delete(id)
      toast({ title: 'Deleted', description: 'Workspace deleted' })
      await load()
    } catch (e) {
      const fe = formatApiError(e, 'Delete failed')
      toast({ title: fe.title, description: fe.message })
    }
  }

  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-2xl font-semibold">Workspaces</h2>
        <div>
          <button onClick={() => setShowCreate(true)} className="btn btn-primary">Create workspace</button>
        </div>
      </div>

      {loading && <div>Loading...</div>}

      {!loading && workspaces.length === 0 && (
        <div className="text-muted">No workspaces yet. Create one to get started.</div>
      )}

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
        {workspaces.map((w: any) => (
          <WorkspaceCard key={w.Id} workspace={w} onSetDefault={() => handleSetDefault(w.Id)} onDelete={() => handleDelete(w.Id)} />
        ))}
      </div>

      {showCreate && <WorkspaceForm onClose={() => setShowCreate(false)} onCreate={handleCreate} />}
    </div>
  )
}
