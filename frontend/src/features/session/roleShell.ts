import type { MeDto } from "@/api/generated/tutoring-centre";

export function isPortalRole(role: MeDto["activeRole"]): boolean {
  return role === "parent" || role === "student";
}

export function sessionHome(me: MeDto): string {
  if (me.mustChangePassword) return "/change-password";
  if (me.activeCentreId === null) return "/select-centre";
  return isPortalRole(me.activeRole) ? "/portal" : "/";
}
