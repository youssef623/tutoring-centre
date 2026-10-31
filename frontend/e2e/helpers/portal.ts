import { type Page, expect } from "@playwright/test";
/** Credentials stay in the isolated browser context; this helper does not log or persist them. */
export async function signInNewParent(page: Page, email: string, temporaryPassword: string) {
  await page.goto("/login");
  await page.getByLabel("Email").fill(email);
  await page.getByLabel("Password").fill(temporaryPassword);
  await page.getByRole("button", { name: "Sign in", exact: true }).click();
  await page.locator("#current-password").fill(temporaryPassword);
  await page.locator("#new-password").fill("E2E-Month5-Parent-Safe!");
  await page.locator("#confirm-password").fill("E2E-Month5-Parent-Safe!");
  await page.getByRole("button", { name: "Change password", exact: true }).click();
  await page.getByRole("button", { name: /Nile Tutoring Centre/ }).click();
  await expect(page).toHaveURL(/\/portal$/);
}
