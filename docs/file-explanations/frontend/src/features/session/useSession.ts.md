# frontend/src/features/session/useSession.ts

## Purpose

Hook returning the signed-in session: `me` (profile or `null`), loading and error flags.

## Where It Fits

frontend/src/features/session. Used by `AuthenticatedLayout`, the dashboard page and the centre picker.

## Walkthrough

`useSession()` (5-14): `useQuery(meQueryOptions)` and returns `{ me: data ?? null, isLoading: isPending, isError, error }`. `me` is `MeDto` (userId, displayName, email, preferredLocale, activeCentreId, activeRole, memberships) or `null`.

## Concepts Used

### Server state with TanStack Query

#### What it means

Server state (data owned by the API) needs caching, refetching, retry and loading/error flags. `useQuery` subscribes a component to a cache entry addressed by a *query key*, runs the `queryFn`, and re-renders the component when the entry changes.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Wrapping a query in a domain hook.

#### How it works here

Lines 5-14.

#### Why it matters here

Callers see a small, stable API instead of the whole query result.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/features/session/meQueryOptions.ts`](meQueryOptions.ts.md)
- [`frontend/src/features/session/useSession.test.ts`](useSession.test.ts.md)
- [`frontend/src/routes/_authenticated.tsx`](../../routes/_authenticated.tsx.md)
