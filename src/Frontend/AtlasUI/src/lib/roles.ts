export enum Role {
  Developer = 'developer',
  Designer = 'designer',
  SecOps = 'cybersecurity',
  Marketer = 'marketer',
  TeamLeader = 'team-leader',
}

export const professionToRole = (profession: number | string | undefined | null): Role | null => {
  if (profession === undefined || profession === null) return null
  const map: Record<string | number, Role> = {
    1: Role.Developer,
    2: Role.Designer,
    3: Role.SecOps,
    4: Role.Marketer,
    5: Role.TeamLeader,
    Developer: Role.Developer,
    developer: Role.Developer,
    Designer: Role.Designer,
    designer: Role.Designer,
    CyberSecurity: Role.SecOps,
    cybersecurity: Role.SecOps,
    DigitalMarketing: Role.Marketer,
    marketer: Role.Marketer,
    ProductManager: Role.TeamLeader,
    TeamLeader: Role.TeamLeader,
    'team-leader': Role.TeamLeader,
  }
  return (map as any)[profession] ?? null
}

export const RoleLabels: Record<Role, string> = {
  [Role.Developer]: 'Developer',
  [Role.Designer]: 'Designer',
  [Role.SecOps]: 'SecOps',
  [Role.Marketer]: 'Marketer',
  [Role.TeamLeader]: 'Team Leader',
}

