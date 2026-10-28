import { expect, test } from "@playwright/test";
import {
  assignAttendanceTeacher,
  attendanceUserId,
  createAttendanceSession,
  signInAttendance,
} from "./attendanceHelpers.js";

test("attendance: teacher marks, secretary corrects, owner sees the audited reason", async ({
  browser,
}) => {
  test.setTimeout(540_000);
  const ownerContext = await browser.newContext();
  const teacherContext = await browser.newContext({ viewport: { width: 390, height: 844 } });
  const secretaryContext = await browser.newContext();
  try {
    const owner = await ownerContext.newPage();
    owner.setDefaultNavigationTimeout(30_000);
    owner.setDefaultTimeout(15_000);
    const teacher = await teacherContext.newPage();
    teacher.setDefaultNavigationTimeout(30_000);
    teacher.setDefaultTimeout(15_000);
    const secretary = await secretaryContext.newPage();
    secretary.setDefaultNavigationTimeout(30_000);
    secretary.setDefaultTimeout(15_000);
    await signInAttendance(owner, "owner@nile.test");
    await signInAttendance(teacher, "teacher@both.test");
    const fixture = await createAttendanceSession(owner, await attendanceUserId(teacher));
    await teacher.goto(`/attendance?date=${fixture.date}`);
    const card = teacher.getByRole("link", { name: new RegExp(fixture.groupName) });
    await expect(card.getByText("Open", { exact: true })).toBeVisible();
    await card.click();
    await teacher.getByRole("button", { name: "Mark all present" }).click();
    await teacher
      .getByRole("radiogroup", { name: `Attendance status for ${fixture.studentName}` })
      .getByRole("radio", { name: "Absent", exact: true })
      .click();
    await teacher.getByRole("button", { name: "Save", exact: true }).click();
    await expect(teacher.getByText(/^Saved:/)).toBeVisible();
    await expect(teacher.getByText("No unsaved changes", { exact: true })).toBeVisible();
    await teacher.getByRole("link", { name: "Back to sessions" }).click();
    await expect(teacher.getByRole("heading", { name: "Attendance", exact: true })).toBeVisible();
    console.log("Attendance fixture marked; waiting for session start.");
    // Free the seeded teacher before the session starts. The later restoration preserves this session's substitute.
    await assignAttendanceTeacher(owner, fixture.groupId, fixture.fixtureTeacherId);
    // A real clock boundary: corrections must never be enabled early just to make the journey faster.
    await expect
      .poll(() => Date.now() >= Date.parse(fixture.startsAt), {
        timeout: 450_000,
        intervals: [1000],
      })
      .toBe(true);
    await assignAttendanceTeacher(owner, fixture.groupId, fixture.teacherId);
    await teacher.goto(`/groups/${fixture.groupId}?tab=attendance`, {
      waitUntil: "domcontentloaded",
    });
    await expect(teacher.getByText("No attendance recorded in this period")).toBeVisible();
    console.log("Substitute history scope verified.");
    await signInAttendance(secretary, "secretary@nile.test");
    await secretary.goto(`/attendance/sessions/${fixture.sessionId}`, {
      waitUntil: "domcontentloaded",
    });
    await secretary
      .getByRole("button", { name: `Correct ${fixture.studentName}`, exact: true })
      .click();
    const dialog = secretary.getByRole("dialog", { name: "Correct attendance" });
    await dialog.getByRole("radio", { name: "Present", exact: true }).click();
    const reason = `Arrival confirmed for ${fixture.groupName}`;
    await dialog.getByLabel("Reason for the correction").fill(reason);
    await dialog.getByRole("button", { name: "Save correction" }).click();
    await expect(secretary.getByText("Correction saved", { exact: true })).toBeVisible();
    await expect(secretary.getByText("Corrected once", { exact: true })).toBeVisible();
    console.log("Secretary correction verified.");
    await secretary.goto(`/groups/${fixture.groupId}?tab=attendance`, {
      waitUntil: "domcontentloaded",
    });
    await expect(secretary.getByRole("table").getByLabel("Present", { exact: true })).toBeVisible();
    await expect(
      secretary.getByRole("table").getByRole("img", { name: "Corrected once" }),
    ).toBeVisible();
    await secretary.goto(`/students/${fixture.studentId}`, { waitUntil: "domcontentloaded" });
    await expect(
      secretary.getByText("In this period: 1 present, 0 late, 0 absent, 0 excused"),
    ).toBeVisible();
    console.log("Student attendance card verified.");
    await owner.goto("/audit", { waitUntil: "domcontentloaded" });
    await owner.getByLabel("Entity type").click();
    await owner.getByRole("option", { name: "Attendance record", exact: true }).click();
    await expect(owner.getByText(reason, { exact: true })).toBeVisible();
    console.log("Owner audit reason verified.");
  } finally {
    await secretaryContext.close();
    await teacherContext.close();
    await ownerContext.close();
  }
});

test("attendance: teacher pasting another teacher's session URL sees Session not found", async ({
  browser,
}) => {
  const ownerContext = await browser.newContext();
  const teacherContext = await browser.newContext();
  try {
    const owner = await ownerContext.newPage();
    owner.setDefaultNavigationTimeout(30_000);
    owner.setDefaultTimeout(15_000);
    const teacher = await teacherContext.newPage();
    teacher.setDefaultNavigationTimeout(30_000);
    teacher.setDefaultTimeout(15_000);
    await signInAttendance(owner, "owner@nile.test");
    const fixture = await createAttendanceSession(owner, null);
    await signInAttendance(teacher, "teacher@both.test");
    await teacher.goto(`/attendance/sessions/${fixture.sessionId}`);
    await expect(teacher.getByText("Session not found", { exact: true })).toBeVisible();
  } finally {
    await teacherContext.close();
    await ownerContext.close();
  }
});
