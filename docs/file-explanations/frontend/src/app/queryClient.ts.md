# frontend/src/app/queryClient.ts

## Purpose
The app-wide TanStack Query `QueryClient` with shared defaults: **one retry** (hides a blip without delaying a real failure long) and **30 s `staleTime`** (remounting a screen doesn't refetch immediately).

## Where it fits
App shell (`src/app/`). Provided by `main.tsx:25` through `QueryClientProvider`. Tests don't use it; they create their own client with `retry: false` in `src/test/render.tsx`.

## Walkthrough
- **Lines 4–11:** `new QueryClient({ defaultOptions: { queries: { retry: 1, staleTime: 30_000 } } })`.

## Concepts used
- **Server-state cache:** queries are cached by key.
- **Stale-while-revalidate:** fresh data is served without refetching, stale data is refetched.
- **Module singleton.**

## Data and control flow
Hooks → `QueryClient` cache → `queryFn` when stale or missing.

## Configuration and environment
None.

## Gotchas and issues
- **Polling overrides staleness:** `useReadiness` polls every 15 s regardless of the 30 s `staleTime`.

## Related files
- [main.tsx](../main.tsx.md)
- [useReadiness.ts](../features/status/useReadiness.ts.md)
- [test/render.tsx](../test/render.tsx.md)
