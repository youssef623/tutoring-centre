# `frontend/src/features/session/useCan.ts`

## Purpose

Source comment (intent): Whether the signed-in session's active role holds the given permission. Advice for the UI only: the dispatcher's permission step is the actual decision.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/session/useCan.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 8 lines and 400 bytes. Source hash: `e9fcbafab1a5de1ba29fb016757d3b9da58896aa0500de8c173a901457647c90`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useSession } from "./useSession";`
- Line 2: `import type { Permission } from "./permissions";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../../frontend/src/features/session/useCan.ts#L5): `export function useCan(permission: Permission): boolean {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/GroupRosterTab.tsx:19](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L19)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.tsx:16](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx#L16)
- [frontend/src/features/groups/GroupsList.tsx:24](../../../../../../frontend/src/features/groups/GroupsList.tsx#L24)
- [frontend/src/features/schedule/GroupScheduleTab.tsx:19](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L19)
- [frontend/src/features/schedule/TimetablePage.tsx:23](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L23)
- [frontend/src/features/session/Can.tsx:2](../../../../../../frontend/src/features/session/Can.tsx#L2)
- [frontend/src/features/session/RouteGuard.tsx:6](../../../../../../frontend/src/features/session/RouteGuard.tsx#L6)
- [frontend/src/features/students/StudentsList.tsx:28](../../../../../../frontend/src/features/students/StudentsList.tsx#L28)
- [frontend/src/features/subjects/SubjectsList.tsx:46](../../../../../../frontend/src/features/subjects/SubjectsList.tsx#L46)
- [frontend/src/features/teachers/TeachersList.tsx:52](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L52)
- [frontend/src/routes/_authenticated/groups.$groupId.tsx:53](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L53)
- [frontend/src/routes/_authenticated/parents.$parentId.tsx:26](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L26)
- [frontend/src/routes/_authenticated/students.$studentId.tsx:43](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L43)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { useSession } from "./useSession";
import type { Permission } from "./permissions";

/** Whether the signed-in session's active role holds the given permission. Advice for the UI only: the dispatcher's permission step is the actual decision. */
export function useCan(permission: Permission): boolean {
  const { me } = useSession();
  return me?.permissions.includes(permission) ?? false;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
