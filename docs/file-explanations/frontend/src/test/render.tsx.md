# frontend/src/test/render.tsx

## Purpose

Test helpers that render a component or a hook inside a fresh `QueryClientProvider`.

## Where It Fits

frontend/src/test. Used by `StatusCard.test.tsx`, `SystemInfoCard.test.tsx` and `useSession.test.ts`. Route-level tests use `renderRouter.tsx` instead.

## Walkthrough

`newTestQueryClient()` (5-9): `new QueryClient({ defaultOptions: { queries: { retry: false } } })`. `renderWithQueryClient(ui)` (12-16): renders inside a provider with a fresh client. `renderHookWithQueryClient(hook)` (19-28): same for hooks via `renderHook` with a `wrapper` component, returning `RenderHookResult`. Fresh client per call = no shared cache between tests; `retry: false` so failures surface immediately.

## Concepts Used

### Server state with TanStack Query

#### What it means

Server state (data owned by the API) needs caching, refetching, retry and loading/error flags. `useQuery` subscribes a component to a cache entry addressed by a *query key*, runs the `queryFn`, and re-renders the component when the entry changes.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Fresh client per test.

#### How it works here

Lines 5-9.

#### Why it matters here

Tests never share cached data and a failed request is reported at once instead of after a retry.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/test/renderRouter.tsx`](renderRouter.tsx.md)
- [`frontend/src/features/status/StatusCard.test.tsx`](../features/status/StatusCard.test.tsx.md)
- [`frontend/src/features/session/useSession.test.ts`](../features/session/useSession.test.ts.md)
- [`frontend/src/app/queryClient.ts`](../app/queryClient.ts.md)
