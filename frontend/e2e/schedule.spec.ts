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
    await expect(dialog).toBeHidden();
  } finally {
    await context.close();
  }
}

test("owner: schedules two groups of one teacher, is refused the overlap, and cancels a session", async ({ page }) => {
  const suffix = Date.now().toString();
  const teacherName = `سلمى فاروق ${suffix}`;
  const firstGroup = `فيزياء سبت أ ${suffix}`;
  const secondGroup = `فيزياء سبت ب ${suffix}`;
  const saturday = nextSaturdayInCairo();

  await signIn(page, "owner@nile.test");
  await addTeacher(page, teacherName);
  const firstPath = await createGroup(page, firstGroup, teacherName);
  const secondPath = await createGroup(page, secondGroup, teacherName);

  // Saturday 18:00 for 90 minutes on the first group.
  // Three or four Saturdays fall in the four-week horizon, depending on the weekday the suite runs.
  const firstDialog = await addWeeklySession(page, firstPath, { hour: "6", minute: "00", length: "1 hour 30 minutes", startsOn: saturday });
  await expect(page.getByText(/^[34] sessions scheduled$/)).toBeVisible();
  await expect(firstDialog).toBeHidden();
  await expect(page.getByRole("row", { name: /Saturday/ })).toBeVisible();

  // The timetable shows it on Saturday, at 18:00 in the centre's time.
  await page.getByRole("navigation").getByRole("link", { name: "Timetable" }).click();
  await expect(page.getByRole("heading", { name: "Timetable" })).toBeVisible();
  await page.goto(`/timetable?week=${saturday}`);
  const saturdayColumn = page.getByRole("region", { name: /^Saturday/ });
  const firstCard = saturdayColumn.getByRole("article", { name: new RegExp(`^${firstGroup}, 6:00\\s?–\\s?7:30\\sPM$`, "u") });
  await expect(firstCard).toBeVisible();

  // 19:00 starts inside the teacher's 18:00-19:30 session: the database refuses it, and the dialog says why.
  const overlap = await addWeeklySession(page, secondPath, { hour: "7", minute: "00", length: "1 hour", startsOn: saturday });
  await expect(overlap.getByText("The group's teacher already has a session at that time. Choose another day or time.")).toBeVisible();
  await expect(overlap).toBeVisible();

  // Moved to 19:30, it fits.
  await choose(page, overlap, "Minute", "30");
  await overlap.getByRole("button", { name: "Add weekly session" }).click();
  await expect(overlap).toBeHidden();

  // Cancel the first group's session: it stays on the timetable, marked.
  await page.goto(`/timetable?week=${saturday}`);
  await expect(saturdayColumn.getByRole("article", { name: new RegExp(`^${secondGroup}, 7:30\\s?–\\s?8:30\\sPM$`, "u") })).toBeVisible();
  await firstCard.getByRole("button", { name: /^Actions for/ }).click();
  await page.getByRole("menuitem", { name: "Cancel session" }).click();
  const confirm = page.getByRole("alertdialog");
  await expect(confirm.getByRole("heading", { name: new RegExp(`^Cancel ${firstGroup} on .+ at 6:00\\sPM\\?$`, "u") })).toBeVisible();
  await confirm.getByRole("button", { name: "Cancel session" }).click();
  await expect(page.getByText("Session cancelled")).toBeVisible();
  await expect(firstCard.getByText("Cancelled", { exact: true })).toBeVisible();
});

test("teacher: sees only their own sessions in Nile, with no schedule controls", async ({ page, browser }) => {
  const saturday = nextSaturdayInCairo();
  const othersGroup = `فيزياء غيري ${Date.now().toString()}`;
  await scheduleAnotherTeachersGroup(browser, othersGroup, saturday);

  await signIn(page, "teacher@both.test");
  await page.getByRole("button", { name: "Nile Tutoring Centre" }).click();
  await expect(page.getByRole("heading", { name: "Welcome, Two-Centre Teacher" })).toBeVisible();

  await page.getByRole("navigation").getByRole("link", { name: "Timetable" }).click();
  await expect(page.getByRole("heading", { name: "Timetable" })).toBeVisible();
  await page.goto(`/timetable?week=${saturday}`);
  await expect(page.getByRole("navigation", { name: /^Week of/ })).toBeVisible();

  // Another teacher's session in this same week is not shown, and every session shown is the linked teacher's.
  await expect(page.getByText(othersGroup)).toHaveCount(0);
  const cards = page.getByRole("article");
  for (const card of await cards.all()) {
    await expect(card.getByText("كريم فتحي")).toBeVisible();
  }

  // No manage control: no extend action, no session menu, no teacher filter.
  await expect(page.getByRole("button", { name: "Extend schedule" })).toHaveCount(0);
  await expect(page.getByRole("button", { name: /^Actions for/ })).toHaveCount(0);
  await expect(page.getByRole("combobox", { name: "Teacher" })).toHaveCount(0);
});
