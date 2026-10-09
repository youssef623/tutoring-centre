# Authentication

How staff sign in, how a session is carried and revalidated, and how the frontend defends every
state-changing request against CSRF. Written from the implementation in `src/TutoringCentre.Api/Auth/`,
`src/TutoringCentre.Application/Identity/` and `src/TutoringCentre.Infrastructure/Identity/` — every name,
code and number below is checked against that code, not against the plan that produced it.

## Login

`POST /api/auth/login` (`AuthEndpoints.LoginAsync`), anonymous, rate-limited, CSRF-protected.

1. The request body is validated (`LoginRequestValidator`); a validation failure returns 400 before any
   credential check runs.
2. Credentials are verified through a port, `IAuthenticationService.VerifyCredentialsAsync` — not a CQRS
   command, since signing in has no transaction boundary of its own. The one implementation,
   `IdentityAuthenticationService` (Infrastructure), calls ASP.NET Core Identity's `UserManager<ApplicationUser>`
   directly: it is the one intentional write path outside the dispatcher pipeline, because `UserManager` saves
   its own counters (failed-attempt count, lockout end, security stamp) as it goes.
   - An unknown email still pays a real password-hash verification, against a fixed dummy user, so response
     time carries no signal about whether the account exists.
   - A locked-out user, a wrong password, and an unknown email all return the identical failure:
     `auth.invalid_credentials`, 401.
3. On success, the request's actor is set to a `StaffActor` with no centre yet
   (`CurrentActorContext.Reauthenticate`, not `Set` — see "A request that already carries a session" below).
4. `GetMyMembershipsQuery` is dispatched (through the normal read pipeline, not the Identity write path) to
   read the user's active memberships.
5. If the user has exactly one active membership **and** their password change is not pending
   (`MeDto.MustChangePassword` is `false`), it is auto-selected; otherwise no centre is selected and the
   frontend sends the user to the centre picker (or, while a change is pending, must change the password
   first — see "Password change and the first-login gate" below).
6. The session cookie is issued (`SessionPrincipalFactory.Create` → `HttpContext.SignInAsync`) **after** the
   membership read, not before — a login that fails partway through never leaves a cookie behind.
7. The response is the same `MeDto` shape as `GET /api/me`, with `activeCentreId`/`activeRole` filled in from
   the auto-selected membership, if any.

### A request that already carries a session

Every authenticated request has its actor set once, by `ActorMiddleware`, before the endpoint runs
(`CurrentActorContext.Set`, which throws if called twice — a deliberate guard against two pieces of code
racing to set the actor). Login is the one endpoint allowed to override that: a request can arrive with a
valid session cookie (for example, two tabs, or a stale frontend state) and then authenticate as a different
identity, which must immediately supersede the old actor rather than collide with it.
`CurrentActorContext.Reauthenticate(StaffActor)` exists for exactly this; `Set` is otherwise unchanged and
still throws for every other caller.

## Centre selection

`POST /api/session/centre` (`AuthEndpoints.SelectCentreAsync`), authenticated, CSRF-protected. The only way a
centre ID enters a session.

1. `GetActiveMembershipQuery` is dispatched with the requested centre ID. It returns the identical failure —
   `tenant.no_membership`, 403 — whether the centre does not exist, the user has no membership in it, or the
   membership is inactive. `NotFound` would let a caller probe which centre IDs exist; this does not.
2. On failure, the existing cookie is left completely unchanged — nothing is written before the membership
   check succeeds.
3. On success, the cookie is re-issued with the **same** user ID and security stamp (carried over from the
   current session, never taken from the request) and the newly selected centre and role. 204, no body.

## Password change and the first-login gate

`POST /api/auth/change-password` (`AuthEndpoints.ChangePasswordAsync`), any signed-in user, no centre
required, CSRF-protected, rate-limited per user (`ChangePasswordRateLimiting`).

1. The request body is validated (`ChangePasswordRequestValidator`, shape only); a validation failure
   returns 400 before any credential check runs.
2. The change itself goes through the same kind of port as login, not a CQRS command, for the reason ADR
   0005 gives: `IAuthenticationService.ChangePasswordAsync` (extended for Day 30), implemented by
   `IdentityAuthenticationService`, which verifies the current password, rejects a new password identical to
   the current one, applies the password policy to the new one through `UserManager`, and — on success —
   rotates the security stamp and clears `ApplicationUser.MustChangePassword` together (one tracked entity,
   one save).
3. On success the session is re-signed-in with the new stamp and **no centre**, even if one was selected
   before — the same shape a fresh login without an auto-selected membership produces. Every other session
   for that user fails its next revalidation on the stamp mismatch (see "Revalidation and revocation" below).
4. Failures: wrong current password is `auth.current_password_invalid` (422, a business rule, not a 401 —
   the caller is already authenticated); a new password that fails the policy, or matches the current one,
   is `auth.password_too_weak` (400) with `errors.newPassword`.

**The first-login gate.** A new staff account (`StaffAccountService.EnsureAccountAsync`, Day 29) is created
with `MustChangePassword = true` and a random temporary password. The gate that forces its replacement
before the account can do anything tenant-scoped is built from two existing checks, not a new pipeline step:

- **At login** (step 5 above): the flag blocks auto-selecting a centre, even for a user with exactly one
  membership. The session is issued with no centre either way.
- **At centre selection**: `GetActiveMembershipHandler` — already "the only way a centre ID enters a
  session" — checks `IMembershipReadService.MustChangePasswordAsync` first and refuses with
  `auth.password_change_required` (403) before it ever looks at the membership. Every tenant-scoped request
  downstream therefore already fails with `tenant.not_selected`, because no centre was ever selected.

`GET /api/me` exposes the flag as `mustChangePassword`, read from the same profile query that already backs
the response, so the frontend can route a gated user straight to a forced-change screen (Day 33) without a
separate call.

## Every authenticated request

In pipeline order (`Program.cs`):

1. **`UseAuthentication`** — the cookie, if present, is decrypted and its claims parsed into a
   `ClaimsPrincipal`. `OnValidatePrincipal` (`SessionRevalidationHandler.ValidateAsync`) runs here —
   see "Revalidation" below. A principal it rejects is cleared before the request proceeds.
2. **`ActorMiddleware`** — the one and only place claims become an `StaffActor` (or the request stays
   anonymous). No other code reads session claims.
3. **`UseAuthorization`** — the fallback policy requires an authenticated user by default; an endpoint is
   anonymous only if it explicitly opts out with `.AllowAnonymous()`. A forgotten attribute protects by
   default instead of exposing.
4. **`UseRateLimiter`** — only two policies are attached to anything: `login` (the login endpoint) and
   `change-password` (the password-change endpoint, Day 30); every other route is unaffected.
5. **`AntiforgeryEndpointFilter`** — applied once to the whole `/api` route group as an endpoint filter (so
   it runs as part of endpoint execution, after routing/authorization/rate limiting, immediately before the
   handler): every request whose method is not GET, HEAD or OPTIONS must carry a valid antiforgery token,
   login included.

### Anonymous routes

Exactly these opt out of the authentication-by-default policy:

| Route | Why |
|---|---|
| `GET /health` | Liveness probe |
| `GET /health/ready` | Readiness probe |
| `GET /api/system/info` | Version/migration status, no session needed to read it |
| `GET /api/auth/antiforgery` | Needed before a session exists, to log in at all |
| `POST /api/auth/login` | The endpoint that creates a session |
| `GET /api/{**path}` fallback | An unmatched `/api/*` route answers the uniform 404, not a 401 — a signed-out caller probing a typo'd URL sees the same answer as anyone else |

A second, pattern-less fallback (`app.MapFallback(...)`, also anonymous) covers unmatched routes *outside*
`/api` for the same reason: without it, the authorization fallback policy would turn "no endpoint matched"
into 401 instead of preserving 404. It stands in for the SPA's own static file serving, which Month 2 adds.

## Logout

`POST /api/auth/logout` (`AuthEndpoints.LogoutAsync`), authenticated (no explicit
`.RequireAuthorization()` — the fallback policy already covers it), CSRF-protected.
`HttpContext.SignOutAsync` clears the cookie (an expired `Set-Cookie` for the same name); 204, no body.

## The cookie and its claims

Configured in `AuthenticationSetup.AddApiAuthentication`:

| Property | Value |
|---|---|
| Name | `__Host-tcm.session` |
| `HttpOnly` | `true` |
| `Secure` | Always (the `__Host-` prefix requires it; the browser rejects the cookie otherwise) |
| `SameSite` | `Lax` |
| `Path` | `/` (also required by `__Host-`) |
| Expiration | 8 hours, sliding |

Claims (`SessionClaimNames`), deliberately minimal — no email, no display name (those come from
`GET /api/me`), no permissions (Month 2 resolves them server-side on every request):

| Claim | Meaning |
|---|---|
| `sub` | User ID |
| `stamp` | Security stamp, for revocation (see below) |
| `centre` | Selected centre ID — present only once a centre is selected |
| `role` | Role in that centre — always present together with `centre`, never alone |

## CSRF

Double-submit pattern (`AntiforgerySetup`, `AntiforgeryEndpointFilter`):

- A second cookie, `__Host-tcm.xsrf` (`HttpOnly`, `Secure` always, `SameSite=Strict` — stricter than the
  session cookie, since this cookie is never needed on a top-level cross-site navigation, only on
  same-origin script requests).
- `GET /api/auth/antiforgery` issues the token pair and returns the request token in the JSON body (never in
  a script-readable cookie), with `Cache-Control: no-store`.
- The frontend sends it back as the `X-XSRF-TOKEN` header on every unsafe request.
- The filter is attached once, to the whole `/api` group — a new endpoint is covered automatically, with no
  per-endpoint attribute to remember.
- Login is covered too, deliberately: without it, an attacker could use a `<form>` on another site to sign a
  victim into the attacker's own account (login CSRF) — `SameSite` alone does not stop this, since the
  attacker's form submission is itself cross-site, not a replay of the victim's session.
- A missing or invalid token fails with no side effects — the handler never runs — and returns
  `auth.csrf_invalid`, 403.

## Revalidation and revocation

Cookies are stateless by default: once issued, nothing stops them from being presented after the membership
behind them has been revoked or the password has been changed. `SessionRevalidationHandler.ValidateAsync`
(wired as `OnValidatePrincipal`) closes that gap by re-checking the claims against the database on
(approximately) every request:

1. `ValidateStaffSessionQuery` is dispatched with the user ID, security stamp, centre ID **and role** the
   cookie claims (Day 29). It is valid only if the stamp still matches, (when a centre is set) that
   membership is still active, **and** the membership's current role still equals the role the cookie
   claims — a changed role is rejected exactly like a deactivated membership, never silently re-issued
   with the new one. A session with no centre selected carries no role claim either
   (`SessionPrincipalFactory`), so this check compares `null` to `null` and never fires for it.
2. A **valid** result is cached in memory, keyed by `(user, stamp, centre, role)`, for
   `SessionValidation:CacheDuration` — 60 seconds by default (`SessionValidationOptions.DefaultCacheDuration`),
   configurable, zero disables caching entirely (used by the test suite, so revocation and role-change tests
   see the effect on the very next request).
3. An **invalid** result is never cached — revocation is never masked by a stale cache hit — and the request
   is rejected and signed out immediately (`RejectPrincipal` plus `SignOutAsync`, so a rejected session does
   not keep presenting the same now-useless cookie).

In practice: a revoked membership, a changed role, or a changed security stamp ends the session within the
cache window — at most 60 seconds, and instantly for any request after the first one that triggers the
re-check (since that failure is never cached and signs the cookie out). The demoted or promoted user's next
request is a plain 401; the frontend's existing handling for that (Day 17) returns them to `/login` — nothing
about role changes needed a frontend change.

## Lockout and rate limiting

Two different defenses against two different attacks:

- **Lockout** (`Infrastructure`, ASP.NET Core Identity's own counters on `ApplicationUser`) protects **one
  account** against many guesses: 5 failed attempts locks it for 15 minutes
  (`Lockout.MaxFailedAccessAttempts = 5`, `Lockout.DefaultLockoutTimeSpan = 15 minutes`). A locked-out
  account returns the same `auth.invalid_credentials` as any other failure — enumeration-safe, same as
  everything else in this flow.
- **Rate limiting** (`LoginRateLimiting`, `Microsoft.AspNetCore.RateLimiting`) protects against **spraying
  many accounts from one source**: a fixed window of 10 requests per minute per client IP, attached only to
  the login endpoint. The 11th request in a window gets `auth.rate_limited`, 429, with a `Retry-After`
  header. Forwarded-header-aware client IPs (so this works correctly behind a real proxy) are Month 2; today
  it reads `HttpContext.Connection.RemoteIpAddress` directly.
  - The test suite varies the partition key via an `X-Test-RateLimit-Partition` header, honoured only when
    `IHostEnvironment.IsEnvironment("Testing")` — Production and Development never read it, so a real caller
    cannot use this header to dodge the limit.
- **Password-change rate limiting** (`ChangePasswordRateLimiting`, Day 30) protects the same account against
  repeated guesses at its *current* password, once already signed in: 5 attempts per minute, keyed by user
  ID rather than IP — unlike login, this request always carries a session, so who is acting is the natural
  key, not where from. Both rate limiters share one `OnRejected` handler (`RateLimitRejection`) — the
  options type's `OnRejected` is a single delegate, not a per-policy list, so a second registration would
  otherwise silently overwrite the first.

## Error codes

Every code this flow can produce, and the status it comes with:

| Code | Status | Where |
|---|---|---|
| `auth.invalid_credentials` | 401 | Unknown email, wrong password, or a locked-out account — identical for all three |
| `auth.not_authenticated` | 401 | Defensive: a query that needs a `StaffActor` finds none. In normal operation the authorization fallback policy already returns 401 before a handler is reached, so this is a safety net, not the common path |
| `auth.csrf_invalid` | 403 | Missing or invalid antiforgery token on an unsafe `/api` request |
| `auth.rate_limited` | 429 | 11th+ login attempt from one IP, or 6th+ password-change attempt from one user, within the current one-minute window |
| `tenant.no_membership` | 403 | `POST /api/session/centre` for a centre the user has no active membership in (or that does not exist) |
| `auth.password_change_required` | 403 | `POST /api/session/centre` while a password change is still pending |
| `auth.current_password_invalid` | 422 | `POST /api/auth/change-password` with the wrong current password |
| `auth.password_too_weak` | 400 | `POST /api/auth/change-password` with a new password that fails the policy, or matches the current one |
| `route.not_found` | 404 | Any unmatched route, `/api` or not, anonymous |

## What Month 2 adds

Deliberately out of scope today, called out at the point in the code where each one will land:

- **Permissions.** Roles are stored (`StaffRole` on `Membership`) and carried in the session (`role` claim).
  What each role may do with it is now enforced — see `docs/architecture/authorization.md`.
- **Persisted Data Protection keys.** `AddDataProtection()` uses the framework's local key ring today
  (`AuthenticationSetup`, "the framework default" — fine for one Development process, but a restart or a
  second instance cannot decrypt cookies the first one issued). Month 2 persists the key ring.
- **Forwarded headers.** `LoginRateLimiting` reads the direct connection's remote IP; behind a real reverse
  proxy that is the proxy's own address, not the client's. Forwarded-header-aware client IPs are Month 2.
