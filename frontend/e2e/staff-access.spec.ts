import { type Page, expect, test } from "@playwright/test";

// The seeded dev password is never written here: it is supplied by whoever runs the suite
// (the CI job sets a throwaway, Task 18.5; locally the developer sets their own, Task 18.3).
const password: string = process.env.SEED_PASSWORD ?? "";
if (password === "") {
  throw new Error("SEED_PASSWORD must be set to run the E2E staff/audit journeys (see README).");
}

async function signIn(page: Page, email: string): Promise<void> {
  await page.goto("/");
  await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();
  await page.getByLabel("Email").fill(email);
  await page.getByLabel("Password").fill(password);
  await page.getByRole("button", { name: "Sign in" }).click();
  await expect(page.getByRole("heading", { name: /^(Welcome,|Choose a centre)/ })).toBeVisible();
}

test("secretary: no owner-only nav items, and no owner-only page by direct URL", async ({ page }) => {
  await signIn(page, "secretary@nile.test");

  const sidebar = page.getByRole("navigation");
  await expect(sidebar.getByRole("link", { name: "Dashboard" })).toBeVisible();
  await expect(sidebar.getByRole("link", { name: "Subjects" })).toBeVisible();
  await expect(sidebar.getByRole("link", { name: "Staff" })).not.toBeAttached();
  await expect(sidebar.getByRole("link", { name: "Settings" })).not.toBeAttached();
  await expect(sidebar.getByRole("link", { name: "Audit log" })).not.toBeAttached();

  const guardedRequests: string[] = [];
  page.on("request", (request) => {
    const url = request.url();
    if (url.includes("/api/staff") || url.includes("/api/centre/settings") || url.includes("/api/audit")) {
      guardedRequests.push(url);
    }
  });

  for (const path of ["/staff", "/settings", "/audit"]) {
    await page.goto(path);
    await expect(page.getByText("You don't have access to this page")).toBeVisible();
    await expect(page.getByRole("link", { name: "Back to dashboard" })).toBeVisible();
  }

  expect(guardedRequests).toEqual([]);
});
