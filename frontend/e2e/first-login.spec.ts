import { type Page, expect, test } from "@playwright/test";

// The seeded dev password is never written here: it is supplied by whoever runs the suite
// (the CI job sets a throwaway, Task 18.5; locally the developer sets their own, Task 18.3).
const password: string = process.env.SEED_PASSWORD ?? "";
if (password === "") {
  throw new Error("SEED_PASSWORD must be set to run the E2E first-login journey (see README).");
}

// This journey reads a one-time temporary password off the screen. Tracing and video are switched off for
// it specifically — not just left at the project default — so a retry can never leave the password sitting
// in a recording kept as a CI artefact (Day 33 rule).
test.use({ trace: "off", video: "off" });

async function signIn(page: Page, email: string, withPassword: string): Promise<void> {
  await page.goto("/");
  await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();
  await page.getByLabel("Email").fill(email);
  await page.getByLabel("Password").fill(withPassword);
  await page.getByRole("button", { name: "Sign in" }).click();
}

test("a new staff member is forced through a password change on first sign-in", async ({ page, browser }) => {
  const newEmail = `e2e-firstlogin-${Date.now().toString()}@nile.test`;
  const newDisplayName = "E2E First Login";

  await signIn(page, "owner@nile.test", password);
  await expect(page.getByRole("heading", { name: "Welcome, Nile Owner" })).toBeVisible();
  await page.goto("/staff");

  await page.getByRole("button", { name: "Add staff" }).click();
  await page.getByLabel("Email").fill(newEmail);
  await page.getByLabel("Name", { exact: true }).fill(newDisplayName);
  await page.getByRole("button", { name: "Save" }).click();

  await expect(page.getByRole("heading", { name: "Temporary password" })).toBeVisible();
  // A fresh 16-character password from the seeded alphabet (Infrastructure's StaffAccountService) — this
  // is the only place in the whole run this value is ever read, and it goes straight into a new, isolated
  // browser context below rather than anywhere that could be logged or retained.
  const temporaryPassword = await page.getByText(/^[A-Za-z0-9]{16}$/).innerText();
  await page.getByRole("button", { name: "Done" }).click();

  // A second, fully isolated session for the new staff member — separate cookies from the owner's session
  // above, mirroring them signing in from their own device for the first time.
  const newUserContext = await browser.newContext();
  const newUserPage = await newUserContext.newPage();
  try {
    await signIn(newUserPage, newEmail, temporaryPassword);

    await expect(newUserPage.getByRole("heading", { name: "Change password" })).toBeVisible();
    await expect(
      newUserPage.getByText("You're signing in with a temporary password. Choose a new password to continue."),
    ).toBeVisible();

    // Visiting the centre picker directly while still flagged is bounced straight back here.
    await newUserPage.goto("/select-centre");
    await expect(newUserPage.getByRole("heading", { name: "Change password" })).toBeVisible();

    const newPassword = "E2E-Replacement-Pass-1";
    await newUserPage.getByLabel("Current password").fill(temporaryPassword);
    await newUserPage.getByLabel("New password", { exact: true }).fill(newPassword);
    await newUserPage.getByLabel("Confirm new password").fill(newPassword);
    await newUserPage.getByRole("button", { name: "Change password" }).click();

    await expect(newUserPage.getByText("Password changed.")).toBeVisible();
    await expect(newUserPage.getByRole("heading", { name: "Choose a centre" })).toBeVisible();

    await newUserPage.getByRole("button", { name: "Nile Tutoring Centre" }).click();
    await expect(newUserPage.getByRole("heading", { name: `Welcome, ${newDisplayName}` })).toBeVisible();

    // A teacher by default: no owner-only nav.
    await expect(newUserPage.getByRole("navigation").getByRole("link", { name: "Staff" })).not.toBeAttached();
  } finally {
    await newUserContext.close();
  }
});
