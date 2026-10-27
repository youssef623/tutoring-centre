# `frontend/src/features/teachers/TeachersList.tsx`

## Purpose

Source comment (intent): The teacher directory: summary tiles, then the table. Every manage affordance (row menu, empty-state button) sits behind `teachers.manage`, so a secretary — who holds `teachers.view` only — gets the same page read-only.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/teachers/TeachersList.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 344 lines and 12,748 bytes. Source hash: `86d32153711a3223ef895597c1037668f374639b7ec55f77d8a2ad3c7f50494f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useQueryClient } from "@tanstack/react-query";`
- Line 2: `import { Link2, MoreVertical, Plus, UserCheck, Users, UsersRound } from "lucide-react";`
- Line 3: `import { useMemo, useState } from "react";`
- Line 4: `import { useTranslation } from "react-i18next";`
- Line 5: `import { toast } from "sonner";`
- Line 6: `import { asApiError } from "@/api/apiFetch";`
- Line 7: `import { messageFor } from "@/api/errorMessages";`
- Line 8: `import { showApiError } from "@/api/showApiError";`
- Line 9: `import {`
- Line 16: `import {`
- Line 24: `import { EmptyState } from "@/components/shared/EmptyState";`
- Line 25: `import { PhoneText } from "@/components/shared/PhoneText";`
- Line 26: `import { StatTile } from "@/components/shared/StatTile";`
- Line 27: `import { StatusBadge } from "@/components/shared/StatusBadge";`
- Line 28: `import {`
- Line 38: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 39: `import { Badge } from "@/components/ui/badge";`
- Line 40: `import { Button } from "@/components/ui/button";`
- Line 41: `import {`
- Line 48: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 49: `import { Can } from "@/features/session/Can";`
- Line 50: `import { Permissions } from "@/features/session/permissions";`
- Line 51: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 52: `import { useCan } from "@/features/session/useCan";`
- Line 53: `import { TeacherAccountDialog } from "@/features/teachers/TeacherAccountDialog";`
- Line 54: `import { initialsOf } from "@/lib/initials";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 60](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L60): `export function TeachersList({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/teachers.tsx:12](../../../../../../frontend/src/routes/_authenticated/teachers.tsx#L12)
- [tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs:628](../../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L628)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useQueryClient } from "@tanstack/react-query";
import { Link2, MoreVertical, Plus, UserCheck, Users, UsersRound } from "lucide-react";
import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { showApiError } from "@/api/showApiError";
import {
  getListTeachersQueryKey,
  useArchiveTeacher,
  useListTeachers,
  useRestoreTeacher,
  type TeacherDto,
} from "@/api/generated/tutoring-centre";
import {
  DataTable,
  DataTableBody,
  DataTableCell,
  DataTableColumn,
  DataTableHead,
  DataTableRow,
} from "@/components/shared/DataTable";
import { EmptyState } from "@/components/shared/EmptyState";
import { PhoneText } from "@/components/shared/PhoneText";
import { StatTile } from "@/components/shared/StatTile";
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
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Skeleton } from "@/components/ui/skeleton";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { useCan } from "@/features/session/useCan";
import { TeacherAccountDialog } from "@/features/teachers/TeacherAccountDialog";
import { initialsOf } from "@/lib/initials";

/**
 * The teacher directory: summary tiles, then the table. Every manage affordance (row menu, empty-state button)
 * sits behind `teachers.manage`, so a secretary — who holds `teachers.view` only — gets the same page read-only.
 */
export function TeachersList({
  includeArchived,
  onCreate,
  onEdit,
}: {
  includeArchived: boolean;
  onCreate: () => void;
  onEdit: (teacher: TeacherDto) => void;
}) {
  const { t } = useTranslation("teachers");
  const { t: tCommon } = useTranslation("common");
  const queryClient = useQueryClient();
  const canManage = useCan(Permissions.TeachersManage);
  const { data, error, isPending, refetch } = useListTeachers({ includeArchived });
  const archiveMutation = useArchiveTeacher();
  const restoreMutation = useRestoreTeacher();
  const [archiveTarget, setArchiveTarget] = useState<TeacherDto | null>(null);
  const [accountTarget, setAccountTarget] = useState<TeacherDto | null>(null);
  const [restoringId, setRestoringId] = useState<string | null>(null);

  const linkedUserIds = useMemo(
    () => new Set((data?.items ?? []).flatMap((teacher) => (teacher.userId === null ? [] : [teacher.userId]))),
    [data],
  );

  const invalidate = () => queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });

  const handleArchiveConfirm = async () => {
    if (archiveTarget === null) {
      return;
    }
```

The excerpt stops at line 90; use the original file for the remaining 254 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
