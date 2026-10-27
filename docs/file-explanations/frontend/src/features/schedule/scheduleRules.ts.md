# `frontend/src/features/schedule/scheduleRules.ts`

## Purpose

Source comment (intent): Where a rule stands on `today` (the centre's date): ended once its last date has passed, so a rule ended for next week still reads "Ends …". Dates compare as strings: "YYYY-MM-DD" sorts as it counts.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/scheduleRules.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 35 lines and 1,644 bytes. Source hash: `92f6e551a7845ab652fcab3d52fde0502d4e892e26b12d7082450ddec63e101f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useTranslation } from "react-i18next";`
- Line 2: `import type { ScheduleRuleDto } from "@/api/generated/tutoring-centre";`
- Line 3: `import { addMinutes, formatLocalTimeRange, minutesOf } from "@/features/schedule/centreTime";`
- Line 4: `import { WeekdaysFromSaturday } from "@/features/schedule/week";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../frontend/src/features/schedule/scheduleRules.ts#L6): `export type RuleState = "active" | "upcoming" | "ending" | "ended";`
- [Line 12](../../../../../../frontend/src/features/schedule/scheduleRules.ts#L12): `export function ruleState(rule: ScheduleRuleDto, today: string | null): RuleState {`
- [Line 25](../../../../../../frontend/src/features/schedule/scheduleRules.ts#L25): `export function useRuleTimeRange() {`
- [Line 32](../../../../../../frontend/src/features/schedule/scheduleRules.ts#L32): `export function sortRules(rules: ScheduleRuleDto[]): ScheduleRuleDto[] {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/GroupScheduleTab.tsx:15](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L15)
- [frontend/src/features/schedule/ScheduleRulesTable.tsx:13](../../../../../../frontend/src/features/schedule/ScheduleRulesTable.tsx#L13)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
