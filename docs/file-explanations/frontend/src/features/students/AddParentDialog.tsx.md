# `frontend/src/features/students/AddParentDialog.tsx`

## Purpose

Source comment (intent): Found: link the existing parent directly. Not found: this dialog creates the parent, then links the new id — two requests, never presented as atomic (Day 42 rules; only registration's single transaction gets to say "one request"). If the link half fails after the parent now exists, the dialog remembers that id (`createdParentId`) and a retry only repeats the link, never a second create.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/students/AddParentDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 256 lines and 9,467 bytes. Source hash: `892e6e11aa4ef70c71e1d651a9db73f80f8fe70f08d0af06ebb05cd028a9da5f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { zodResolver } from "@hookform/resolvers/zod";`
- Line 2: `import { useQueryClient } from "@tanstack/react-query";`
- Line 3: `import { Loader2 } from "lucide-react";`
- Line 4: `import { useState } from "react";`
- Line 5: `import { Controller, useForm } from "react-hook-form";`
- Line 6: `import { useTranslation } from "react-i18next";`
- Line 7: `import { toast } from "sonner";`
- Line 8: `import { asApiError } from "@/api/apiFetch";`
- Line 9: `import { applyFieldErrors, showApiError } from "@/api/showApiError";`
- Line 10: `import {`
- Line 18: `import { ParentLookupField } from "@/components/shared/ParentLookupField";`
- Line 19: `import { useParentLookup } from "@/components/shared/useParentLookup";`
- Line 20: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 21: `import { addParentFormSchema, type AddParentFormValues } from "@/features/students/addParentSchema";`
- Line 22: `import { Alert, AlertDescription } from "@/components/ui/alert";`
- Line 23: `import { Button } from "@/components/ui/button";`
- Line 24: `import {`
- Line 32: `import { Input } from "@/components/ui/input";`
- Line 33: `import { Label } from "@/components/ui/label";`
- Line 34: `import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 44](../../../../../../frontend/src/features/students/AddParentDialog.tsx#L44): `export function AddParentDialog({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/students/ParentsCard.tsx:36](../../../../../../frontend/src/features/students/ParentsCard.tsx#L36)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import {
  getGetParentQueryKey,
  getGetStudentQueryKey,
  useCreateParent,
  useLinkStudentParent,
  ParentRelationship,
  type StudentDetailDto,
} from "@/api/generated/tutoring-centre";
import { ParentLookupField } from "@/components/shared/ParentLookupField";
import { useParentLookup } from "@/components/shared/useParentLookup";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { addParentFormSchema, type AddParentFormValues } from "@/features/students/addParentSchema";
import { Alert, AlertDescription } from "@/components/ui/alert";
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

const KnownFields = ["fullName", "phone"] as const;

/**
 * Found: link the existing parent directly. Not found: this dialog creates the parent, then links the new id —
 * two requests, never presented as atomic (Day 42 rules; only registration's single transaction gets to say
 * "one request"). If the link half fails after the parent now exists, the dialog remembers that id
 * (`createdParentId`) and a retry only repeats the link, never a second create.
 */
export function AddParentDialog({
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
  const lookup = useParentLookup();
  const createMutation = useCreateParent();
  const linkMutation = useLinkStudentParent();
  const [createdParentId, setCreatedParentId] = useState<string | null>(null);
  const [linkFailed, setLinkFailed] = useState(false);

  const {
    register,
    control,
    handleSubmit,
    setError,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<AddParentFormValues>({
    resolver: zodResolver(addParentFormSchema),
    defaultValues: { phone: "", fullName: "", relationship: ParentRelationship.father },
  });

  const closeAndReset = () => {
    onOpenChange(false);
    reset({ phone: "", fullName: "", relationship: ParentRelationship.father });
    lookup.reset();
    setCreatedParentId(null);
    setLinkFailed(false);
  };

  const invalidate = (parentId: string) =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) }),
      queryClient.invalidateQueries({ queryKey: getGetParentQueryKey(parentId) }),
    ]);

  const linkParent = async (parentId: string, relationship: AddParentFormValues["relationship"]) => {
    try {
      await linkMutation.mutateAsync({ id: student.id, data: { parentId, relationship, version: student.version } });
      await invalidate(parentId);
```

The excerpt stops at line 90; use the original file for the remaining 166 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
