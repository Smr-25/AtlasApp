import { useEffect, useMemo, useState } from 'react'
import api from '../lib/apiClient'
import { useAuth, UserRole } from '../context/AuthContext'

// A lightweight hook that exposes the user's primary role, fetched feature flags and a simple permission check.
// It tries to reuse data from AuthContext (user.role). If feature flags aren't available locally, it will
// fetch profile details from /api/profiles/me (if authorized) and cache them for the session.

type FeatureFlags = Record<string, boolean>

export function useRole() {
  const { user } = useAuth()
  const [primary, setPrimary] = useState<UserRole>(() => (user?.role as UserRole) ?? 'developer')
  const [flags, setFlags] = useState<FeatureFlags | null>(null)
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if (user?.role) setPrimary(user.role)
  }, [user?.role])

  useEffect(() => {
    let mounted = true
    const load = async () => {
      // If we already have flags or user lacks a token, skip
      if (flags !== null) return
      setLoading(true)
      try {
        // Attempt to fetch profile which may contain feature flags
        const profile = await api.profiles.me()
        if (!mounted) return
        // Normalize feature flags if backend provides them in different shapes
        const ff: FeatureFlags = (profile?.FeatureFlags as FeatureFlags) ?? (profile?.featureFlags as FeatureFlags) ?? {}
        setFlags(ff)
      } catch (e) {
        // ignore fetch errors; leave flags as empty object to avoid retry storms
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

export default useRole
