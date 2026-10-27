# `frontend/src/features/parents/ParentEditDialog.tsx`

## Purpose

Source comment (intent): A changed phone is also the lookup key future registrations search by, and (from Month 4) the WhatsApp identity — this dialog says so in one sentence rather than letting the consequence be a surprise. On success every student detail query is invalidated, not just this parent's — a parent appears on each of their children's pages, and there is no per-child list of "which detail queries are mounted right now" to invalidate individually. Matched by query key prefix (every `useGetStudent` key is `[\`/api/students/${id}\`]`, distinct from the list query's exact `/api/students` key) rather than by enumerating student ids.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/parents/ParentEditDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 176 lines and 6,119 bytes. Source hash: `e3a024c27acbe9bf4e09518bd5c4e282a43a64d79db2b16ad8e9820e76242cf8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
- Line 9: `import { getGetParentQueryKey, useUpdateParent, type ParentDetailDto } from "@/api/generated/tutoring-centre";`
- Line 10: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 11: `import { updateParentFormSchema, type UpdateParentFormValues } from "@/features/parents/updateParentSchema";`
- Line 12: `import { Button } from "@/components/ui/button";`
- Line 13: `import {`
- Line 21: `import { Input } from "@/components/ui/input";`
- Line 22: `import { Label } from "@/components/ui/label";`
- Line 23: `import { PhoneInput } from "@/components/shared/PhoneInput";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 36](../../../../../../frontend/src/features/parents/ParentEditDialog.tsx#L36): `export function ParentEditDialog({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/parents.$parentId.tsx:23](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L23)

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
import { getGetParentQueryKey, useUpdateParent, type ParentDetailDto } from "@/api/generated/tutoring-centre";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { updateParentFormSchema, type UpdateParentFormValues } from "@/features/parents/updateParentSchema";
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

const KnownFields = ["fullName", "phone"] as const;

/**
 * A changed phone is also the lookup key future registrations search by, and (from Month 4) the WhatsApp
 * identity — this dialog says so in one sentence rather than letting the consequence be a surprise.
 *
 * On success every student detail query is invalidated, not just this parent's — a parent appears on each of
 * their children's pages, and there is no per-child list of "which detail queries are mounted right now" to
 * invalidate individually. Matched by query key prefix (every `useGetStudent` key is `[\`/api/students/${id}\`]`,
 * distinct from the list query's exact `/api/students` key) rather than by enumerating student ids.
 */
export function ParentEditDialog({
  parent,
  open,
  onOpenChange,
}: {
  parent: ParentDetailDto;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const { t } = useTranslation("parents");
  const queryClient = useQueryClient();
  const mutation = useUpdateParent();

  const {
    register,
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting, isDirty },
  } = useForm<UpdateParentFormValues>({
    resolver: zodResolver(updateParentFormSchema),
    values: open ? { fullName: parent.fullName, phone: parent.phone } : undefined,
  });

  const invalidate = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetParentQueryKey(parent.id) }),
      queryClient.invalidateQueries({
        predicate: (query) =>
          typeof query.queryKey[0] === "string" && query.queryKey[0].startsWith("/api/students/"),
      }),
    ]);

  const onSubmit = handleSubmit(async (values) => {
    try {
      await mutation.mutateAsync({
        id: parent.id,
        data: { fullName: values.fullName, phone: values.phone, version: parent.version },
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

      if (apiError.code === "concurrency.stale" || apiError.code === "parent.not_found") {
        await invalidate();
```

The excerpt stops at line 90; use the original file for the remaining 86 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
