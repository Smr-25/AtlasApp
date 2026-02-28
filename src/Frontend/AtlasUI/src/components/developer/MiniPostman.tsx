import React, { useState } from 'react'

export default function MiniPostman() {
  const [url, setUrl] = useState('https://api.example.test/health')
  const [method, setMethod] = useState<'GET' | 'POST' | 'PUT' | 'DELETE'>('GET')
  const [body, setBody] = useState('')
  const [response, setResponse] = useState<string | null>(null)

  const send = async () => {
    // For now, don't call network — show mock response for UI testing
    setResponse(JSON.stringify({ ok: true, url, method, receivedBody: body || null }, null, 2))
  }

  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm">
      <h4 className="font-semibold mb-3">Mini-Postman</h4>
      <div className="space-y-2">
        <input className="input w-full" value={url} onChange={e => setUrl(e.target.value)} />
        <div className="flex gap-2">
          <select className="select" value={method} onChange={e => setMethod(e.target.value as any)}>
            <option>GET</option>
            <option>POST</option>
            <option>PUT</option>
            <option>DELETE</option>
          </select>
          <button className="btn" onClick={send}>Send</button>
        </div>
        <textarea className="textarea w-full" rows={4} value={body} onChange={e => setBody(e.target.value)} />
        {response && (
          <pre className="mt-2 p-2 bg-gray-50 border rounded text-xs overflow-auto">{response}</pre>
        )}
      </div>
    </div>
  )
}

