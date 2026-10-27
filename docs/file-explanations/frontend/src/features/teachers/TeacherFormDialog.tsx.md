# `frontend/src/features/teachers/TeacherFormDialog.tsx`

## Purpose

Source comment (intent): Create and edit share one form: full name and an optional phone, nothing else. The account link is never a field here — it is its own dialog and its own request (PUT .../account), so editing details cannot touch it.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/teachers/TeacherFormDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 195 lines and 6,404 bytes. Source hash: `0a842358c1d84c19a970727c77ea5badf13dd32bb6dded642bc80b64239416e3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { zodResolver } from "@hookform/resolvers/zod";`
- Line 2: `import { useQueryClient } from "@tanstack/react-query";`
- Line 3: `import { Loader2 } from "lucide-react";`
- Line 4: `import { Controller, useForm } from "react-hook-form";`
- Line 5: `import { useTranslation } from "react-i18next";`
- Line 6: `import { toast } from "sonner";`
- Line 7: `import { asApiError } from "@/api/apiFetch";`
- Line 8: `import { applyTranslatedFieldErrors } from "@/api/fieldErrors";`
- Line 9: `import { showApiError } from "@/api/showApiError";`
- Line 10: `import {`
- Line 16: `import { PhoneInput } from "@/components/shared/PhoneInput";`
- Line 17: `import { Button } from "@/components/ui/button";`
- Line 18: `import {`
- Line 26: `import { Input } from "@/components/ui/input";`
- Line 27: `import { Label } from "@/components/ui/label";`
- Line 28: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 29: `import { teacherFormSchema, type TeacherFormValues } from "@/features/teachers/teacherSchema";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 31](../../../../../../frontend/src/features/teachers/TeacherFormDialog.tsx#L31): `export type TeacherDialogState = { mode: "create" } | { mode: "edit"; teacher: TeacherDto } | null;`
- [Line 39](../../../../../../frontend/src/features/teachers/TeacherFormDialog.tsx#L39): `export function TeacherFormDialog({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/teachers.tsx:11](../../../../../../frontend/src/routes/_authenticated/teachers.tsx#L11)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyTranslatedFieldErrors } from "@/api/fieldErrors";
import { showApiError } from "@/api/showApiError";
import {
  getListTeachersQueryKey,
  useCreateTeacher,
  useUpdateTeacher,
  type TeacherDto,
} from "@/api/generated/tutoring-centre";
import { PhoneInput } from "@/components/shared/PhoneInput";
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
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { teacherFormSchema, type TeacherFormValues } from "@/features/teachers/teacherSchema";

export type TeacherDialogState = { mode: "create" } | { mode: "edit"; teacher: TeacherDto } | null;

const KnownFields = ["fullName", "phone"] as const;

/**
 * Create and edit share one form: full name and an optional phone, nothing else. The account link is never a
 * field here — it is its own dialog and its own request (PUT .../account), so editing details cannot touch it.
 */
export function TeacherFormDialog({
  state,
  onOpenChange,
}: {
  state: TeacherDialogState;
  onOpenChange: (open: boolean) => void;
}) {
  const { t } = useTranslation("teachers");
  const queryClient = useQueryClient();
  const createMutation = useCreateTeacher();
  const updateMutation = useUpdateTeacher();
  const open = state !== null;
  const mode = state?.mode;

  const {
    register,
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting, isDirty },
  } = useForm<TeacherFormValues>({
    resolver: zodResolver(teacherFormSchema),
    values:
      state === null
        ? undefined
        : state.mode === "edit"
          ? { fullName: state.teacher.fullName, phone: state.teacher.phone ?? "" }
          : { fullName: "", phone: "" },
  });

  const invalidate = () => queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });

  const onSubmit = handleSubmit(async (values) => {
    if (state === null) {
      return;
    }

    const phone = values.phone === "" ? null : values.phone;

    try {
      if (state.mode === "create") {
        await createMutation.mutateAsync({ data: { fullName: values.fullName, phone } });
        await invalidate();
        toast.success(t("toasts.created"));
      } else {
        await updateMutation.mutateAsync({
          id: state.teacher.id,
          data: { fullName: values.fullName, phone, version: state.teacher.version },
        });
        await invalidate();
        toast.success(t("toasts.updated"));
      }
```

The excerpt stops at line 90; use the original file for the remaining 105 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
