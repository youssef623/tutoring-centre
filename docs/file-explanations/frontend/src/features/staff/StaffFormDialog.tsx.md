# `frontend/src/features/staff/StaffFormDialog.tsx`

## Purpose

Source comment (intent): Create-member dialog, followed by the one-time password panel. The temporary password lives only in this component's own state (`revealed`), set directly from the mutation's resolved value — never read back from the mutation's own cached `data` — and is discarded the moment the dialog closes: nothing is retained in the query cache, the URL, storage, a toast or a log.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/staff/StaffFormDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 284 lines and 10,398 bytes. Source hash: `c2b0b1c90d191f6246647beba239579ea1a2d21af4522391d136ea410f81c770`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { zodResolver } from "@hookform/resolvers/zod";`
- Line 2: `import { useQueryClient } from "@tanstack/react-query";`
- Line 3: `import { Check, Copy, Loader2 } from "lucide-react";`
- Line 4: `import { useState } from "react";`
- Line 5: `import { Controller, useForm } from "react-hook-form";`
- Line 6: `import { useTranslation } from "react-i18next";`
- Line 7: `import { toast } from "sonner";`
- Line 8: `import { asApiError } from "@/api/apiFetch";`
- Line 9: `import { applyFieldErrors, showApiError } from "@/api/showApiError";`
- Line 10: `import { getListStaffQueryKey, useCreateStaff, StaffRole } from "@/api/generated/tutoring-centre";`
- Line 11: `import { createStaffFormSchema, type CreateStaffFormValues } from "@/features/staff/staffSchema";`
- Line 12: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 13: `import { Button } from "@/components/ui/button";`
- Line 14: `import {`
- Line 22: `import { Input } from "@/components/ui/input";`
- Line 23: `import { Label } from "@/components/ui/label";`
- Line 24: `import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 34](../../../../../../frontend/src/features/staff/StaffFormDialog.tsx#L34): `export function StaffFormDialog({ open, onOpenChange }: { open: boolean; onOpenChange: (open: boolean) => void }) {`
- [Line 225](../../../../../../frontend/src/features/staff/StaffFormDialog.tsx#L225): `function RoleField({`
- [Line 254](../../../../../../frontend/src/features/staff/StaffFormDialog.tsx#L254): `function LocaleField({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/staff.tsx:6](../../../../../../frontend/src/routes/_authenticated/staff.tsx#L6)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Check, Copy, Loader2 } from "lucide-react";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import { getListStaffQueryKey, useCreateStaff, StaffRole } from "@/api/generated/tutoring-centre";
import { createStaffFormSchema, type CreateStaffFormValues } from "@/features/staff/staffSchema";
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
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

const KnownFields = ["email", "displayName"] as const;

/**
 * Create-member dialog, followed by the one-time password panel. The temporary password lives only in this
 * component's own state (`revealed`), set directly from the mutation's resolved value — never read back from
 * the mutation's own cached `data` — and is discarded the moment the dialog closes: nothing is retained in the
 * query cache, the URL, storage, a toast or a log.
 */
export function StaffFormDialog({ open, onOpenChange }: { open: boolean; onOpenChange: (open: boolean) => void }) {
  const { t } = useTranslation("staff");
  const queryClient = useQueryClient();
  const createMutation = useCreateStaff();
  const [revealed, setRevealed] = useState<{ displayName: string; password: string } | null>(null);
  const [existingAccountName, setExistingAccountName] = useState<string | null>(null);
  const [copied, setCopied] = useState(false);

  const {
    register,
    control,
    handleSubmit,
    setError,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<CreateStaffFormValues>({
    resolver: zodResolver(createStaffFormSchema),
    defaultValues: { email: "", displayName: "", role: StaffRole.teacher, preferredLocale: "en" },
  });

  const closeAndReset = () => {
    onOpenChange(false);
    reset({ email: "", displayName: "", role: StaffRole.teacher, preferredLocale: "en" });
    setRevealed(null);
    setExistingAccountName(null);
    setCopied(false);
    createMutation.reset();
  };

  const onSubmit = handleSubmit(async (values) => {
    try {
      const result = await createMutation.mutateAsync({ data: values });
      await queryClient.invalidateQueries({ queryKey: getListStaffQueryKey() });

      if (result.temporaryPassword !== null) {
        setRevealed({ displayName: values.displayName, password: result.temporaryPassword });
      } else {
        setExistingAccountName(values.displayName);
      }

      toast.success(t("toasts.created"));
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (apiError.fieldErrors !== undefined) {
        applyFieldErrors(apiError, setError, KnownFields);
        return;
      }

      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
    }
  });

  const handleCopy = async () => {
    if (revealed === null) {
      return;
```

The excerpt stops at line 90; use the original file for the remaining 194 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
