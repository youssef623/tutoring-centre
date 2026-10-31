import { execFile } from "node:child_process";
import { promisify } from "node:util";
import { resolve } from "node:path";
import { type Page, expect } from "@playwright/test";
const run = promisify(execFile);

export async function driveRelay() {
  await run(
    "dotnet",
    [resolve("../src/TutoringCentre.Api/bin/Release/net10.0/TutoringCentre.Api.dll"), "relay-once"],
    {
      cwd: resolve("../src/TutoringCentre.Api"),
      timeout: 90_000,
      env: {
        ...process.env,
        DOTNET_ENVIRONMENT: "Testing",
        ASPNETCORE_ENVIRONMENT: "Testing",
        Jobs__RunServer: "false",
        Outbox__RunRelay: "false",
        Assistant__Provider: "scripted",
        Assistant__ScriptPath: resolve("e2e/fixtures/scripted-model.json"),
      },
    },
  );
}
export async function createFeeGroupThroughUi(
  page: Page,
  facts: { name: string; subjectId: string; teacherId: string; capacity: number },
) {
  const subjects = (await (await page.request.get("/api/subjects")).json()) as {
    items: { id: string; name: string }[];
  };
  const teachers = (await (await page.request.get("/api/teachers")).json()) as {
    items: { id: string; fullName: string }[];
  };
  const subject = subjects.items.find((s) => s.id === facts.subjectId);
  const teacher = teachers.items.find((t) => t.id === facts.teacherId);
  if (!subject || !teacher) throw new Error("Fixture reference missing.");
  await page.goto("/groups");
  await page.getByRole("button", { name: "Create group", exact: true }).first().click();
  const dialog = page.getByRole("dialog", { name: "Create group", exact: true });
  await dialog.getByLabel("Name", { exact: true }).fill(facts.name);
  await dialog.getByLabel("Subject", { exact: true }).click();
  await page
    .getByRole("option", {
      name: subject.name,
      exact: true,
    })
    .click();
  await dialog.getByLabel("Teacher", { exact: true }).click();
  await page
    .getByRole("option", {
      name: teacher.fullName,
      exact: true,
    })
    .click();
  await dialog.getByLabel("Monthly fee (EGP)").fill("300.00");
  const response = page.waitForResponse(
    (r) => r.url().endsWith("/api/groups") && r.request().method() === "POST",
  );
  await dialog.getByRole("button", { name: "Create group", exact: true }).click();
  const created = await response;
  expect(created.status()).toBe(201);
  return (await created.json()) as { id: string };
}
export async function enrollThroughUi(page: Page, groupId: string, studentName: string) {
  await page.goto(`/groups/${groupId}?tab=roster`);
  await page.getByRole("button", { name: "Enroll student", exact: true }).click();
  await page.getByLabel("Search students", { exact: true }).fill(studentName);
  await page.getByRole("option").filter({ hasText: studentName }).click();
  await page.getByRole("button", { name: "Enroll", exact: true }).click();
  await expect(page.getByRole("dialog")).toHaveCount(0);
  await driveRelay();
}

/** Uses the shared .NET DatabaseSnapshot, including audit and every business table. */
export async function businessSnapshot() {
  const result = await run(
    "dotnet",
    [
      resolve(
        "../tests/TutoringCentre.Evaluation/bin/Release/net10.0/TutoringCentre.Evaluation.dll",
      ),
      "journey-snapshot",
    ],
    { timeout: 90_000, env: process.env },
  );
  const fingerprint = result.stdout.trim();
  if (!/^[A-F0-9]{64}$/.test(fingerprint)) throw new Error("Snapshot evidence missing.");
  return fingerprint;
}
