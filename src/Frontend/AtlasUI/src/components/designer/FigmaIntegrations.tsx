import React from 'react'

type Comment = {
  id: string
  author: string
  text: string
  time: string
  resolved?: boolean
}

const MOCK_COMMENTS: Comment[] = [
  { id: 'c1', author: 'Zehra', text: 'Needs spacing tweak on header', time: '2h ago' },
  { id: 'c2', author: 'Tarlan', text: "Button color contrast low on mobile", time: '5h ago' },
]

export default function FigmaIntegrations() {
  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm">
      <div className="flex items-center justify-between mb-2">
        <h4 className="font-semibold">Figma Integrations</h4>
        <div className="text-sm text-muted-foreground">Connected</div>
      </div>

      <div className="space-y-3 max-h-48 overflow-auto">
        {MOCK_COMMENTS.map((c) => (
          <div key={c.id} className="p-3 border rounded">
            <div className="flex items-center justify-between">
              <div className="font-medium">{c.author}</div>
              <div className="text-xs text-muted-foreground">{c.time}</div>
            </div>
            <div className="text-sm text-muted-foreground mt-1">{c.text}</div>
            <div className="mt-2 flex gap-2">
              <button className="btn btn-sm">Open in Figma</button>
              <button className="btn btn-ghost btn-sm">Mark resolved</button>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

