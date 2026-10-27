# `frontend/e2e/schedule.spec.ts`

## Purpose

Source comment (intent): The first Saturday strictly after today in Cairo, as "YYYY-MM-DD": computed for each run, never a fixed date, and never today, so its 18:00 session is always in the future (cancellable) whatever time the suite runs.

## Where it fits

Category: **test**. [Open current file](../../../../frontend/e2e/schedule.spec.ts). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 171 lines and 9,348 bytes. Source hash: `9dbeff848cba65801562b344ea8a4976b4d06b9a5be2c848d92e5b63343c51c3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { type Browser, type Locator, type Page, expect, test } from "@playwright/test";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../frontend/e2e/schedule.spec.ts#L14): `function nextSaturdayInCairo(): string {`
- [Line 25](../../../../frontend/e2e/schedule.spec.ts#L25): `async function signIn(page: Page, email: string): Promise<void> {`
- [Line 35](../../../../frontend/e2e/schedule.spec.ts#L35): `async function choose(page: Page, scope: Locator, label: string, option: string): Promise<void> {`
- [Line 41](../../../../frontend/e2e/schedule.spec.ts#L41): `async function addTeacher(page: Page, fullName: string): Promise<void> {`
- [Line 50](../../../../frontend/e2e/schedule.spec.ts#L50): `async function createGroup(page: Page, name: string, teacherName: string): Promise<string> {`
- [Line 63](../../../../frontend/e2e/schedule.spec.ts#L63): `async function addWeeklySession(`
- [Line 82](../../../../frontend/e2e/schedule.spec.ts#L82): `async function scheduleAnotherTeachersGroup(browser: Browser, groupName: string, saturday: string): Promise<void> {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { type Browser, type Locator, type Page, expect, test } from "@playwright/test";

// The seeded dev password is never written here: it is supplied by whoever runs the suite
// (the CI job sets a throwaway, Task 18.5; locally the developer sets their own, Task 18.3).
const password: string = process.env.SEED_PASSWORD ?? "";
if (password === "") {
  throw new Error("SEED_PASSWORD must be set to run the E2E scheduling journeys (see README).");
}

/**
 * The first Saturday strictly after today in Cairo, as "YYYY-MM-DD": computed for each run, never a fixed date, and
 * never today, so its 18:00 session is always in the future (cancellable) whatever time the suite runs.
 */
function nextSaturdayInCairo(): string {
  const parts = new Intl.DateTimeFormat("en-US", { year: "numeric", month: "2-digit", day: "2-digit", timeZone: "Africa/Cairo" }).formatToParts(
    new Date(),
  );
  const part = (type: Intl.DateTimeFormatPartTypes) => Number(parts.find((candidate) => candidate.type === type)?.value);
  const today = new Date(Date.UTC(part("year"), part("month") - 1, part("day")));
  const daysAhead = ((6 - today.getUTCDay() + 7) % 7) || 7;
  today.setUTCDate(today.getUTCDate() + daysAhead);
  return today.toISOString().slice(0, 10);
}

async function signIn(page: Page, email: string): Promise<void> {
  await page.goto("/");
  await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();
  await page.getByLabel("Email").fill(email);
  await page.getByLabel("Password").fill(password);
  await page.getByRole("button", { name: "Sign in" }).click();
  await expect(page.getByRole("heading", { name: /^(Welcome,|Choose a centre)/ })).toBeVisible();
}

/** Picks an option in a Base UI select inside `scope` by the trigger's label. */
async function choose(page: Page, scope: Locator, label: string, option: string): Promise<void> {
  await scope.getByRole("combobox", { name: label }).click();
  await page.getByRole("option", { name: option, exact: true }).click();
}

/** As the signed-in owner: a new teacher (so no seeded session can collide with this run's times). */
async function addTeacher(page: Page, fullName: string): Promise<void> {
  await page.goto("/teachers");
  await page.getByRole("button", { name: "Add teacher" }).first().click();
  await page.getByLabel("Full name").fill(fullName);
  await page.getByRole("button", { name: "Save" }).click();
  await expect(page.getByRole("row", { name: fullName })).toBeVisible();
}

/** As the signed-in owner: a physics group taught by `teacherName`; returns the group page's path. */
async function createGroup(page: Page, name: string, teacherName: string): Promise<string> {
  await page.goto("/groups");
  await page.getByRole("button", { name: "Create group" }).first().click();
  const dialog = page.getByRole("dialog");
  await dialog.getByRole("textbox", { name: "Name" }).fill(name);
  await choose(page, dialog, "Subject", "فيزياء");
  await choose(page, dialog, "Teacher", teacherName);
  await dialog.getByRole("button", { name: "Create group" }).click();
  await expect(page.getByRole("heading", { name })).toBeVisible();
  return new URL(page.url()).pathname;
}

/** Fills the Add weekly session dialog on a group's Schedule tab and submits it; returns the still-open dialog. */
async function addWeeklySession(
  page: Page,
  groupPath: string,
  slot: { hour: string; minute: string; length: string; startsOn: string },
): Promise<Locator> {
  await page.goto(`${groupPath}?tab=schedule`);
  await page.getByRole("button", { name: "Add weekly session" }).first().click();
  const dialog = page.getByRole("dialog", { name: "Add weekly session" });
  await choose(page, dialog, "Day", "Saturday");
  await choose(page, dialog, "Hour", slot.hour);
  await choose(page, dialog, "Minute", slot.minute);
  await choose(page, dialog, "AM or PM", "PM");
  await choose(page, dialog, "Length", slot.length);
  await dialog.getByLabel("Starts on").fill(slot.startsOn);
  await dialog.getByRole("button", { name: "Add weekly session" }).click();
  return dialog;
}

/** As the Nile owner in its own context: a fresh teacher's group with a Saturday 18:00 session in `week`. */
async function scheduleAnotherTeachersGroup(browser: Browser, groupName: string, saturday: string): Promise<void> {
  const context = await browser.newContext();
  const page = await context.newPage();
  try {
    await signIn(page, "owner@nile.test");
    const teacherName = `معلم آخر ${groupName}`;
    await addTeacher(page, teacherName);
    const groupPath = await createGroup(page, groupName, teacherName);
    const dialog = await addWeeklySession(page, groupPath, { hour: "6", minute: "00", length: "1 hour 30 minutes", startsOn: saturday });
```

The excerpt stops at line 90; use the original file for the remaining 81 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
