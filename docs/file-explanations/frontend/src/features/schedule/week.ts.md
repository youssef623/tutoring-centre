# `frontend/src/features/schedule/week.ts`

## Purpose

Source comment (intent): Pure calendar-date helpers for "YYYY-MM-DD" strings (Day 53). A calendar date has no time zone: these never read the browser's zone and never convert one. Arithmetic runs on `Date.UTC` day numbers, where every day is exactly 24 hours long, so no daylight-saving change can shift a date. Never `new Date("YYYY-MM-DD")`.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/week.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 94 lines and 4,148 bytes. Source hash: `fc38aabcb1370cb4033dd7659c8b38fb7e9a86eced499b4eb2d63fa75956a1ea`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../frontend/src/features/schedule/week.ts#L11): `export const WeekStartDay = 6;`
- [Line 14](../../../../../../frontend/src/features/schedule/week.ts#L14): `export const WeekdaysFromSaturday = ["saturday", "sunday", "monday", "tuesday", "wednesday", "thursday", "friday"] as const;`
- [Line 16](../../../../../../frontend/src/features/schedule/week.ts#L16): `export type Weekday = (typeof WeekdaysFromSaturday)[number];`
- [Line 21](../../../../../../frontend/src/features/schedule/week.ts#L21): `export function isIsoDate(value: unknown): value is string {`
- [Line 36](../../../../../../frontend/src/features/schedule/week.ts#L36): `function toDayNumber(isoDate: string): number {`
- [Line 45](../../../../../../frontend/src/features/schedule/week.ts#L45): `function fromDayNumber(dayNumber: number): string {`
- [Line 53](../../../../../../frontend/src/features/schedule/week.ts#L53): `export function addDays(isoDate: string, days: number): string {`
- [Line 58](../../../../../../frontend/src/features/schedule/week.ts#L58): `export function dayOfWeekNumber(isoDate: string): number {`
- [Line 63](../../../../../../frontend/src/features/schedule/week.ts#L63): `export function weekdayOf(isoDate: string): Weekday {`
- [Line 68](../../../../../../frontend/src/features/schedule/week.ts#L68): `export function startOfWeek(isoDate: string): string {`
- [Line 73](../../../../../../frontend/src/features/schedule/week.ts#L73): `export function weekDates(weekStart: string): string[] {`
- [Line 78](../../../../../../frontend/src/features/schedule/week.ts#L78): `export function daysBetween(from: string, to: string): number {`
- [Line 86](../../../../../../frontend/src/features/schedule/week.ts#L86): `export function formatCalendarDate(isoDate: string, locale: string, options: Intl.DateTimeFormatOptions = { dateStyle: "medium" }): string {`
- [Line 91](../../../../../../frontend/src/features/schedule/week.ts#L91): `export function formatCalendarRange(from: string, to: string, locale: string): string {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:22](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L22)
- [frontend/src/features/schedule/CancelSessionDialog.tsx:17](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx#L17)
- [frontend/src/features/schedule/EndScheduleRuleDialog.tsx:20](../../../../../../frontend/src/features/schedule/EndScheduleRuleDialog.tsx#L20)
- [frontend/src/features/schedule/ExtendScheduleDialog.tsx:19](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.tsx#L19)
- [frontend/src/features/schedule/ScheduleRulesTable.tsx:14](../../../../../../frontend/src/features/schedule/ScheduleRulesTable.tsx#L14)
- [frontend/src/features/schedule/TimetableGrid.tsx:5](../../../../../../frontend/src/features/schedule/TimetableGrid.tsx#L5)
- [frontend/src/features/schedule/TimetablePage.tsx:20](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L20)
- [frontend/src/features/schedule/WeekNavigator.tsx:4](../../../../../../frontend/src/features/schedule/WeekNavigator.tsx#L4)
- [frontend/src/features/schedule/scheduleRules.ts:4](../../../../../../frontend/src/features/schedule/scheduleRules.ts#L4)
- [frontend/src/features/schedule/scheduleSchemas.ts:2](../../../../../../frontend/src/features/schedule/scheduleSchemas.ts#L2)
- [frontend/src/features/schedule/week.test.ts:2](../../../../../../frontend/src/features/schedule/week.test.ts#L2)
- [frontend/src/routes/_authenticated/timetable.tsx:3](../../../../../../frontend/src/routes/_authenticated/timetable.tsx#L3)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

The excerpt stops at line 90; use the original file for the remaining 4 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
