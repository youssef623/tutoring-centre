# `frontend/src/features/schedule/scheduleTestData.ts`

## Purpose

Source comment (intent): Tuesday 20 April 2027, 10:00 in Cairo: the week of Saturday 24 April is "next", 17 April is "this week".

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/scheduleTestData.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 92 lines and 2,876 bytes. Source hash: `fc13aa95cafac55199b70365820b0577168504b8c93f3a18732a10eb5d62af86`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../../frontend/src/features/schedule/scheduleTestData.ts#L5): `export const Now = new Date("2027-04-20T08:00:00Z");`
- [Line 7](../../../../../../frontend/src/features/schedule/scheduleTestData.ts#L7): `export function me(role: "owner" | "secretary" | "teacher", permissions: string[]) {`
- [Line 21](../../../../../../frontend/src/features/schedule/scheduleTestData.ts#L21): `export const secretaryMe = me("secretary", [`
- [Line 34](../../../../../../frontend/src/features/schedule/scheduleTestData.ts#L34): `export const teacherMe = me("teacher", ["enrollments.view", "groups.view", "schedule.view", "subjects.view"]);`
- [Line 36](../../../../../../frontend/src/features/schedule/scheduleTestData.ts#L36): `export function group(overrides: Record<string, unknown> = {}) {`
- [Line 52](../../../../../../frontend/src/features/schedule/scheduleTestData.ts#L52): `export function rule(overrides: Record<string, unknown> = {}) {`
- [Line 66](../../../../../../frontend/src/features/schedule/scheduleTestData.ts#L66): `export function session(overrides: Record<string, unknown> = {}) {`
- [Line 85](../../../../../../frontend/src/features/schedule/scheduleTestData.ts#L85): `export function timetable(weekStart: string, sessions: ReturnType<typeof session>[] = [], timeZoneId = "Africa/Cairo") {`
- [Line 89](../../../../../../frontend/src/features/schedule/scheduleTestData.ts#L89): `export const teachers = [`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:16](../../../../../onboarding-work/extend_backend_purposes.py#L16)
- [frontend/e2e/full-centre-setup.spec.ts:67](../../../../../../frontend/e2e/full-centre-setup.spec.ts#L67)
- [frontend/e2e/schedule.spec.ts:42](../../../../../../frontend/e2e/schedule.spec.ts#L42)
- [frontend/e2e/schedule.spec.ts:119](../../../../../../frontend/e2e/schedule.spec.ts#L119)
- [frontend/src/api/generated/tutoring-centre.ts:1911](../../../../../../frontend/src/api/generated/tutoring-centre.ts#L1911)
- [frontend/src/api/generated/tutoring-centre.ts:3998](../../../../../../frontend/src/api/generated/tutoring-centre.ts#L3998)
- [frontend/src/features/groups/CreateGroupDialog.tsx:31](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L31)
- [frontend/src/features/groups/GroupEditDialogs.tsx:41](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L41)
- [frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx:5](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx#L5)
- [frontend/src/features/schedule/ExtendScheduleDialog.test.tsx:5](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.test.tsx#L5)
- [frontend/src/features/schedule/GroupScheduleTab.test.tsx:5](../../../../../../frontend/src/features/schedule/GroupScheduleTab.test.tsx#L5)
- [frontend/src/features/schedule/SessionCard.tsx:93](../../../../../../frontend/src/features/schedule/SessionCard.tsx#L93)
- [frontend/src/features/schedule/TimetableGrid.tsx:38](../../../../../../frontend/src/features/schedule/TimetableGrid.tsx#L38)
- [frontend/src/features/schedule/TimetablePage.test.tsx:5](../../../../../../frontend/src/features/schedule/TimetablePage.test.tsx#L5)
- [frontend/src/features/schedule/TimetablePage.tsx:25](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L25)
- [frontend/src/features/schedule/TimetablePage.tsx:62](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L62)
- [frontend/src/features/schedule/WeekNavigator.tsx:23](../../../../../../frontend/src/features/schedule/WeekNavigator.tsx#L23)
- [frontend/src/features/session/permissions.ts:11](../../../../../../frontend/src/features/session/permissions.ts#L11)
- [frontend/src/features/shell/AppShell.tsx:141](../../../../../../frontend/src/features/shell/AppShell.tsx#L141)
- [frontend/src/features/shell/AppShell.tsx:153](../../../../../../frontend/src/features/shell/AppShell.tsx#L153)
- 73 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
// Synthetic data shared by the schedule component tests (no real people; emails on the .test domain). Every date
// is fixed: tests set the clock with vi.setSystemTime(Now), so nothing depends on the day they run.

/** Tuesday 20 April 2027, 10:00 in Cairo: the week of Saturday 24 April is "next", 17 April is "this week". */
export const Now = new Date("2027-04-20T08:00:00Z");

export function me(role: "owner" | "secretary" | "teacher", permissions: string[]) {
  return {
    userId: `u-${role}`,
    displayName: `Nile ${role}`,
    email: `${role}@nile.test`,
    preferredLocale: "en",
    mustChangePassword: false,
    activeCentreId: "c-nile",
    activeRole: role,
    memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role, timeZoneId: "Africa/Cairo" }],
    permissions,
  };
}

export const secretaryMe = me("secretary", [
  "enrollments.manage",
  "enrollments.view",
  "groups.manage",
  "groups.view",
  "schedule.manage",
  "schedule.view",
  "students.view",
  "subjects.view",
  "teachers.view",
]);

/** What a group's own teacher holds: schedules are visible, nothing to manage, no teachers list. */
export const teacherMe = me("teacher", ["enrollments.view", "groups.view", "schedule.view", "subjects.view"]);

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

export function rule(overrides: Record<string, unknown> = {}) {
  return {
    id: "r-1",
    dayOfWeek: "saturday",
    startTime: "18:00",
    durationMinutes: 90,
    room: "Room 1",
    effectiveFrom: "2027-04-01",
    effectiveTo: null,
    version: 5,
    ...overrides,
  };
}

export function session(overrides: Record<string, unknown> = {}) {
  return {
    id: "s-1",
    groupId: "g-1",
    groupName: "Physics A",
    subjectName: "Physics",
    teacherId: "t-1",
    teacherName: "Amina Hassan",
    localDate: "2027-04-24",
    // Saturday 24 April, 18:00-19:30 in Cairo (UTC+2 before the 30 April change).
    startsAt: "2027-04-24T16:00:00Z",
    endsAt: "2027-04-24T17:30:00Z",
    room: "Room 1",
    status: "scheduled",
    version: 9,
    ...overrides,
  };
}

export function timetable(weekStart: string, sessions: ReturnType<typeof session>[] = [], timeZoneId = "Africa/Cairo") {
  return { timeZoneId, weekStart, sessions };
}

export const teachers = [
  { id: "t-1", fullName: "Amina Hassan", phone: null, status: "active", userId: null, userDisplayName: null, activeGroupCount: 1, version: 1 },
```

The excerpt stops at line 90; use the original file for the remaining 2 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
