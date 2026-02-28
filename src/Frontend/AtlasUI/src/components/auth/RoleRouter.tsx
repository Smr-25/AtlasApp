import { Navigate } from 'react-router-dom'
import { useAuth, UserRole } from '@/context/AuthContext'

const ROLE_PATHS: Record<UserRole, string> = {
  developer: '/developer',
  designer: '/designer',
  cybersecurity: '/secops',
  marketer: '/marketer',
  'team-leader': '/leader',
}

export default function RoleRouter() {
  const { user, isAuthenticated } = useAuth()
  if (!isAuthenticated || !user) return <Navigate to="/login" replace />
  if (!user.onboardingComplete) return <Navigate to="/onboarding" replace />
  const role = (user.role as UserRole) ?? 'developer'
  return <Navigate to={ROLE_PATHS[role] ?? '/leader'} replace />
}

