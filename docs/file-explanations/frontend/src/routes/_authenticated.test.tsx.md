# frontend/src/routes/_authenticated.test.tsx

## Purpose

Component-level tests of the protected layout guard and of global 401 / logout handling, using the real route tree.

## Where It Fits

frontend/src/routes. Uses `renderRouter` (real `routeTree`, memory history) and MSW. `routeFileIgnorePattern` in `vite.config.ts` stops the router plugin from treating `*.test.tsx` as routes.

## Walkthrough

Helpers: `me(overrides)` builds a `MeDto`-shaped object for a two-centre teacher (default active centre `c-nile`). Guard tests: signed-out visitor at `/` ends at `/login` with search `{ redirect: "/" }`; a signed-in user with `activeCentreId: null` ends at `/select-centre`; with a centre set the shell shows the display name and centre name. Global 401 tests: with a signed-in session, a later `POST /api/session/centre` that returns 401 (opened through the user menu -> `Switch centre` -> picking `Maadi Learning Hub`) leads to `/login` and `queryClient.getQueryData(meQueryOptions.queryKey)` is `undefined` (cache cleared); logout (menu item `Log out`, 204) also ends at `/login` with the cache cleared. 5 tests.

## Concepts Used

### Frontend route guards, session state and global 401 handling

#### What it means

A *route guard* decides, before a page renders, whether the user may see it (here by reading the session query and redirecting). It only shapes the UI; the API remains the security boundary. A *global 401 handler* reacts to any request that finds the session gone.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling](../../../../PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling).)

#### Where it appears in this file

Guard and 401 handling tested together.

#### How it works here

All five tests.

#### Why it matters here

The tests drive the real router, so they prove the wiring between `beforeLoad`, `queryClient.ts` and `main.tsx`-style setup.

### Mocking the network with MSW

#### What it means

Mock Service Worker intercepts `fetch` at the network layer, so components and hooks run unchanged while the test decides what the 'server' answers.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Per-test server states.

#### How it works here

`server.use(...)` calls.

#### Why it matters here

Signed-in / signed-out / revoked states are simulated by changing what `/api/me` and `/api/session/centre` return.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/routes/_authenticated.tsx`](_authenticated.tsx.md)
- [`frontend/src/test/renderRouter.tsx`](../test/renderRouter.tsx.md)
- [`frontend/src/app/queryClient.ts`](../app/queryClient.ts.md)
- [`frontend/src/features/shell/AppShell.tsx`](../features/shell/AppShell.tsx.md)
