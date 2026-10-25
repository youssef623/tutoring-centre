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
  { id: "t-2", fullName: "Mona Said", phone: null, status: "active", userId: null, userDisplayName: null, activeGroupCount: 1, version: 1 },
];
