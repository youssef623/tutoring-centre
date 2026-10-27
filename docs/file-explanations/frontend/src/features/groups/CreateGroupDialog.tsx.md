# `frontend/src/features/groups/CreateGroupDialog.tsx`

## Purpose

Source comment (intent): Creates a group from exactly four fields. Subject and teacher are selects backed by their own lists (active entries only), never free text; a refusal about either (archived meanwhile, or unknown in this centre) lands under that select and refetches its list so the stale entry disappears. On success the new group's own page opens.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 233 lines and 8,686 bytes. Source hash: `c36e9b7b9372cc245ab63cd72b7aa6d45fa4385d31726743b56b267139f5df08`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { zodResolver } from "@hookform/resolvers/zod";`
- Line 2: `import { useQueryClient } from "@tanstack/react-query";`
- Line 3: `import { useNavigate } from "@tanstack/react-router";`
- Line 4: `import { Loader2 } from "lucide-react";`
- Line 5: `import { Controller, useForm } from "react-hook-form";`
- Line 6: `import { useTranslation } from "react-i18next";`
- Line 7: `import { toast } from "sonner";`
- Line 8: `import { asApiError } from "@/api/apiFetch";`
- Line 9: `import { applyTranslatedFieldErrors, setCodedFieldError } from "@/api/fieldErrors";`
- Line 10: `import { showApiError } from "@/api/showApiError";`
- Line 11: `import {`
- Line 17: `import { Button } from "@/components/ui/button";`
- Line 18: `import {`
- Line 26: `import { Input } from "@/components/ui/input";`
- Line 27: `import { Label } from "@/components/ui/label";`
- Line 28: `import { createGroupFormSchema, type CreateGroupFormValues } from "@/features/groups/groupSchemas";`
- Line 29: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 30: `import { SubjectSelect } from "@/features/subjects/SubjectSelect";`
- Line 31: `import { TeacherSelect } from "@/features/teachers/TeacherSelect";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 45](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L45): `export function CreateGroupDialog({ open, onOpenChange }: { open: boolean; onOpenChange: (open: boolean) => void }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/groups.index.tsx:8](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx#L8)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyTranslatedFieldErrors, setCodedFieldError } from "@/api/fieldErrors";
import { showApiError } from "@/api/showApiError";
import {
  getListGroupsQueryKey,
  getListSubjectsQueryKey,
  getListTeachersQueryKey,
  useCreateGroup,
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
import { createGroupFormSchema, type CreateGroupFormValues } from "@/features/groups/groupSchemas";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { SubjectSelect } from "@/features/subjects/SubjectSelect";
import { TeacherSelect } from "@/features/teachers/TeacherSelect";

const KnownFields = ["name", "subjectId", "teacherId", "capacity"] as const;
const SubjectRefusals = new Set(["group.subject_archived", "subject.not_found"]);
const TeacherRefusals = new Set(["group.teacher_archived", "teacher.not_found"]);

const EmptyValues = { name: "", subjectId: "", teacherId: "", capacity: 20 } satisfies CreateGroupFormValues;

/**
 * Creates a group from exactly four fields. Subject and teacher are selects backed by their own lists (active
 * entries only), never free text; a refusal about either (archived meanwhile, or unknown in this centre) lands
 * under that select and refetches its list so the stale entry disappears. On success the new group's own
 * page opens.
 */
export function CreateGroupDialog({ open, onOpenChange }: { open: boolean; onOpenChange: (open: boolean) => void }) {
  const { t } = useTranslation("groups");
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const mutation = useCreateGroup();

  const {
    register,
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<CreateGroupFormValues>({
    resolver: zodResolver(createGroupFormSchema),
    values: open ? EmptyValues : undefined,
  });

  const onSubmit = handleSubmit(async (values) => {
    try {
      const created = await mutation.mutateAsync({
        data: { name: values.name, subjectId: values.subjectId, teacherId: values.teacherId, capacity: values.capacity },
      });
      await queryClient.invalidateQueries({ queryKey: getListGroupsQueryKey() });
      toast.success(t("toasts.created"));
      onOpenChange(false);
      await navigate({ to: "/groups/$groupId", params: { groupId: created.id }, search: { tab: "overview" } });
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (SubjectRefusals.has(apiError.code)) {
        setCodedFieldError(apiError, setError, "subjectId");
        await queryClient.invalidateQueries({ queryKey: getListSubjectsQueryKey() });
        return;
      }

      if (TeacherRefusals.has(apiError.code)) {
        setCodedFieldError(apiError, setError, "teacherId");
        await queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });
        return;
      }

      if (applyTranslatedFieldErrors(apiError, setError, KnownFields)) {
        return;
      }

      showApiError(apiError);
```

The excerpt stops at line 90; use the original file for the remaining 143 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
