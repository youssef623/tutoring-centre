# `frontend/src/features/schedule/TimetablePage.tsx`

## Purpose

Source comment (intent): One week of sessions in the centre's time, the same page for every role: the server returns a teacher only their own sessions, so this page holds no role check. The week and both filters live in the URL. Every time is formatted from its UTC instant in the response's `timeZoneId`; "today" and the current week come from the centre's zone, never the browser's. Manage actions sit inside `schedule.manage`; the teacher filter (and its request to /api/teachers) exists only with `teachers.view`.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/TimetablePage.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 258 lines and 9,106 bytes. Source hash: `2e904957e2787e27bd0c1c3238ea1ab3cdb3e45651f10c4738a4c6913b95d2b9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { CalendarOff, CalendarSync } from "lucide-react";`
- Line 2: `import { useState } from "react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import { asApiError } from "@/api/apiFetch";`
- Line 5: `import { messageFor } from "@/api/errorMessages";`
- Line 6: `import { useGetTimetable, useListGroups, type TimetableSessionDto } from "@/api/generated/tutoring-centre";`
- Line 7: `import { EmptyState } from "@/components/shared/EmptyState";`
- Line 8: `import { FilterBar } from "@/components/shared/FilterBar";`
- Line 9: `import { PageHeader } from "@/components/shared/PageHeader";`
- Line 10: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 11: `import { Button } from "@/components/ui/button";`
- Line 12: `import { Label } from "@/components/ui/label";`
- Line 13: `import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";`
- Line 14: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 15: `import { CancelSessionDialog } from "@/features/schedule/CancelSessionDialog";`
- Line 16: `import { todayIn } from "@/features/schedule/centreTime";`
- Line 17: `import { ExtendScheduleDialog } from "@/features/schedule/ExtendScheduleDialog";`
- Line 18: `import { TimetableGrid } from "@/features/schedule/TimetableGrid";`
- Line 19: `import { WeekNavigator } from "@/features/schedule/WeekNavigator";`
- Line 20: `import { startOfWeek } from "@/features/schedule/week";`
- Line 21: `import { Can } from "@/features/session/Can";`
- Line 22: `import { Permissions } from "@/features/session/permissions";`
- Line 23: `import { useCan } from "@/features/session/useCan";`
- Line 24: `import { useCentreTimeZone } from "@/features/session/useCentreTimeZone";`
- Line 25: `import { TeacherSelect } from "@/features/teachers/TeacherSelect";`
- Line 26: `import { isolateLtr } from "@/lib/bidi";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 28](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L28): `export interface TimetableSearchPatch {`
- [Line 41](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L41): `export function TimetablePage({`
- [Line 130](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L130): `function TimetableWeek({`
- [Line 227](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L227): `function GroupFilterSelect({ value, onChange }: { value: string | null; onChange: (value: string | null) => void }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/TimetablePage.test.tsx:9](../../../../../../frontend/src/features/schedule/TimetablePage.test.tsx#L9)
- [frontend/src/routes/_authenticated/timetable.tsx:2](../../../../../../frontend/src/routes/_authenticated/timetable.tsx#L2)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { CalendarOff, CalendarSync } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useGetTimetable, useListGroups, type TimetableSessionDto } from "@/api/generated/tutoring-centre";
import { EmptyState } from "@/components/shared/EmptyState";
import { FilterBar } from "@/components/shared/FilterBar";
import { PageHeader } from "@/components/shared/PageHeader";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { CancelSessionDialog } from "@/features/schedule/CancelSessionDialog";
import { todayIn } from "@/features/schedule/centreTime";
import { ExtendScheduleDialog } from "@/features/schedule/ExtendScheduleDialog";
import { TimetableGrid } from "@/features/schedule/TimetableGrid";
import { WeekNavigator } from "@/features/schedule/WeekNavigator";
import { startOfWeek } from "@/features/schedule/week";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { useCan } from "@/features/session/useCan";
import { useCentreTimeZone } from "@/features/session/useCentreTimeZone";
import { TeacherSelect } from "@/features/teachers/TeacherSelect";
import { isolateLtr } from "@/lib/bidi";

export interface TimetableSearchPatch {
  week?: string;
  group?: string | undefined;
  teacher?: string | undefined;
}

/**
 * One week of sessions in the centre's time, the same page for every role: the server returns a teacher only
 * their own sessions, so this page holds no role check. The week and both filters live in the URL. Every time is
 * formatted from its UTC instant in the response's `timeZoneId`; "today" and the current week come from the
 * centre's zone, never the browser's. Manage actions sit inside `schedule.manage`; the teacher filter (and its
 * request to /api/teachers) exists only with `teachers.view`.
 */
export function TimetablePage({
  week,
  groupId,
  teacherId,
  onSearchChange,
}: {
  week: string | undefined;
  groupId: string | undefined;
  teacherId: string | undefined;
  onSearchChange: (patch: TimetableSearchPatch) => void;
}) {
  const { t } = useTranslation("schedule");
  const zone = useCentreTimeZone();
  const today = zone === null ? null : todayIn(zone);
  const currentWeekStart = today === null ? null : startOfWeek(today);
  const weekStart = week ?? currentWeekStart;
  const [extendOpen, setExtendOpen] = useState(false);

  return (
    <section className="space-y-6">
      <PageHeader
        title={t("timetable.title")}
        description={t("timetable.subtitle")}
        actions={
          <Can permission={Permissions.ScheduleManage}>
            <Button
              type="button"
              variant="outline"
              disabled={today === null}
              onClick={() => {
                setExtendOpen(true);
              }}
            >
              <CalendarSync className="size-4" aria-hidden="true" />
              {t("actions.extend")}
            </Button>
          </Can>
        }
      />

      <FilterBar label={t("timetable.filtersLabel")}>
        <Can permission={Permissions.GroupsView}>
          <div className="w-full space-y-1.5 sm:w-56">
            <Label htmlFor="timetable-filter-group">{t("timetable.groupFilter")}</Label>
            <GroupFilterSelect
              value={groupId ?? null}
              onChange={(next) => {
                onSearchChange({ group: next ?? undefined });
              }}
            />
```

The excerpt stops at line 90; use the original file for the remaining 168 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
