# frontend/src/app/queryClient.ts

## Purpose

Creates the single shared TanStack Query client with project-wide defaults.

## Where It Fits

frontend/src/app. Imported by `main.tsx` (provider). Tests do not use it (they build fresh clients).

## Walkthrough

`export const queryClient = new QueryClient({ defaultOptions: { queries: { retry: 1, staleTime: 30_000 } } })`. `retry: 1` - one retry hides a blip without delaying a real failure (comment). `staleTime: 30_000` - data stays fresh for 30 s so remounting does not refetch immediately (comment). Individual hooks can override (the readiness hook adds `refetchInterval`).

## Concepts Used

### Server state with TanStack Query

#### What it means

Server state (data owned by the API) needs caching, refetching, retry and loading/error flags. `useQuery` subscribes a component to a cache entry addressed by a *query key*, runs the `queryFn`, and re-renders the component when the entry changes.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Client defaults.

#### How it works here

Whole file.

#### Why it matters here

Caching and retry policy in one place.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Because `refetchInterval` forces refetches regardless of `staleTime`, the 30 s default does not slow the 15 s polling.

## Related Files

- [`frontend/src/main.tsx`](../main.tsx.md)
- [`frontend/src/features/status/useReadiness.ts`](../features/status/useReadiness.ts.md)
- [`frontend/src/test/render.tsx`](../test/render.tsx.md)
