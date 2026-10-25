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
