import { useEffect, useMemo, useState } from 'react'
import api from '../lib/apiClient'
import { useAuth } from '../context/AuthContext'

type FeatureFlags = Record<string, boolean>

export function useFeatureFlags() {
  const { user } = useAuth()
  const [primary, setPrimary] = useState(user?.role ?? 'developer')
  const [flags, setFlags] = useState<FeatureFlags | null>(null)
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if (user?.role) setPrimary(user.role)
  }, [user?.role])

  useEffect(() => {
    let mounted = true
    const load = async () => {
      if (flags !== null) return
      setLoading(true)
      try {
        const profile = await api.profiles.me()
        if (!mounted) return
        const ff: FeatureFlags = (profile?.FeatureFlags as FeatureFlags) ?? (profile?.featureFlags as FeatureFlags) ?? {}
        setFlags(ff)
      } catch (e) {
        setFlags({})
      } finally {
        if (mounted) setLoading(false)
      }
    }
    load()
    return () => {
      mounted = false
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const can = useMemo(() => {
    return (featureKey: string) => {
      if (!flags) return false
      return Boolean(flags[featureKey])
    }
  }, [flags])

  return { primary, featureFlags: flags ?? {}, can, loading }
}

export default useFeatureFlags

