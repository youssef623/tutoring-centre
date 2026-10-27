# `frontend/playwright.config.ts`

## Purpose

Source comment (intent): Deviation from the Day 18 plan's literal text, reported per its own "report the difference" rule (Task 18.6): the plan calls for the API's plain-http launch profile. That breaks antiforgery — `AntiforgeryOptions.Cookie.SecurePolicy = Always` makes `GetAndStoreTokens`/`ValidateRequestAsync` hard-check `HttpContext.Request.IsHttps` on the request *the backend itself receives*, independent of "localhost is a trustworthy origin" (that exception covers the browser accepting a Secure cookie from http://localhost:5173, not whether Kestrel saw TLS). This is the same reason Day 16 moved dev to HTTPS rather than weakening that policy. E2E therefore uses the same "https" profile as normal development; the readiness probe below targets that profile's plain-http sibling binding purely to dodge the self-signed certificate during the Node-side health check (the API itself is still reached over HTTPS).

## Where it fits

Category: **configuration**. [Open current file](../../../frontend/playwright.config.ts). [Complete file index](../INDEX.md). [Current teaching overview](../../PROJECT_OVERVIEW3.md).

The reviewed file has 44 lines and 1,850 bytes. Source hash: `aa46941503cc9abd908af953d42b8b8f8959f3a06371a042e14f5a8ccf096e89`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { defineConfig, devices } from "@playwright/test";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { defineConfig, devices } from "@playwright/test";

/**
 * Deviation from the Day 18 plan's literal text, reported per its own "report the difference" rule
 * (Task 18.6): the plan calls for the API's plain-http launch profile. That breaks antiforgery —
 * `AntiforgeryOptions.Cookie.SecurePolicy = Always` makes `GetAndStoreTokens`/`ValidateRequestAsync`
 * hard-check `HttpContext.Request.IsHttps` on the request *the backend itself receives*, independent of
 * "localhost is a trustworthy origin" (that exception covers the browser accepting a Secure cookie from
 * http://localhost:5173, not whether Kestrel saw TLS). This is the same reason Day 16 moved dev to HTTPS
 * rather than weakening that policy. E2E therefore uses the same "https" profile as normal development;
 * the readiness probe below targets that profile's plain-http sibling binding purely to dodge the
 * self-signed certificate during the Node-side health check (the API itself is still reached over HTTPS).
 */
const apiHttpsTarget = "https://localhost:7197";
const apiReadinessUrl = "http://localhost:5245/health/ready";

export default defineConfig({
  testDir: "./e2e",
  fullyParallel: true,
  retries: process.env.CI ? 1 : 0,
  reporter: "html",
  use: {
    baseURL: "http://localhost:5173",
    trace: "on-first-retry",
  },
  projects: [{ name: "chromium", use: { ...devices["Desktop Chrome"] } }],
  webServer: [
    {
      command: "dotnet run --project ../src/TutoringCentre.Api --launch-profile https",
      url: apiReadinessUrl,
      reuseExistingServer: !process.env.CI,
      timeout: 120_000,
      stdout: "ignore",
    },
    {
      command: "npm run dev",
      url: "http://localhost:5173",
      reuseExistingServer: !process.env.CI,
      timeout: 60_000,
      env: { API_TARGET: apiHttpsTarget },
      stdout: "ignore",
    },
  ],
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
