# `frontend/src/features/schedule/scheduleSchemas.ts`

## Purpose

Source comment (intent): Client-side convenience only; the server repeats every rule (ScheduleRule.Create, the validators, the handlers' date windows) and the database refuses any overlap. Ranges mirror the server's: a duration of 30–240 minutes in steps of 5, a start on a 5-minute boundary, a room of at most 40 characters. The time is collected the way it is shown (hour 1–12, minute, AM/PM) and sent as a 24-hour "HH:mm" local time: never an offset or an instant. Messages are translation keys in the "schedule" namespace.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/scheduleSchemas.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 63 lines and 2,946 bytes. Source hash: `25bf6f698f68879fc33f97ba44cb8e8bef75f1fb9ab2e5728e6d8a3e233775fe`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { z } from "zod";`
- Line 2: `import { isIsoDate, WeekdaysFromSaturday } from "@/features/schedule/week";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../frontend/src/features/schedule/scheduleSchemas.ts#L11): `export const MinDurationMinutes = 30;`
- [Line 12](../../../../../../frontend/src/features/schedule/scheduleSchemas.ts#L12): `export const MaxDurationMinutes = 240;`
- [Line 13](../../../../../../frontend/src/features/schedule/scheduleSchemas.ts#L13): `export const RoomMaxLength = 40;`
- [Line 16](../../../../../../frontend/src/features/schedule/scheduleSchemas.ts#L16): `export const DurationOptions = Array.from(`
- [Line 21](../../../../../../frontend/src/features/schedule/scheduleSchemas.ts#L21): `export const Hours12 = Array.from({ length: 12 }, (_, index) => index + 1);`
- [Line 23](../../../../../../frontend/src/features/schedule/scheduleSchemas.ts#L23): `export const MinuteOptions = Array.from({ length: 12 }, (_, index) => index * 5);`
- [Line 25](../../../../../../frontend/src/features/schedule/scheduleSchemas.ts#L25): `export const scheduleRuleSchema = z.object({`
- [Line 45](../../../../../../frontend/src/features/schedule/scheduleSchemas.ts#L45): `export type ScheduleRuleFormValues = z.infer<typeof scheduleRuleSchema>;`
- [Line 48](../../../../../../frontend/src/features/schedule/scheduleSchemas.ts#L48): `export function toLocalTime(hour: number, minute: number, period: "am" | "pm"): string {`
- [Line 54](../../../../../../frontend/src/features/schedule/scheduleSchemas.ts#L54): `export function toAddScheduleRuleRequest(values: ScheduleRuleFormValues) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:25](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L25)
- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:26](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L26)
- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:27](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L27)
- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:28](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L28)
- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:29](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L29)
- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:30](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L30)
- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:31](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L31)
- [src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs:15](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs#L15)
- [src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs:16](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs#L16)
- [src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs:17](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs#L17)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs:46](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs#L46)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ScheduleRuleConfiguration.cs:30](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ScheduleRuleConfiguration.cs#L30)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ScheduleRuleConfiguration.cs:43](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ScheduleRuleConfiguration.cs#L43)
- [tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs:89](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L89)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { z } from "zod";
import { isIsoDate, WeekdaysFromSaturday } from "@/features/schedule/week";

/**
 * Client-side convenience only; the server repeats every rule (ScheduleRule.Create, the validators, the handlers'
 * date windows) and the database refuses any overlap. Ranges mirror the server's: a duration of 30–240 minutes
 * in steps of 5, a start on a 5-minute boundary, a room of at most 40 characters. The time is collected the way
 * it is shown (hour 1–12, minute, AM/PM) and sent as a 24-hour "HH:mm" local time: never an offset or an instant.
 * Messages are translation keys in the "schedule" namespace.
 */
export const MinDurationMinutes = 30;
export const MaxDurationMinutes = 240;
export const RoomMaxLength = 40;

/** The lengths offered in the select: every quarter hour from 30 minutes to 4 hours. */
export const DurationOptions = Array.from(
  { length: (MaxDurationMinutes - MinDurationMinutes) / 15 + 1 },
  (_, index) => MinDurationMinutes + index * 15,
);

export const Hours12 = Array.from({ length: 12 }, (_, index) => index + 1);

export const MinuteOptions = Array.from({ length: 12 }, (_, index) => index * 5);

export const scheduleRuleSchema = z.object({
  dayOfWeek: z.enum(WeekdaysFromSaturday, { error: "validation.dayRequired" }),
  hour: z.number({ error: "validation.timeRequired" }).int().min(1, "validation.timeRequired").max(12, "validation.timeRequired"),
  minute: z
    .number({ error: "validation.timeRequired" })
    .int()
    .min(0, "validation.timeRequired")
    .max(55, "validation.timeRequired")
    .refine((value) => value % 5 === 0, "validation.timeRequired"),
  period: z.enum(["am", "pm"], { error: "validation.timeRequired" }),
  durationMinutes: z
    .number({ error: "validation.durationRange" })
    .int()
    .min(MinDurationMinutes, "validation.durationRange")
    .max(MaxDurationMinutes, "validation.durationRange")
    .refine((value) => value % 5 === 0, "validation.durationRange"),
  room: z.string().trim().max(RoomMaxLength, "validation.roomTooLong"),
  effectiveFrom: z.string().refine(isIsoDate, "validation.dateRequired"),
});

export type ScheduleRuleFormValues = z.infer<typeof scheduleRuleSchema>;

/** "6", "05", "pm" as the 24-hour local time the API takes: "18:05". */
export function toLocalTime(hour: number, minute: number, period: "am" | "pm"): string {
  const hour24 = (hour % 12) + (period === "pm" ? 12 : 0);
  return `${String(hour24).padStart(2, "0")}:${String(minute).padStart(2, "0")}`;
}

/** The request body, exactly the contract's five fields: no teacher, no centre, no offset. */
export function toAddScheduleRuleRequest(values: ScheduleRuleFormValues) {
  const room = values.room.trim();
  return {
    dayOfWeek: values.dayOfWeek,
    startTime: toLocalTime(values.hour, values.minute, values.period),
    durationMinutes: values.durationMinutes,
    room: room === "" ? null : room,
    effectiveFrom: values.effectiveFrom,
  };
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
