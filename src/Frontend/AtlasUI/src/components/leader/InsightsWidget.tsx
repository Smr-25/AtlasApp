import React from 'react'

interface Props { summary?: { totalIssues?: number; velocity?: number; reviewTurnaround?: number } }

export default function InsightsWidget({ summary }: Props) {
  const s = summary || { totalIssues: 12, velocity: 34, reviewTurnaround: 18 }
  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm">
      <h4 className="font-semibold mb-3">Insights</h4>
      <div className="grid grid-cols-3 gap-3">
        <div className="p-3 border rounded text-center">
          <div className="text-sm text-muted-foreground">Open Issues</div>
          <div className="text-xl font-bold">{s.totalIssues}</div>
        </div>
        <div className="p-3 border rounded text-center">
          <div className="text-sm text-muted-foreground">Sprint Velocity</div>
          <div className="text-xl font-bold">{s.velocity}</div>
        </div>
        <div className="p-3 border rounded text-center">
          <div className="text-sm text-muted-foreground">Review (hrs)</div>
          <div className="text-xl font-bold">{s.reviewTurnaround}</div>
        </div>
      </div>
    </div>
  )
}

