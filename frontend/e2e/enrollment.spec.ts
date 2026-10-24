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
  await expect(page.getByText("Full", { exact: true })).toBeVisible();
  await expect(page.getByRole("button", { name: "Enroll student" })).toHaveAttribute("aria-disabled", "true");

  // The late window is refused by the server, in words, with the dialog still open.
  await late.dialog.getByRole("button", { name: "Enroll", exact: true }).click();
  await expect(late.dialog.getByRole("alert")).toHaveText("This group is full");
  await expect(late.dialog).toBeVisible();
  await late.context.close();

  // Transfer one student to the other mathematics group: both rosters change.
  await page.getByRole("button", { name: "Actions for أحمد محمود" }).click();
  await page.getByRole("menuitem", { name: "Transfer to another group" }).click();
  const transfer = page.getByRole("dialog", { name: "Transfer أحمد محمود" });
  await transfer.getByRole("radio", { name: new RegExp(otherGroup) }).click();
  await transfer.getByRole("button", { name: "Transfer", exact: true }).click();
  await expect(page.getByText(`أحمد محمود was transferred to ${otherGroup}.`)).toBeVisible();
  await expect(page.getByRole("table").getByRole("link", { name: "أحمد محمود" })).toHaveCount(0);
  await expect(page.getByText("1 / 2")).toBeVisible();

  await page.goto(otherRoster);
  await expect(page.getByRole("table").getByRole("link", { name: "أحمد محمود" })).toBeVisible();

  // Withdraw, after a confirmation that names the student and the group.
  await page.getByRole("button", { name: "Actions for أحمد محمود" }).click();
  await page.getByRole("menuitem", { name: "Withdraw" }).click();
  const confirm = page.getByRole("alertdialog");
  await expect(confirm.getByRole("heading", { name: `Withdraw أحمد محمود from ${otherGroup}?` })).toBeVisible();
  await confirm.getByRole("button", { name: "Withdraw", exact: true }).click();
  await expect(page.getByText("No students enrolled yet")).toBeVisible();
});

test("teacher: sees their own group's roster, names only, with no actions", async ({ page }) => {
  await signIn(page, "teacher@both.test");
  await page.getByRole("button", { name: "Nile Tutoring Centre" }).click();
  await expect(page.getByRole("heading", { name: "Welcome, Two-Centre Teacher" })).toBeVisible();

  await page.getByRole("navigation").getByRole("link", { name: "Groups" }).click();
  await page.getByRole("link", { name: "رياضيات - المجموعة أ" }).click();
  await page.getByRole("tab", { name: "Roster" }).click();

  const table = page.getByRole("table");
  await expect(table.getByRole("row").nth(1)).toBeVisible();
  await expect(table.getByRole("link")).toHaveCount(0);
  await expect(page.getByRole("button", { name: "Enroll student" })).toHaveCount(0);
  await expect(page.getByRole("button", { name: /^Actions for/ })).toHaveCount(0);
});
