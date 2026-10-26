import { toast } from "sonner";
import i18next from "@/i18n";
import { showApiError } from "@/api/showApiError";
import type { ApiError } from "@/api/errors";

/**
 * Enrolment-related refusals with their own explanation in the "groups" namespace (they start occurring on
 * Day 48, when enrolments exist), and the two that schedules add (Day 52: reassigning to a busy teacher, archiving
 * a scheduled group); every other error falls back to the shared translated toast.
 */
const GroupRuleMessages: Record<string, string> = {
  "group.capacity_below_enrolled": "ruleErrors.capacityBelowEnrolled",
  "group.has_active_enrollments": "ruleErrors.hasActiveEnrollments",
  "schedule.teacher_double_booked": "ruleErrors.teacherDoubleBooked",
  "group.has_active_schedule": "ruleErrors.hasActiveSchedule",
};

export function showGroupError(error: ApiError): void {
  const key = GroupRuleMessages[error.code];
  if (key === undefined) {
    showApiError(error);
    return;
  }

  toast.error(i18next.t(key, { ns: "groups" }));
}
