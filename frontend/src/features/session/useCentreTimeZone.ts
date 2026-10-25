import { useSession } from "./useSession";

/**
 * The active centre's IANA zone, from the signed-in session's membership (Day 53), or null before a centre is
 * selected. Every role has it, unlike the centre settings, so any page can work out "today" in the centre's
 * calendar. Never falls back to the browser's zone.
 */
export function useCentreTimeZone(): string | null {
  const { me } = useSession();
  return me?.memberships.find((membership) => membership.centreId === me.activeCentreId)?.timeZoneId ?? null;
}
