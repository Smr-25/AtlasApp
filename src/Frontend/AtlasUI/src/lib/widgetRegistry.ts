import React from 'react'
import { UserRole } from '@/context/AuthContext'

export type WidgetMeta = {
  id: string
  title: string
  rolesAllowed?: UserRole[]
  featureKey?: string
  load: () => Promise<{ default: React.ComponentType<any> }>
  preload?: boolean
}

const registry: Record<string, WidgetMeta> = {
  heatmap: {
    id: 'heatmap',
    title: 'Focus Heatmap',
    rolesAllowed: ['developer', 'team-leader'],
    featureKey: 'dev:heatmap',
    load: () => import('../components/dashboard/HeatmapWidget'),
    preload: false,
  },
}

export function registerWidget(meta: WidgetMeta) {
  registry[meta.id] = meta
}

export function getWidget(id: string) {
  return registry[id]
}

export function listWidgets() {
  return Object.values(registry)
}

export default { registerWidget, getWidget, listWidgets }

