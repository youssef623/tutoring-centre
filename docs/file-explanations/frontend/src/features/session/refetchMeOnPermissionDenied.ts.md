# `frontend/src/features/session/refetchMeOnPermissionDenied.ts`

## Purpose

Source comment (intent): After a 403 auth.permission_denied, refetches /api/me so the UI's permission-based hiding catches up with a role that changed server-side mid-session.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/session/refetchMeOnPermissionDenied.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 555 bytes. Source hash: `9f159eaaeeaf835e0adac2585f411b8dfdfae499f7677dd7f167b88f30950d06`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import type { QueryClient } from "@tanstack/react-query";`
- Line 2: `import { getGetMeQueryKey } from "@/api/generated/tutoring-centre";`
- Line 3: `import type { ApiError } from "@/api/errors";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../frontend/src/features/session/refetchMeOnPermissionDenied.ts#L6): `export function refetchMeOnPermissionDenied(error: ApiError, queryClient: QueryClient): void {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/useEnrollmentMutations.ts:13](../../../../../../frontend/src/features/enrollments/useEnrollmentMutations.ts#L13)
- [frontend/src/features/groups/CreateGroupDialog.tsx:29](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L29)
- [frontend/src/features/groups/GroupEditDialogs.tsx:40](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L40)
- [frontend/src/features/parents/ParentEditDialog.tsx:10](../../../../../../frontend/src/features/parents/ParentEditDialog.tsx#L10)
- [frontend/src/features/schedule/useScheduleMutations.ts:13](../../../../../../frontend/src/features/schedule/useScheduleMutations.ts#L13)
- [frontend/src/features/staff/ChangeRoleDialog.tsx:14](../../../../../../frontend/src/features/staff/ChangeRoleDialog.tsx#L14)
- [frontend/src/features/staff/StaffFormDialog.tsx:12](../../../../../../frontend/src/features/staff/StaffFormDialog.tsx#L12)
- [frontend/src/features/staff/StaffList.tsx:9](../../../../../../frontend/src/features/staff/StaffList.tsx#L9)
- [frontend/src/features/students/AddParentDialog.tsx:20](../../../../../../frontend/src/features/students/AddParentDialog.tsx#L20)
- [frontend/src/features/students/ParentsCard.tsx:37](../../../../../../frontend/src/features/students/ParentsCard.tsx#L37)
- [frontend/src/features/students/RegisterStudentForm.tsx:18](../../../../../../frontend/src/features/students/RegisterStudentForm.tsx#L18)
- [frontend/src/features/students/StudentEditDialog.tsx:15](../../../../../../frontend/src/features/students/StudentEditDialog.tsx#L15)
- [frontend/src/features/subjects/SubjectFormDialog.tsx:17](../../../../../../frontend/src/features/subjects/SubjectFormDialog.tsx#L17)
- [frontend/src/features/subjects/SubjectsList.tsx:45](../../../../../../frontend/src/features/subjects/SubjectsList.tsx#L45)
- [frontend/src/features/teachers/TeacherAccountDialog.tsx:32](../../../../../../frontend/src/features/teachers/TeacherAccountDialog.tsx#L32)
- [frontend/src/features/teachers/TeacherFormDialog.tsx:28](../../../../../../frontend/src/features/teachers/TeacherFormDialog.tsx#L28)
- [frontend/src/features/teachers/TeachersList.tsx:51](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L51)
- [frontend/src/routes/_authenticated/groups.$groupId.tsx:51](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L51)
- [frontend/src/routes/_authenticated/settings.tsx:25](../../../../../../frontend/src/routes/_authenticated/settings.tsx#L25)
- [frontend/src/routes/_authenticated/students.$studentId.tsx:41](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L41)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import type { QueryClient } from "@tanstack/react-query";
import { getGetMeQueryKey } from "@/api/generated/tutoring-centre";
import type { ApiError } from "@/api/errors";

/** After a 403 auth.permission_denied, refetches /api/me so the UI's permission-based hiding catches up with a role that changed server-side mid-session. */
export function refetchMeOnPermissionDenied(error: ApiError, queryClient: QueryClient): void {
  if (error.code === "auth.permission_denied") {
    void queryClient.invalidateQueries({ queryKey: getGetMeQueryKey() });
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
