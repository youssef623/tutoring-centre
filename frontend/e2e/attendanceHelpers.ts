import { type Page, expect, test } from "@playwright/test";
// E2E response shapes stay local: the generated browser client uses bundler resolution,
// while Playwright's configuration uses NodeNext resolution.
type AttendanceSessionsDto = {
  date: string;
  timeZoneId: string;
  items: { sessionId: string; groupId: string; startsAt: string }[];
};
type TeacherDto = { id: string; userId: string | null };
type SubjectDto = { id: string };
type MeDto = { userId: string };

export async function signInAttendance(page: Page, email: string) {
  const password = process.env.SEED_PASSWORD;
  if (!password)
    throw new Error("SEED_PASSWORD must be set to run attendance journeys (see README).");
  await page.goto("/login");
  await page.getByLabel("Email").fill(email);
  await page.getByLabel("Password").fill(password);
  await page.getByRole("button", { name: "Sign in" }).click();
  if (email === "teacher@both.test") {
    await expect(page.getByRole("heading", { name: "Choose a centre" })).toBeVisible();
    await page.getByRole("button", { name: /Nile/ }).click();
  }
  await expect(page.getByRole("heading", { name: /^Welcome,/ })).toBeVisible();
}

export async function attendanceWrite(page: Page, url: string, data: unknown, method = "POST") {
  const csrf = await page.request.get("/api/auth/antiforgery");
  const { token } = (await csrf.json()) as { token: string };
  const result = await page.request.fetch(url, {
    method,
    data,
    headers: { "X-XSRF-TOKEN": token },
  });
  expect(result.ok(), `${method} ${url}: ${await result.text()}`).toBeTruthy();
  return result;
}

/** Own group and today's rule through the public Month 3 API; no seeded weekday or roster counts. */
export async function createAttendanceSession(owner: Page, teacherUserId: string | null) {
  const suffix = `${Date.now().toString()}-${Math.random().toString(36).slice(2, 7)}`;
  const sessionsResponse = await owner.request.get("/api/attendance/sessions");
  const calendar = (await sessionsResponse.json()) as AttendanceSessionsDto;
  const zone = calendar.timeZoneId;
  const parts = new Intl.DateTimeFormat("en-GB", {
    timeZone: zone,
    hour: "2-digit",
    hourCycle: "h23",
  }).formatToParts(new Date());
  test.skip(
    Number(parts.find((part) => part.type === "hour")?.value) >= 23,
    "No demo session between 23:00 and midnight in the centre zone.",
  );
  const instant = new Date(Math.ceil((Date.now() + 2 * 60000) / 300000) * 300000);
  const startTime = new Intl.DateTimeFormat("en-GB", {
    timeZone: zone,
    hour: "2-digit",
    minute: "2-digit",
    hourCycle: "h23",
  }).format(instant);
  const subjects = (await (await owner.request.get("/api/subjects")).json()) as {
    items: SubjectDto[];
  };
  const fixtureTeacherId = (
    (await (
      await attendanceWrite(owner, "/api/teachers", {
        fullName: `Attendance teacher ${suffix}`,
        phone: null,
      })
    ).json()) as { id: string }
  ).id;
  let teacherId = fixtureTeacherId;
  if (teacherUserId !== null) {
    const teachers = (await (await owner.request.get("/api/teachers")).json()) as {
      items: TeacherDto[];
    };
    const teacher = teachers.items.find((item) => item.userId === teacherUserId);
    if (!teacher) throw new Error("Seeded teacher has no linked Nile teacher profile.");
    teacherId = teacher.id;
  }
  const name = `Attendance ${suffix}`;
  const subject = subjects.items.at(0);
  if (!subject) throw new Error("No subject is available for the attendance fixture.");
  const group = (await (
    await attendanceWrite(owner, "/api/groups", {
      name,
      subjectId: subject.id,
      teacherId: fixtureTeacherId,
      capacity: 10,
    })
  ).json()) as { id: string };
  const studentName = `Attendance student ${suffix}`;
  const student = (await (
    await attendanceWrite(owner, "/api/students", {
      fullName: studentName,
      phone: null,
      schoolYear: null,
      parent: {
        fullName: `Attendance parent ${suffix}`,
        phone: `+2010${Date.now().toString().slice(-8)}`,
      },
      relationship: "father",
    })
  ).json()) as { id: string };
  await attendanceWrite(owner, `/api/groups/${group.id}/enrollments`, { studentId: student.id });
  const dayOfWeek = new Intl.DateTimeFormat("en-US", { timeZone: zone, weekday: "long" })
    .format(instant)
    .toLowerCase();
  const added = (await (
    await attendanceWrite(owner, `/api/groups/${group.id}/schedule-rules`, {
      dayOfWeek,
      startTime,
      durationMinutes: 30,
      room: null,
      effectiveFrom: calendar.date,
    })
  ).json()) as { id: string };
  const rules = (await (
    await owner.request.get(`/api/groups/${group.id}/schedule-rules`)
  ).json()) as { items: { id: string; version: number | string }[] };
  const rule = rules.items.find((item) => item.id === added.id);
  if (!rule) throw new Error("Created rule missing.");
  await attendanceWrite(owner, `/api/schedule-rules/${rule.id}/end`, {
    lastDate: calendar.date,
    version: rule.version,
  });
  // Generate and end the horizon before assigning the seeded teacher, so later weeks cannot collide.
  if (teacherId !== fixtureTeacherId) await assignAttendanceTeacher(owner, group.id, teacherId);
  const sessions = (await (
    await owner.request.get(`/api/attendance/sessions?date=${calendar.date}`)
  ).json()) as AttendanceSessionsDto;
  const session = sessions.items.find((item) => item.groupId === group.id);
  if (!session) throw new Error("Created session missing.");
  return {
    teacherId,
    fixtureTeacherId,
    sessionId: session.sessionId,
    startsAt: session.startsAt,
    groupId: group.id,
    groupName: name,
    studentId: student.id,
    studentName,
    date: calendar.date,
  };
}

export async function attendanceUserId(page: Page) {
  return ((await (await page.request.get("/api/me")).json()) as MeDto).userId;
}

export async function assignAttendanceTeacher(owner: Page, groupId: string, teacherId: string) {
  const group = (await (await owner.request.get(`/api/groups/${groupId}`)).json()) as {
    version: number | string;
  };
  await attendanceWrite(
    owner,
    `/api/groups/${groupId}/teacher`,
    { teacherId, version: group.version },
    "PUT",
  );
}
