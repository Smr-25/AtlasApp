import React from 'react'
import { MOCK_RESOURCES } from './types'

interface Props {
  open: boolean
  onClose: () => void
}

export default function UtilitiesModal({ open, onClose }: Props) {
  if (!open) return null
  return (
    <div role="dialog" aria-modal="true" className="fixed inset-0 z-40 flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-black opacity-30" onClick={onClose} />
      <div className="relative w-full max-w-2xl bg-white rounded-lg shadow-lg p-6 z-10">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-semibold">Utilities</h3>
          <button aria-label="Close utilities" className="btn" onClick={onClose}>Close</button>
        </div>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <button className="p-3 border rounded text-left">Meeting Mode <div className="text-xs text-muted-foreground">Silence notifications and open notes</div></button>
          <button className="p-3 border rounded text-left">Blocked Task Blaster <div className="text-xs text-muted-foreground">Ping owners of blocked tasks</div></button>
          <button className="p-3 border rounded text-left">Bulk Reassign <div className="text-xs text-muted-foreground">Redistribute tasks quickly</div></button>
          <button className="p-3 border rounded text-left">Capacity Calculator <div className="text-xs text-muted-foreground">Estimate team capacity</div></button>
        </div>
      </div>
    </div>
  )
}

