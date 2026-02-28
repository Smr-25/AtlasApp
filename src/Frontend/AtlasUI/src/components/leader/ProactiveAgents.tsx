import React from 'react'
import { AgentStatus } from './types'

interface Props { agents: AgentStatus[] }

export default function ProactiveAgents({ agents }: Props) {
  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm">
      <h4 className="font-semibold mb-3">Proactive Agents</h4>
      <ul className="space-y-2">
        {agents.map(a => (
          <li key={a.id} className="flex items-start justify-between">
            <div>
              <div className="font-medium">{a.name}</div>
              {a.note && <div className="text-xs text-muted-foreground">{a.note}</div>}
            </div>
            <div className={a.status === 'alert' ? 'text-red-600' : a.status === 'active' ? 'text-green-600' : 'text-muted-foreground'}>
              {a.status}
            </div>
          </li>
        ))}
      </ul>
    </div>
  )
}

