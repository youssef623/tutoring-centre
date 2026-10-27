# `frontend/e2e/teachers-groups.spec.ts`

## Purpose

Source comment (intent): Picks an option in a Base UI select inside `scope` by the trigger's label.

## Where it fits

Category: **test**. [Open current file](../../../../frontend/e2e/teachers-groups.spec.ts). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 128 lines and 6,647 bytes. Source hash: `b87f07c904b66fde92ee6601beb5db782132746d8acfd161dcdb9d11903e8552`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { type Browser, type Page, expect, test } from "@playwright/test";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../frontend/e2e/teachers-groups.spec.ts#L10): `async function signIn(page: Page, email: string): Promise<void> {`
- [Line 20](../../../../frontend/e2e/teachers-groups.spec.ts#L20): `async function choose(page: Page, scope: ReturnType<Page["getByRole"]>, label: string, option: string): Promise<void> {`
- [Line 29](../../../../frontend/e2e/teachers-groups.spec.ts#L29): `async function urlOfGroupTaughtByMona(browser: Browser): Promise<string> {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { type Browser, type Page, expect, test } from "@playwright/test";

// The seeded dev password is never written here: it is supplied by whoever runs the suite
// (the CI job sets a throwaway, Task 18.5; locally the developer sets their own, Task 18.3).
const password: string = process.env.SEED_PASSWORD ?? "";
if (password === "") {
  throw new Error("SEED_PASSWORD must be set to run the E2E teacher and group journeys (see README).");
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
async function choose(page: Page, scope: ReturnType<Page["getByRole"]>, label: string, option: string): Promise<void> {
  await scope.getByRole("combobox", { name: label }).click();
  await page.getByRole("option", { name: option, exact: true }).click();
}

/**
 * The journey's own setup: as the Nile owner, in a separate browser context, read the URL of a seeded group
 * taught by "منى السيد" (the unlinked teacher) — never a hard-coded ID, never another spec's leftovers.
 */
async function urlOfGroupTaughtByMona(browser: Browser): Promise<string> {
  const context = await browser.newContext();
  const page = await context.newPage();
  try {
    await signIn(page, "owner@nile.test");
    await page.goto("/groups");
    const row = page.getByRole("row").filter({ hasText: "منى السيد" }).first();
    await expect(row).toBeVisible();
    const href = await row.getByRole("link").getAttribute("href");
    if (href === null) {
      throw new Error("The seeded group row has no link.");
    }

    return href;
  } finally {
    await context.close();
  }
}

test("owner: adds a teacher, creates a group for a seeded subject with them, then changes its capacity", async ({ page }) => {
  const suffix = Date.now().toString();
  const teacherName = `هالة سمير ${suffix}`;
  const groupName = `فيزياء ٢ث ${suffix}`;

  await signIn(page, "owner@nile.test");

  await page.getByRole("navigation").getByRole("link", { name: "Teachers" }).click();
  await expect(page.getByRole("heading", { name: "Teachers" })).toBeVisible();
  await page.getByRole("button", { name: "Add teacher" }).first().click();
  await page.getByLabel("Full name").fill(teacherName);
  await page.getByRole("button", { name: "Save" }).click();
  await expect(page.getByRole("row", { name: teacherName })).toBeVisible();

  await page.getByRole("navigation").getByRole("link", { name: "Groups" }).click();
  await expect(page.getByRole("heading", { name: "Groups" })).toBeVisible();
  await page.getByRole("button", { name: "Create group" }).first().click();
  const dialog = page.getByRole("dialog");
  await dialog.getByRole("textbox", { name: "Name" }).fill(groupName);
  await choose(page, dialog, "Subject", "فيزياء");
  await choose(page, dialog, "Teacher", teacherName);
  await dialog.getByRole("spinbutton", { name: "Capacity" }).fill("15");
  await dialog.getByRole("button", { name: "Create group" }).click();

  // Lands on the new group's own page.
  await expect(page.getByRole("heading", { name: groupName })).toBeVisible();
  await expect(page.getByText("0 / 15")).toBeVisible();

  await page.getByRole("button", { name: "Change capacity" }).click();
  await page.getByRole("dialog").getByRole("spinbutton", { name: "Capacity" }).fill("18");
  await page.getByRole("dialog").getByRole("button", { name: "Save" }).click();
  await expect(page.getByText("Capacity updated.")).toBeVisible();
  await expect(page.getByText("0 / 18")).toBeVisible();
});

test("teacher: one account sees only its own group in Nile, not another teacher's, and only Maadi's groups in Maadi", async ({
  page,
  browser,
}) => {
  const otherTeachersGroupUrl = await urlOfGroupTaughtByMona(browser);

  await signIn(page, "teacher@both.test");
  await page.getByRole("button", { name: "Nile Tutoring Centre" }).click();
```

The excerpt stops at line 90; use the original file for the remaining 38 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
