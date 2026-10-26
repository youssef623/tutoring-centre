import i18next from "@/i18n";
import { messageFor } from "@/api/errorMessages";
import type { ApiError } from "@/api/errors";

/**
 * Schedule refusals are ordinary outcomes of a busy office (a teacher already booked, a session someone else
 * cancelled first), so they get short, specific wording in the "schedule" namespace; anything else falls back to
 * the shared translated message.
 */
const OutcomeMessages: Record<string, string> = {
  "schedule.teacher_double_booked": "outcomes.teacherDoubleBooked",
  "schedule.group_double_booked": "outcomes.groupDoubleBooked",
  "group.archived": "outcomes.groupArchived",
  "schedule.range_invalid": "outcomes.rangeInvalid",
  "schedule.rule_already_ended": "outcomes.ruleAlreadyEnded",
  "schedule.rule_end_invalid": "outcomes.ruleEndInvalid",
  "schedule.rule_not_found": "outcomes.ruleNotFound",
  "schedule.session_already_cancelled": "outcomes.sessionAlreadyCancelled",
  "schedule.session_in_past": "outcomes.sessionInPast",
  "schedule.session_not_found": "outcomes.sessionNotFound",
  "concurrency.stale": "outcomes.stale",
};

export function scheduleMessageFor(error: ApiError): string {
  const key = OutcomeMessages[error.code];
  return key === undefined ? messageFor(error) : i18next.t(key, { ns: "schedule" });
}

/** The two exclusion-constraint conflicts. */
export function isDoubleBooking(error: ApiError): boolean {
  return error.code === "schedule.teacher_double_booked" || error.code === "schedule.group_double_booked";
}
