# frontend/src/test/renderRouter.tsx

## Purpose

Test helper that renders the app's real route tree in memory with a fresh query client wired like production, including the session-expiry redirect.

## Where It Fits

frontend/src/test. Used by `_authenticated.test.tsx`, `login.test.tsx`, `select-centre.test.tsx`. Imports `createAppQueryClient`, `setLoginRedirect` and the generated `routeTree`.

## Walkthrough

`renderRouter(initialPath)` (12-28): `queryClient = createAppQueryClient()`; `history = createMemoryHistory({ initialEntries: [initialPath] })`; `router = createRouter({ routeTree, history, context: { queryClient } })`; `setLoginRedirect(target => void router.navigate({ to: "/login", search: { redirect: target } }))` (so a global 401 in a test navigates through *this* test's router, not an older one); renders `QueryClientProvider` + `RouterProvider`; returns the render utilities plus `router` and `queryClient`.

## Concepts Used

### File-based routing with TanStack Router

#### What it means

A router maps URLs to components. In file-based routing a Vite plugin scans `src/routes/` and generates a route tree file, so adding a file adds a route with type-safe links.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Memory history.

#### How it works here

Lines 14-15.

#### Why it matters here

Tests drive navigation without a browser URL.

### Frontend route guards, session state and global 401 handling

#### What it means

A *route guard* decides, before a page renders, whether the user may see it (here by reading the session query and redirecting). It only shapes the UI; the API remains the security boundary. A *global 401 handler* reacts to any request that finds the session gone.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling](../../../../PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling).)

#### Where it appears in this file

Real wiring in tests.

#### How it works here

Lines 13, 17-19.

#### Why it matters here

Uses the same factory as production so the global 401 behaviour is tested, not re-implemented.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/app/queryClient.ts`](../app/queryClient.ts.md)
- [`frontend/src/main.tsx`](../main.tsx.md)
- [`frontend/src/routes/_authenticated.test.tsx`](../routes/_authenticated.test.tsx.md)
