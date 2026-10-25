import { useTranslation } from "react-i18next";
import type { ScheduleRuleDto } from "@/api/generated/tutoring-centre";
import { addMinutes, formatLocalTimeRange, minutesOf } from "@/features/schedule/centreTime";
import { WeekdaysFromSaturday } from "@/features/schedule/week";

export type RuleState = "active" | "upcoming" | "ending" | "ended";

/**
 * Where a rule stands on `today` (the centre's date): ended once its last date has passed, so a rule ended for
 * next week still reads "Ends …". Dates compare as strings: "YYYY-MM-DD" sorts as it counts.
 */
export function ruleState(rule: ScheduleRuleDto, today: string | null): RuleState {
  if (today !== null && rule.effectiveTo !== null && rule.effectiveTo < today) {
    return "ended";
  }

  if (rule.effectiveTo !== null) {
    return "ending";
  }

  return today !== null && rule.effectiveFrom > today ? "upcoming" : "active";
}

/** The rule's start and end as people read them, from its local start time and length; no zone involved. */
export function useRuleTimeRange() {
  const { i18n } = useTranslation();
  return (rule: ScheduleRuleDto) =>
    formatLocalTimeRange(rule.startTime, addMinutes(rule.startTime, Number(rule.durationMinutes)), i18n.language);
}

/** Saturday first, then by start time: the order of the week in the UI (the API orders from Sunday). */
export function sortRules(rules: ScheduleRuleDto[]): ScheduleRuleDto[] {
  const dayIndex = (rule: ScheduleRuleDto) => (WeekdaysFromSaturday as readonly string[]).indexOf(rule.dayOfWeek);
  return [...rules].sort((a, b) => dayIndex(a) - dayIndex(b) || minutesOf(a.startTime) - minutesOf(b.startTime));
}
