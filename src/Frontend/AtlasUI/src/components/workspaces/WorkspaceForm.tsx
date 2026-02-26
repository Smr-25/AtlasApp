import { useState } from 'react'

export default function WorkspaceForm({ onClose, onCreate }: any) {
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e: any) => {
    e.preventDefault()
    if (!name) return alert('Name is required')
    setLoading(true)
    try {
      const ok = await onCreate({ Name: name, Description: description })
      if (ok) onClose()
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded w-full max-w-md">
        <h3 className="text-lg font-medium mb-4">Create Workspace</h3>
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label className="block text-sm mb-1">Name</label>
            <input className="input w-full" value={name} onChange={(e) => setName(e.target.value)} />
          </div>
          <div className="mb-3">
            <label className="block text-sm mb-1">Description</label>
            <input className="input w-full" value={description} onChange={(e) => setDescription(e.target.value)} />
          </div>
          <div className="flex justify-end space-x-2">
            <button type="button" className="btn" onClick={onClose} disabled={loading}>Cancel</button>
            <button type="submit" className="btn btn-primary" disabled={loading}>{loading ? 'Creating...' : 'Create'}</button>
          </div>
        </form>
      </div>
    </div>
  )
}

