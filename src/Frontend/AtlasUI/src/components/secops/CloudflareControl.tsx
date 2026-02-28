import React, { useState } from 'react'

const MOCK_ATTACKS = [
  { id: 'a1', source: '192.0.2.1', type: 'HTTP flood', time: '2m ago' },
  { id: 'a2', source: '198.51.100.23', type: 'BOT traffic', time: '10m ago' },
]

export default function CloudflareControl() {
  const [underAttack, setUnderAttack] = useState(false)

  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm">
      <div className="flex items-center justify-between mb-2">
        <h4 className="font-semibold">Cloudflare / WAF</h4>
        <div className={`text-sm ${underAttack ? 'text-red-600' : 'text-green-600'}`}>
          {underAttack ? 'Under Attack' : 'Normal'}
        </div>
      </div>

      <div className="mb-3">
        <p className="text-sm text-muted-foreground">Quick actions to protect the site</p>
        <div className="mt-2 flex gap-2">
          <button className="btn" onClick={() => setUnderAttack(true)}>Enable Defense Mode</button>
          <button className="btn btn-ghost" onClick={() => setUnderAttack(false)}>Disable Defense Mode</button>
        </div>
      </div>

      <div>
        <div className="text-sm font-medium mb-2">Recent suspicious traffic</div>
        <ul className="space-y-2 max-h-36 overflow-auto">
          {MOCK_ATTACKS.map((it) => (
            <li key={it.id} className="p-2 border rounded flex justify-between items-center">
              <div>
                <div className="font-medium">{it.type}</div>
                <div className="text-xs text-muted-foreground">{it.source}</div>
              </div>
              <div className="text-xs text-muted-foreground">{it.time}</div>
            </li>
          ))}
        </ul>
      </div>
    </div>
  )
}

