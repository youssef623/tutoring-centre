# `frontend/src/features/students/StudentEditDialog.tsx`

## Purpose

Source comment (intent): Prefilled from the student detail query's own data, not a copy kept in this component's state: `values` stays tied to `student` while the dialog is open, so closing and reopening (or a background refetch) always shows the latest loaded row, never a stale snapshot from the moment the dialog first mounted.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/students/StudentEditDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 195 lines and 6,642 bytes. Source hash: `9d7186702495cfc0e6d33764f3f568f6c5b110af8430a90aeeedfa58a11071c9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { zodResolver } from "@hookform/resolvers/zod";`
- Line 2: `import { useQueryClient } from "@tanstack/react-query";`
- Line 3: `import { Loader2 } from "lucide-react";`
- Line 4: `import { Controller, useForm } from "react-hook-form";`
- Line 5: `import { useTranslation } from "react-i18next";`
- Line 6: `import { toast } from "sonner";`
- Line 7: `import { asApiError } from "@/api/apiFetch";`
- Line 8: `import { applyFieldErrors, showApiError } from "@/api/showApiError";`
- Line 9: `import {`
- Line 15: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 16: `import { updateStudentFormSchema, type UpdateStudentFormValues } from "@/features/students/updateStudentSchema";`
- Line 17: `import { Button } from "@/components/ui/button";`
- Line 18: `import {`
- Line 26: `import { Input } from "@/components/ui/input";`
- Line 27: `import { Label } from "@/components/ui/label";`
- Line 28: `import { PhoneInput } from "@/components/shared/PhoneInput";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 37](../../../../../../frontend/src/features/students/StudentEditDialog.tsx#L37): `export function StudentEditDialog({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/students.$studentId.tsx:38](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L38)

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
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import {
  getGetStudentQueryKey,
  getListStudentsQueryKey,
  useUpdateStudent,
  type StudentDetailDto,
} from "@/api/generated/tutoring-centre";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { updateStudentFormSchema, type UpdateStudentFormValues } from "@/features/students/updateStudentSchema";
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
import { PhoneInput } from "@/components/shared/PhoneInput";

const KnownFields = ["fullName", "phone", "schoolYear"] as const;

/**
 * Prefilled from the student detail query's own data, not a copy kept in this component's state: `values`
 * stays tied to `student` while the dialog is open, so closing and reopening (or a background refetch) always
 * shows the latest loaded row, never a stale snapshot from the moment the dialog first mounted.
 */
export function StudentEditDialog({
  student,
  open,
  onOpenChange,
}: {
  student: StudentDetailDto;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const { t } = useTranslation("students");
  const queryClient = useQueryClient();
  const mutation = useUpdateStudent();

  const {
    register,
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting, isDirty },
  } = useForm<UpdateStudentFormValues>({
    resolver: zodResolver(updateStudentFormSchema),
    values: open ? { fullName: student.fullName, phone: student.phone ?? "", schoolYear: student.schoolYear ?? "" } : undefined,
  });

  const invalidate = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) }),
      queryClient.invalidateQueries({ queryKey: getListStudentsQueryKey() }),
    ]);

  const onSubmit = handleSubmit(async (values) => {
    try {
      await mutation.mutateAsync({
        id: student.id,
        data: {
          fullName: values.fullName,
          phone: values.phone === "" ? null : values.phone,
          schoolYear: values.schoolYear === "" ? null : values.schoolYear,
          version: student.version,
        },
      });
      await invalidate();
      toast.success(t("toasts.updated"));
      onOpenChange(false);
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (apiError.fieldErrors !== undefined) {
        applyFieldErrors(apiError, setError, KnownFields);
        return;
      }

      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
```

The excerpt stops at line 90; use the original file for the remaining 105 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
