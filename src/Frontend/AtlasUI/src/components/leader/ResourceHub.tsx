import React from 'react'
import { ResourceItem } from './types'

interface Props { open: boolean; onClose: () => void; resources: ResourceItem[] }

export default function ResourceHub({ open, onClose, resources }: Props) {
  if (!open) return null
  return (
    <div role="dialog" aria-modal="true" className="fixed inset-0 z-40 flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-black opacity-30" onClick={onClose} />
      <div className="relative w-full max-w-2xl bg-white rounded-lg shadow-lg p-6 z-10">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-semibold">Resource Hub</h3>
          <button aria-label="Close resources" className="btn" onClick={onClose}>Close</button>
        </div>
        <ul className="space-y-3">
          {resources.map(r => (
            <li key={r.id} className="p-3 border rounded">
              <a href={r.url} target="_blank" rel="noreferrer" className="font-medium">{r.title}</a>
              {r.summary && <div className="text-sm text-muted-foreground">{r.summary}</div>}
            </li>
          ))}
        </ul>
      </div>
    </div>
  )
}

