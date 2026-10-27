# `frontend/e2e/enrollment.spec.ts`

## Purpose

Source comment (intent): Picks an option in a Base UI select inside `scope` by the trigger's label.

## Where it fits

Category: **test**. [Open current file](../../../../frontend/e2e/enrollment.spec.ts). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 136 lines and 7,585 bytes. Source hash: `702735e21be08181846862ba63afbabc4b1c4e0a971fa2c286aa8e269c1a51c8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { type Browser, type Locator, type Page, expect, test } from "@playwright/test";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../frontend/e2e/enrollment.spec.ts#L10): `async function signIn(page: Page, email: string): Promise<void> {`
- [Line 20](../../../../frontend/e2e/enrollment.spec.ts#L20): `async function choose(page: Page, scope: Locator, label: string, option: string): Promise<void> {`
- [Line 26](../../../../frontend/e2e/enrollment.spec.ts#L26): `async function createGroup(page: Page, name: string, capacity: number): Promise<string> {`
- [Line 41](../../../../frontend/e2e/enrollment.spec.ts#L41): `async function enrollBySearch(page: Page, search: string, studentName: string): Promise<void> {`
- [Line 52](../../../../frontend/e2e/enrollment.spec.ts#L52): `async function prepareEnrollFromStudentPage(browser: Browser, studentName: string, groupName: string) {`

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
  throw new Error("SEED_PASSWORD must be set to run the E2E enrollment journeys (see README).");
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

/** Creates a mathematics group taught by a seeded teacher and returns its roster URL. */
async function createGroup(page: Page, name: string, capacity: number): Promise<string> {
  await page.goto("/groups");
  await page.getByRole("button", { name: "Create group" }).first().click();
  const dialog = page.getByRole("dialog");
  await dialog.getByRole("textbox", { name: "Name" }).fill(name);
  await choose(page, dialog, "Subject", "رياضيات");
  await choose(page, dialog, "Teacher", "منى السيد");
  await dialog.getByRole("spinbutton", { name: "Capacity" }).fill(String(capacity));
  await dialog.getByRole("button", { name: "Create group" }).click();
  await expect(page.getByRole("heading", { name })).toBeVisible();

  return `${new URL(page.url()).pathname}?tab=roster`;
}

/** Enrolls a seeded student found by an Arabic search typed without hamza, the way people actually type it. */
async function enrollBySearch(page: Page, search: string, studentName: string): Promise<void> {
  await page.getByRole("button", { name: "Enroll student" }).first().click();
  const dialog = page.getByRole("dialog", { name: "Enroll a student" });
  await dialog.getByRole("searchbox", { name: "Search students" }).fill(search);
  await dialog.getByRole("option", { name: new RegExp(studentName) }).click();
  await dialog.getByRole("button", { name: "Enroll", exact: true }).click();
  await expect(dialog).toBeHidden();
  await expect(page.getByRole("table").getByRole("link", { name: studentName })).toBeVisible();
}

/** A second secretary window on a student's page, with the enroll-in-a-group picker opened on `groupName`. */
async function prepareEnrollFromStudentPage(browser: Browser, studentName: string, groupName: string) {
  const context = await browser.newContext();
  const page = await context.newPage();
  await signIn(page, "secretary@nile.test");
  await page.getByRole("navigation").getByRole("link", { name: "Students" }).click();
  await page.getByRole("searchbox").fill(studentName);
  await page.getByRole("link", { name: studentName, exact: true }).click();
  await expect(page.getByRole("heading", { name: studentName })).toBeVisible();
  await page.getByRole("button", { name: "Enroll in a group" }).click();
  const dialog = page.getByRole("dialog", { name: "Enroll in a group" });
  await choose(page, dialog, "Subject", "رياضيات");
  await dialog.getByRole("radio", { name: new RegExp(groupName) }).click();

  return { context, dialog };
}

test("secretary: fills a two-seat group, sees a full group refuse politely, transfers and withdraws", async ({
  page,
  browser,
}) => {
  const suffix = Date.now().toString();
  const smallGroup = `جبر مقعدان ${suffix}`;
  const otherGroup = `جبر مسائي ${suffix}`;

  await signIn(page, "secretary@nile.test");
  const otherRoster = await createGroup(page, otherGroup, 5);
  const smallRoster = await createGroup(page, smallGroup, 2);

  await page.goto(smallRoster);
  await expect(page.getByText("No students enrolled yet")).toBeVisible();
  await enrollBySearch(page, "احمد محمود", "أحمد محمود");
  await expect(page.getByText("1 / 2")).toBeVisible();

  // Another window opens the student page's picker while the group still shows a free seat...
  const late = await prepareEnrollFromStudentPage(browser, "عمر خالد", smallGroup);

  // ...and this window takes that seat first.
  await enrollBySearch(page, "ياسمين", "ياسمين علي");
  await expect(page.getByText("2 / 2")).toBeVisible();
```

The excerpt stops at line 90; use the original file for the remaining 46 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
