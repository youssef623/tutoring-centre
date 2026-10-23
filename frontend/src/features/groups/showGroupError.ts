import { toast } from "sonner";
import i18next from "@/i18n";
import { showApiError } from "@/api/showApiError";
import type { ApiError } from "@/api/errors";

/**
 * Enrolment-related refusals with their own explanation in the "groups" namespace (they start occurring on
 * Day 48, when enrolments exist); every other error falls back to the shared translated toast.
 */
const GroupRuleMessages: Record<string, string> = {
  "group.capacity_below_enrolled": "ruleErrors.capacityBelowEnrolled",
  "group.has_active_enrollments": "ruleErrors.hasActiveEnrollments",
};

export function showGroupError(error: ApiError): void {
  const key = GroupRuleMessages[error.code];
  if (key === undefined) {
    showApiError(error);
    return;
  }

  toast.error(i18next.t(key, { ns: "groups" }));
}
