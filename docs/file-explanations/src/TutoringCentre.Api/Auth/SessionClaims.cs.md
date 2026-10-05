# src/TutoringCentre.Api/Auth/SessionClaims.cs

## Purpose

Defines the four session claim names and the factory that builds the principal stored in the encrypted cookie.

## Where It Fits

Api/Auth, `internal`. Used by `AuthEndpoints` (create), `ActorMiddleware` and `SessionRevalidationHandler` (read).

## Walkthrough

`SessionClaimNames` (12-18): `UserId = "sub"`, `SecurityStamp = "stamp"`, `CentreId = "centre"`, `Role = "role"`. Doc: minimal by design - no email or display name (from `GET /api/me`), no permissions (Month 2 resolves them server-side).
`SessionPrincipalFactory.Create(userId, securityStamp, centreId?, role?)` (23-40): always adds `sub` and `stamp`; adds `centre` and `role` **only together** (comment: a role with no selected centre is meaningless); returns `new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme))`.

## Concepts Used

### Cookie authentication and server-side sessions

#### What it means

After a successful login the server must remember *who* the browser is on later requests (HTTP itself is stateless). Cookie authentication does that with one cookie that the browser attaches automatically. Here the cookie holds an **encrypted, tamper-proof ticket** (the user's claims); only the server can read or create it, and flags such as `HttpOnly`, `Secure`, `SameSite` and the `__Host-` name prefix limit where and how browsers send it.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#624-cookie-authentication-and-server-side-sessions](../../../../PROJECT_OVERVIEW2.md#624-cookie-authentication-and-server-side-sessions).)

#### Where it appears in this file

Claim set in the ticket.

#### How it works here

Whole file.

#### Why it matters here

Fewer claims = less to leak and less to go stale.

### Session revalidation and revocation

#### What it means

A self-contained cookie cannot be cancelled by itself. Revalidation re-checks the cookie's claims against the database on requests (here via a *security stamp* and the active membership), so a changed password or revoked access ends the session; a short cache bounds the database cost.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#627-session-revalidation-and-revocation](../../../../PROJECT_OVERVIEW2.md#627-session-revalidation-and-revocation).)

#### Where it appears in this file

`stamp` claim.

#### How it works here

Line 15, 28.

#### Why it matters here

Compared against the database on every request to revoke sessions.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The claim `role` is stored as the enum name (`Owner`, `Teacher`, `Secretary`) and parsed back with `Enum.TryParse`, whereas the database stores lowercase text.

## Related Files

- [`src/TutoringCentre.Api/Auth/ActorMiddleware.cs`](ActorMiddleware.cs.md)
- [`src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs`](SessionRevalidationHandler.cs.md)
- [`src/TutoringCentre.Api/Auth/AuthEndpoints.cs`](AuthEndpoints.cs.md)
