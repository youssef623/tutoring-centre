# `frontend/src/features/schedule/centreTime.ts`

## Purpose

Source comment (intent): The one place the frontend turns times into text for the schedule (Day 53). Every function takes the centre's IANA zone as an argument and never reads the browser's: a secretary abroad, or a laptop set to another zone, sees the centre's wall-clock time. `Intl` applies the zone's rules for each instant's own date, so 16:00Z in April's winter time and 15:00Z in May's summer time both read 18:00 in Cairo, exactly as the server generated them. No date library.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/centreTime.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 79 lines and 3,761 bytes. Source hash: `808067b2d42f819a873e39480d38910fa25cc669f6cfe2ea3e6cbec34279b7dd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 2](../../../../../../frontend/src/features/schedule/centreTime.ts#L2): `* The one place the frontend turns times into text for the schedule (Day 53). Every function takes the centre's`
- [Line 14](../../../../../../frontend/src/features/schedule/centreTime.ts#L14): `export function formatTime(instant: string, timeZoneId: string, locale: string): string {`
- [Line 19](../../../../../../frontend/src/features/schedule/centreTime.ts#L19): `export function formatTimeRange(startsAt: string, endsAt: string, timeZoneId: string, locale: string): string {`
- [Line 24](../../../../../../frontend/src/features/schedule/centreTime.ts#L24): `export function todayIn(timeZoneId: string, now: Date = new Date()): string {`
- [Line 36](../../../../../../frontend/src/features/schedule/centreTime.ts#L36): `export function isLocalTime(value: string): boolean {`
- [Line 41](../../../../../../frontend/src/features/schedule/centreTime.ts#L41): `export function minutesOf(localTime: string): number {`
- [Line 51](../../../../../../frontend/src/features/schedule/centreTime.ts#L51): `export function addMinutes(localTime: string, minutes: number): string {`
- [Line 60](../../../../../../frontend/src/features/schedule/centreTime.ts#L60): `export function formatLocalTime(localTime: string, locale: string): string {`
- [Line 67](../../../../../../frontend/src/features/schedule/centreTime.ts#L67): `export function formatLocalTimeRange(start: string, end: string, locale: string): string {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/CancelSessionDialog.tsx:14](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx#L14)
- [frontend/src/features/schedule/EndScheduleRuleDialog.tsx:17](../../../../../../frontend/src/features/schedule/EndScheduleRuleDialog.tsx#L17)
- [frontend/src/features/schedule/GroupScheduleTab.tsx:13](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L13)
- [frontend/src/features/schedule/SessionCard.tsx:8](../../../../../../frontend/src/features/schedule/SessionCard.tsx#L8)
- [frontend/src/features/schedule/TimetablePage.tsx:16](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L16)
- [frontend/src/features/schedule/centreTime.test.ts:2](../../../../../../frontend/src/features/schedule/centreTime.test.ts#L2)
- [frontend/src/features/schedule/scheduleRules.ts:3](../../../../../../frontend/src/features/schedule/scheduleRules.ts#L3)
- [frontend/src/features/schedule/scheduleSchemas.ts:47](../../../../../../frontend/src/features/schedule/scheduleSchemas.ts#L47)
- [src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs:110](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs#L110)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
