# `frontend/src/features/subjects/SubjectFormDialog.tsx`

## Purpose

Declares `SubjectFormDialog`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/subjects/SubjectFormDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 162 lines and 5,011 bytes. Source hash: `e3a851fc62f63ea1f949849679d588f54ed849fe04e97cd5c0bfa5d59c4196c8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { zodResolver } from "@hookform/resolvers/zod";`
- Line 2: `import { useQueryClient } from "@tanstack/react-query";`
- Line 3: `import { Loader2 } from "lucide-react";`
- Line 4: `import { useEffect } from "react";`
- Line 5: `import { useForm } from "react-hook-form";`
- Line 6: `import { useTranslation } from "react-i18next";`
- Line 7: `import { toast } from "sonner";`
- Line 8: `import { asApiError } from "@/api/apiFetch";`
- Line 9: `import { applyFieldErrors, showApiError } from "@/api/showApiError";`
- Line 10: `import {`
- Line 16: `import { subjectFormSchema, type SubjectFormValues } from "@/features/subjects/subjectSchema";`
- Line 17: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 18: `import { Button } from "@/components/ui/button";`
- Line 19: `import {`
- Line 27: `import { Input } from "@/components/ui/input";`
- Line 28: `import { Label } from "@/components/ui/label";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 30](../../../../../../frontend/src/features/subjects/SubjectFormDialog.tsx#L30): `export type SubjectDialogState = { mode: "create" } | { mode: "rename"; subject: SubjectDto } | null;`
- [Line 34](../../../../../../frontend/src/features/subjects/SubjectFormDialog.tsx#L34): `export function SubjectFormDialog({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/subjects.tsx:5](../../../../../../frontend/src/routes/_authenticated/subjects.tsx#L5)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import {
  getListSubjectsQueryKey,
  useCreateSubject,
  useRenameSubject,
  type SubjectDto,
} from "@/api/generated/tutoring-centre";
import { subjectFormSchema, type SubjectFormValues } from "@/features/subjects/subjectSchema";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
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

export type SubjectDialogState = { mode: "create" } | { mode: "rename"; subject: SubjectDto } | null;

const KnownFields = ["name"] as const;

export function SubjectFormDialog({
  state,
  onOpenChange,
}: {
  state: SubjectDialogState;
  onOpenChange: (open: boolean) => void;
}) {
  const { t } = useTranslation("subjects");
  const queryClient = useQueryClient();
  const createMutation = useCreateSubject();
  const renameMutation = useRenameSubject();

  const {
    register,
    handleSubmit,
    setError,
    reset,
    formState: { errors, isSubmitting, isDirty },
  } = useForm<SubjectFormValues>({ resolver: zodResolver(subjectFormSchema), defaultValues: { name: "" } });

  const open = state !== null;
  const mode = state?.mode;

  useEffect(() => {
    if (state !== null) {
      reset({ name: state.mode === "rename" ? state.subject.name : "" });
    }
  }, [state, reset]);

  const invalidate = () => queryClient.invalidateQueries({ queryKey: getListSubjectsQueryKey() });

  const onSubmit = handleSubmit(async (values) => {
    if (state === null) {
      return;
    }

    try {
      if (state.mode === "create") {
        await createMutation.mutateAsync({ data: { name: values.name } });
        await invalidate();
        toast.success(t("toasts.created"));
        onOpenChange(false);
      } else {
        await renameMutation.mutateAsync({
          id: state.subject.id,
          data: { name: values.name, version: state.subject.version },
        });
        await invalidate();
        toast.success(t("toasts.renamed"));
        onOpenChange(false);
      }
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (apiError.fieldErrors !== undefined) {
        applyFieldErrors(apiError, setError, KnownFields);
        return;
```

The excerpt stops at line 90; use the original file for the remaining 72 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
