# `frontend/src/features/groups/GroupsList.tsx`

## Purpose

Source comment (intent): One list for every role: it renders whatever `GET /api/groups` returns, and the API has already scoped that to the actor (a teacher gets only their own groups). Nothing here asks who the user is; the only branch is on a permission — without `groups.manage` the empty state speaks to someone with assigned groups, not to someone who creates them.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/groups/GroupsList.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 186 lines and 7,291 bytes. Source hash: `3bd9c525bdd6159017a01d6f5bef54f0d8b3a0fcb393f7e1bfd1f6b998a2e1b0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Link } from "@tanstack/react-router";`
- Line 2: `import { Armchair, CircleSlash, Plus, UsersRound } from "lucide-react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import { asApiError } from "@/api/apiFetch";`
- Line 5: `import { messageFor } from "@/api/errorMessages";`
- Line 6: `import { useListGroups } from "@/api/generated/tutoring-centre";`
- Line 7: `import {`
- Line 15: `import { EmptyState } from "@/components/shared/EmptyState";`
- Line 16: `import { StatTile } from "@/components/shared/StatTile";`
- Line 17: `import { StatusBadge } from "@/components/shared/StatusBadge";`
- Line 18: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 19: `import { Button } from "@/components/ui/button";`
- Line 20: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 21: `import { SeatMeter } from "@/features/enrollments/SeatMeter";`
- Line 22: `import { Can } from "@/features/session/Can";`
- Line 23: `import { Permissions } from "@/features/session/permissions";`
- Line 24: `import { useCan } from "@/features/session/useCan";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 32](../../../../../../frontend/src/features/groups/GroupsList.tsx#L32): `export function GroupsList({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/groups.index.tsx:9](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx#L9)
- [tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs:765](../../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L765)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Link } from "@tanstack/react-router";
import { Armchair, CircleSlash, Plus, UsersRound } from "lucide-react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useListGroups } from "@/api/generated/tutoring-centre";
import {
  DataTable,
  DataTableBody,
  DataTableCell,
  DataTableColumn,
  DataTableHead,
  DataTableRow,
} from "@/components/shared/DataTable";
import { EmptyState } from "@/components/shared/EmptyState";
import { StatTile } from "@/components/shared/StatTile";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { SeatMeter } from "@/features/enrollments/SeatMeter";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { useCan } from "@/features/session/useCan";

/**
 * One list for every role: it renders whatever `GET /api/groups` returns, and the API has already scoped that
 * to the actor (a teacher gets only their own groups). Nothing here asks who the user is; the only branch is
 * on a permission — without `groups.manage` the empty state speaks to someone with assigned groups, not to
 * someone who creates them.
 */
export function GroupsList({
  subjectId,
  teacherId,
  includeArchived,
  onlyFreeSeats = false,
  onCreate,
}: {
  subjectId: string | undefined;
  teacherId: string | undefined;
  includeArchived: boolean;
  onlyFreeSeats?: boolean;
  onCreate: () => void;
}) {
  const { t } = useTranslation("groups");
  const { t: tCommon } = useTranslation("common");
  const canManage = useCan(Permissions.GroupsManage);
  const { data, error, isPending, refetch } = useListGroups({
    ...(subjectId === undefined ? {} : { subjectId }),
    ...(teacherId === undefined ? {} : { teacherId }),
    includeArchived,
  });
  const isFiltered = subjectId !== undefined || teacherId !== undefined;

  if (isPending) {
    return (
      <div role="status" aria-label={t("loading.label")} className="space-y-4">
        <div className="grid grid-cols-3 gap-2 sm:gap-3">
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
        </div>
        <Skeleton className="h-14 w-full" />
        <Skeleton className="h-14 w-full" />
        <Skeleton className="h-14 w-full" />
      </div>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    return (
      <Alert variant="destructive">
        <AlertTitle>{t("error.title")}</AlertTitle>
        <AlertDescription className="flex flex-col gap-2">
          <p>{messageFor(apiError)}</p>
          {apiError.correlationId !== undefined && (
            <p>
              {t("error.reference")} <bdi>{apiError.correlationId}</bdi>
            </p>
          )}
          <Button
            type="button"
            variant="outline"
            size="sm"
            className="self-start"
            onClick={() => {
              void refetch();
            }}
```

The excerpt stops at line 90; use the original file for the remaining 96 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
