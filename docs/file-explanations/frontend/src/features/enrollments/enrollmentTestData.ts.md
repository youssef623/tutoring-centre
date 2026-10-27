# `frontend/src/features/enrollments/enrollmentTestData.ts`

## Purpose

Source comment (intent): What a group's own teacher holds: the roster is visible, nothing else about enrollments or students.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/enrollmentTestData.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 71 lines and 2,101 bytes. Source hash: `697f284e930ef2929492b58f3d5bcb98879a036a5406a008492d127f4901bb7b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../../frontend/src/features/enrollments/enrollmentTestData.ts#L3): `export function me(role: "owner" | "secretary" | "teacher", permissions: string[]) {`
- [Line 17](../../../../../../frontend/src/features/enrollments/enrollmentTestData.ts#L17): `export const secretaryMe = me("secretary", [`
- [Line 28](../../../../../../frontend/src/features/enrollments/enrollmentTestData.ts#L28): `export const teacherMe = me("teacher", ["enrollments.view", "groups.view", "subjects.view"]);`
- [Line 30](../../../../../../frontend/src/features/enrollments/enrollmentTestData.ts#L30): `export function group(overrides: Record<string, unknown> = {}) {`
- [Line 46](../../../../../../frontend/src/features/enrollments/enrollmentTestData.ts#L46): `export function rosterEntry(overrides: Record<string, unknown> = {}) {`
- [Line 57](../../../../../../frontend/src/features/enrollments/enrollmentTestData.ts#L57): `export function roster(items = [rosterEntry(), rosterEntry({ enrollmentId: "e-2", studentId: "st-2", studentName: "Sara Ali", version: 4 })], overrides: Record<string, unknown> = {}) {`
- [Line 61](../../../../../../frontend/src/features/enrollments/enrollmentTestData.ts#L61): `export function studentListItem(overrides: Record<string, unknown> = {}) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:9](../../../../../onboarding-work/extend_backend_purposes.py#L9)
- [frontend/e2e/enrollment.spec.ts:25](../../../../../../frontend/e2e/enrollment.spec.ts#L25)
- [frontend/e2e/full-centre-setup.spec.ts:181](../../../../../../frontend/e2e/full-centre-setup.spec.ts#L181)
- [frontend/src/api/generated/tutoring-centre.ts:3292](../../../../../../frontend/src/api/generated/tutoring-centre.ts#L3292)
- [frontend/src/features/dashboard/DashboardOverview.test.tsx:38](../../../../../../frontend/src/features/dashboard/DashboardOverview.test.tsx#L38)
- [frontend/src/features/dashboard/DashboardOverview.tsx:142](../../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L142)
- [frontend/src/features/enrollments/EnrollStudentDialog.test.tsx:5](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.test.tsx#L5)
- [frontend/src/features/enrollments/EnrollmentRowMenu.tsx:11](../../../../../../frontend/src/features/enrollments/EnrollmentRowMenu.tsx#L11)
- [frontend/src/features/enrollments/GroupRosterTab.test.tsx:5](../../../../../../frontend/src/features/enrollments/GroupRosterTab.test.tsx#L5)
- [frontend/src/features/enrollments/GroupRosterTab.tsx:40](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L40)
- [frontend/src/features/enrollments/RosterTable.tsx:34](../../../../../../frontend/src/features/enrollments/RosterTable.tsx#L34)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx:91](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx#L91)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.tsx:58](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx#L58)
- [frontend/src/features/enrollments/TransferDialog.test.tsx:4](../../../../../../frontend/src/features/enrollments/TransferDialog.test.tsx#L4)
- [frontend/src/routes/_authenticated/groups.$groupId.test.tsx:73](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.test.tsx#L73)
- [frontend/src/routes/_authenticated/groups.$groupId.tsx:55](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L55)
- [src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs:39](../../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L39)
- [src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs:20](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs#L20)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs:160](../../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L160)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs:48](../../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L48)
- 7 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
// Synthetic data shared by the enrollments component tests (no real people; emails on the .test domain).

export function me(role: "owner" | "secretary" | "teacher", permissions: string[]) {
  return {
    userId: `u-${role}`,
    displayName: `Nile ${role}`,
    email: `${role}@nile.test`,
    preferredLocale: "en",
    mustChangePassword: false,
    activeCentreId: "c-nile",
    activeRole: role,
    memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role }],
    permissions,
  };
}

export const secretaryMe = me("secretary", [
  "enrollments.manage",
  "enrollments.view",
  "groups.manage",
  "groups.view",
  "students.manage",
  "students.view",
  "subjects.view",
]);

/** What a group's own teacher holds: the roster is visible, nothing else about enrollments or students. */
export const teacherMe = me("teacher", ["enrollments.view", "groups.view", "subjects.view"]);

export function group(overrides: Record<string, unknown> = {}) {
  return {
    id: "g-1",
    name: "Physics A",
    subjectId: "s-phys",
    subjectName: "Physics",
    teacherId: "t-1",
    teacherName: "Amina Hassan",
    capacity: 20,
    enrolledCount: 2,
    status: "active",
    version: 3,
    ...overrides,
  };
}

export function rosterEntry(overrides: Record<string, unknown> = {}) {
  return {
    enrollmentId: "e-1",
    studentId: "st-1",
    studentName: "Ahmed Mahmoud",
    enrolledAt: "2026-10-01T09:00:00+00:00",
    version: 7,
    ...overrides,
  };
}

export function roster(items = [rosterEntry(), rosterEntry({ enrollmentId: "e-2", studentId: "st-2", studentName: "Sara Ali", version: 4 })], overrides: Record<string, unknown> = {}) {
  return { groupId: "g-1", groupName: "Physics A", capacity: 20, enrolledCount: items.length, items, ...overrides };
}

export function studentListItem(overrides: Record<string, unknown> = {}) {
  return {
    id: "st-3",
    fullName: "Omar Khaled",
    schoolYear: "Grade 10",
    status: "active",
    primaryParentName: "Khaled Omar",
    primaryParentPhone: "+201000000099",
    ...overrides,
  };
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
