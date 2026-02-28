import { createContext, useContext, type ReactNode, useMemo } from 'react'
import { useAuth } from '@/context/AuthContext'
import { Role, professionToRole, RoleLabels } from '@/lib/roles'

interface RoleContextType {
  roles: Role[]
  primary?: Role | null
  hasRole: (r: Role | Role[]) => boolean
  labelFor: (r: Role) => string
}

const RoleContext = createContext<RoleContextType>({} as RoleContextType)

export const useRole = () => useContext(RoleContext)

export const RoleProvider = ({ children }: { children: ReactNode }) => {
  const { user } = useAuth()

  const roles = useMemo(() => {
    if (!user) return []
    const r = (user.role as any) ?? null
    const mapped = professionToRole(r as any)
    return mapped ? [mapped] : []
  }, [user?.role])

  const primary = roles[0] ?? null

  const hasRole = (r: Role | Role[]) => {
    if (!roles.length) return false
    if (Array.isArray(r)) return r.some(rr => roles.includes(rr))
    return roles.includes(r)
  }

  const labelFor = (r: Role) => RoleLabels[r]

  return (
    <RoleContext.Provider value={{ roles, primary, hasRole, labelFor }}>
      {children}
    </RoleContext.Provider>
  )
}
