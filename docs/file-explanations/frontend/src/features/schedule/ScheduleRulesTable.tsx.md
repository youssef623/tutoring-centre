# `frontend/src/features/schedule/ScheduleRulesTable.tsx`

## Purpose

Source comment (intent): A group's weekly rules, ended ones included (muted, and labelled "Ended" in words). The action column exists only when the caller passes one, which the Schedule tab does only inside `schedule.manage`.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/ScheduleRulesTable.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 85 lines and 3,688 bytes. Source hash: `1f647b36de276158137d6a7cbf5514fe4d7d8aab895f187a1528cc3df7561d32`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import type { ReactNode } from "react";`
- Line 2: `import { useTranslation } from "react-i18next";`
- Line 3: `import type { ScheduleRuleDto } from "@/api/generated/tutoring-centre";`
- Line 4: `import {`
- Line 12: `import { Badge } from "@/components/ui/badge";`
- Line 13: `import { ruleState, sortRules, useRuleTimeRange, type RuleState } from "@/features/schedule/scheduleRules";`
- Line 14: `import { formatCalendarDate } from "@/features/schedule/week";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 20](../../../../../../frontend/src/features/schedule/ScheduleRulesTable.tsx#L20): `export function ScheduleRulesTable({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/GroupScheduleTab.tsx:16](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L16)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import type { ReactNode } from "react";
import { useTranslation } from "react-i18next";
import type { ScheduleRuleDto } from "@/api/generated/tutoring-centre";
import {
  DataTable,
  DataTableBody,
  DataTableCell,
  DataTableColumn,
  DataTableHead,
  DataTableRow,
} from "@/components/shared/DataTable";
import { Badge } from "@/components/ui/badge";
import { ruleState, sortRules, useRuleTimeRange, type RuleState } from "@/features/schedule/scheduleRules";
import { formatCalendarDate } from "@/features/schedule/week";

/**
 * A group's weekly rules, ended ones included (muted, and labelled "Ended" in words). The action column exists
 * only when the caller passes one, which the Schedule tab does only inside `schedule.manage`.
 */
export function ScheduleRulesTable({
  rules,
  today,
  rowAction,
}: {
  rules: ScheduleRuleDto[];
  today: string | null;
  rowAction?: (rule: ScheduleRuleDto, state: RuleState) => ReactNode;
}) {
  const { t, i18n } = useTranslation("schedule");
  const timeRange = useRuleTimeRange();
  const date = (isoDate: string) => formatCalendarDate(isoDate, i18n.language);

  return (
    <DataTable className="sm:min-w-[640px]">
      <DataTableHead>
        <DataTableColumn>{t("rules.columns.day")}</DataTableColumn>
        <DataTableColumn>{t("rules.columns.time")}</DataTableColumn>
        <DataTableColumn>{t("rules.columns.room")}</DataTableColumn>
        <DataTableColumn>{t("rules.columns.effective")}</DataTableColumn>
        <DataTableColumn>{t("rules.columns.status")}</DataTableColumn>
        {rowAction !== undefined && <DataTableColumn hidden>{t("rules.columns.actions")}</DataTableColumn>}
      </DataTableHead>
      <DataTableBody>
        {sortRules(rules).map((rule) => {
          const state = ruleState(rule, today);

          return (
            <DataTableRow key={rule.id} muted={state === "ended"}>
              <DataTableCell kind="primary">
                <span className="font-medium">{t(`weekdays.${rule.dayOfWeek}`)}</span>
              </DataTableCell>
              <DataTableCell label={t("rules.columns.time")}>
                <span dir="ltr" className="tabular-nums">
                  {timeRange(rule)}
                </span>
              </DataTableCell>
              <DataTableCell label={t("rules.columns.room")}>
                {rule.room === null ? (
                  <span className="text-muted-foreground">{t("rules.noRoom")}</span>
                ) : (
                  <bdi>{rule.room}</bdi>
                )}
              </DataTableCell>
              <DataTableCell label={t("rules.columns.effective")} className="text-muted-foreground">
                {rule.effectiveTo === null
                  ? t("rules.effectiveOpen", { from: date(rule.effectiveFrom) })
                  : t("rules.effectiveRange", { from: date(rule.effectiveFrom), to: date(rule.effectiveTo) })}
              </DataTableCell>
              <DataTableCell label={t("rules.columns.status")}>
                <Badge variant={state === "ended" ? "outline" : state === "active" ? "default" : "secondary"}>
                  {state === "upcoming"
                    ? t("rules.status.upcoming", { date: date(rule.effectiveFrom) })
                    : state === "ending" && rule.effectiveTo !== null
                      ? t("rules.status.ending", { date: date(rule.effectiveTo) })
                      : t(`rules.status.${state}`)}
                </Badge>
              </DataTableCell>
              {rowAction !== undefined && <DataTableCell kind="actions">{rowAction(rule, state)}</DataTableCell>}
            </DataTableRow>
          );
        })}
      </DataTableBody>
    </DataTable>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
