import { expect, test } from "@playwright/test";
import {
  attendanceWrite,
  createAttendanceSession,
  createAttendanceTeacher,
  signInAttendance,
} from "./attendanceHelpers.js";
test.use({ trace: "off", video: "off", screenshot: "off" });
test("teacher attendance agrees with parent assistant and staff activity", async ({ browser }) => {
  test.setTimeout(540_000);
  const ownerContext = await browser.newContext();
  const teacherContext = await browser.newContext();
  const parentContext = await browser.newContext();
  try {
    const owner = await ownerContext.newPage();
    const teacher = await teacherContext.newPage();
    const parent = await parentContext.newPage();
    await signInAttendance(owner, "owner@nile.test");
    console.log("journey: owner signed in");
    const teacherUser = await createAttendanceTeacher(owner, teacher);
    console.log("journey: teacher created");
    const fixture = await createAttendanceSession(owner, teacherUser, "أحمد");
    console.log("journey: session created");
    await teacher.goto(`/attendance/sessions/${fixture.sessionId}`);
    await teacher.getByRole("button", { name: "Mark all present" }).click();
    await teacher.getByRole("button", { name: "Save", exact: true }).click();
    await expect(teacher.getByText(/^Saved:/)).toBeVisible();
    console.log("journey: attendance marked");
    const student = (await (
      await owner.request.get(`/api/students/${fixture.studentId}`)
    ).json()) as { parents: { parentId: string; fullName: string }[] };
    const email = `assistant-${Date.now().toString()}@example.test`;
    const account = (await (
      await attendanceWrite(owner, "/api/portal-accounts", {
        kind: "parent",
        subjectId: student.parents[0].parentId,
        email,
        preferredLocale: "en",
      })
    ).json()) as { temporaryPassword: string; email?: string };
    console.log("journey: portal account created");
    // Account email is retained only in this isolated context; credentials never enter artifacts.
    await parent.goto("/login");
    await parent.getByLabel("Email").fill(email);
    await parent.getByLabel("Password").fill(account.temporaryPassword);
    await parent.getByRole("button", { name: "Sign in", exact: true }).click();
    await parent.locator("#current-password").fill(account.temporaryPassword);
    const password = `Journey-${Date.now().toString()}-Safe!`;
    await parent.locator("#new-password").fill(password);
    await parent.locator("#confirm-password").fill(password);
    await parent.getByRole("button", { name: "Change password", exact: true }).click();
    await parent.getByRole("button", { name: /Nile Tutoring Centre/ }).click();
    console.log("journey: parent signed in");
    await parent.bringToFront();
    await parent.goto("/portal/chat");
    await expect
      .poll(() =>
        parent.evaluate(
          () =>
            (globalThis as unknown as { document: { visibilityState: string } }).document
              .visibilityState,
        ),
      )
      .toBe("visible");
    await parent.getByLabel("Message", { exact: true }).fill("هو أحمد حضر النهارده؟");
    await parent.getByRole("button", { name: "Send", exact: true }).click();
    await expect(parent.getByText("أحمد حاضر اليوم.", { exact: true })).toBeVisible({
      timeout: 70_000,
    });
    console.log("journey: assistant replied");
    await parent.goto("/portal/attendance");
    await expect(parent.getByRole("img", { name: "Present", exact: true })).toBeVisible();
    await expect(parent.getByText(fixture.groupName)).toBeVisible();
    await parent.screenshot({ path: "../artifacts/m5-attendance-desktop.png", fullPage: true });
    await parent.getByRole("button", { name: "العربية", exact: true }).click();
    await expect(parent.locator("html")).toHaveAttribute("dir", "rtl");
    await parent.setViewportSize({ width: 390, height: 844 });
    await parent.screenshot({ path: "../artifacts/m5-attendance-mobile.png", fullPage: true });
    console.log("journey: attendance page agrees");
    await owner.bringToFront();
    await owner.goto("/conversations");
    await owner.getByRole("button").filter({ hasText: student.parents[0].fullName }).click();
    await owner.getByText("Assistant activity", { exact: true }).click();
    await expect(owner.getByText("get_attendance", { exact: true })).toHaveCount(1);
    await expect(owner.getByText("Allowed", { exact: true })).toBeVisible();
    await expect(owner.getByText("1 record", { exact: true })).toBeVisible();
    await owner.screenshot({ path: "../artifacts/m5-timeline-desktop.png", fullPage: true });
  } finally {
    await Promise.allSettled([ownerContext.close(), teacherContext.close(), parentContext.close()]);
  }
});
