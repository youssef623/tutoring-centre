import i18next from "@/i18n";
import { messageFor } from "@/api/errorMessages";
import type { ApiError } from "@/api/errors";

/**
 * Enrollment refusals are ordinary outcomes of a busy office (two secretaries, one last seat), so they get short,
 * specific wording in the "enrollments" namespace; anything else falls back to the shared translated message.
 */
const OutcomeMessages: Record<string, string> = {
  "group.full": "outcomes.groupFull",
  "group.archived": "outcomes.groupArchived",
  "enrollment.already_enrolled": "outcomes.alreadyEnrolled",
  "enrollment.student_archived": "outcomes.studentArchived",
  "enrollment.already_ended": "outcomes.alreadyEnded",
  "enrollment.same_group": "outcomes.sameGroup",
  "enrollment.subject_mismatch": "outcomes.subjectMismatch",
  "concurrency.stale": "outcomes.stale",
};

export function enrollmentMessageFor(error: ApiError): string {
  const key = OutcomeMessages[error.code];
  return key === undefined ? messageFor(error) : i18next.t(key, { ns: "enrollments" });
}
