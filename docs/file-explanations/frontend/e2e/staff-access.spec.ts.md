# `frontend/e2e/staff-access.spec.ts`

## Purpose

Declares `signIn`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../frontend/e2e/staff-access.spec.ts). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 44 lines and 2,032 bytes. Source hash: `cced064afc99e9d83ae41df66a51830af06e0c39ae44a2a136072c62c673a3a2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { type Page, expect, test } from "@playwright/test";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../frontend/e2e/staff-access.spec.ts#L10): `async function signIn(page: Page, email: string): Promise<void> {`

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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
