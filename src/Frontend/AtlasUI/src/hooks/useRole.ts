import { useRole as useRoleContext } from '@/context/RoleContext'
import { Role } from '@/lib/roles'

export const useRole = () => {
  const ctx = useRoleContext()
  return {
    roles: ctx.roles,
    primary: ctx.primary,
    hasRole: ctx.hasRole,
    labelFor: ctx.labelFor,
    isDeveloper: () => ctx.hasRole(Role.Developer),
    isDesigner: () => ctx.hasRole(Role.Designer),
    isSecOps: () => ctx.hasRole(Role.SecOps),
    isMarketer: () => ctx.hasRole(Role.Marketer),
    isLeader: () => ctx.hasRole(Role.TeamLeader),
  }
}

export default useRole

