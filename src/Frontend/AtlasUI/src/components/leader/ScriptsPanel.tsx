import React from 'react'
import { ScriptItem, MOCK_SCRIPTS } from './types'

interface Props {
  open: boolean
  onClose: () => void
  scripts?: ScriptItem[]
}

export default function ScriptsPanel({ open, onClose, scripts }: Props) {
  if (!open) return null
  const items = scripts || MOCK_SCRIPTS
  return (
    <div role="dialog" aria-modal="true" className="fixed inset-0 z-40 flex items-end md:items-center justify-center p-4">
      <div className="absolute inset-0 bg-black opacity-30" onClick={onClose} />
      <div className="relative w-full max-w-xl bg-white rounded-t-lg md:rounded-lg shadow-lg p-6 z-10">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-semibold">Automation Scripts</h3>
          <button aria-label="Close scripts" className="btn" onClick={onClose}>Close</button>
        </div>
        <ul className="space-y-3">
          {items.map(s => (
            <li key={s.id} className="flex items-center justify-between p-3 border rounded">
              <div>
                <div className="font-medium">{s.name}</div>
                <div className="text-sm text-muted-foreground">{s.description}</div>
              </div>
              <div className="flex items-center gap-2">
                <div className="text-sm text-muted-foreground">{s.lastRun}</div>
                <button className="btn btn-sm">Run</button>
              </div>
            </li>
          ))}
        </ul>
      </div>
    </div>
  )
}

