# `frontend/src/components/ui/badge.tsx`

## Purpose

Declares `Badge`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/ui/badge.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 29 lines and 666 bytes. Source hash: `846b9735c9d51f3bfb32fbd0c4b86544bc341789b5d8b3b06c3316a8993b080e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { mergeProps } from "@base-ui/react/merge-props"`
- Line 2: `import { useRender } from "@base-ui/react/use-render"`
- Line 3: `import type { VariantProps } from "class-variance-authority"`
- Line 4: `import { cn } from "cn"`
- Line 5: `import { badgeVariants } from "./badge-variants"`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../frontend/src/components/ui/badge.tsx#L7): `function Badge({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/components/shared/StatusBadge.tsx:2](../../../../../../frontend/src/components/shared/StatusBadge.tsx#L2)
- [frontend/src/features/audit/AuditLog.tsx:9](../../../../../../frontend/src/features/audit/AuditLog.tsx#L9)
- [frontend/src/features/enrollments/SeatMeter.tsx:2](../../../../../../frontend/src/features/enrollments/SeatMeter.tsx#L2)
- [frontend/src/features/schedule/ScheduleRulesTable.tsx:12](../../../../../../frontend/src/features/schedule/ScheduleRulesTable.tsx#L12)
- [frontend/src/features/schedule/SessionCard.tsx:5](../../../../../../frontend/src/features/schedule/SessionCard.tsx#L5)
- [frontend/src/features/schedule/TimetableGrid.tsx:3](../../../../../../frontend/src/features/schedule/TimetableGrid.tsx#L3)
- [frontend/src/features/shell/AppShell.tsx:22](../../../../../../frontend/src/features/shell/AppShell.tsx#L22)
- [frontend/src/features/staff/StaffList.tsx:28](../../../../../../frontend/src/features/staff/StaffList.tsx#L28)
- [frontend/src/features/status/SystemInfoCard.tsx:6](../../../../../../frontend/src/features/status/SystemInfoCard.tsx#L6)
- [frontend/src/features/students/ParentsCard.tsx:27](../../../../../../frontend/src/features/students/ParentsCard.tsx#L27)
- [frontend/src/features/teachers/TeachersList.tsx:39](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L39)
- [frontend/src/routes/_authenticated/parents.$parentId.tsx:11](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L11)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { mergeProps } from "@base-ui/react/merge-props"
import { useRender } from "@base-ui/react/use-render"
import type { VariantProps } from "class-variance-authority"
import { cn } from "cn"
import { badgeVariants } from "./badge-variants"

function Badge({
  className,
  variant = "default",
  render,
  ...props
}: useRender.ComponentProps<"span"> & VariantProps<typeof badgeVariants>) {
  return useRender({
    defaultTagName: "span",
    props: mergeProps<"span">(
      {
        className: cn(badgeVariants({ variant }), className),
      },
      props
    ),
    render,
    state: {
      slot: "badge",
      variant,
    },
  })
}

export { Badge }
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
