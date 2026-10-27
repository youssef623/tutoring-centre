# `frontend/src/features/session/Can.tsx`

## Purpose

Source comment (intent): Renders its children only when the signed-in session holds the given permission. Hides for usability; never the security boundary.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/session/Can.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 8 lines and 409 bytes. Source hash: `6971034cb3dee6641af9be72a6209b343163fe7ed17fd8cf683d37ad877da695`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import type { ReactNode } from "react";`
- Line 2: `import { useCan } from "./useCan";`
- Line 3: `import type { Permission } from "./permissions";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../frontend/src/features/session/Can.tsx#L6): `export function Can({ permission, children }: { permission: Permission; children: ReactNode }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/dashboard/DashboardOverview.tsx:13](../../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L13)
- [frontend/src/features/enrollments/GroupRosterTab.tsx:17](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L17)
- [frontend/src/features/groups/GroupsList.tsx:22](../../../../../../frontend/src/features/groups/GroupsList.tsx#L22)
- [frontend/src/features/schedule/GroupScheduleTab.tsx:17](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L17)
- [frontend/src/features/schedule/TimetablePage.tsx:21](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L21)
- [frontend/src/features/shell/AppShell.tsx:25](../../../../../../frontend/src/features/shell/AppShell.tsx#L25)
- [frontend/src/features/teachers/TeachersList.tsx:49](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L49)
- [frontend/src/routes/_authenticated/groups.$groupId.tsx:49](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L49)
- [frontend/src/routes/_authenticated/groups.index.tsx:10](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx#L10)
- [frontend/src/routes/_authenticated/index.tsx:5](../../../../../../frontend/src/routes/_authenticated/index.tsx#L5)
- [frontend/src/routes/_authenticated/staff.tsx:10](../../../../../../frontend/src/routes/_authenticated/staff.tsx#L10)
- [frontend/src/routes/_authenticated/students.$studentId.tsx:39](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L39)
- [frontend/src/routes/_authenticated/students.index.tsx:9](../../../../../../frontend/src/routes/_authenticated/students.index.tsx#L9)
- [frontend/src/routes/_authenticated/subjects.tsx:10](../../../../../../frontend/src/routes/_authenticated/subjects.tsx#L10)
- [frontend/src/routes/_authenticated/teachers.tsx:8](../../../../../../frontend/src/routes/_authenticated/teachers.tsx#L8)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import type { ReactNode } from "react";
import { useCan } from "./useCan";
import type { Permission } from "./permissions";

/** Renders its children only when the signed-in session holds the given permission. Hides for usability; never the security boundary. */
export function Can({ permission, children }: { permission: Permission; children: ReactNode }) {
  return useCan(permission) ? children : null;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
