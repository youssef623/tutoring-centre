# frontend/src/test/render.tsx

## Purpose

Test helper that renders a component inside a fresh `QueryClientProvider`.

## Where It Fits

frontend/src/test. Used by `StatusCard.test.tsx`.

## Walkthrough

`renderWithQueryClient(ui)`: `new QueryClient({ defaultOptions: { queries: { retry: false } } })` then `render(<QueryClientProvider client={...}>{ui}</QueryClientProvider>)`. Fresh client per call = no shared cache between tests; `retry: false` so failures surface immediately.

## Concepts Used

### Server state with TanStack Query

#### What it means

Server state (data owned by the API) needs caching, refetching, retry and loading/error flags. `useQuery` subscribes a component to a cache entry addressed by a *query key*, runs the `queryFn`, and re-renders the component when the entry changes.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Isolated client.

#### How it works here

Function body.

#### Why it matters here

Deterministic tests.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/features/status/StatusCard.test.tsx`](../features/status/StatusCard.test.tsx.md)
- [`frontend/src/app/queryClient.ts`](../app/queryClient.ts.md)
