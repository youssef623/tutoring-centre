# `frontend/src/features/teachers/TeacherAccountDialog.tsx`

## Purpose

Source comment (intent): The two refusals that are about the chosen account itself, shown under the select rather than as a toast.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/teachers/TeacherAccountDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 238 lines and 9,431 bytes. Source hash: `0666b709ff0386a5d7a1efcf82fd5f73f79ab13be744d66d5ef81ab3e7457bf2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useQueryClient } from "@tanstack/react-query";`
- Line 2: `import { Link } from "@tanstack/react-router";`
- Line 3: `import { Link2, Link2Off, Loader2, UserPlus } from "lucide-react";`
- Line 4: `import { useState } from "react";`
- Line 5: `import { useTranslation } from "react-i18next";`
- Line 6: `import { toast } from "sonner";`
- Line 7: `import { asApiError } from "@/api/apiFetch";`
- Line 8: `import { messageFor } from "@/api/errorMessages";`
- Line 9: `import { showApiError } from "@/api/showApiError";`
- Line 10: `import {`
- Line 17: `import { eligibleStaffForTeacher } from "@/features/teachers/eligibleStaff";`
- Line 18: `import { Alert, AlertDescription } from "@/components/ui/alert";`
- Line 19: `import { Button } from "@/components/ui/button";`
- Line 20: `import { buttonVariants } from "@/components/ui/button-variants";`
- Line 21: `import {`
- Line 29: `import { Label } from "@/components/ui/label";`
- Line 30: `import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";`
- Line 31: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 32: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 38](../../../../../../frontend/src/features/teachers/TeacherAccountDialog.tsx#L38): `* Links a teacher record to a staff account, or removes the link. Reuses the Staff module's own endpoint`
- [Line 45](../../../../../../frontend/src/features/teachers/TeacherAccountDialog.tsx#L45): `export function TeacherAccountDialog({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/teachers/TeachersList.tsx:53](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L53)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useQueryClient } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import { Link2, Link2Off, Loader2, UserPlus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { showApiError } from "@/api/showApiError";
import {
  getListStaffQueryKey,
  getListTeachersQueryKey,
  useListStaff,
  useSetTeacherAccount,
  type TeacherDto,
} from "@/api/generated/tutoring-centre";
import { eligibleStaffForTeacher } from "@/features/teachers/eligibleStaff";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { buttonVariants } from "@/components/ui/button-variants";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";

/** The two refusals that are about the chosen account itself, shown under the select rather than as a toast. */
const AccountFieldCodes = new Set(["teacher.user_not_eligible", "teacher.account_taken"]);

/**
 * Links a teacher record to a staff account, or removes the link. Reuses the Staff module's own endpoint
 * (`GET /api/staff`, `staff.view`) for the picker: both that permission and `teachers.manage` are owner-only,
 * so the emails it shows reach exactly the people who can use this dialog. The list is narrowed on the client
 * to active Teacher members not already linked to another teacher — a convenience; the server still decides
 * eligibility and its refusal is shown under the select. Mounted only while open, so the staff list (and its
 * emails) is never requested otherwise.
 */
export function TeacherAccountDialog({
  teacher,
  linkedUserIds,
  onOpenChange,
}: {
  teacher: TeacherDto;
  linkedUserIds: ReadonlySet<string>;
  onOpenChange: (open: boolean) => void;
}) {
  const { t } = useTranslation("teachers");
  const queryClient = useQueryClient();
  const staffQuery = useListStaff();
  const mutation = useSetTeacherAccount();
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);
  const [fieldError, setFieldError] = useState<string | null>(null);

  const eligible = eligibleStaffForTeacher(staffQuery.data?.items ?? [], teacher.userId, linkedUserIds);
  const selected = eligible.find((member) => member.userId === selectedUserId);

  const send = async (userId: string | null) => {
    setFieldError(null);
    try {
      await mutation.mutateAsync({ id: teacher.id, data: { userId, version: teacher.version } });
      await queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });
      toast.success(t(userId === null ? "toasts.accountUnlinked" : "toasts.accountLinked"));
      onOpenChange(false);
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (AccountFieldCodes.has(apiError.code)) {
        setFieldError(messageFor(apiError));
        setSelectedUserId(null);
        await queryClient.invalidateQueries({ queryKey: getListStaffQueryKey() });
        return;
      }

      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
      await queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });
      onOpenChange(false);
    }
  };

  return (
    <Dialog
      open
```

The excerpt stops at line 90; use the original file for the remaining 148 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
