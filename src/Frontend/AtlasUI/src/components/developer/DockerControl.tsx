import React from 'react'

interface Container { id: string; name: string; status: 'running' | 'stopped' }
interface Props { containers?: Container[] }

export default function DockerControl({ containers }: Props) {
  const items = containers || [
    { id: 'c1', name: 'db', status: 'running' },
    { id: 'c2', name: 'worker', status: 'stopped' },
  ]
  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm">
      <h4 className="font-semibold mb-3">Docker Control</h4>
      <ul className="space-y-3">
        {items.map(c => (
          <li key={c.id} className="flex items-center justify-between">
            <div>
              <div className="font-medium">{c.name}</div>
              <div className="text-xs text-muted-foreground">{c.status}</div>
            </div>
            <div className="flex items-center gap-2">
              <button className="btn btn-sm">Start</button>
              <button className="btn btn-sm">Stop</button>
              <button className="btn btn-ghost btn-sm">Logs</button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  )
}

