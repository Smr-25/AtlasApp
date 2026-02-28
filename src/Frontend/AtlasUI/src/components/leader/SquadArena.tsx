import React from 'react'

export type SquadArenaComparison = { id: string; left: { name: string; score: number }; right: { name: string; score: number }; winner?: 'left' | 'right' | 'tie' }
interface Props { comparisons: SquadArenaComparison[] }

export default function SquadArena({ comparisons }: Props) {
  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm">
      <h4 className="font-semibold mb-3">Squad Arena</h4>
      <ul className="space-y-3">
        {comparisons.map(c => (
          <li key={c.id} className="p-3 border rounded flex items-center justify-between">
            <div className="flex items-center gap-4">
              <div className="text-sm font-medium">{c.left.name}</div>
              <div className="text-sm">{c.left.score}</div>
              <div className="text-sm text-muted-foreground">vs</div>
              <div className="text-sm font-medium">{c.right.name}</div>
              <div className="text-sm">{c.right.score}</div>
            </div>
            <div className="font-semibold">{c.winner || '—'}</div>
          </li>
        ))}
      </ul>
    </div>
  )
}
