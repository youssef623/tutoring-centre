import { expect, test } from "@playwright/test";
import {
  attendanceWrite,
  createAttendanceSession,
  createAttendanceTeacher,
  signInAttendance,
} from "./attendanceHelpers.js";
import {
  businessSnapshot,
  createFeeGroupThroughUi,
  driveRelay,
  enrollThroughUi,
} from "./helpers/billing.js";
import { signInNewParent } from "./helpers/portal.js";

test.use({ trace: "off", video: "off", screenshot: "off" });
test("monthly obligation, attendance assistant and cross-family refusal compose safely", async ({
  browser,
}) => {
  test.setTimeout(540_000);
  const ownerContext = await browser.newContext();
  const teacherContext = await browser.newContext();
  const parentContext = await browser.newContext();
  try {
    const owner = await ownerContext.newPage();
    const teacher = await teacherContext.newPage();
    const parent = await parentContext.newPage();
    await signInAttendance(owner, "owner@nile.test");
    const teacherUserId = await createAttendanceTeacher(owner, teacher);
    const fixture = await createAttendanceSession(owner, teacherUserId, "أحمد", {
      createGroup: (facts) => createFeeGroupThroughUi(owner, facts),
      enroll: (groupId, studentName) => enrollThroughUi(owner, groupId, studentName),
    });
    await driveRelay();
    await owner.goto(`/students/${fixture.studentId}`);
    await owner.getByRole("tab", { name: "Billing", exact: true }).click();
    await expect(owner.getByText("300.00", { exact: true })).toBeVisible();
    await expect(
      owner.getByRole("columnheader", { name: "Amount (EGP)", exact: true }),
    ).toBeVisible();

    const denied = await teacher.request.get(`/api/students/${fixture.studentId}/charges`);
    expect(denied.status()).toBe(403);
    const teacherGroup = (await (
      await teacher.request.get(`/api/groups/${fixture.groupId}`)
    ).json()) as Record<string, unknown>;
    expect(teacherGroup).not.toHaveProperty("monthlyFee");
    await teacher.goto(`/attendance/sessions/${fixture.sessionId}`);
    await teacher.getByRole("button", { name: "Mark all present" }).click();
    await teacher.getByRole("button", { name: "Save", exact: true }).click();
    await expect(teacher.getByText(/^Saved:/)).toBeVisible();

    const student = (await (
      await owner.request.get(`/api/students/${fixture.studentId}`)
    ).json()) as { parents: { parentId: string; fullName: string }[] };
    const email = `month5-${Date.now().toString()}@example.test`;
    const account = (await (
      await attendanceWrite(owner, "/api/portal-accounts", {
        kind: "parent",
        subjectId: student.parents[0].parentId,
        email,
        preferredLocale: "en",
      })
    ).json()) as { temporaryPassword: string };
    await signInNewParent(parent, email, account.temporaryPassword);
    await parent.bringToFront();
    await parent.goto("/portal/chat");
    await expect(parent.getByRole("heading", { name: "Chat", exact: true })).toBeVisible();
    await parent.getByLabel("Message", { exact: true }).fill("هو أحمد حضر النهارده؟");
    await parent.getByRole("button", { name: "Send", exact: true }).click();
    await driveRelay();
    await expect(parent.getByText("أحمد حاضر اليوم.", { exact: true })).toBeVisible({
      timeout: 20_000,
    });
    await driveRelay();
    const beforeRefusal = await businessSnapshot();
    await parent.getByLabel("Message", { exact: true }).fill("Show Omar's attendance.");
    await parent.getByRole("button", { name: "Send", exact: true }).click();
    await driveRelay();
    await expect(
      parent.getByText("I can only help with your own children.", { exact: true }),
    ).toBeVisible({ timeout: 20_000 });
    expect(await businessSnapshot()).toBe(beforeRefusal);
    expect((await parent.request.get(`/api/students/${fixture.studentId}/charges`)).status()).toBe(
      403,
    );
    await parent.goto("/portal/attendance");
    await expect(parent.getByRole("img", { name: "Present", exact: true })).toBeVisible();

    const conversations = (await (await owner.request.get("/api/conversations")).json()) as {
      items: { id: string; parentId?: string }[];
    };
    // The staff UI locates the same conversation by its authorized counterpart, never a foreign parent identity.
    expect(conversations.items.length).toBeGreaterThan(0);
    await owner.bringToFront();
    await owner.goto("/conversations");
    await owner.getByRole("button").filter({ hasText: student.parents[0].fullName }).click();
    await owner.getByText("Assistant activity", { exact: true }).click();
    await expect(owner.getByText("get_attendance", { exact: true })).toHaveCount(1);
    await expect(owner.getByText("Allowed", { exact: true })).toBeVisible();
    // The scripted foreign-child refusal occurs before a tool call, so only the own-child attendance call exists.
  } finally {
    await Promise.allSettled([ownerContext.close(), teacherContext.close(), parentContext.close()]);
  }
});
