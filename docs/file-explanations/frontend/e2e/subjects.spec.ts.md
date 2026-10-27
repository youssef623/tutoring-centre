# `frontend/e2e/subjects.spec.ts`

## Purpose

Declares `signIn`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../frontend/e2e/subjects.spec.ts). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 107 lines and 5,724 bytes. Source hash: `a6f7ecc8d89309519ee9771ec8900504f55ca0f26fc07d8536454c6b7efe8f3e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { type Page, expect, test } from "@playwright/test";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../frontend/e2e/subjects.spec.ts#L10): `async function signIn(page: Page, email: string): Promise<void> {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
  // Active rows carry no status badge; restoring removes the "Archived" one.
  await expect(archivedRow.getByText("Archived")).toHaveCount(0);
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
```

The excerpt stops at line 90; use the original file for the remaining 17 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
