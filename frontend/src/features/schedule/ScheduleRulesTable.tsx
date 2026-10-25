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
