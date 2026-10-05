# frontend/src/features/session/useSignOut.ts

## Purpose

Hook returning a function that logs out, refreshes the CSRF token, clears all cached queries and navigates to `/login`.

## Where It Fits

frontend/src/features/session. Used by the user menu in `AppShell` and the empty state of `routes/select-centre.tsx`.

## Walkthrough

`useSignOut()` (7-18): `useLogout()` (generated mutation hook), `useQueryClient()`, `useNavigate()`. Returned async function: `await logoutMutation.mutateAsync()` (POST `/api/auth/logout`); `await refreshCsrfToken()` (the token was bound to the now signed-out user); `queryClient.clear()` (drop every cached query so no data of the old user remains in memory); `await navigate({ to: "/login" })`.

## Concepts Used

### Frontend route guards, session state and global 401 handling

#### What it means

A *route guard* decides, before a page renders, whether the user may see it (here by reading the session query and redirecting). It only shapes the UI; the API remains the security boundary. A *global 401 handler* reacts to any request that finds the session gone.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling](../../../../../PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling).)

#### Where it appears in this file

Logout flow on the client.

#### How it works here

Lines 12-17.

#### Why it matters here

Order matters: server logout first, then token refresh, then clearing caches, then navigation - so a stale cache is never shown to the next user.

### CSRF protection with antiforgery tokens

#### What it means

Because browsers attach cookies automatically, a malicious *other* site can make the victim's browser send a state-changing request. CSRF protection adds a second, explicit proof that the request came from the application's own script: a token that must be sent in a header, which a foreign site cannot read or set.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens](../../../../../PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens).)

#### Where it appears in this file

Token refresh after logout.

#### How it works here

Line 14.

#### Why it matters here

The CSRF token is tied to the signed-in user; a leftover token would be rejected.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/api/csrf.ts`](../../api/csrf.ts.md)
- `frontend/src/api/generated/tutoring-centre.ts` (generated / lockfile / media: no separate explanation, see INDEX)
- [`frontend/src/features/shell/AppShell.tsx`](../shell/AppShell.tsx.md)
- [`frontend/src/routes/select-centre.tsx`](../../routes/select-centre.tsx.md)
