# `frontend/src/features/subjects/SubjectsList.tsx`

## Purpose

Declares `SubjectsList`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/subjects/SubjectsList.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 247 lines and 8,377 bytes. Source hash: `0d61252c467393d5b9ae5070fce20419fa2bdf08bf30945f2908e74f9825fa17`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useQueryClient } from "@tanstack/react-query";`
- Line 2: `import { MoreVertical } from "lucide-react";`
- Line 3: `import { useState } from "react";`
- Line 4: `import { useTranslation } from "react-i18next";`
- Line 5: `import { toast } from "sonner";`
- Line 6: `import { asApiError } from "@/api/apiFetch";`
- Line 7: `import { messageFor } from "@/api/errorMessages";`
- Line 8: `import { showApiError } from "@/api/showApiError";`
- Line 9: `import {`
- Line 16: `import {`
- Line 26: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 27: `import {`
- Line 35: `import { StatusBadge } from "@/components/shared/StatusBadge";`
- Line 36: `import { Button } from "@/components/ui/button";`
- Line 37: `import {`
- Line 43: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 44: `import { Permissions } from "@/features/session/permissions";`
- Line 45: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 46: `import { useCan } from "@/features/session/useCan";`
- Line 47: `import { isolateLtr } from "@/lib/bidi";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 49](../../../../../../frontend/src/features/subjects/SubjectsList.tsx#L49): `export function SubjectsList({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/enrich_frontend.py:14](../../../../../onboarding-work/enrich_frontend.py#L14)
- [frontend/src/routes/_authenticated/subjects.tsx:6](../../../../../../frontend/src/routes/_authenticated/subjects.tsx#L6)
- [tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs:101](../../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L101)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useQueryClient } from "@tanstack/react-query";
import { MoreVertical } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { showApiError } from "@/api/showApiError";
import {
  getListSubjectsQueryKey,
  useArchiveSubject,
  useListSubjects,
  useRestoreSubject,
  type SubjectDto,
} from "@/api/generated/tutoring-centre";
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
import {
  DataTable,
  DataTableBody,
  DataTableCell,
  DataTableColumn,
  DataTableHead,
  DataTableRow,
} from "@/components/shared/DataTable";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Skeleton } from "@/components/ui/skeleton";
import { Permissions } from "@/features/session/permissions";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { useCan } from "@/features/session/useCan";
import { isolateLtr } from "@/lib/bidi";

export function SubjectsList({
  includeArchived,
  onRename,
}: {
  includeArchived: boolean;
  onRename: (subject: SubjectDto) => void;
}) {
  const { t } = useTranslation("subjects");
  const { t: tCommon } = useTranslation("common");
  const queryClient = useQueryClient();
  const canManage = useCan(Permissions.SubjectsManage);
  const { data, error, isPending, refetch } = useListSubjects({ includeArchived });
  const archiveMutation = useArchiveSubject();
  const restoreMutation = useRestoreSubject();
  const [archiveTarget, setArchiveTarget] = useState<SubjectDto | null>(null);
  const [restoringId, setRestoringId] = useState<string | null>(null);

  const invalidate = () => queryClient.invalidateQueries({ queryKey: getListSubjectsQueryKey() });

  const handleArchiveConfirm = async () => {
    if (archiveTarget === null) {
      return;
    }

    try {
      await archiveMutation.mutateAsync({ id: archiveTarget.id, data: { version: archiveTarget.version } });
      await invalidate();
      toast.success(t("toasts.archived"));
      setArchiveTarget(null);
    } catch (thrown) {
      const apiError = asApiError(thrown);
      await invalidate();
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
      setArchiveTarget(null);
    }
  };

  const handleRestore = async (subject: SubjectDto) => {
    setRestoringId(subject.id);
    try {
      await restoreMutation.mutateAsync({ id: subject.id, data: { version: subject.version } });
```

The excerpt stops at line 90; use the original file for the remaining 157 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
