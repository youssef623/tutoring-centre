# `frontend/src/components/ui/alert.tsx`

## Purpose

Declares `Alert`, `AlertTitle`, `AlertDescription`, `AlertAction`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/ui/alert.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 75 lines and 2,037 bytes. Source hash: `a3521f446f0e66965a8de9ff29998d8036847286e1bb838f1a317c0709af85f8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import * as React from "react"`
- Line 2: `import { cva, type VariantProps } from "class-variance-authority"`
- Line 3: `import { cn } from "cn"`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../../frontend/src/components/ui/alert.tsx#L21): `function Alert({`
- [Line 36](../../../../../../frontend/src/components/ui/alert.tsx#L36): `function AlertTitle({ className, ...props }: React.ComponentProps<"div">) {`
- [Line 49](../../../../../../frontend/src/components/ui/alert.tsx#L49): `function AlertDescription({`
- [Line 65](../../../../../../frontend/src/components/ui/alert.tsx#L65): `function AlertAction({ className, ...props }: React.ComponentProps<"div">) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/audit/AuditLog.tsx:8](../../../../../../frontend/src/features/audit/AuditLog.tsx#L8)
- [frontend/src/features/dashboard/DashboardOverview.tsx:8](../../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L8)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:8](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L8)
- [frontend/src/features/enrollments/EnrollStudentDialog.tsx:6](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L6)
- [frontend/src/features/enrollments/GroupRosterTab.tsx:7](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L7)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.tsx:8](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx#L8)
- [frontend/src/features/enrollments/TransferDialog.tsx:8](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L8)
- [frontend/src/features/groups/GroupsList.tsx:18](../../../../../../frontend/src/features/groups/GroupsList.tsx#L18)
- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:9](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L9)
- [frontend/src/features/schedule/ExtendScheduleDialog.tsx:5](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.tsx#L5)
- [frontend/src/features/schedule/GroupScheduleTab.tsx:8](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L8)
- [frontend/src/features/schedule/TimetablePage.tsx:10](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L10)
- [frontend/src/features/staff/StaffList.tsx:27](../../../../../../frontend/src/features/staff/StaffList.tsx#L27)
- [frontend/src/features/status/StatusCard.tsx:2](../../../../../../frontend/src/features/status/StatusCard.tsx#L2)
- [frontend/src/features/status/SystemInfoCard.tsx:5](../../../../../../frontend/src/features/status/SystemInfoCard.tsx#L5)
- [frontend/src/features/students/AddParentDialog.tsx:22](../../../../../../frontend/src/features/students/AddParentDialog.tsx#L22)
- [frontend/src/features/students/StudentsList.tsx:12](../../../../../../frontend/src/features/students/StudentsList.tsx#L12)
- [frontend/src/features/subjects/SubjectsList.tsx:26](../../../../../../frontend/src/features/subjects/SubjectsList.tsx#L26)
- [frontend/src/features/teachers/TeacherAccountDialog.tsx:18](../../../../../../frontend/src/features/teachers/TeacherAccountDialog.tsx#L18)
- [frontend/src/features/teachers/TeachersList.tsx:38](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L38)
- 5 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import * as React from "react"
import { cva, type VariantProps } from "class-variance-authority"
import { cn } from "cn"

const alertVariants = cva(
  "group/alert relative grid w-full gap-0.5 rounded-lg border px-2.5 py-2 text-start text-sm has-data-[slot=alert-action]:relative has-data-[slot=alert-action]:pe-18 has-[>svg]:grid-cols-[auto_1fr] has-[>svg]:gap-x-2 *:[svg]:row-span-2 *:[svg]:translate-y-0.5 *:[svg]:text-current *:[svg:not([class*='size-'])]:size-4",
  {
    variants: {
      variant: {
        default: "bg-card text-card-foreground",
        destructive:
          "bg-card text-destructive *:data-[slot=alert-description]:text-destructive/90 *:[svg]:text-current",
      },
    },
    defaultVariants: {
      variant: "default",
    },
  }
)

function Alert({
  className,
  variant,
  ...props
}: React.ComponentProps<"div"> & VariantProps<typeof alertVariants>) {
  return (
    <div
      data-slot="alert"
      role="alert"
      className={cn(alertVariants({ variant }), className)}
      {...props}
    />
  )
}

function AlertTitle({ className, ...props }: React.ComponentProps<"div">) {
  return (
    <div
      data-slot="alert-title"
      className={cn(
        "font-medium group-has-[>svg]/alert:col-start-2 [&_a]:underline [&_a]:underline-offset-3 [&_a]:hover:text-foreground",
        className
      )}
      {...props}
    />
  )
}

function AlertDescription({
  className,
  ...props
}: React.ComponentProps<"div">) {
  return (
    <div
      data-slot="alert-description"
      className={cn(
        "text-sm text-balance text-muted-foreground md:text-pretty [&_a]:underline [&_a]:underline-offset-3 [&_a]:hover:text-foreground [&_p:not(:last-child)]:mb-4",
        className
      )}
      {...props}
    />
  )
}

function AlertAction({ className, ...props }: React.ComponentProps<"div">) {
  return (
    <div
      data-slot="alert-action"
      className={cn("absolute top-2 end-2", className)}
      {...props}
    />
  )
}

export { Alert, AlertTitle, AlertDescription, AlertAction }
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
