import React from 'react'
import { FeedItem } from './types'

interface Props { items: FeedItem[] }

export default function OmniFeed({ items }: Props) {
  return (
    <div className="p-4 rounded-lg bg-white border shadow-sm h-full">
      <h4 className="font-semibold mb-3">Omni-Feed</h4>
      <div className="space-y-3 max-h-[60vh] overflow-auto">
        {items.map(it => (
          <div key={it.id} className="p-3 border rounded">
            <div className="flex items-center justify-between">
              <div className="font-medium">{it.title}</div>
              <div className="text-xs text-muted-foreground">{it.time}</div>
            </div>
            {it.description && <div className="text-sm text-muted-foreground mt-1">{it.description}</div>}
          </div>
        ))}
      </div>
    </div>
  )
}

