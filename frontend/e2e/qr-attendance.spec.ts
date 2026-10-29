import { expect, test } from "@playwright/test";
import { createAttendanceSession, createAttendanceTeacher, signInAttendance } from "./attendanceHelpers.js";

// The fixture creates a temporary staff password. Keep it and disposable card credentials out of artifacts.
test.use({ trace: "off", video: "off", screenshot: "off" });

test("QR attendance: manual scan, repeat, replaced card and QR roster source", async ({ browser }) => {
  test.setTimeout(180_000);
  const ownerContext = await browser.newContext();
  const teacherContext = await browser.newContext({ viewport: { width: 390, height: 844 } });
  const secretaryContext = await browser.newContext();
  try {
    const owner = await ownerContext.newPage();
    const teacher = await teacherContext.newPage();
    const secretary = await secretaryContext.newPage();
    for (const page of [owner, teacher, secretary]) { page.setDefaultTimeout(15_000); page.setDefaultNavigationTimeout(30_000); }
    await signInAttendance(owner, "owner@nile.test");
    const teacherId = await createAttendanceTeacher(owner, teacher);
    const fixture = await createAttendanceSession(owner, teacherId);
    await signInAttendance(secretary, "secretary@nile.test");
    await secretary.goto(`/students/${fixture.studentId}`);
    const issuedResponse = secretary.waitForResponse((response) => response.url().endsWith(`/students/${fixture.studentId}/card`) && response.request().method() === "POST");
    await secretary.getByRole("button", { name: "Issue", exact: true }).click();
    const issued = await (await issuedResponse).json() as { token: string };
    await secretary.locator('[data-slot="dialog-footer"]').getByRole("button", { name: "Close", exact: true }).click();

    await teacher.goto(`/attendance/sessions/${fixture.sessionId}`);
    await teacher.getByRole("link", { name: "Scan cards", exact: true }).click();
    await teacher.getByRole("button", { name: "Manual entry", exact: true }).click();
    const input = teacher.getByLabel("Card token", { exact: true });
    let sent = 0;
    teacher.on("request", (request) => { if (request.url().endsWith(`/sessions/${fixture.sessionId}/scans`)) sent++; });
    await input.fill("TCM1.short"); await input.press("Enter");
    await expect(teacher.getByText("Enter a valid student card token.")).toBeVisible();
    expect(sent).toBe(0);
    await input.fill(issued.token); await input.press("Enter");
    await expect(teacher.locator('[data-outcome="green"]')).toContainText(fixture.studentName);
    await expect(teacher.locator('[data-outcome="green"]')).toContainText("Recorded — Present");
    await expect(input).toHaveValue("");
    let after = Date.now() + 3100;
    await expect.poll(() => Date.now() >= after, { timeout: 5000, intervals: [100] }).toBe(true);
    await input.fill(issued.token); await input.press("Enter");
    await expect(teacher.locator('[data-outcome="amber"]')).toContainText("Already recorded");
    after = Date.now() + 3100;

    await secretary.getByRole("button", { name: "Reissue", exact: true }).click();
    const replacementResponse = secretary.waitForResponse((response) => response.url().endsWith(`/students/${fixture.studentId}/card`) && response.request().method() === "POST");
    await secretary.getByRole("button", { name: "Confirm reissue", exact: true }).click();
    const replacement = await (await replacementResponse).json() as { token: string };
    expect(replacement.token !== issued.token).toBe(true);
    await secretary.locator('[data-slot="dialog-footer"]').getByRole("button", { name: "Close", exact: true }).click();
    await expect.poll(() => Date.now() >= after, { timeout: 5000, intervals: [100] }).toBe(true);
    await input.fill(issued.token); await input.press("Enter");
    await expect(teacher.locator('[data-outcome="red"]')).toContainText("This card was replaced or revoked");
    await expect(input).toHaveValue("");
    expect((await teacher.content()).includes(issued.token)).toBe(false);
    await teacher.getByRole("link", { name: "Back to roster", exact: true }).click();
    await expect(teacher.getByText("QR code", { exact: true })).toBeVisible();
    await expect(teacher.getByRole("radiogroup", { name: `Attendance status for ${fixture.studentName}` }).getByRole("radio", { name: "Present", exact: true })).toBeChecked();
  } finally {
    await Promise.all([ownerContext.close(), teacherContext.close(), secretaryContext.close()]);
  }
});
