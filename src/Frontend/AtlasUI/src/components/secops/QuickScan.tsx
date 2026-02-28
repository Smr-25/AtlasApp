import React, { useState } from 'react'

const MOCK_RESULTS = [
  { id: 'r1', ip: '192.168.1.45', vendor: 'Apple', status: 'known' },
  { id: 'r2', ip: '192.168.1.83', vendor: 'Unknown', status: 'suspicious' },
]

export default function QuickScan() {
  const [running, setRunning] = useState(false)
  const [results, setResults] = useState(MOCK_RESULTS)

  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm">
      <div className="flex items-center justify-between mb-2">
        <h4 className="font-semibold">Quick Scan</h4>
        <div className="text-sm text-muted-foreground">Network</div>
      </div>

      <div className="mb-3">
        <p className="text-sm text-muted-foreground">Scan local network for unknown devices</p>
        <div className="mt-2 flex gap-2">
          <button className="btn" onClick={() => { setRunning(true); setTimeout(()=> setRunning(false), 1200) }}>Run Scan</button>
          <button className="btn btn-ghost">Export Report</button>
        </div>
      </div>

      <div>
        <ul className="space-y-2 max-h-36 overflow-auto">
          {results.map((r) => (
            <li key={r.id} className="p-2 border rounded flex justify-between items-center">
              <div>
                <div className="font-medium">{r.ip}</div>
                <div className="text-xs text-muted-foreground">{r.vendor}</div>
              </div>
              <div className={`${r.status === 'suspicious' ? 'text-red-600' : 'text-green-600'} text-sm`}>{r.status}</div>
            </li>
          ))}
        </ul>
      </div>
    </div>
  )
}

