# `frontend/src/features/staff/StaffList.tsx`

## Purpose

Declares `StaffList`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/staff/StaffList.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 251 lines and 9,013 bytes. Source hash: `7345679e6374786dc2cc615ce13315d44cd953cabc7f79e0363f330bc80d217b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
- Line 9: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 10: `import {`
- Line 17: `import {`
- Line 27: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 28: `import { Badge } from "@/components/ui/badge";`
- Line 29: `import { Button } from "@/components/ui/button";`
- Line 30: `import {`
- Line 36: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 37: `import {`
- Line 45: `import { isolateLtr } from "@/lib/bidi";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 47](../../../../../../frontend/src/features/staff/StaffList.tsx#L47): `export function StaffList({ onChangeRole }: { onChangeRole: (member: StaffMemberDto) => void }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/staff.tsx:7](../../../../../../frontend/src/routes/_authenticated/staff.tsx#L7)
- [tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs:208](../../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L208)

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
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import {
  getListStaffQueryKey,
  useDeactivateStaff,
  useListStaff,
  useReactivateStaff,
  type StaffMemberDto,
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
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Skeleton } from "@/components/ui/skeleton";
import {
  DataTable,
  DataTableBody,
  DataTableCell,
  DataTableColumn,
  DataTableHead,
  DataTableRow,
} from "@/components/shared/DataTable";
import { isolateLtr } from "@/lib/bidi";

export function StaffList({ onChangeRole }: { onChangeRole: (member: StaffMemberDto) => void }) {
  const { t } = useTranslation("staff");
  const { t: tAuth } = useTranslation("auth");
  const { t: tCommon } = useTranslation("common");
  const queryClient = useQueryClient();
  const { data, error, isPending, refetch } = useListStaff();
  const reactivateMutation = useReactivateStaff();
  const deactivateMutation = useDeactivateStaff();
  const [deactivateTarget, setDeactivateTarget] = useState<StaffMemberDto | null>(null);
  const [reactivatingId, setReactivatingId] = useState<string | null>(null);

  const invalidate = () => queryClient.invalidateQueries({ queryKey: getListStaffQueryKey() });

  const handleDeactivateConfirm = async () => {
    if (deactivateTarget === null) {
      return;
    }

    try {
      await deactivateMutation.mutateAsync({
        membershipId: deactivateTarget.membershipId,
        data: { version: deactivateTarget.version },
      });
      await invalidate();
      toast.success(t("toasts.deactivated"));
      setDeactivateTarget(null);
    } catch (thrown) {
      const apiError = asApiError(thrown);
      await invalidate();
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
      setDeactivateTarget(null);
    }
  };

  const handleReactivate = async (member: StaffMemberDto) => {
    setReactivatingId(member.membershipId);
    try {
      await reactivateMutation.mutateAsync({ membershipId: member.membershipId, data: { version: member.version } });
      await invalidate();
      toast.success(t("toasts.reactivated"));
    } catch (thrown) {
      const apiError = asApiError(thrown);
      await invalidate();
```

The excerpt stops at line 90; use the original file for the remaining 161 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
