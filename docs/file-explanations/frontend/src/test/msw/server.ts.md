# frontend/src/test/msw/server.ts

## Purpose
Creates the single MSW **Node** server instance (`setupServer(...handlers)`) shared by all tests.

## Where it fits
Test infrastructure. Started, reset and stopped by `src/test/setup.ts`. Overridden per test via `server.use` in `StatusCard.test.tsx`.

## Walkthrough
- **Line 4:** `export const server = setupServer(...handlers);`

## Concepts used
- **MSW in Node:** it intercepts `fetch` at the request-module level, so no browser service worker is needed.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
None found.

## Related files
- [handlers.ts](handlers.ts.md)
- [setup.ts](../setup.ts.md)
