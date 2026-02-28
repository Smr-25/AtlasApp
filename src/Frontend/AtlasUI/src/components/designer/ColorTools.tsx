import React from 'react'

const MOCK_PALETTE = ['#1f2937', '#3b82f6', '#f97316', '#10b981', '#f43f5e']

export default function ColorTools() {
  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm">
      <div className="flex items-center justify-between mb-2">
        <h4 className="font-semibold">Color Tools</h4>
        <div className="text-sm text-muted-foreground">Palette</div>
      </div>

      <div className="flex gap-2 mb-3">
        {MOCK_PALETTE.map((c) => (
          <div key={c} className="w-10 h-10 rounded" style={{ background: c }} title={c} />
        ))}
      </div>

      <div>
        <div className="text-sm text-muted-foreground mb-1">Contrast check</div>
        <div className="p-3 border rounded bg-gray-50">
          <div className="text-sm">#3b82f6 on #ffffff: PASS</div>
          <div className="text-sm">#f43f5e on #ffffff: FAIL</div>
        </div>
      </div>
    </div>
  )
}

