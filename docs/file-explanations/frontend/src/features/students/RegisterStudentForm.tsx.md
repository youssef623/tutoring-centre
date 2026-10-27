# `frontend/src/features/students/RegisterStudentForm.tsx`

## Purpose

Source comment (intent): The form the lookup prevents from registering a sibling's parent twice: the client never decides that a phone is valid or unique, it asks the server (via `lookup`) and renders whichever branch the answer picks. Section order follows the Day 42 contract exactly — student fields, then the parent's phone, relationship, and only then either the existing-parent panel or the new parent's name — because a reviewer checks the screen against that order, not just against the request it finally sends.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/students/RegisterStudentForm.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 277 lines and 10,442 bytes. Source hash: `c6a36e117939c3bf372cd6d419136e9b3219013c6a2ccb90aa977bde5f88fc9e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { zodResolver } from "@hookform/resolvers/zod";`
- Line 2: `import { useQueryClient } from "@tanstack/react-query";`
- Line 3: `import { Link, useNavigate } from "@tanstack/react-router";`
- Line 4: `import { Loader2 } from "lucide-react";`
- Line 5: `import { Controller, useForm } from "react-hook-form";`
- Line 6: `import { useTranslation } from "react-i18next";`
- Line 7: `import { toast } from "sonner";`
- Line 8: `import { asApiError } from "@/api/apiFetch";`
- Line 9: `import { applyFieldErrors, showApiError } from "@/api/showApiError";`
- Line 10: `import {`
- Line 16: `import { ParentLookupField } from "@/components/shared/ParentLookupField";`
- Line 17: `import { useParentLookup } from "@/components/shared/useParentLookup";`
- Line 18: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 19: `import { registerStudentFormSchema, type RegisterStudentFormValues } from "@/features/students/registerStudentSchema";`
- Line 20: `import { Button } from "@/components/ui/button";`
- Line 21: `import { buttonVariants } from "@/components/ui/button-variants";`
- Line 22: `import { Input } from "@/components/ui/input";`
- Line 23: `import { Label } from "@/components/ui/label";`
- Line 24: `import { PhoneInput } from "@/components/shared/PhoneInput";`
- Line 25: `import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 36](../../../../../../frontend/src/features/students/RegisterStudentForm.tsx#L36): `export function RegisterStudentForm() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/students.new.test.tsx:41](../../../../../../frontend/src/routes/_authenticated/students.new.test.tsx#L41)
- [frontend/src/routes/_authenticated/students.new.tsx:3](../../../../../../frontend/src/routes/_authenticated/students.new.tsx#L3)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import {
  getListStudentsQueryKey,
  useRegisterStudent,
  ParentRelationship,
  type RegisterStudentRequestParent,
} from "@/api/generated/tutoring-centre";
import { ParentLookupField } from "@/components/shared/ParentLookupField";
import { useParentLookup } from "@/components/shared/useParentLookup";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { registerStudentFormSchema, type RegisterStudentFormValues } from "@/features/students/registerStudentSchema";
import { Button } from "@/components/ui/button";
import { buttonVariants } from "@/components/ui/button-variants";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { PhoneInput } from "@/components/shared/PhoneInput";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

const KnownFields = ["fullName", "phone", "schoolYear", "parent.phone", "parent.fullName"] as const;

/**
 * The form the lookup prevents from registering a sibling's parent twice: the client never decides that a
 * phone is valid or unique, it asks the server (via `lookup`) and renders whichever branch the answer picks.
 * Section order follows the Day 42 contract exactly — student fields, then the parent's phone, relationship,
 * and only then either the existing-parent panel or the new parent's name — because a reviewer checks the
 * screen against that order, not just against the request it finally sends.
 */
export function RegisterStudentForm() {
  const { t } = useTranslation("students");
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const registerMutation = useRegisterStudent();
  const lookup = useParentLookup();

  const {
    register,
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<RegisterStudentFormValues>({
    resolver: zodResolver(registerStudentFormSchema),
    defaultValues: {
      fullName: "",
      phone: "",
      schoolYear: "",
      relationship: ParentRelationship.father,
      parent: { phone: "", fullName: "" },
    },
  });

  const onSubmit = handleSubmit(async (values) => {
    const outcome = await lookup.ensureLookup(values.parent.phone);
    if (!outcome.ok) {
      return;
    }

    let parent: RegisterStudentRequestParent;
    if (outcome.parent !== null) {
      parent = { id: outcome.parent.id };
    } else {
      const parentFullName = values.parent.fullName.trim();
      if (parentFullName === "") {
        setError("parent.fullName", { type: "manual", message: "validation.parentFullNameRequired" });
        return;
      }

      parent = { fullName: parentFullName, phone: values.parent.phone };
    }

    try {
      const result = await registerMutation.mutateAsync({
        data: {
          fullName: values.fullName,
          phone: values.phone === "" ? null : values.phone,
          schoolYear: values.schoolYear === "" ? null : values.schoolYear,
          parent,
          relationship: values.relationship,
        },
      });

      await queryClient.invalidateQueries({ queryKey: getListStudentsQueryKey() });
```

The excerpt stops at line 90; use the original file for the remaining 187 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
