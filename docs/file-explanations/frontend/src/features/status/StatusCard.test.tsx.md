# frontend/src/features/status/StatusCard.test.tsx

## Purpose
Component tests for `StatusCard` with the network mocked by MSW:
- 200 shows the healthy state;
- 503 shows the database-unavailable state with Retry, and *not* "Cannot reach the API";
- Retry after a 503 refetches and shows healthy.

## Where it fits
Co-located feature test. It uses `src/test/msw/server.ts` (`server.use` overrides), `src/test/render.tsx` (a fresh `QueryClient` with `retry: false`) and the global setup in `src/test/setup.ts`.

## Walkthrough
- **Lines 8–10:** `readinessUrl`, plus `healthy()` and `unavailable()` response factories with plain-text bodies.
- **Lines 13–22:** override to 200 → `findByText("API and database are reachable")`.
- **Lines 24–35:** override to 503 → find the amber title and the Retry button; assert the unreachable text is absent. The comment: "503 is an answer from the API, not a network failure."
- **Lines 37–46:**
  - start with 503 and find Retry;
  - switch the handler to 200 with `server.use`;
  - `fireEvent.click(retryButton)`;
  - the healthy text appears.

## Concepts used
- **MSW request interception** at the network layer. Real `fetch` runs and MSW intercepts it in Node.
- **Testing Library queries:** `findBy*` waits for async UI; `getByRole` is the accessibility-first query.
- **Per-test handler overrides.**

## Data and control flow
Test → render → hook → `fetch` → MSW handler → UI → assertions.

## Configuration and environment
None.

## Gotchas and issues
- **The "Cannot reach the API" branch isn't tested.** That would need `HttpResponse.error()` or a non-200/503 status.
- **The loading state isn't asserted.**
- **`fireEvent` vs `userEvent`.** It uses `fireEvent.click`; `@testing-library/user-event` isn't installed.

## Related files
- [StatusCard.tsx](StatusCard.tsx.md)
- [test/msw/server.ts](../../test/msw/server.ts.md)
- [test/render.tsx](../../test/render.tsx.md)
- [test/setup.ts](../../test/setup.ts.md)
