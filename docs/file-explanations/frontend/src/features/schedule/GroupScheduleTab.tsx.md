# `frontend/src/features/schedule/GroupScheduleTab.tsx`

## Purpose

Source comment (intent): A group's weekly rules. Anyone with `schedule.view` (the group's own teacher included) reads them; `schedule.manage` adds the add and end actions. No role check: the controls follow the permission, and a teacher's 404 for another teacher's group is the page's "Group not found". "Today" is the centre's date.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 178 lines and 6,562 bytes. Source hash: `5974b9b946c36aeb474575d668ca3378df1cec3349a64105ac608d825e73d538`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { CalendarClock, CalendarPlus } from "lucide-react";`
- Line 2: `import { useState } from "react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import { asApiError } from "@/api/apiFetch";`
- Line 5: `import { messageFor } from "@/api/errorMessages";`
- Line 6: `import { useListGroupScheduleRules, type GroupDto, type ScheduleRuleDto } from "@/api/generated/tutoring-centre";`
- Line 7: `import { EmptyState } from "@/components/shared/EmptyState";`
- Line 8: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 9: `import { Button } from "@/components/ui/button";`
- Line 10: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 11: `import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";`
- Line 12: `import { AddScheduleRuleDialog } from "@/features/schedule/AddScheduleRuleDialog";`
- Line 13: `import { todayIn } from "@/features/schedule/centreTime";`
- Line 14: `import { EndScheduleRuleDialog } from "@/features/schedule/EndScheduleRuleDialog";`
- Line 15: `import { useRuleTimeRange } from "@/features/schedule/scheduleRules";`
- Line 16: `import { ScheduleRulesTable } from "@/features/schedule/ScheduleRulesTable";`
- Line 17: `import { Can } from "@/features/session/Can";`
- Line 18: `import { Permissions } from "@/features/session/permissions";`
- Line 19: `import { useCan } from "@/features/session/useCan";`
- Line 20: `import { useCentreTimeZone } from "@/features/session/useCentreTimeZone";`
- Line 21: `import { isolateLtr } from "@/lib/bidi";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 28](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L28): `export function GroupScheduleTab({ group }: { group: GroupDto }) {`
- [Line 144](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L144): `function AddRuleButton({ archived, onClick }: { archived: boolean; onClick: () => void }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/GroupScheduleTab.test.tsx:12](../../../../../../frontend/src/features/schedule/GroupScheduleTab.test.tsx#L12)
- [frontend/src/routes/_authenticated/groups.$groupId.tsx:47](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L47)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
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
```

The excerpt stops at line 90; use the original file for the remaining 88 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
