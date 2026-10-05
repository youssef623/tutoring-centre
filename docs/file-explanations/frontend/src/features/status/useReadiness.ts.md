# frontend/src/features/status/useReadiness.ts

## Purpose

TanStack Query hook providing the API readiness state, cached and polled.

## Where It Fits

frontend/src/features/status. Used by `StatusCard`; calls `fetchReadiness`.

## Walkthrough

`useQuery({ queryKey: ["health","ready"], queryFn: fetchReadiness, refetchInterval: 15_000 })`. The query key is the cache address; the `refetchInterval` makes the library refetch every 15 s while the component is mounted. Retry (1) and `staleTime` (30 s) come from the shared client defaults. The hook returns the query result object (`isPending`, `isError`, `data`, `refetch`).

## Concepts Used

### Server state with TanStack Query

#### What it means

Server state (data owned by the API) needs caching, refetching, retry and loading/error flags. `useQuery` subscribes a component to a cache entry addressed by a *query key*, runs the `queryFn`, and re-renders the component when the entry changes.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

`useQuery`.

#### How it works here

The hook body.

#### Why it matters here

Loading/error/caching without hand-written state.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Polling continues while the tab is open; TanStack's default refetch-on-window-focus also applies (library default, not configured here).

## Related Files

- [`frontend/src/features/status/api.ts`](api.ts.md)
- [`frontend/src/features/status/StatusCard.tsx`](StatusCard.tsx.md)
- [`frontend/src/app/queryClient.ts`](../../app/queryClient.ts.md)
