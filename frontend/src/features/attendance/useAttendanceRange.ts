import { todayIn } from "@/features/schedule/centreTime";
import { addDays, isIsoDate } from "@/features/schedule/week";
import { useCentreTimeZone } from "@/features/session/useCentreTimeZone";

/** Calendar arithmetic stays in the centre's zone, including across DST changes. */
export function useAttendanceRange(from?: string, to?: string, days = 28) {
  const timeZoneId = useCentreTimeZone();
  const today = timeZoneId === null ? undefined : todayIn(timeZoneId);
  const end = to ?? today;
  const start = from ?? (today === undefined ? undefined : addDays(today, -(days - 1)));
  const valid =
    start !== undefined &&
    end !== undefined &&
    isIsoDate(start) &&
    isIsoDate(end) &&
    start <= end &&
    end <= addDays(start, 92);
  return { from: start, to: end, valid, timeZoneId, today };
}
