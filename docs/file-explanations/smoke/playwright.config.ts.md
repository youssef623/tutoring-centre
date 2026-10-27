# `smoke/playwright.config.ts`

## Purpose

Source comment (intent): Task 35.8: a smoke test asks one question — is this revision fit to serve? — against a real, already-running instance. Unlike frontend/e2e, this project starts no local server and takes its target entirely from the environment, so it can run unchanged against the local production stack, a deployed instance, or (one day) the public URL.

## Where it fits

Category: **configuration**. [Open current file](../../../smoke/playwright.config.ts). [Complete file index](../INDEX.md). [Current teaching overview](../../PROJECT_OVERVIEW3.md).

The reviewed file has 31 lines and 1,266 bytes. Source hash: `ae2fbb9879523480c6048e944c1bf156b1ded53ce7beec1f71076a964fa4d118`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
 * Task 35.8: a smoke test asks one question — is this revision fit to serve? — against a real, already-running
 * instance. Unlike frontend/e2e, this project starts no local server and takes its target entirely from the
 * environment, so it can run unchanged against the local production stack, a deployed instance, or (one day)
 * the public URL.
 */
const baseURL = process.env.SMOKE_BASE_URL;
if (baseURL === undefined || baseURL === "") {
  throw new Error("SMOKE_BASE_URL must be set to the target instance's base URL (see README).");
}

export default defineConfig({
  testDir: "./tests",
  fullyParallel: false,
  retries: 0,
  timeout: 30_000,
  reporter: "list",
  use: {
    baseURL,
    // Signs in with a real (if throwaway) demo password: no trace, screenshot or video is ever recorded, so
    // none can retain it (Task 35.8's own constraint — read directly from `process.env`, never logged).
    trace: "off",
    screenshot: "off",
    video: "off",
    // The local production stack (Day 34) and early deployments use a self-signed or not-yet-trusted certificate.
    ignoreHTTPSErrors: true,
  },
  projects: [{ name: "chromium", use: { ...devices["Desktop Chrome"] } }],
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
