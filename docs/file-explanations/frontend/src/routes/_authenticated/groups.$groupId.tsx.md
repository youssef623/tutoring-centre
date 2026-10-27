# `frontend/src/routes/_authenticated/groups.$groupId.tsx`

## Purpose

Source comment (intent): The one state for every reason a group is unavailable: an unknown ID, another centre's group, or — for a teacher — a group taught by someone else. The API answers all three with the same 404, and the page adds nothing that could tell them apart.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 418 lines and 15,204 bytes. Source hash: `f8bfa8b9c57dabb377be058147ca818f67c048ca259886999ab467c3292c24df`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useQueryClient } from "@tanstack/react-query";`
- Line 2: `import { Link, createFileRoute, useNavigate } from "@tanstack/react-router";`
- Line 3: `import {`
- Line 15: `import { useState } from "react";`
- Line 16: `import { useTranslation } from "react-i18next";`
- Line 17: `import { toast } from "sonner";`
- Line 18: `import { asApiError } from "@/api/apiFetch";`
- Line 19: `import { messageFor } from "@/api/errorMessages";`
- Line 20: `import {`
- Line 28: `import { StatusBadge } from "@/components/shared/StatusBadge";`
- Line 29: `import {`
- Line 39: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 40: `import { Button } from "@/components/ui/button";`
- Line 41: `import { buttonVariants } from "@/components/ui/button-variants";`
- Line 42: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 43: `import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";`
- Line 44: `import { AssignGroupTeacherDialog, ChangeGroupCapacityDialog, RenameGroupDialog } from "@/features/groups/GroupEditDialogs";`
- Line 45: `import { GroupRosterTab } from "@/features/enrollments/GroupRosterTab";`
- Line 46: `import { SeatMeter } from "@/features/enrollments/SeatMeter";`
- Line 47: `import { GroupScheduleTab } from "@/features/schedule/GroupScheduleTab";`
- Line 48: `import { showGroupError } from "@/features/groups/showGroupError";`
- Line 49: `import { Can } from "@/features/session/Can";`
- Line 50: `import { Permissions } from "@/features/session/permissions";`
- Line 51: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 52: `import { RouteGuard } from "@/features/session/RouteGuard";`
- Line 53: `import { useCan } from "@/features/session/useCan";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 58](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L58): `export interface GroupDetailSearch {`
- [Line 62](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L62): `function isGroupTab(value: unknown): value is GroupTab {`
- [Line 66](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L66): `export const Route = createFileRoute("/_authenticated/groups/$groupId")({`
- [Line 73](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L73): `function GroupDetailPage() {`
- [Line 81](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L81): `function GroupDetailPageContent() {`
- [Line 143](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L143): `function GroupNotFound() {`
- [Line 160](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L160): `function GroupDetailContent({ group }: { group: GroupDto }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/groups.$groupId.test.tsx:66](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.test.tsx#L66)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useQueryClient } from "@tanstack/react-query";
import { Link, createFileRoute, useNavigate } from "@tanstack/react-router";
import {
  Archive,
  ArchiveRestore,
  ArrowLeft,
  BookOpen,
  CalendarDays,
  LayoutGrid,
  Pencil,
  Presentation,
  SearchX,
  Users,
} from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import {
  getGetGroupQueryKey,
  getListGroupsQueryKey,
  useArchiveGroup,
  useGetGroup,
  useRestoreGroup,
  type GroupDto,
} from "@/api/generated/tutoring-centre";
import { StatusBadge } from "@/components/shared/StatusBadge";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { buttonVariants } from "@/components/ui/button-variants";
import { Skeleton } from "@/components/ui/skeleton";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { AssignGroupTeacherDialog, ChangeGroupCapacityDialog, RenameGroupDialog } from "@/features/groups/GroupEditDialogs";
import { GroupRosterTab } from "@/features/enrollments/GroupRosterTab";
import { SeatMeter } from "@/features/enrollments/SeatMeter";
import { GroupScheduleTab } from "@/features/schedule/GroupScheduleTab";
import { showGroupError } from "@/features/groups/showGroupError";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { RouteGuard } from "@/features/session/RouteGuard";
import { useCan } from "@/features/session/useCan";

const GroupTabs = ["overview", "roster", "schedule"] as const;
type GroupTab = (typeof GroupTabs)[number];

export interface GroupDetailSearch {
  tab: GroupTab;
}

function isGroupTab(value: unknown): value is GroupTab {
  return typeof value === "string" && (GroupTabs as readonly string[]).includes(value);
}

export const Route = createFileRoute("/_authenticated/groups/$groupId")({
  validateSearch: (search: Record<string, unknown>): GroupDetailSearch => ({
    tab: isGroupTab(search.tab) ? search.tab : "overview",
  }),
  component: GroupDetailPage,
});

function GroupDetailPage() {
  return (
    <RouteGuard permission={Permissions.GroupsView}>
      <GroupDetailPageContent />
    </RouteGuard>
  );
}

function GroupDetailPageContent() {
  const { groupId } = Route.useParams();
  const { t } = useTranslation("groups");
  const { t: tCommon } = useTranslation("common");
  const { data, error, isPending, refetch } = useGetGroup(groupId);

  if (isPending) {
    return (
      <section className="space-y-4">
        <div role="status" aria-label={t("loading.detailLabel")} className="space-y-4">
```

The excerpt stops at line 90; use the original file for the remaining 328 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
