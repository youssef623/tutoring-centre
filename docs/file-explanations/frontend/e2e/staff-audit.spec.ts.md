# `frontend/e2e/staff-audit.spec.ts`

## Purpose

Source comment (intent): As the signed-in owner: a new staff member of this run's own, a Teacher by default.

## Where it fits

Category: **test**. [Open current file](../../../../frontend/e2e/staff-audit.spec.ts). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 74 lines and 4,138 bytes. Source hash: `7e19b9de4aa3ecbb14faaf9bc6981e32cb968eae26971d6bff09be3078f1d650`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { type Page, expect, test } from "@playwright/test";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../frontend/e2e/staff-audit.spec.ts#L14): `async function signIn(page: Page, email: string): Promise<void> {`
- [Line 24](../../../../frontend/e2e/staff-audit.spec.ts#L24): `async function addStaffMember(page: Page, email: string, displayName: string): Promise<void> {`
- [Line 34](../../../../frontend/e2e/staff-audit.spec.ts#L34): `async function changeRole(page: Page, displayName: string, roleName: "Teacher" | "Assistant"): Promise<void> {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/staff/StaffList.tsx:187](../../../../frontend/src/features/staff/StaffList.tsx#L187)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { type Page, expect, test } from "@playwright/test";

// The seeded dev password is never written here: it is supplied by whoever runs the suite
// (the CI job sets a throwaway, Task 18.5; locally the developer sets their own, Task 18.3).
const password: string = process.env.SEED_PASSWORD ?? "";
if (password === "") {
  throw new Error("SEED_PASSWORD must be set to run the E2E staff/audit journeys (see README).");
}

// This journey shows a one-time temporary password when it adds its staff member. It never reads it, but tracing
// and video stay off so a retry can never keep it in a recording (Day 33 rule, as in first-login.spec.ts).
test.use({ trace: "off", video: "off" });

async function signIn(page: Page, email: string): Promise<void> {
  await page.goto("/");
  await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();
  await page.getByLabel("Email").fill(email);
  await page.getByLabel("Password").fill(password);
  await page.getByRole("button", { name: "Sign in" }).click();
  await expect(page.getByRole("heading", { name: /^(Welcome,|Choose a centre)/ })).toBeVisible();
}

/** As the signed-in owner: a new staff member of this run's own, a Teacher by default. */
async function addStaffMember(page: Page, email: string, displayName: string): Promise<void> {
  await page.getByRole("button", { name: "Add staff" }).click();
  await page.getByLabel("Email").fill(email);
  await page.getByLabel("Name", { exact: true }).fill(displayName);
  await page.getByRole("button", { name: "Save" }).click();
  await expect(page.getByRole("heading", { name: "Temporary password" })).toBeVisible();
  await page.getByRole("button", { name: "Done" }).click();
  await expect(page.getByRole("row", { name: new RegExp(displayName) })).toBeVisible();
}

async function changeRole(page: Page, displayName: string, roleName: "Teacher" | "Assistant"): Promise<void> {
  const row = page.getByRole("row", { name: new RegExp(displayName) });
  await row.getByRole("button", { name: "Open actions menu" }).click();
  await page.getByRole("menuitem", { name: "Change role" }).click();
  await expect(page.getByRole("heading", { name: `Change role for ${displayName}` })).toBeVisible();
  await page.getByRole("combobox", { name: "Role" }).click();
  await page.getByRole("option", { name: roleName }).click();
  await page.getByRole("button", { name: "Save" }).click();
  await expect(page.getByText("Role changed.")).toBeVisible();
}

test("owner: changing a member's role appears in the audit log with translated before/after values", async ({
  page,
}) => {
  // Its own staff member, never a seeded account: other journeys sign in as the seeded secretary in parallel, and
  // a role borrowed from them for this test would change what they see (later.md, 2026-10-23).
  const suffix = Date.now().toString();
  const email = `e2e-role-change-${suffix}@nile.test`;
  const displayName = `E2E Role Change ${suffix}`;

  await signIn(page, "owner@nile.test");
  await page.goto("/staff");
  await addStaffMember(page, email, displayName);
  await expect(page.getByRole("row", { name: new RegExp(displayName) }).getByText("Teacher")).toBeVisible();

  await changeRole(page, displayName, "Assistant");
  await expect(page.getByRole("row", { name: new RegExp(displayName) }).getByText("Assistant")).toBeVisible();

  await page.goto("/audit");
  await page.getByLabel("Entity type").click();
  await page.getByRole("option", { name: "Staff membership" }).click();

  // Other journeys (first-login.spec.ts, full-centre-setup.spec.ts) and earlier runs of this one also write
  // membership entries, so this test's entry is not necessarily the newest. Its direction (Teacher -> Assistant)
  // plus "newest first" picks it out: no other journey changes a role that way.
  const thisChange = page.getByText("Role:Teacher→Assistant").first();
  await expect(thisChange).toBeVisible();
  const thisEntry = thisChange.locator("xpath=ancestor::div[contains(@class,'rounded-xl')][1]");
  await expect(thisEntry.getByText("Nile Owner")).toBeVisible();
  await expect(thisEntry.getByText("Updated")).toBeVisible();
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
