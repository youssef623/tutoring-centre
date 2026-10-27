# `frontend/src/components/ui/button.tsx`

## Purpose

Declares `Button`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/ui/button.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 21 lines and 527 bytes. Source hash: `18d4119dff18d4f8b503af04e5595adfc0bd1d2000db9df4ff620bfe42f4a0c2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Button as ButtonPrimitive } from "@base-ui/react/button"`
- Line 2: `import type { VariantProps } from "class-variance-authority"`
- Line 3: `import { cn } from "cn"`
- Line 4: `import { buttonVariants } from "./button-variants"`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../frontend/src/components/ui/button.tsx#L6): `function Button({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/components/shared/LoadMoreButton.tsx:2](../../../../../../frontend/src/components/shared/LoadMoreButton.tsx#L2)
- [frontend/src/components/ui/alert-dialog.tsx:5](../../../../../../frontend/src/components/ui/alert-dialog.tsx#L5)
- [frontend/src/components/ui/dialog.tsx:5](../../../../../../frontend/src/components/ui/dialog.tsx#L5)
- [frontend/src/features/audit/AuditLog.tsx:10](../../../../../../frontend/src/features/audit/AuditLog.tsx#L10)
- [frontend/src/features/dashboard/DashboardOverview.tsx:9](../../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L9)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:9](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L9)
- [frontend/src/features/enrollments/EnrollStudentDialog.tsx:7](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L7)
- [frontend/src/features/enrollments/EnrollmentRowMenu.tsx:3](../../../../../../frontend/src/features/enrollments/EnrollmentRowMenu.tsx#L3)
- [frontend/src/features/enrollments/GroupRosterTab.tsx:8](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L8)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.tsx:9](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx#L9)
- [frontend/src/features/enrollments/StudentPicker.tsx:9](../../../../../../frontend/src/features/enrollments/StudentPicker.tsx#L9)
- [frontend/src/features/enrollments/TransferDialog.tsx:9](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L9)
- [frontend/src/features/groups/CreateGroupDialog.tsx:17](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L17)
- [frontend/src/features/groups/GroupEditDialogs.tsx:20](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L20)
- [frontend/src/features/groups/GroupsList.tsx:19](../../../../../../frontend/src/features/groups/GroupsList.tsx#L19)
- [frontend/src/features/language/LanguageSwitcher.tsx:2](../../../../../../frontend/src/features/language/LanguageSwitcher.tsx#L2)
- [frontend/src/features/parents/ParentEditDialog.tsx:12](../../../../../../frontend/src/features/parents/ParentEditDialog.tsx#L12)
- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:10](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L10)
- [frontend/src/features/schedule/EndScheduleRuleDialog.tsx:6](../../../../../../frontend/src/features/schedule/EndScheduleRuleDialog.tsx#L6)
- [frontend/src/features/schedule/ExtendScheduleDialog.tsx:6](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.tsx#L6)
- 30 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Button as ButtonPrimitive } from "@base-ui/react/button"
import type { VariantProps } from "class-variance-authority"
import { cn } from "cn"
import { buttonVariants } from "./button-variants"

function Button({
  className,
  variant = "default",
  size = "default",
  ...props
}: ButtonPrimitive.Props & VariantProps<typeof buttonVariants>) {
  return (
    <ButtonPrimitive
      data-slot="button"
      className={cn(buttonVariants({ variant, size, className }))}
      {...props}
    />
  )
}

export { Button }
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
