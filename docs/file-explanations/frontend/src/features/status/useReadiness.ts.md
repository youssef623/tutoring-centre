# frontend/src/features/status/useReadiness.ts

## Purpose
The TanStack Query hook for the status page. It caches readiness under the key `["health", "ready"]` and **polls every 15 seconds** (line 4).

## Where it fits
Feature layer. It wraps `fetchReadiness` (`api.ts`) and is consumed by `StatusCard.tsx`. Inherits `retry: 1` and `staleTime: 30_000` from `app/queryClient.ts` (or `retry: false` in tests).

## Walkthrough
- **Lines 5–11:** `useQuery({ queryKey: ["health","ready"], queryFn: fetchReadiness, refetchInterval: 15_000 })`.

## Concepts used
- **Custom hooks.**
- **Query keys:** hierarchical cache identifiers.
- **Interval polling** (`refetchInterval`).

## Data and control flow
Component → hook → cache → `fetchReadiness` every 15 s → `{ isPending, isError, data, refetch }`.

## Configuration and environment
None.

## Gotchas and issues
- **Polling pauses in background tabs.** That's TanStack's default (`refetchIntervalInBackground: false`), and it's library behaviour, not set here.

## Related files
- [api.ts](api.ts.md)
- [StatusCard.tsx](StatusCard.tsx.md)
- [queryClient.ts](../../app/queryClient.ts.md)
