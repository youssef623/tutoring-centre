# frontend/src/routes/select-centre.tsx

## Purpose

The `/select-centre` page: lists the user's active centres with their role, posts the choice, or shows an empty state with a log-out button.

## Where It Fits

frontend/src/routes. Guarded by its own `beforeLoad` (signed-out visitors go to `/login`) but, unlike `_authenticated`, it does not require an active centre. Reached after login for multi-centre users and from the user menu.

## Walkthrough

File header: ESLint disable for `only-throw-error` (router redirect idiom). `Route` (19-27): `beforeLoad` reads the session with `queryClient.query({ ...meQueryOptions, staleTime: "static" })` and redirects to `/login` when it is `null`. `SelectCentrePage` (29-103): `useSession`, `useSelectCentre()`, `useSignOut()`. `handleSelect(centreId)` (42-51): `selectCentreMutation.mutateAsync({ data: { centreId } })` (POST `/api/session/centre`, 204), `refreshCsrfToken()`, `queryClient.invalidateQueries({ queryKey: meQueryOptions.queryKey })` (so the next read has the new active centre), `navigate({ to: "/" })`; errors go to `showApiError`. Render: if `me.memberships.length === 0` an empty-state card with `selectCentre.empty` and a log-out button; else a heading and one button per membership showing the centre name (`dir="auto"`) and role `Badge` (`roles.<role>`).

## Concepts Used

### Frontend route guards, session state and global 401 handling

#### What it means

A *route guard* decides, before a page renders, whether the user may see it (here by reading the session query and redirecting). It only shapes the UI; the API remains the security boundary. A *global 401 handler* reacts to any request that finds the session gone.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling](../../../../PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling).)

#### Where it appears in this file

Own guard without the centre requirement.

#### How it works here

Lines 20-25.

#### Why it matters here

A signed-in user without an active centre must be able to reach this page, so it cannot live under `_authenticated`.

### Authorization by default, the actor pipeline and the tenant gate

#### What it means

A *fallback authorization policy* makes every endpoint require a signed-in user unless it explicitly opts out (fail closed). A single middleware translates the HTTP identity into the application's own `StaffActor`; the *tenant gate* is the one check that decides which centre a session may act in.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#629-authorization-by-default-the-actor-pipeline-and-the-tenant-gate](../../../../PROJECT_OVERVIEW2.md#629-authorization-by-default-the-actor-pipeline-and-the-tenant-gate).)

#### Where it appears in this file

Tenant selection is a server decision.

#### How it works here

Line 44.

#### Why it matters here

The page only *asks*; the server verifies the membership and answers 204 or 403 `tenant.no_membership`.

### CSRF protection with antiforgery tokens

#### What it means

Because browsers attach cookies automatically, a malicious *other* site can make the victim's browser send a state-changing request. CSRF protection adds a second, explicit proof that the request came from the application's own script: a token that must be sent in a header, which a foreign site cannot read or set.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens](../../../../PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens).)

#### Where it appears in this file

Token refresh after a session change.

#### How it works here

Line 45.

#### Why it matters here

The centre selection rewrites the session cookie and therefore the user-bound token.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/routes/select-centre.test.tsx`](select-centre.test.tsx.md)
- [`frontend/src/routes/_authenticated.tsx`](_authenticated.tsx.md)
- [`frontend/src/features/session/useSignOut.ts`](../features/session/useSignOut.ts.md)
- [`frontend/src/api/showApiError.ts`](../api/showApiError.ts.md)
- `frontend/src/api/generated/tutoring-centre.ts` (generated / lockfile / media: no separate explanation, see INDEX)
