import { CalendarClock, CalendarPlus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useListGroupScheduleRules, type GroupDto, type ScheduleRuleDto } from "@/api/generated/tutoring-centre";
import { EmptyState } from "@/components/shared/EmptyState";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";
import { AddScheduleRuleDialog } from "@/features/schedule/AddScheduleRuleDialog";
import { todayIn } from "@/features/schedule/centreTime";
import { EndScheduleRuleDialog } from "@/features/schedule/EndScheduleRuleDialog";
import { useRuleTimeRange } from "@/features/schedule/scheduleRules";
import { ScheduleRulesTable } from "@/features/schedule/ScheduleRulesTable";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { useCan } from "@/features/session/useCan";
import { useCentreTimeZone } from "@/features/session/useCentreTimeZone";
import { isolateLtr } from "@/lib/bidi";

/**
 * A group's weekly rules. Anyone with `schedule.view` (the group's own teacher included) reads them;
 * `schedule.manage` adds the add and end actions. No role check: the controls follow the permission, and a
 * teacher's 404 for another teacher's group is the page's "Group not found". "Today" is the centre's date.
 */
export function GroupScheduleTab({ group }: { group: GroupDto }) {
  const { t } = useTranslation("schedule");
  const { t: tCommon } = useTranslation("common");
  const timeZoneId = useCentreTimeZone();
  const canManage = useCan(Permissions.ScheduleManage);
  const { data, error, isPending, refetch } = useListGroupScheduleRules(group.id);
  const [addOpen, setAddOpen] = useState(false);
  const [endTarget, setEndTarget] = useState<ScheduleRuleDto | null>(null);
  const timeRange = useRuleTimeRange();
  const today = timeZoneId === null ? null : todayIn(timeZoneId);

  if (isPending) {
    return (
      <div role="status" aria-label={t("rules.loadingLabel")} className="space-y-2">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
      </div>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    return (
      <Alert variant="destructive">
        <AlertTitle>{t("rules.errorTitle")}</AlertTitle>
        <AlertDescription className="flex flex-col gap-2">
          <p>{messageFor(apiError)}</p>
          {apiError.correlationId !== undefined && <p>{t("rules.errorReference", { correlationId: isolateLtr(apiError.correlationId) })}</p>}
          <Button
            type="button"
            variant="outline"
            size="sm"
            className="self-start"
            onClick={() => {
              void refetch();
            }}
          >
            {tCommon("retry")}
          </Button>
        </AlertDescription>
      </Alert>
    );
  }

  const addButton = (
    <AddRuleButton
      archived={group.status === "archived"}
      onClick={() => {
        setAddOpen(true);
      }}
    />
  );

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        {timeZoneId !== null && <p className="text-sm text-muted-foreground">{t("timeZoneNote", { zone: timeZoneId })}</p>}
        {data.items.length > 0 && <Can permission={Permissions.ScheduleManage}>{addButton}</Can>}
      </div>

      {data.items.length === 0 ? (
        <EmptyState
          icon={CalendarClock}
          title={t("rules.emptyTitle")}
          description={t(canManage ? "rules.emptyDescription" : "rules.emptyDescriptionReadOnly")}
          action={<Can permission={Permissions.ScheduleManage}>{addButton}</Can>}
        />
      ) : (
        <ScheduleRulesTable
          rules={data.items}
          today={today}
          rowAction={
            canManage
              ? (rule, state) =>
                  state === "active" || state === "upcoming" ? (
                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      className="text-destructive hover:text-destructive"
                      aria-label={t("actions.endLabel", { day: t(`weekdays.${rule.dayOfWeek}`), time: timeRange(rule) })}
                      onClick={() => {
                        setEndTarget(rule);
                      }}
                    >
                      {t("actions.end")}
                    </Button>
                  ) : null
              : undefined
          }
        />
      )}

      {timeZoneId !== null && today !== null && (
        <Can permission={Permissions.ScheduleManage}>
          <AddScheduleRuleDialog group={group} today={today} timeZoneId={timeZoneId} open={addOpen} onOpenChange={setAddOpen} />
          <EndScheduleRuleDialog
            rule={endTarget}
            groupId={group.id}
            today={today}
            onClose={() => {
              setEndTarget(null);
            }}
          />
        </Can>
      )}
    </div>
  );
}

/**
 * Disabled for an archived group as a courtesy only (the server answers group.archived anyway). It stays
 * focusable while disabled so the reason can be read, in the tooltip and through `aria-describedby`.
 */
function AddRuleButton({ archived, onClick }: { archived: boolean; onClick: () => void }) {
  const { t } = useTranslation("schedule");

  if (!archived) {
    return (
      <Button type="button" onClick={onClick}>
        <CalendarPlus className="size-4" aria-hidden="true" />
        {t("actions.add")}
      </Button>
    );
  }

  return (
    <Tooltip>
      <TooltipTrigger
        render={
          <Button
            type="button"
            disabled
            focusableWhenDisabled
            aria-describedby="add-rule-disabled-archived"
            className="cursor-not-allowed opacity-50"
          />
        }
      >
        <CalendarPlus className="size-4" aria-hidden="true" />
        {t("actions.add")}
      </TooltipTrigger>
      <TooltipContent>{t("addDisabled.archived")}</TooltipContent>
      <span id="add-rule-disabled-archived" className="sr-only">
        {t("addDisabled.archived")}
      </span>
    </Tooltip>
  );
}
