import React, { Suspense, useEffect, useState } from 'react'
import { getWidget } from '../../lib/widgetRegistry'
import useFeatureFlags from '../../hooks/useFeatureFlags'
import { useRole as useRoleContext } from '../../context/RoleContext'

const Fallback = ({ title }: { title?: string }) => (
  <div className="bg-card rounded-2xl border border-border p-5">{title ? <h3 className="text-sm font-semibold">{title}</h3> : null}<div className="h-24 mt-2 bg-muted rounded" /></div>
)

export const WidgetLoader = ({ id }: { id: string }) => {
  const meta = getWidget(id)
  const { primary } = useRoleContext()
  const { can } = useFeatureFlags()
  const [Component, setComponent] = useState<React.ComponentType<any> | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let mounted = true
    if (!meta) {
      setError('Widget not found')
      return
    }
    // role check
    if (meta.rolesAllowed && !meta.rolesAllowed.includes(primary as any)) {
      setError('Not allowed')
      return
    }
    if (meta.featureKey && !can(meta.featureKey)) {
      setError('Unavailable')
      return
    }
    meta
      .load()
      .then((m) => {
        if (!mounted) return
        setComponent(() => m.default)
      })
      .catch((e) => {
        setError(String(e?.message || e))
      })

    return () => {
      mounted = false
    }
  }, [id, meta, primary])

  if (error) return <div className="text-xs text-muted-foreground">{error}</div>
  if (!Component) return <Fallback title={meta?.title} />

  return (
    <Suspense fallback={<Fallback title={meta?.title} />}>
      <Component />
    </Suspense>
  )
}

export default WidgetLoader
