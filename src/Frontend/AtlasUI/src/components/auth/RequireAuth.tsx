import { ReactNode } from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import { useAuth } from '@/context/AuthContext'

interface Props { children: ReactNode }

export default function RequireAuth({ children }: Props) {
  const { isAuthenticated, user, initializing } = useAuth()
  const location = useLocation()

  // While auth is initializing (checking tokens / restoring session) avoid redirecting.
  if (initializing) return null

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />
  }

  // If user exists but onboarding not complete, redirect to onboarding
  if (user && !user.onboardingComplete) {
    return <Navigate to="/onboarding" replace />
  }

  return <>{children}</>
}
