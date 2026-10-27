# `frontend/src/features/dashboard/DashboardOverview.tsx`

## Purpose

Source comment (intent): A group counts as "filling up" from this share of its seats taken; the same threshold as the seat meter's badge.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/dashboard/DashboardOverview.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 161 lines and 6,876 bytes. Source hash: `7278d6542d888a561da44d8e409b6a04b496909876e8d0495448559f1c01c866`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Link } from "@tanstack/react-router";`
- Line 2: `import { Armchair, ArrowRight, CircleSlash, GraduationCap, UserPlus, UsersRound } from "lucide-react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import { asApiError } from "@/api/apiFetch";`
- Line 5: `import { messageFor } from "@/api/errorMessages";`
- Line 6: `import { useListGroups, type GroupDto } from "@/api/generated/tutoring-centre";`
- Line 7: `import { StatTile } from "@/components/shared/StatTile";`
- Line 8: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 9: `import { Button } from "@/components/ui/button";`
- Line 10: `import { buttonVariants } from "@/components/ui/button-variants";`
- Line 11: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 12: `import { SeatMeter } from "@/features/enrollments/SeatMeter";`
- Line 13: `import { Can } from "@/features/session/Can";`
- Line 14: `import { Permissions } from "@/features/session/permissions";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L21): `export function QuickActions() {`
- [Line 53](../../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L53): `export function GroupsOverview() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/index.tsx:4](../../../../../../frontend/src/routes/_authenticated/index.tsx#L4)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Link } from "@tanstack/react-router";
import { Armchair, ArrowRight, CircleSlash, GraduationCap, UserPlus, UsersRound } from "lucide-react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useListGroups, type GroupDto } from "@/api/generated/tutoring-centre";
import { StatTile } from "@/components/shared/StatTile";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { buttonVariants } from "@/components/ui/button-variants";
import { Skeleton } from "@/components/ui/skeleton";
import { SeatMeter } from "@/features/enrollments/SeatMeter";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";

/** A group counts as "filling up" from this share of its seats taken; the same threshold as the seat meter's badge. */
const FillingUpShare = 0.8;
const FillingUpLimit = 5;

/** The shortcuts a session may use, each behind its own permission; nothing renders for a session with none. */
export function QuickActions() {
  const { t } = useTranslation("shell");

  return (
    <div className="flex flex-wrap gap-2">
      <Can permission={Permissions.StudentsManage}>
        <Link to="/students/new" className={buttonVariants()}>
          <UserPlus className="size-4" aria-hidden="true" />
          {t("dashboard.actions.registerStudent")}
        </Link>
      </Can>
      <Can permission={Permissions.StudentsView}>
        <Link to="/students" className={buttonVariants({ variant: "outline" })}>
          <GraduationCap className="size-4" aria-hidden="true" />
          {t("dashboard.actions.students")}
        </Link>
      </Can>
      <Can permission={Permissions.GroupsView}>
        <Link to="/groups" className={buttonVariants({ variant: "outline" })}>
          <UsersRound className="size-4" aria-hidden="true" />
          {t("dashboard.actions.groups")}
        </Link>
      </Can>
    </div>
  );
}

/**
 * Seats across the centre's active groups (for a teacher, the API has already narrowed them to their own), and
 * the groups closest to full. Every figure is summed from the server's own `enrolledCount` and `capacity`.
 * Rendered only with `groups.view`, so without it no request is sent.
 */
export function GroupsOverview() {
  const { t } = useTranslation("shell");
  const { t: tCommon } = useTranslation("common");
  const { data, error, isPending, refetch } = useListGroups();

  if (isPending) {
    return (
      <div role="status" aria-label={t("dashboard.loadingLabel")} className="space-y-4">
        <div className="grid grid-cols-2 gap-2 sm:grid-cols-4 sm:gap-3">
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
        </div>
        <Skeleton className="h-48 w-full" />
      </div>
    );
  }

  if (error) {
    return (
      <Alert variant="destructive">
        <AlertTitle>{t("dashboard.errorTitle")}</AlertTitle>
        <AlertDescription className="flex flex-col gap-2">
          <p>{messageFor(asApiError(error))}</p>
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
```

The excerpt stops at line 90; use the original file for the remaining 71 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
