import { type Page, expect, test } from "@playwright/test";

// The seeded dev password is never written here: it is supplied by whoever runs the suite
// (the CI job sets a throwaway, Task 18.5; locally the developer sets their own, Task 18.3).
const password: string = process.env.SEED_PASSWORD ?? "";
if (password === "") {
  throw new Error("SEED_PASSWORD must be set to run the E2E subjects journeys (see README).");
}

async function signIn(page: Page, email: string): Promise<void> {
  await page.goto("/");
  await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();
  await page.getByLabel("Email").fill(email);
  await page.getByLabel("Password").fill(password);
  await page.getByRole("button", { name: "Sign in" }).click();
  // Wait for the post-login SPA navigation (dashboard, or the centre picker for a two-centre account)
  // to settle before any further navigation, so a later page.goto never races the login request.
  await expect(page.getByRole("heading", { name: /^(Welcome,|Choose a centre)/ })).toBeVisible();
}

test("owner: add, rename, archive, show archived, restore a subject", async ({ page }) => {
  const subjectName = `E2E Biology ${Date.now().toString()}`;
  const renamedName = `${subjectName} (renamed)`;

  await signIn(page, "owner@nile.test");
  await page.goto("/subjects");

  // The three seeded Nile subjects are present, in Arabic.
  await expect(page.getByRole("row", { name: "رياضيات" })).toBeVisible();
  await expect(page.getByRole("row", { name: "فيزياء" })).toBeVisible();
  await expect(page.getByRole("row", { name: "كيمياء" })).toBeVisible();

  // Add.
  await page.getByRole("button", { name: "Add subject" }).click();
  await page.getByLabel("Name", { exact: true }).fill(subjectName);
  await page.getByRole("button", { name: "Save" }).click();
  await expect(page.getByText("Subject added.")).toBeVisible();
  const newRow = page.getByRole("row", { name: subjectName });
  await expect(newRow).toBeVisible();

  // Rename.
  await newRow.getByRole("button", { name: "Open actions menu" }).click();
  await page.getByRole("menuitem", { name: "Rename" }).click();
  await page.getByLabel("Name", { exact: true }).fill(renamedName);
  await page.getByRole("button", { name: "Save" }).click();
  await expect(page.getByText("Subject renamed.")).toBeVisible();
  const renamedRow = page.getByRole("row", { name: renamedName });
  await expect(renamedRow).toBeVisible();

  // Archive (with confirmation).
  await renamedRow.getByRole("button", { name: "Open actions menu" }).click();
  await page.getByRole("menuitem", { name: "Archive" }).click();
  await expect(page.getByRole("heading", { name: `Archive "${renamedName}"?` })).toBeVisible();
  await page.getByRole("button", { name: "Archive subject" }).click();
  await expect(page.getByText("Subject archived.")).toBeVisible();
  await expect(page.getByRole("row", { name: renamedName })).not.toBeVisible();

  // Show archived.
  await page.getByRole("switch", { name: "Show archived" }).click();
  const archivedRow = page.getByRole("row", { name: renamedName });
  await expect(archivedRow).toBeVisible();
  await expect(archivedRow.getByText("Archived")).toBeVisible();

  // Restore.
  await archivedRow.getByRole("button", { name: "Open actions menu" }).click();
  await page.getByRole("menuitem", { name: "Restore" }).click();
  await expect(page.getByText("Subject restored.")).toBeVisible();
  await expect(archivedRow.getByText("Active")).toBeVisible();
});

test("two centres see different subject lists, including for a teacher switching between them", async ({ page }) => {
  await signIn(page, "owner@maadi.test");
  await page.goto("/subjects");

  await expect(page.getByRole("row", { name: "Mathematics" })).toBeVisible();
  await expect(page.getByRole("row", { name: "Physics" })).toBeVisible();
  await expect(page.getByText("رياضيات")).not.toBeVisible();
  await expect(page.getByText("فيزياء")).not.toBeVisible();
  await expect(page.getByText("كيمياء")).not.toBeVisible();

  await page.getByRole("button", { name: "Maadi Owner" }).click();
  await page.getByRole("menuitem", { name: "Log out" }).click();
  await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();

  await signIn(page, "teacher@both.test");
  await page.getByRole("button", { name: "Nile Tutoring Centre" }).click();
  await expect(page.getByRole("heading", { name: "Welcome, Two-Centre Teacher" })).toBeVisible();

  await page.goto("/subjects");
  await expect(page.getByRole("row", { name: "رياضيات" })).toBeVisible();
  await expect(page.getByText("Mathematics", { exact: true })).not.toBeVisible();
  await expect(page.getByText("Physics", { exact: true })).not.toBeVisible();

  await page.getByRole("button", { name: "Two-Centre Teacher" }).click();
  await page.getByRole("menuitem", { name: "Switch centre" }).click();
  await page.getByRole("button", { name: "Maadi Learning Hub" }).click();
  // Wait for the dashboard (not the picker's own button, which shares this text) to confirm the
  // switch has actually landed before navigating away, so a later page.goto never races the mutation.
  await expect(page.getByRole("heading", { name: "Welcome, Two-Centre Teacher" })).toBeVisible();
  await expect(page.getByRole("banner").getByText("Maadi Learning Hub", { exact: true })).toBeVisible();

  await page.goto("/subjects");
  await expect(page.getByRole("row", { name: "Mathematics" })).toBeVisible();
  await expect(page.getByRole("row", { name: "Physics" })).toBeVisible();
  await expect(page.getByText("رياضيات")).not.toBeVisible();
});
