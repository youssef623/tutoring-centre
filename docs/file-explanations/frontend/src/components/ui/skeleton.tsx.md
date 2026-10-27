# `frontend/src/components/ui/skeleton.tsx`

## Purpose

Declares `Skeleton`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/ui/skeleton.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 13 lines and 266 bytes. Source hash: `4c2af7fa9c645358a0fb188779deab6f0b981733b62befc25e18aaebea1350dc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { cn } from "cn"`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../../frontend/src/components/ui/skeleton.tsx#L3): `function Skeleton({ className, ...props }: React.ComponentProps<"div">) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/audit/AuditLog.tsx:11](../../../../../../frontend/src/features/audit/AuditLog.tsx#L11)
- [frontend/src/features/dashboard/DashboardOverview.tsx:11](../../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L11)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:19](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L19)
- [frontend/src/features/enrollments/GroupRosterTab.tsx:9](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L9)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.tsx:10](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx#L10)
- [frontend/src/features/enrollments/StudentPicker.tsx:10](../../../../../../frontend/src/features/enrollments/StudentPicker.tsx#L10)
- [frontend/src/features/enrollments/TransferDialog.tsx:18](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L18)
- [frontend/src/features/groups/GroupsList.tsx:20](../../../../../../frontend/src/features/groups/GroupsList.tsx#L20)
- [frontend/src/features/schedule/GroupScheduleTab.tsx:10](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L10)
- [frontend/src/features/schedule/TimetablePage.tsx:14](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L14)
- [frontend/src/features/staff/StaffList.tsx:36](../../../../../../frontend/src/features/staff/StaffList.tsx#L36)
- [frontend/src/features/status/StatusCard.tsx:5](../../../../../../frontend/src/features/status/StatusCard.tsx#L5)
- [frontend/src/features/status/SystemInfoCard.tsx:9](../../../../../../frontend/src/features/status/SystemInfoCard.tsx#L9)
- [frontend/src/features/students/StudentsList.tsx:15](../../../../../../frontend/src/features/students/StudentsList.tsx#L15)
- [frontend/src/features/subjects/SubjectsList.tsx:43](../../../../../../frontend/src/features/subjects/SubjectsList.tsx#L43)
- [frontend/src/features/teachers/TeacherAccountDialog.tsx:31](../../../../../../frontend/src/features/teachers/TeacherAccountDialog.tsx#L31)
- [frontend/src/features/teachers/TeachersList.tsx:48](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L48)
- [frontend/src/routes/_authenticated/groups.$groupId.tsx:42](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L42)
- [frontend/src/routes/_authenticated/parents.$parentId.tsx:22](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L22)
- [frontend/src/routes/_authenticated/settings.tsx:23](../../../../../../frontend/src/routes/_authenticated/settings.tsx#L23)
- 1 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { cn } from "cn"

function Skeleton({ className, ...props }: React.ComponentProps<"div">) {
  return (
    <div
      data-slot="skeleton"
      className={cn("animate-pulse rounded-md bg-muted", className)}
      {...props}
    />
  )
}

export { Skeleton }
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
