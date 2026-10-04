# ADR 0005: Encrypted cookie authentication over JWT

- **Status:** Accepted
- **Date:** 2026-10-05

## Context

The frontend is a same-origin SPA served from the same host as the API (via the Vite dev proxy today, a
single deployable in production) — there is no third-party client, no mobile app and no cross-origin caller
to support. Users are staff only (owners, teachers, secretaries), not the public, and a user can belong to
several centres at once, so a session has to carry not just "who" but "acting in which centre" — and that
second part changes mid-session, through its own explicit action, without a fresh login.

## Decision

An encrypted, `HttpOnly`, `__Host-`-prefixed cookie (`__Host-tcm.session`) holds the session: user ID,
security stamp, and the selected centre and role once one is chosen. The browser never reads or stores the
token itself — `HttpOnly` makes it inaccessible to JavaScript, closing off the main XSS exfiltration path a
token held in `localStorage` or a JS-readable cookie would leave open.

Centre selection is server-side and server-validated: `POST /api/session/centre` re-issues the cookie only
after confirming an active membership (`tenant.no_membership` otherwise, identical whether the centre
doesn't exist, isn't the caller's, or is inactive) — the client never asserts which centre it is acting in on
a later request, only which one it asked to switch into.

Because a stateless cookie cannot be revoked by itself, every request re-validates the claims against the
database (`SessionRevalidationHandler`, cached 60 seconds on success, never on failure) — a changed security
stamp or a revoked membership ends the session within that window, not only at next login.

Because cookies are sent automatically by the browser, every state-changing request needs a second, explicit
signal that the request was actually initiated by the app's own script: a double-submit antiforgery token
(`X-XSRF-TOKEN`), enforced on the whole `/api` group including login, which otherwise would be open to login
CSRF.

Login is a port (`IAuthenticationService`), not a CQRS command: signing in writes through ASP.NET Core
Identity's own `UserManager` (failed-attempt counters, lockout, security stamp) as it verifies credentials,
and that write has no use-case transaction boundary of its own to fit the dispatcher's begin/commit/rollback
pipeline around.

## Alternatives considered

1. **JWT in `localStorage`.** Readable by any script on the page, so one XSS vulnerability anywhere in the
   SPA's dependency tree becomes full session theft; also requires the client to manage attaching the token,
   refreshing it, and clearing it correctly everywhere it is used. Rejected for a same-origin app that gains
   nothing from a token model and inherits its weakest point.
2. **JWT in a cookie.** Keeps the cookie's automatic transport but still needs CSRF protection (a cookie,
   JWT or not, is sent automatically) and adds JWT's own problem on top: a self-contained token cannot be
   revoked before it expires without a server-side blocklist, which is just session state again, built twice.
3. **ASP.NET Core Identity's full `SignInManager` + Identity UI.** Built for server-rendered Razor Pages with
   redirect-based sign-in; this is a JSON API behind an SPA, and minimal APIs do not get automatic antiforgery
   validation the way Identity's own form-bound pages do, so the explicit filter in this decision would be
   needed regardless. `UserManager` alone — the credential and lockout half of Identity, without the sign-in
   and UI half — is the right-sized slice.

## Consequences

- CSRF protection is mandatory, not optional, and must cover every unsafe `/api` request including login —
  a single endpoint missing the filter would reopen the gap the cookie's automatic transport creates.
- Revocation is not instant: a revoked membership or changed stamp takes effect within the 60-second cache
  window, not on the next request after the database write. Acceptable for staff account changes, which are
  not time-critical in the way, say, payment authorization would be.
- The client stays simple and has little to get wrong: no token storage, no manual attach-to-every-request
  logic, no refresh flow. The browser's own cookie jar does the carrying; the frontend's job is limited to
  fetching and resending the CSRF header.
- Same-origin only: this design assumes the SPA and the API share an origin (or one proxies to the other, as
  in development). A genuinely cross-origin client would need a different approach — `SameSite` cookies do
  not cross origins by design.
