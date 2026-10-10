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

async function changeSecretaryRole(page: Page, roleName: "Teacher" | "Assistant"): Promise<void> {
  const row = page.getByRole("row", { name: "Nile Secretary" });
  await row.getByRole("button", { name: "Open actions menu" }).click();
  await page.getByRole("menuitem", { name: "Change role" }).click();
  await expect(page.getByRole("heading", { name: "Change role for Nile Secretary" })).toBeVisible();
  await page.getByRole("combobox", { name: "Role" }).click();
  await page.getByRole("option", { name: roleName }).click();
  await page.getByRole("button", { name: "Save" }).click();
  await expect(page.getByText("Role changed.")).toBeVisible();
}

test("owner: changing a member's role appears in the audit log with translated before/after values", async ({
  page,
}) => {
  await signIn(page, "owner@nile.test");
  await page.goto("/staff");

  try {
    await expect(page.getByRole("row", { name: "Nile Secretary" }).getByText("Assistant")).toBeVisible();

    await changeSecretaryRole(page, "Teacher");
    await expect(page.getByRole("row", { name: "Nile Secretary" }).getByText("Teacher")).toBeVisible();

    await page.goto("/audit");
    await page.getByLabel("Entity type").click();
    await page.getByRole("option", { name: "Staff membership" }).click();

    // Another journey (first-login.spec.ts) may run at the same time and also write a membership entry
    // (a new staff member being created), and past runs of this very test leave entries behind too — so
    // this test's own entry is not necessarily the newest one. Direction (Assistant -> Teacher, not the
    // reverse) plus "newest first" picks it out unambiguously.
    const thisChange = page.getByText("Role:Assistant→Teacher").first();
    await expect(thisChange).toBeVisible();
    const thisEntry = thisChange.locator("xpath=ancestor::div[contains(@class,'rounded-xl')][1]");
    await expect(thisEntry.getByText("Nile Owner")).toBeVisible();
    await expect(thisEntry.getByText("Updated")).toBeVisible();
  } finally {
    // Leave the seeded fixture as every other spec (and a human re-running this one) expects it.
    await page.goto("/staff");
    await changeSecretaryRole(page, "Assistant");
  }
});
