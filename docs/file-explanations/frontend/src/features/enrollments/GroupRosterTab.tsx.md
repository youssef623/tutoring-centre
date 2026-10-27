# `frontend/src/features/enrollments/GroupRosterTab.tsx`

## Purpose

Source comment (intent): One view, three permission levels: anyone with `enrollments.view` (including the group's own teacher) sees the roster; `enrollments.manage` adds the actions; `students.view` turns names into links. The view is never hidden, only the controls are gated.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 209 lines and 7,429 bytes. Source hash: `12ddb6bee8b9feb03d5a8df84dd63a764c8ae2785f8a0c16f0b7df14cb77ebbd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { UserPlus, UsersRound } from "lucide-react";`
- Line 2: `import { useMemo, useState } from "react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import { asApiError } from "@/api/apiFetch";`
- Line 5: `import { messageFor } from "@/api/errorMessages";`
- Line 6: `import { useGetGroupRoster, type GroupDto, type RosterEntryDto } from "@/api/generated/tutoring-centre";`
- Line 7: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 8: `import { Button } from "@/components/ui/button";`
- Line 9: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 10: `import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";`
- Line 11: `import { EnrollmentRowMenu } from "@/features/enrollments/EnrollmentRowMenu";`
- Line 12: `import { EnrollStudentDialog } from "@/features/enrollments/EnrollStudentDialog";`
- Line 13: `import { RosterTable } from "@/features/enrollments/RosterTable";`
- Line 14: `import { SeatMeter } from "@/features/enrollments/SeatMeter";`
- Line 15: `import { TransferDialog } from "@/features/enrollments/TransferDialog";`
- Line 16: `import { WithdrawEnrollmentDialog, type EnrollmentTarget } from "@/features/enrollments/WithdrawEnrollmentDialog";`
- Line 17: `import { Can } from "@/features/session/Can";`
- Line 18: `import { Permissions } from "@/features/session/permissions";`
- Line 19: `import { useCan } from "@/features/session/useCan";`
- Line 20: `import { isolateLtr } from "@/lib/bidi";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 27](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L27): `export function GroupRosterTab({ group }: { group: GroupDto }) {`
- [Line 167](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L167): `function EnrollButton({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/GroupRosterTab.test.tsx:12](../../../../../../frontend/src/features/enrollments/GroupRosterTab.test.tsx#L12)
- [frontend/src/routes/_authenticated/groups.$groupId.tsx:45](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L45)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { UserPlus, UsersRound } from "lucide-react";
import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useGetGroupRoster, type GroupDto, type RosterEntryDto } from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";
import { EnrollmentRowMenu } from "@/features/enrollments/EnrollmentRowMenu";
import { EnrollStudentDialog } from "@/features/enrollments/EnrollStudentDialog";
import { RosterTable } from "@/features/enrollments/RosterTable";
import { SeatMeter } from "@/features/enrollments/SeatMeter";
import { TransferDialog } from "@/features/enrollments/TransferDialog";
import { WithdrawEnrollmentDialog, type EnrollmentTarget } from "@/features/enrollments/WithdrawEnrollmentDialog";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { useCan } from "@/features/session/useCan";
import { isolateLtr } from "@/lib/bidi";

/**
 * One view, three permission levels: anyone with `enrollments.view` (including the group's own teacher) sees the
 * roster; `enrollments.manage` adds the actions; `students.view` turns names into links. The view is never hidden,
 * only the controls are gated.
 */
export function GroupRosterTab({ group }: { group: GroupDto }) {
  const { t } = useTranslation("enrollments");
  const { t: tCommon } = useTranslation("common");
  const linkStudents = useCan(Permissions.StudentsView);
  const canManage = useCan(Permissions.EnrollmentsManage);
  const { data, error, isPending, refetch } = useGetGroupRoster(group.id);
  const [enrollOpen, setEnrollOpen] = useState(false);
  const [withdrawTarget, setWithdrawTarget] = useState<EnrollmentTarget | null>(null);
  const [transferTarget, setTransferTarget] = useState<EnrollmentTarget | null>(null);
  const enrolledStudentIds = useMemo(() => new Set(data?.items.map((entry) => entry.studentId)), [data]);

  if (isPending) {
    return (
      <div role="status" aria-label={t("roster.loadingLabel")} className="space-y-2">
        <Skeleton className="h-12 w-48" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
      </div>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    return (
      <Alert variant="destructive">
        <AlertTitle>{t("roster.errorTitle")}</AlertTitle>
        <AlertDescription className="flex flex-col gap-2">
          <p>{messageFor(apiError)}</p>
          {apiError.correlationId !== undefined && (
            <p>{t("roster.errorReference", { correlationId: isolateLtr(apiError.correlationId) })}</p>
          )}
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

  const enrolled = Number(data.enrolledCount);
  const capacity = Number(data.capacity);
  const isFull = capacity > 0 && enrolled >= capacity;
  const isArchived = group.status === "archived";
  const toTarget = (entry: RosterEntryDto): EnrollmentTarget => ({
    enrollmentId: entry.enrollmentId,
    version: entry.version,
    studentId: entry.studentId,
    studentName: entry.studentName,
    groupId: group.id,
    groupName: group.name,
  });
  const enrollButton = (
    <EnrollButton
      disabledReason={isArchived ? "archived" : isFull ? "full" : null}
```

The excerpt stops at line 90; use the original file for the remaining 119 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
