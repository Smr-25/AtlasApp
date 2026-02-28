import React from 'react'
import { Integration, MOCK_INTEGRATIONS } from './types'

interface Props {
  open: boolean
  onClose: () => void
  integrations?: Integration[]
}

export default function IntegrationsModal({ open, onClose, integrations }: Props) {
  if (!open) return null
  const items = integrations || MOCK_INTEGRATIONS
  return (
    <div role="dialog" aria-modal="true" className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-black opacity-40" onClick={onClose} />
      <div className="relative w-full max-w-2xl bg-white rounded-lg shadow-lg p-6 z-10">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-semibold">Integrations</h3>
          <button aria-label="Close integrations" className="btn" onClick={onClose}>Close</button>
        </div>
        <div className="space-y-3">
          {items.map(i => (
            <div key={i.id} className="flex items-center justify-between p-3 border rounded">
              <div>
                <div className="font-medium">{i.name}</div>
                <div className="text-sm text-muted-foreground">{i.provider}</div>
              </div>
              <div className="text-sm">
                <span className={i.status === 'connected' ? 'text-green-600' : i.status === 'pending' ? 'text-yellow-600' : 'text-red-600'}>{i.status}</span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}

