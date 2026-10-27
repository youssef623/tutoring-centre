# `frontend/src/features/students/ParentsCard.tsx`

## Purpose

Source comment (intent): Renders the aggregate's rules as affordances only — no Unlink on the only or the primary parent, no "Make primary" on the one already primary, no "Add parent" at four — while the server still enforces every one of them. A direct request that bypasses these (browser console, another client) still gets 422; this card only decides what's convenient to offer, never what's allowed.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/students/ParentsCard.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 213 lines and 7,830 bytes. Source hash: `2f4cda7c73226411af6d4a796b05fafb2e60cb58b25fd4869cceedcc8aa9e9e9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useQueryClient } from "@tanstack/react-query";`
- Line 2: `import { Link } from "@tanstack/react-router";`
- Line 3: `import { MoreVertical, Plus } from "lucide-react";`
- Line 4: `import { useState } from "react";`
- Line 5: `import { useTranslation } from "react-i18next";`
- Line 6: `import { toast } from "sonner";`
- Line 7: `import { asApiError } from "@/api/apiFetch";`
- Line 8: `import { showApiError } from "@/api/showApiError";`
- Line 9: `import {`
- Line 17: `import {`
- Line 27: `import { Badge } from "@/components/ui/badge";`
- Line 28: `import { Button } from "@/components/ui/button";`
- Line 29: `import {`
- Line 35: `import { PhoneText } from "@/components/shared/PhoneText";`
- Line 36: `import { AddParentDialog } from "@/features/students/AddParentDialog";`
- Line 37: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 47](../../../../../../frontend/src/features/students/ParentsCard.tsx#L47): `export function ParentsCard({ student, canManage }: { student: StudentDetailDto; canManage: boolean }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/students.$studentId.test.tsx:47](../../../../../../frontend/src/routes/_authenticated/students.$studentId.test.tsx#L47)
- [frontend/src/routes/_authenticated/students.$studentId.tsx:37](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L37)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useQueryClient } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import { MoreVertical, Plus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { showApiError } from "@/api/showApiError";
import {
  getGetParentQueryKey,
  getGetStudentQueryKey,
  useMakeStudentParentPrimary,
  useUnlinkStudentParent,
  type StudentDetailDto,
  type StudentParentDto,
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
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { PhoneText } from "@/components/shared/PhoneText";
import { AddParentDialog } from "@/features/students/AddParentDialog";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";

const MaxParents = 4;

/**
 * Renders the aggregate's rules as affordances only — no Unlink on the only or the primary parent, no
 * "Make primary" on the one already primary, no "Add parent" at four — while the server still enforces every
 * one of them. A direct request that bypasses these (browser console, another client) still gets 422; this
 * card only decides what's convenient to offer, never what's allowed.
 */
export function ParentsCard({ student, canManage }: { student: StudentDetailDto; canManage: boolean }) {
  const { t } = useTranslation("students");
  const queryClient = useQueryClient();
  const makePrimaryMutation = useMakeStudentParentPrimary();
  const unlinkMutation = useUnlinkStudentParent();
  const [addOpen, setAddOpen] = useState(false);
  const [unlinkTarget, setUnlinkTarget] = useState<StudentParentDto | null>(null);

  const sortedParents = [...student.parents].sort((a, b) => Number(b.isPrimary) - Number(a.isPrimary));
  const isOnlyParent = student.parents.length === 1;
  const canAddMore = student.parents.length < MaxParents;

  const invalidate = (parentId: string) =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) }),
      queryClient.invalidateQueries({ queryKey: getGetParentQueryKey(parentId) }),
    ]);

  const handleMakePrimary = async (parent: StudentParentDto) => {
    try {
      await makePrimaryMutation.mutateAsync({
        id: student.id,
        parentId: parent.parentId,
        data: { version: student.version },
      });
      await invalidate(parent.parentId);
      toast.success(t("toasts.parentMadePrimary"));
    } catch (thrown) {
      const apiError = asApiError(thrown);
      await queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) });
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
    }
  };

  const handleUnlinkConfirm = async () => {
    if (unlinkTarget === null) {
      return;
    }

    try {
      await unlinkMutation.mutateAsync({
        id: student.id,
        parentId: unlinkTarget.parentId,
```

The excerpt stops at line 90; use the original file for the remaining 123 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
