# `smoke/tests/smoke.spec.ts`

## Purpose

Source comment (intent): Task 35.8. Read-mostly and small on purpose: this proves a deployed revision is fit to serve, not that every feature works. It creates no data, so it is safe to run on every deploy. The deployment workflow (Day 35's `deploy.yml`, not yet wired against real Azure in this slice — no subscription provisioned) runs it as the smoke step and rolls back on failure.

## Where it fits

Category: **test**. [Open current file](../../../../smoke/tests/smoke.spec.ts). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 88 lines and 4,158 bytes. Source hash: `cf49f7cf495bc4cee87c607d91d1a38784058e0672e30d3d32afa2278ef0cfe0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { type Page, expect, test } from "@playwright/test";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 82](../../../../smoke/tests/smoke.spec.ts#L82): `async function signIn(page: Page, email: string): Promise<void> {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { type Page, expect, test } from "@playwright/test";

/**
 * Task 35.8. Read-mostly and small on purpose: this proves a deployed revision is fit to serve, not that every
 * feature works. It creates no data, so it is safe to run on every deploy. The deployment workflow (Day 35's
 * `deploy.yml`, not yet wired against real Azure in this slice — no subscription provisioned) runs it as the
 * smoke step and rolls back on failure.
 */

const expectedSha: string = process.env.SMOKE_EXPECTED_SHA ?? "";
if (expectedSha === "") {
  throw new Error("SMOKE_EXPECTED_SHA must be set to the commit SHA the target instance is expected to run (see README).");
}

// Never logged, never written to a trace, screenshot or artefact (the config turns all three off): read once
// here and passed straight into the one form field that needs it.
const password: string = process.env.SMOKE_PASSWORD ?? "";
if (password === "") {
  throw new Error("SMOKE_PASSWORD must be set to the demo Nile owner's password (see README).");
}

// ASP.NET Core's HstsMiddleware excludes "localhost", "127.0.0.1" and "[::1]" from the header by default — a
// built-in loopback exemption, not a bug in this app. The local production stack's Caddyfile site address is
// literally "localhost:8443" (Task 34.8), so the header is correctly absent there and correctly present on any
// real hostname. Default true (a deployed instance must carry it); the local-stack run sets this to "false".
const expectHsts = process.env.SMOKE_EXPECT_HSTS !== "false";

test.describe("deployed smoke test", () => {
  test("/health/ready is 200", async ({ request }) => {
    const response = await request.get("/health/ready");
    expect(response.status()).toBe(200);
  });

  test("/ returns HTML with the security headers", async ({ request }) => {
    const response = await request.get("/");
    const headers = response.headers();

    expect(headers["content-security-policy"]).toBeTruthy();
    expect(headers["x-content-type-options"]).toBe("nosniff");
    if (expectHsts) {
      // Present at all, not a specific max-age: Task 36.1 raises it post-deployment, and this test must not
      // pin a value that change would then break.
      expect(headers["strict-transport-security"]).toMatch(/^max-age=\d+/);
    } else {
      expect(headers["strict-transport-security"]).toBeUndefined();
    }

    const body = await response.text();
    expect(body.trimStart().toLowerCase().startsWith("<!doctype html")).toBe(true);
  });

  test("/api/system/info reports the deployed commit and demo mode", async ({ request }) => {
    const response = await request.get("/api/system/info");
    expect(response.ok()).toBe(true);

    const info = (await response.json()) as { applicationVersion: string; demoMode: boolean };
    // The SDK's own "+<SourceRevisionId>" suffix (Task 34.2): this is how a deployed instance proves which
    // commit it is actually running, independent of whatever version number the project file carries.
    expect(info.applicationVersion.endsWith(`+${expectedSha}`)).toBe(true);
    expect(info.demoMode).toBe(true);
  });

  test("unauthenticated GET /api/subjects is 401", async ({ request }) => {
    const response = await request.get("/api/subjects");
    expect(response.status()).toBe(401);
  });

  test("owner: sign in, see Subjects, sign out", async ({ page }) => {
    await signIn(page, "owner@nile.test");

    await expect(page.getByRole("heading", { name: /^Welcome,/ })).toBeVisible();

    await page.goto("/subjects");
    await expect(page.getByRole("row").first()).toBeVisible();

    await page.getByRole("button", { name: "Nile Owner" }).click();
    await page.getByRole("menuitem", { name: "Log out" }).click();
    await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();
  });
});

async function signIn(page: Page, email: string): Promise<void> {
  await page.goto("/login");
  await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();
  await page.getByLabel("Email").fill(email);
  await page.getByLabel("Password").fill(password);
  await page.getByRole("button", { name: "Sign in" }).click();
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
