import { type Page, expect, test } from "@playwright/test";

// The seeded dev password is never written here: it is supplied by whoever runs the suite
// (the CI job sets a throwaway, Task 18.5; locally the developer sets their own, Task 18.3).
const password: string = process.env.SEED_PASSWORD ?? "";
if (password === "") {
  throw new Error("SEED_PASSWORD must be set to run the E2E auth journeys (see README).");
}

async function signIn(page: Page, email: string): Promise<void> {
  await page.goto("/");
  await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();
  await page.getByLabel("Email").fill(email);
  await page.getByLabel("Password").fill(password);
  await page.getByRole("button", { name: "Sign in" }).click();
}

test("owner: sign in, dashboard, Arabic RTL, logout", async ({ page }) => {
  await signIn(page, "owner@nile.test");

  await expect(page.getByRole("heading", { name: "Welcome, Nile Owner" })).toBeVisible();
  await expect(page.getByText("Nile Tutoring Centre", { exact: true })).toBeVisible();

  // The language switcher lives inside the user menu, not directly in the header. It is a plain
  // button there (not a Menu.Item), so switching language does not close the menu.
  await page.getByRole("button", { name: "Nile Owner" }).click();
  await page.getByRole("button", { name: "العربية" }).click();
  await expect(page.locator("html")).toHaveAttribute("dir", "rtl");
  await page.getByRole("menuitem", { name: "تسجيل الخروج" }).click();

  await expect(page.getByRole("heading", { name: "تسجيل الدخول" })).toBeVisible();
});

test("teacher: centre picker, header reflects the choice, switch centre", async ({ page }) => {
  await signIn(page, "teacher@both.test");

  await expect(page.getByRole("heading", { name: "Choose a centre" })).toBeVisible();
  await page.getByRole("button", { name: "Maadi Learning Hub" }).click();

  await expect(page.getByText("Maadi Learning Hub", { exact: true })).toBeVisible();
  await expect(page.getByRole("heading", { name: "Welcome, Two-Centre Teacher" })).toBeVisible();

  await page.getByRole("button", { name: "Two-Centre Teacher" }).click();
  await page.getByRole("menuitem", { name: "Switch centre" }).click();
  await page.getByRole("button", { name: "Nile Tutoring Centre" }).click();

  await expect(page.getByText("Nile Tutoring Centre", { exact: true })).toBeVisible();
});

test("a wrong password shows the generic error and stays on /login", async ({ page }) => {
  await page.goto("/");
  await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();
  await page.getByLabel("Email").fill("owner@nile.test");
  await page.getByLabel("Password").fill("definitely-the-wrong-password");
  await page.getByRole("button", { name: "Sign in" }).click();

  await expect(page.getByRole("alert")).toHaveText("Incorrect email or password.");
  await expect(page).toHaveURL(/\/login/);
});
