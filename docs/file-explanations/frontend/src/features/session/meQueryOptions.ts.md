# frontend/src/features/session/meQueryOptions.ts

## Purpose

TanStack Query options for `GET /api/me` that treat 'signed out' as data (`null`), not as an error.

## Where It Fits

frontend/src/features/session. Used by `useSession`, the route guards (`_authenticated.tsx`, `select-centre.tsx`), `login.tsx` (writes the result into the cache) and `select-centre.tsx` (invalidates it).

## Walkthrough

`meQueryOptions = queryOptions({ queryKey: getGetMeQueryKey(), queryFn, retry: false })` (10-24). `queryFn({ signal })` calls the generated `getMe({ signal })`; on error, `asApiError(error).status === 401` returns `null`, anything else is rethrown. The key comes from the generated client (`getGetMeQueryKey()`), so the login page and generated hooks share the same cache entry. `retry: false` because a 401 is a meaningful answer, not a transient failure. Return type `Promise<MeDto | null>`.

## Concepts Used

### Frontend route guards, session state and global 401 handling

#### What it means

A *route guard* decides, before a page renders, whether the user may see it (here by reading the session query and redirecting). It only shapes the UI; the API remains the security boundary. A *global 401 handler* reacts to any request that finds the session gone.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling](../../../../../PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling).)

#### Where it appears in this file

Session as a query.

#### How it works here

Whole file.

#### Why it matters here

A component or guard reading this query never has to treat 'no session' as an error; only real failures (network, 500) are errors.

### Server state with TanStack Query

#### What it means

Server state (data owned by the API) needs caching, refetching, retry and loading/error flags. `useQuery` subscribes a component to a cache entry addressed by a *query key*, runs the `queryFn`, and re-renders the component when the entry changes.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

`queryOptions` helper.

#### How it works here

Line 10.

#### Why it matters here

One object reused for `useQuery`, `queryClient.query(...)` in guards, `setQueryData` and `invalidateQueries`, so keys never drift.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/features/session/useSession.ts`](useSession.ts.md)
- [`frontend/src/routes/_authenticated.tsx`](../../routes/_authenticated.tsx.md)
- [`frontend/src/routes/login.tsx`](../../routes/login.tsx.md)
- `frontend/src/api/generated/tutoring-centre.ts` (generated / lockfile / media: no separate explanation, see INDEX)
- [`frontend/src/api/apiFetch.ts`](../../api/apiFetch.ts.md)
