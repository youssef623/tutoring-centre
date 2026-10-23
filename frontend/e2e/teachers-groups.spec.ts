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
  await expect(page.getByRole("heading", { name: "Welcome, Two-Centre Teacher" })).toBeVisible();

  await page.getByRole("navigation").getByRole("link", { name: "Groups" }).click();
  await expect(page.getByRole("link", { name: "رياضيات - المجموعة أ" })).toBeVisible();
  // Exactly the one seeded own group: a header row plus one body row.
  await expect(page.getByRole("row")).toHaveCount(2);
  await expect(page.getByRole("button", { name: "Create group" })).toHaveCount(0);

  // Pasting the URL of a group that exists but is taught by someone else reveals nothing.
  await page.goto(otherTeachersGroupUrl);
  await expect(page.getByRole("heading", { name: "Group not found" })).toBeVisible();

  await page.getByRole("button", { name: "Two-Centre Teacher" }).click();
  await page.getByRole("menuitem", { name: "Switch centre" }).click();
  await page.getByRole("button", { name: "Maadi Learning Hub" }).click();
  await expect(page.getByRole("heading", { name: "Welcome, Two-Centre Teacher" })).toBeVisible();
  await expect(page.getByRole("banner").getByText("Maadi Learning Hub", { exact: true })).toBeVisible();

  await page.getByRole("navigation").getByRole("link", { name: "Groups" }).click();
  await expect(page.getByRole("link", { name: "Mathematics - Group A" })).toBeVisible();
  await expect(page.getByRole("link", { name: "Physics - Group A" })).toBeVisible();
  await expect(page.getByText("رياضيات - المجموعة أ")).toHaveCount(0);
  await expect(page.getByText("منى السيد")).toHaveCount(0);
});

test("secretary: sees Teachers read-only, and can create groups", async ({ page }) => {
  await signIn(page, "secretary@nile.test");

  await page.getByRole("navigation").getByRole("link", { name: "Teachers" }).click();
  await expect(page.getByRole("heading", { name: "Teachers" })).toBeVisible();
  await expect(page.getByRole("row", { name: /كريم فتحي/ })).toBeVisible();
  await expect(page.getByRole("button", { name: "Add teacher" })).toHaveCount(0);
  await expect(page.getByRole("button", { name: /^Actions for/ })).toHaveCount(0);

  await page.getByRole("navigation").getByRole("link", { name: "Groups" }).click();
  await expect(page.getByRole("heading", { name: "Groups" })).toBeVisible();
  await expect(page.getByRole("button", { name: "Create group" }).first()).toBeVisible();
});
