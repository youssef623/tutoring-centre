# `frontend/src/features/session/permissions.ts`

## Purpose

Source comment (intent): The closed set of permission keys (mirrors the backend's Domain-level `Permissions` catalogue, Day 28).

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/session/permissions.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 21 lines and 785 bytes. Source hash: `c8f7a6e0675f484ca62f38eceebbc845ab7a66b4263eac755e1cc0c58afae43b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 2](../../../../../../frontend/src/features/session/permissions.ts#L2): `export const Permissions = {`
- [Line 21](../../../../../../frontend/src/features/session/permissions.ts#L21): `export type Permission = (typeof Permissions)[keyof typeof Permissions];`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/session/Can.tsx:3](../../../../../../frontend/src/features/session/Can.tsx#L3)
- [frontend/src/features/session/RouteGuard.tsx:7](../../../../../../frontend/src/features/session/RouteGuard.tsx#L7)
- [frontend/src/features/session/useCan.ts:2](../../../../../../frontend/src/features/session/useCan.ts#L2)
- [src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs:212](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L212)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
/** The closed set of permission keys (mirrors the backend's Domain-level `Permissions` catalogue, Day 28). */
export const Permissions = {
  SubjectsView: "subjects.view",
  SubjectsManage: "subjects.manage",
  StaffView: "staff.view",
  StaffManage: "staff.manage",
  AuditView: "audit.view",
  CentreSettingsManage: "centre.settings.manage",
  StudentsView: "students.view",
  StudentsManage: "students.manage",
  TeachersView: "teachers.view",
  TeachersManage: "teachers.manage",
  GroupsView: "groups.view",
  GroupsManage: "groups.manage",
  EnrollmentsView: "enrollments.view",
  EnrollmentsManage: "enrollments.manage",
  ScheduleView: "schedule.view",
  ScheduleManage: "schedule.manage",
} as const;

export type Permission = (typeof Permissions)[keyof typeof Permissions];
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
