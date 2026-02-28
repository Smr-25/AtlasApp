import React from 'react'

const MOCK_ASSETS = [
  { id: 'a1', name: 'icon-check.svg', size: '2KB' },
  { id: 'a2', name: 'hero-image.png', size: '1.2MB' },
  { id: 'a3', name: 'logo.svg', size: '8KB' },
]

export default function AssetExporter() {
  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm">
      <div className="flex items-center justify-between mb-2">
        <h4 className="font-semibold">Asset Exporter</h4>
        <div className="text-sm text-muted-foreground">SVG / PNG</div>
      </div>

      <ul className="space-y-2 mb-3">
        {MOCK_ASSETS.map((a) => (
          <li key={a.id} className="flex items-center justify-between p-2 border rounded">
            <div className="font-medium">{a.name}</div>
            <div className="text-sm text-muted-foreground">{a.size}</div>
          </li>
        ))}
      </ul>

      <div className="flex gap-2">
        <button className="btn">Export Selected</button>
        <button className="btn btn-ghost">Open Assets Folder</button>
      </div>
    </div>
  )
}

