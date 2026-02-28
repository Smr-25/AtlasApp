import React from 'react'
import { SquadSummary } from './types'

interface Props { squads: SquadSummary[] }

export default function SquadRadar({ squads }: Props) {
  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm">
      <h4 className="font-semibold mb-3">Squad Radar</h4>
      <ul className="space-y-3">
        {squads.map(s => (
          <li key={s.id} className="flex items-center justify-between">
            <div>
              <div className="font-medium">{s.name}</div>
              <div className="text-xs text-muted-foreground">{s.focus}</div>
            </div>
            <div className="flex items-center gap-2">
              <div className={s.health === 'green' ? 'text-green-600' : s.health === 'yellow' ? 'text-yellow-600' : 'text-red-600'}>
                {s.health}
              </div>
              <div className="text-sm font-semibold">{s.score}</div>
            </div>
          </li>
        ))}
      </ul>
    </div>
  )
}

