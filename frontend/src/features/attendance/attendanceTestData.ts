/** Shared fixtures for the attendance component tests: who is signed in, and what the API returns. */

export const SessionId = "s-1";
export const sessionsUrl = "/api/attendance/sessions";
export const rosterUrl = `/api/sessions/${SessionId}/attendance`;
export const correctUrl = (recordId: string) => `/api/attendance/${recordId}/correct`;

function signedIn(role: "teacher" | "secretary", displayName: string, permissions: string[]) {
  return {
    userId: "u-me",
    displayName,
    email: `${role}@nile.test`,
    preferredLocale: "en",
    activeCentreId: "c-nile",
    activeRole: role,
    memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role }],
    permissions,
  };
}

/** What a Teacher holds: view and mark, no correct. */
export const teacherMe = signedIn("teacher", "Amina Hassan", ["attendance.view", "attendance.mark", "groups.view"]);
/** What a Secretary holds: view, mark and correct. */
export const secretaryMe = signedIn("secretary", "Nile Secretary", ["attendance.view", "attendance.mark", "attendance.correct"]);

/** A card from `GET /api/attendance/sessions`. Cairo is UTC+2 in December, so 15:00Z is 5:00 PM there. */
export function sessionItem(overrides: Record<string, unknown> = {}) {
  return {
    sessionId: SessionId,
    groupId: "g-1",
    groupName: "Physics A",
    subjectName: "Physics",
    teacherName: "Amina Hassan",
    startsAt: "2026-12-05T15:00:00Z",
    endsAt: "2026-12-05T16:30:00Z",
    room: "Lab 2",
    status: "scheduled",
    window: "open",
    expectedCount: 18,
    recordedCount: 12,
    ...overrides,
  };
}

export function sessionsDto(items: unknown[], overrides: Record<string, unknown> = {}) {
  return { date: "2026-12-05", timeZoneId: "Africa/Cairo", items, ...overrides };
}

export function record(overrides: Record<string, unknown> = {}) {
  return {
    recordId: "r-1",
    status: "present",
    source: "manual",
    markedAt: "2026-12-05T15:05:00Z",
    markedByName: "Amina Hassan",
    correctionCount: 0,
    version: 3,
    ...overrides,
  };
}

export function student(studentId: string, studentName: string, studentRecord: ReturnType<typeof record> | null = null) {
  return { studentId, studentName, record: studentRecord };
}

/** `GET /api/sessions/{id}/attendance`: Laila is recorded Present, Omar and Mona are not recorded yet. */
export function rosterDto(overrides: Record<string, unknown> = {}) {
  return {
    session: {
      sessionId: SessionId,
      groupName: "Physics A",
      subjectName: "Physics",
      teacherName: "Amina Hassan",
      startsAt: "2026-12-05T15:00:00Z",
      endsAt: "2026-12-05T16:30:00Z",
      localDate: "2026-12-05",
      status: "scheduled",
    },
    timeZoneId: "Africa/Cairo",
    window: "open",
    items: [
      student("st-laila", "Laila Samir", record()),
      student("st-omar", "Omar Nabil"),
      student("st-mona", "Mona Adel"),
    ],
    ...overrides,
  };
}
