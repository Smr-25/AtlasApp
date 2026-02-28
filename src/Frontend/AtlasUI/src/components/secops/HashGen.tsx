import React, { useState } from 'react'

export default function HashGen() {
  const [input, setInput] = useState('')
  const [md5, setMd5] = useState('')
  const [sha256, setSha256] = useState('')

  function generate() {
    // Mock deterministic simple hash placeholders for UI demo
    setMd5('d41d8cd98f00b204e9800998ecf8427e')
    setSha256('e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855')
  }

  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm">
      <div className="flex items-center justify-between mb-2">
        <h4 className="font-semibold">Hash Generator</h4>
        <div className="text-sm text-muted-foreground">MD5 / SHA-256</div>
      </div>

      <div className="mb-3">
        <input value={input} onChange={(e)=>setInput(e.target.value)} placeholder="Paste text or file hash" className="input w-full" />
      </div>

      <div className="mb-3 flex gap-2">
        <button className="btn" onClick={generate}>Generate</button>
        <button className="btn btn-ghost" onClick={()=>{ setInput(''); setMd5(''); setSha256('') }}>Clear</button>
      </div>

      <div className="text-sm">
        <div className="mb-1">MD5:</div>
        <div className="p-2 border rounded bg-gray-50 break-words">{md5}</div>
        <div className="mt-2 mb-1">SHA-256:</div>
        <div className="p-2 border rounded bg-gray-50 break-words">{sha256}</div>
      </div>
    </div>
  )
}

