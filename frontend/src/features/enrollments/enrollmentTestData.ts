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
