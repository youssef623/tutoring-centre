# docs/architecture/authentication.md

## Purpose

Authoritative description of the authentication flow: login, centre selection, per-request pipeline, anonymous routes, logout, cookie and claims, CSRF, revalidation, lockout and rate limiting, error codes, and what is deferred.

## Where It Fits

Documentation (docs/architecture). Written from `src/TutoringCentre.Api/Auth/`, `src/TutoringCentre.Application/Identity/` and `src/TutoringCentre.Infrastructure/Identity/` (its own words: every name, code and number was checked against that code). Linked from the root `README.md`, `docs/architecture/overview.md` and ADR 0005.

## Walkthrough

Sections: **Login** (`POST /api/auth/login`: validate body, verify through `IAuthenticationService`, unknown email still pays a real hash verification against a dummy user, identical `auth.invalid_credentials` 401 for unknown / wrong / locked, actor set with `CurrentActorContext.Reauthenticate`, `GetMyMembershipsQuery`, auto-select a single active membership, cookie issued *after* the membership read, response is a `MeDto`); **A request that already carries a session** (why `Reauthenticate` exists); **Centre selection** (`GetActiveMembershipQuery`, identical `tenant.no_membership` 403, cookie unchanged on failure, re-issued with the same user id and stamp on success, 204); **Every authenticated request** (order: `UseAuthentication` with `OnValidatePrincipal`, `ActorMiddleware`, `UseAuthorization`, `UseRateLimiter`, `AntiforgeryEndpointFilter`); **Anonymous routes** table (six routes plus a pattern-less fallback); **Logout** (`SignOutAsync`, 204); **The cookie and its claims** (name, `HttpOnly`, `Secure` Always, `SameSite` Lax, `Path /`, 8 h sliding; claims `sub`, `stamp`, `centre`, `role`); **CSRF** (`__Host-tcm.xsrf` `SameSite=Strict`, token returned in the JSON body with `no-store`, `X-XSRF-TOKEN`, filter on the whole `/api` group, login covered); **Revalidation and revocation** (cached valid results 60 s keyed by user, stamp, centre; invalid never cached and signs out); **Lockout and rate limiting** (5 failures / 15 min; fixed window 10 per minute per IP; the `Testing`-only partition header); **Error codes** table; **What Month 2 adds** (permissions, persisted Data Protection keys, forwarded headers).

## Concepts Used

### Cookie authentication and server-side sessions

#### What it means

After a successful login the server must remember *who* the browser is on later requests (HTTP itself is stateless). Cookie authentication does that with one cookie that the browser attaches automatically. Here the cookie holds an **encrypted, tamper-proof ticket** (the user's claims); only the server can read or create it, and flags such as `HttpOnly`, `Secure`, `SameSite` and the `__Host-` name prefix limit where and how browsers send it.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#624-cookie-authentication-and-server-side-sessions](../../../PROJECT_OVERVIEW2.md#624-cookie-authentication-and-server-side-sessions).)

#### Where it appears in this file

The whole document.

#### How it works here

Cookie and claims sections.

#### Why it matters here

The overview's authentication section is checked against the same code, and where the two agree this document is the shorter statement.

### CSRF protection with antiforgery tokens

#### What it means

Because browsers attach cookies automatically, a malicious *other* site can make the victim's browser send a state-changing request. CSRF protection adds a second, explicit proof that the request came from the application's own script: a token that must be sent in a header, which a foreign site cannot read or set.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens](../../../PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens).)

#### Where it appears in this file

CSRF section.

#### How it works here

Double-submit description.

#### Why it matters here

Matches `AntiforgerySetup` and `AntiforgeryEndpointFilter`.

### Rate limiting, lockout and enumeration resistance

#### What it means

*Rate limiting* caps how many requests one source may make in a time window (stops password spraying from one place). *Lockout* temporarily blocks one account after repeated failures (stops many guesses against one account). *Enumeration resistance* means failure responses (and their timing) reveal nothing about whether an account exists.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#626-rate-limiting-lockout-and-enumeration-resistance](../../../PROJECT_OVERVIEW2.md#626-rate-limiting-lockout-and-enumeration-resistance).)

#### Where it appears in this file

Lockout and rate limiting section.

#### How it works here

Numbers: 5 attempts, 15 minutes, 10 per minute.

#### Why it matters here

The same numbers appear in `DependencyInjection.cs` (Identity options) and `LoginRateLimiting.cs`.

### Session revalidation and revocation

#### What it means

A self-contained cookie cannot be cancelled by itself. Revalidation re-checks the cookie's claims against the database on requests (here via a *security stamp* and the active membership), so a changed password or revoked access ends the session; a short cache bounds the database cost.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#627-session-revalidation-and-revocation](../../../PROJECT_OVERVIEW2.md#627-session-revalidation-and-revocation).)

#### Where it appears in this file

Revalidation section.

#### How it works here

60-second cache, never cache failure.

#### Why it matters here

Matches `SessionValidationOptions` and `SessionRevalidationHandler`.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The document says forwarded-header-aware client IPs and persisted Data Protection keys are future work; both are still missing in code (`LoginRateLimiting` reads `Connection.RemoteIpAddress`; `AddDataProtection()` uses the local key ring).

## Related Files

- [`docs/adr/0005-cookie-authentication.md`](../adr/0005-cookie-authentication.md.md)
- [`src/TutoringCentre.Api/Auth/AuthEndpoints.cs`](../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs.md)
- [`src/TutoringCentre.Api/Auth/AuthenticationSetup.cs`](../../src/TutoringCentre.Api/Auth/AuthenticationSetup.cs.md)
- [`src/TutoringCentre.Api/Auth/AntiforgerySetup.cs`](../../src/TutoringCentre.Api/Auth/AntiforgerySetup.cs.md)
- [`src/TutoringCentre.Api/Auth/LoginRateLimiting.cs`](../../src/TutoringCentre.Api/Auth/LoginRateLimiting.cs.md)
- [`src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs`](../../src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs.md)
- [`src/TutoringCentre.Api/Program.cs`](../../src/TutoringCentre.Api/Program.cs.md)
