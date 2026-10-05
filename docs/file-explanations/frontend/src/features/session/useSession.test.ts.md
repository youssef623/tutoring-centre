# frontend/src/features/session/useSession.test.ts

## Purpose

Tests that `useSession` resolves to `null` on 401, to the profile on success, and reports a real error on a 500.

## Where It Fits

frontend/src/features/session. Uses `renderHookWithQueryClient` and MSW overrides.

## Walkthrough

Test 1 (9-21): default MSW handler returns 401 for `/api/me`; after loading, `me` is `null` and `isError` is false. Test 2 (23-55): override returns a `MeDto`-shaped JSON (Nile Owner, role owner, one membership); `me.displayName` is `Nile Owner`, `activeRole` `owner`. Test 3 (57-76): a 500 Problem Details response gives `isError === true` and `me === null`. 3 tests.

## Concepts Used

### Mocking the network with MSW

#### What it means

Mock Service Worker intercepts `fetch` at the network layer, so components and hooks run unchanged while the test decides what the 'server' answers.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Default 401 handler.

#### How it works here

Test 1 relies on `handlers.ts`.

#### Why it matters here

Shows the default 'signed out' behaviour of the test server.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/features/session/useSession.ts`](useSession.ts.md)
- [`frontend/src/test/render.tsx`](../../test/render.tsx.md)
- [`frontend/src/test/msw/handlers.ts`](../../test/msw/handlers.ts.md)
