/**
 * The one place the frontend turns times into text for the schedule (Day 53). Every function takes the centre's
 * IANA zone as an argument and never reads the browser's: a secretary abroad, or a laptop set to another zone,
 * sees the centre's wall-clock time. `Intl` applies the zone's rules for each instant's own date, so 16:00Z in
 * April's winter time and 15:00Z in May's summer time both read 18:00 in Cairo, exactly as the server generated
 * them. No date library.
 */

const TimeOptions: Intl.DateTimeFormatOptions = { hour: "numeric", minute: "2-digit" };

const LocalTimePattern = /^([01]\d|2[0-3]):([0-5]\d)$/;

/** An instant ("…Z") as a time of day in `timeZoneId`, for people in `locale`, e.g. "6:00 PM" or "٦:٠٠ م". */
export function formatTime(instant: string, timeZoneId: string, locale: string): string {
  return new Intl.DateTimeFormat(locale, { ...TimeOptions, timeZone: timeZoneId }).format(new Date(instant));
}

/** Two instants as a time range in `timeZoneId`, e.g. "6:00 – 7:30 PM". */
export function formatTimeRange(startsAt: string, endsAt: string, timeZoneId: string, locale: string): string {
  return new Intl.DateTimeFormat(locale, { ...TimeOptions, timeZone: timeZoneId }).formatRange(new Date(startsAt), new Date(endsAt));
}

/** Today's date in `timeZoneId`, as "YYYY-MM-DD": the centre's calendar, not the browser's. */
export function todayIn(timeZoneId: string, now: Date = new Date()): string {
  const parts = new Intl.DateTimeFormat("en-US", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    timeZone: timeZoneId,
  }).formatToParts(now);
  const part = (type: Intl.DateTimeFormatPartTypes) => parts.find((candidate) => candidate.type === type)?.value ?? "";
  return `${part("year")}-${part("month")}-${part("day")}`;
}

/** Whether an "HH:mm" local time of day is well formed. */
export function isLocalTime(value: string): boolean {
  return LocalTimePattern.test(value);
}

/** Minutes since midnight of an "HH:mm" local time. */
export function minutesOf(localTime: string): number {
  const match = LocalTimePattern.exec(localTime);
  if (match === null) {
    throw new RangeError(`Not a local time: ${localTime}`);
  }

  return Number(match[1]) * 60 + Number(match[2]);
}

/** The "HH:mm" that is `minutes` after `localTime`, wrapping past midnight. */
export function addMinutes(localTime: string, minutes: number): string {
  const total = (((minutesOf(localTime) + minutes) % 1440) + 1440) % 1440;
  return `${String(Math.floor(total / 60)).padStart(2, "0")}:${String(total % 60).padStart(2, "0")}`;
}

/**
 * A wall-clock "HH:mm" (a weekly rule's start, which has no date and so no instant) for people in `locale`. It is
 * placed on a fixed day and formatted in UTC, so no zone can move it.
 */
export function formatLocalTime(localTime: string, locale: string): string {
  const minutes = minutesOf(localTime);
  const wallClock = new Date(Date.UTC(2000, 0, 1, Math.floor(minutes / 60), minutes % 60));
  return new Intl.DateTimeFormat(locale, { ...TimeOptions, timeZone: "UTC" }).format(wallClock);
}

/** Two wall-clock "HH:mm" values as a range, e.g. "6:00 – 7:30 PM". */
export function formatLocalTimeRange(start: string, end: string, locale: string): string {
  const toDate = (localTime: string) => {
    const minutes = minutesOf(localTime);
    return new Date(Date.UTC(2000, 0, 1, Math.floor(minutes / 60), minutes % 60));
  };
  const endDate = toDate(end);
  // An end past midnight (a rule from 23:30 for 60 minutes) is still later than its start.
  if (endDate < toDate(start)) {
    endDate.setUTCDate(endDate.getUTCDate() + 1);
  }

  return new Intl.DateTimeFormat(locale, { ...TimeOptions, timeZone: "UTC" }).formatRange(toDate(start), endDate);
}
