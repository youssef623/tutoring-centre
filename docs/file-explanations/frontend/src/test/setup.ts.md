# frontend/src/test/setup.ts

## Purpose
The global Vitest setup, run before every test file (`vite.config.ts:31`). It:
- adds the jest-dom matchers;
- starts MSW with **unhandled requests treated as errors**;
- cleans up the DOM and resets handlers after each test;
- closes MSW at the end.

## Where it fits
Test infrastructure. It uses `msw/server.ts` and affects all frontend tests.

## Walkthrough
- **Line 1:** `import "@testing-library/jest-dom/vitest"` adds `toBeInTheDocument()` and friends.
- **Lines 7–9:** `server.listen({ onUnhandledRequest: "error" })`. Any request without a handler fails the test instead of silently hitting the network.
- **Lines 11–14, `afterEach`:**
  - `cleanup()` unmounts rendered trees. It's needed because Vitest globals are off, so Testing Library can't auto-register cleanup.
  - `server.resetHandlers()` drops per-test overrides.
- **Lines 16–18:** `server.close()`.

## Concepts used
- **Test lifecycle hooks.**
- **Fail-closed network mocking.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
None found.

## Related files
- [msw/server.ts](msw/server.ts.md)
- [msw/handlers.ts](msw/handlers.ts.md)
- [vite.config.ts](../../vite.config.ts.md)
