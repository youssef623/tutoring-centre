/**
 * Pure calendar-date helpers for "YYYY-MM-DD" strings (Day 53). A calendar date has no time zone: these never read
 * the browser's zone and never convert one. Arithmetic runs on `Date.UTC` day numbers, where every day is exactly
 * 24 hours long, so no daylight-saving change can shift a date. Never `new Date("YYYY-MM-DD")`.
 */

const IsoDatePattern = /^(\d{4})-(\d{2})-(\d{2})$/;
const MillisecondsPerDay = 86_400_000;

/** The week starts on Saturday, as in Egypt. `dayOfWeek` numbers follow `Date.getUTCDay`: Sunday 0 … Saturday 6. */
export const WeekStartDay = 6;

/** The API's weekday names, Saturday first: the order of the week in the UI. */
export const WeekdaysFromSaturday = ["saturday", "sunday", "monday", "tuesday", "wednesday", "thursday", "friday"] as const;

export type Weekday = (typeof WeekdaysFromSaturday)[number];

const WeekdaysFromSunday: readonly Weekday[] = ["sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday"];

/** True for a real calendar date written as "YYYY-MM-DD" (so "2027-02-30" is false). */
export function isIsoDate(value: unknown): value is string {
  if (typeof value !== "string") {
    return false;
  }

  const match = IsoDatePattern.exec(value);
  if (match === null) {
    return false;
  }

  const [year, month, day] = [Number(match[1]), Number(match[2]), Number(match[3])];
  const date = new Date(Date.UTC(year, month - 1, day));
  return date.getUTCFullYear() === year && date.getUTCMonth() === month - 1 && date.getUTCDate() === day;
}

function toDayNumber(isoDate: string): number {
  const match = IsoDatePattern.exec(isoDate);
  if (match === null || !isIsoDate(isoDate)) {
    throw new RangeError(`Not a calendar date: ${isoDate}`);
  }

  return Date.UTC(Number(match[1]), Number(match[2]) - 1, Number(match[3])) / MillisecondsPerDay;
}

function fromDayNumber(dayNumber: number): string {
  const date = new Date(dayNumber * MillisecondsPerDay);
  const month = String(date.getUTCMonth() + 1).padStart(2, "0");
  const day = String(date.getUTCDate()).padStart(2, "0");
  return `${String(date.getUTCFullYear()).padStart(4, "0")}-${month}-${day}`;
}

/** The date `days` calendar days after (or, negative, before) `isoDate`. */
export function addDays(isoDate: string, days: number): string {
  return fromDayNumber(toDayNumber(isoDate) + days);
}

/** Sunday 0 … Saturday 6. */
export function dayOfWeekNumber(isoDate: string): number {
  return new Date(toDayNumber(isoDate) * MillisecondsPerDay).getUTCDay();
}

/** The API's lower-case weekday name for a date. */
export function weekdayOf(isoDate: string): Weekday {
  return WeekdaysFromSunday[dayOfWeekNumber(isoDate)] ?? "saturday";
}

/** The Saturday on or before `isoDate`. */
export function startOfWeek(isoDate: string): string {
  return addDays(isoDate, -((dayOfWeekNumber(isoDate) - WeekStartDay + 7) % 7));
}

/** The seven dates of the week that starts on `weekStart`, in order. */
export function weekDates(weekStart: string): string[] {
  return Array.from({ length: 7 }, (_, index) => addDays(weekStart, index));
}

/** Whole days from `from` to `to` (negative when `to` is earlier). */
export function daysBetween(from: string, to: string): number {
  return toDayNumber(to) - toDayNumber(from);
}

/**
 * A calendar date written for people in `locale`. The date is placed at UTC midnight and formatted in UTC, so the
 * day shown is the date itself in every browser zone.
 */
export function formatCalendarDate(isoDate: string, locale: string, options: Intl.DateTimeFormatOptions = { dateStyle: "medium" }): string {
  return new Intl.DateTimeFormat(locale, { ...options, timeZone: "UTC" }).format(new Date(toDayNumber(isoDate) * MillisecondsPerDay));
}

/** A range of calendar dates for people in `locale`, e.g. "24 – 30 Apr 2027". */
export function formatCalendarRange(from: string, to: string, locale: string): string {
  const format = new Intl.DateTimeFormat(locale, { day: "numeric", month: "short", year: "numeric", timeZone: "UTC" });
  return format.formatRange(new Date(toDayNumber(from) * MillisecondsPerDay), new Date(toDayNumber(to) * MillisecondsPerDay));
}
