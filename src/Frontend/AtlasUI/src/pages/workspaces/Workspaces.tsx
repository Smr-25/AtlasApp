import { useEffect, useState } from 'react'
import api, { ApiError } from '@/lib/apiClient'
import { useToast } from '@/hooks/use-toast'
import WorkspaceCard from '@/components/workspaces/WorkspaceCard'
import WorkspaceForm from '@/components/workspaces/WorkspaceForm'

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
      if (e instanceof ApiError) toast({ title: 'Failed to load workspaces', description: e.errors?.join(', ') || e.message })
      else toast({ title: 'Failed to load workspaces', description: 'Unknown error' })
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
      if (e instanceof ApiError) toast({ title: 'Create failed', description: e.errors?.join(', ') || e.message })
      else toast({ title: 'Create failed', description: 'Unknown error' })
      return false
    }
  }

  const handleSetDefault = async (id: string) => {
    try {
      await api.workspaces.setDefault(id)
      toast({ title: 'Default set', description: 'Workspace marked as default' })
      await load()
    } catch (e) {
      if (e instanceof ApiError) toast({ title: 'Operation failed', description: e.errors?.join(', ') || e.message })
      else toast({ title: 'Operation failed', description: 'Unknown error' })
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this workspace?')) return
    try {
      await api.workspaces.delete(id)
      toast({ title: 'Deleted', description: 'Workspace deleted' })
      await load()
    } catch (e) {
      if (e instanceof ApiError) toast({ title: 'Delete failed', description: e.errors?.join(', ') || e.message })
      else toast({ title: 'Delete failed', description: 'Unknown error' })
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
