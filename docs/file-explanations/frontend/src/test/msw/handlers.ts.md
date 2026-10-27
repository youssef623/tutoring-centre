# `frontend/src/test/msw/handlers.ts`

## Purpose

Source comment (intent): Default network behaviour for tests: a healthy API. Tests override per case with server.use(...).

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/test/msw/handlers.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 24 lines and 950 bytes. Source hash: `d845f20390268fc6ea3cd342c3eab0374be8c9300d62ac784f25ed0ebfdaa056`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { http, HttpResponse } from "msw";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../../frontend/src/test/msw/handlers.ts#L4): `export const handlers = [`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/reconcile_backend_ledger.py:17](../../../../../onboarding-work/reconcile_backend_ledger.py#L17)
- [frontend/src/test/msw/server.ts:2](../../../../../../frontend/src/test/msw/server.ts#L2)
- [tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs:22](../../../../../../tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs#L22)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { http, HttpResponse } from "msw";

/** Default network behaviour for tests: a healthy API. Tests override per case with server.use(...). */
export const handlers = [
  http.get("/health/ready", () => new HttpResponse("Healthy", { status: 200 })),
  http.get("/api/system/info", () =>
    HttpResponse.json({
      applicationVersion: "1.0.0",
      latestMigration: "20261012_InitialPlatform",
      databaseUpToDate: true,
      demoMode: false,
    }),
  ),
  // No session by default; tests that need a signed-in user override this with server.use(...).
  http.get("/api/me", () =>
    HttpResponse.json(
      { title: "Unauthorized", status: 401 },
      { status: 401 },
    ),
  ),
  // Every non-GET request fetches this first (apiFetch's CSRF handling); a fixed token keeps that invisible
  // to tests that aren't about CSRF themselves.
  http.get("/api/auth/antiforgery", () => HttpResponse.json({ token: "test-csrf-token" })),
];
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
