import React from 'react'

interface PR { id: string; title: string; author: string; status: 'open' | 'reviewed' | 'merged' }
interface Props { prs?: PR[] }

export default function GitHubPRs({ prs }: Props) {
  const items = prs || [
    { id: 'p1', title: 'Fix cart bug', author: 'ali', status: 'open' },
    { id: 'p2', title: 'Refactor auth', author: 'aysel', status: 'reviewed' },
  ]
  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm">
      <h4 className="font-semibold mb-3">GitHub Pull Requests</h4>
      <ul className="space-y-3">
        {items.map(p => (
          <li key={p.id} className="flex items-center justify-between">
            <div>
              <div className="font-medium">{p.title}</div>
              <div className="text-xs text-muted-foreground">{p.author} • {p.status}</div>
            </div>
            <div className="flex items-center gap-2">
              <button className="btn btn-sm">Approve</button>
              <button className="btn btn-ghost btn-sm">Request Changes</button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  )
}

