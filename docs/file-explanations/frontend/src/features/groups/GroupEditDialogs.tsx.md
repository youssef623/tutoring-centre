# `frontend/src/features/groups/GroupEditDialogs.tsx`

## Purpose

Source comment (intent): Toast for anything not shown in the form; a stale version or a vanished group also closes and refetches.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 345 lines and 10,773 bytes. Source hash: `719173452b9f46cc37b86ac0019a70f1379144527de6e007480ccc15a36dfb5b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { zodResolver } from "@hookform/resolvers/zod";`
- Line 2: `import { useQueryClient } from "@tanstack/react-query";`
- Line 3: `import { Loader2 } from "lucide-react";`
- Line 4: `import type { ReactNode } from "react";`
- Line 5: `import { Controller, useForm, type FieldError } from "react-hook-form";`
- Line 6: `import { useTranslation } from "react-i18next";`
- Line 7: `import { toast } from "sonner";`
- Line 8: `import { asApiError } from "@/api/apiFetch";`
- Line 9: `import type { ApiError } from "@/api/errors";`
- Line 10: `import { applyTranslatedFieldErrors, setCodedFieldError } from "@/api/fieldErrors";`
- Line 11: `import {`
- Line 20: `import { Button } from "@/components/ui/button";`
- Line 21: `import {`
- Line 29: `import { Input } from "@/components/ui/input";`
- Line 30: `import { Label } from "@/components/ui/label";`
- Line 31: `import {`
- Line 39: `import { showGroupError } from "@/features/groups/showGroupError";`
- Line 40: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 41: `import { TeacherSelect } from "@/features/teachers/TeacherSelect";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 49](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L49): `interface GroupDialogProps {`
- [Line 55](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L55): `function useGroupMutationOutcome(group: GroupDto, onOpenChange: (open: boolean) => void) {`
- [Line 81](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L81): `function FieldMessage({ id, error, t }: { id: string; error: FieldError | undefined; t: (key: string) => string }) {`
- [Line 93](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L93): `function GroupDialogFrame({`
- [Line 157](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L157): `export function RenameGroupDialog({ group, open, onOpenChange }: GroupDialogProps) {`
- [Line 211](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L211): `export function ChangeGroupCapacityDialog({ group, open, onOpenChange }: GroupDialogProps) {`
- [Line 280](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L280): `export function AssignGroupTeacherDialog({ group, open, onOpenChange }: GroupDialogProps) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/groups.$groupId.tsx:44](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L44)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";
import type { ReactNode } from "react";
import { Controller, useForm, type FieldError } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import type { ApiError } from "@/api/errors";
import { applyTranslatedFieldErrors, setCodedFieldError } from "@/api/fieldErrors";
import {
  getGetGroupQueryKey,
  getListGroupsQueryKey,
  getListTeachersQueryKey,
  useAssignGroupTeacher,
  useChangeGroupCapacity,
  useRenameGroup,
  type GroupDto,
} from "@/api/generated/tutoring-centre";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  assignGroupTeacherFormSchema,
  groupCapacityFormSchema,
  renameGroupFormSchema,
  type AssignGroupTeacherFormValues,
  type GroupCapacityFormValues,
  type RenameGroupFormValues,
} from "@/features/groups/groupSchemas";
import { showGroupError } from "@/features/groups/showGroupError";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { TeacherSelect } from "@/features/teachers/TeacherSelect";

/*
 * Three facets, three dialogs, three requests — never one "Edit group" form posting to several endpoints in
 * sequence, where a failure in the second would leave a half-applied edit and a stale version. Each dialog
 * sends the version the page loaded; on `concurrency.stale` it toasts, closes and the page refetches.
 */

interface GroupDialogProps {
  group: GroupDto;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

function useGroupMutationOutcome(group: GroupDto, onOpenChange: (open: boolean) => void) {
  const queryClient = useQueryClient();

  const invalidate = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetGroupQueryKey(group.id) }),
      queryClient.invalidateQueries({ queryKey: getListGroupsQueryKey() }),
    ]);

  const succeed = async (message: string) => {
    await invalidate();
    toast.success(message);
    onOpenChange(false);
  };

  /** Toast for anything not shown in the form; a stale version or a vanished group also closes and refetches. */
  const fail = async (apiError: ApiError) => {
    showGroupError(apiError);
    refetchMeOnPermissionDenied(apiError, queryClient);
    await invalidate();
    onOpenChange(false);
  };

  return { succeed, fail, queryClient };
}

function FieldMessage({ id, error, t }: { id: string; error: FieldError | undefined; t: (key: string) => string }) {
  if (error === undefined) {
    return null;
  }

  return (
    <p id={id} className="text-sm text-destructive">
      {error.type === "server" ? error.message : t(error.message ?? "")}
    </p>
  );
```

The excerpt stops at line 90; use the original file for the remaining 255 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
