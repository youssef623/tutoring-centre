# `frontend/src/components/ui/input.tsx`

## Purpose

Declares `Input`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/ui/input.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 19 lines and 1,030 bytes. Source hash: `dab990dfefa0ba78854ab6602ad074270dba9d7d1262184d9bfbb62bd990903b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import * as React from "react"`
- Line 2: `import { Input as InputPrimitive } from "@base-ui/react/input"`
- Line 3: `import { cn } from "cn"`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../../frontend/src/components/ui/input.tsx#L5): `function Input({ className, type, ...props }: React.ComponentProps<"input">) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/components/shared/PhoneInput.tsx:1](../../../../../../frontend/src/components/shared/PhoneInput.tsx#L1)
- [frontend/src/components/shared/SearchInput.tsx:4](../../../../../../frontend/src/components/shared/SearchInput.tsx#L4)
- [frontend/src/features/groups/CreateGroupDialog.tsx:26](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L26)
- [frontend/src/features/groups/GroupEditDialogs.tsx:29](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L29)
- [frontend/src/features/parents/ParentEditDialog.tsx:21](../../../../../../frontend/src/features/parents/ParentEditDialog.tsx#L21)
- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:19](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L19)
- [frontend/src/features/schedule/EndScheduleRuleDialog.tsx:15](../../../../../../frontend/src/features/schedule/EndScheduleRuleDialog.tsx#L15)
- [frontend/src/features/schedule/ExtendScheduleDialog.tsx:15](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.tsx#L15)
- [frontend/src/features/staff/StaffFormDialog.tsx:22](../../../../../../frontend/src/features/staff/StaffFormDialog.tsx#L22)
- [frontend/src/features/students/AddParentDialog.tsx:32](../../../../../../frontend/src/features/students/AddParentDialog.tsx#L32)
- [frontend/src/features/students/RegisterStudentForm.tsx:22](../../../../../../frontend/src/features/students/RegisterStudentForm.tsx#L22)
- [frontend/src/features/students/StudentEditDialog.tsx:26](../../../../../../frontend/src/features/students/StudentEditDialog.tsx#L26)
- [frontend/src/features/subjects/SubjectFormDialog.tsx:27](../../../../../../frontend/src/features/subjects/SubjectFormDialog.tsx#L27)
- [frontend/src/features/teachers/TeacherFormDialog.tsx:26](../../../../../../frontend/src/features/teachers/TeacherFormDialog.tsx#L26)
- [frontend/src/routes/_authenticated/audit.tsx:5](../../../../../../frontend/src/routes/_authenticated/audit.tsx#L5)
- [frontend/src/routes/_authenticated/settings.tsx:20](../../../../../../frontend/src/routes/_authenticated/settings.tsx#L20)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import * as React from "react"
import { Input as InputPrimitive } from "@base-ui/react/input"
import { cn } from "cn"

function Input({ className, type, ...props }: React.ComponentProps<"input">) {
  return (
    <InputPrimitive
      type={type}
      data-slot="input"
      className={cn(
        "h-8 w-full min-w-0 rounded-lg border border-input bg-transparent px-2.5 py-1 text-base transition-colors outline-none file:inline-flex file:h-6 file:border-0 file:bg-transparent file:text-sm file:font-medium file:text-foreground placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:pointer-events-none disabled:cursor-not-allowed disabled:bg-input/50 disabled:opacity-50 aria-invalid:border-destructive aria-invalid:ring-3 aria-invalid:ring-destructive/20 md:text-sm dark:bg-input/30 dark:disabled:bg-input/80 dark:aria-invalid:border-destructive/50 dark:aria-invalid:ring-destructive/40",
        className
      )}
      {...props}
    />
  )
}

export { Input }
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
