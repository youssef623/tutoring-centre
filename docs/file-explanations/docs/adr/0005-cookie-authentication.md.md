# docs/adr/0005-cookie-authentication.md

## Purpose

Architecture Decision Record: why sessions are an encrypted `HttpOnly` `__Host-` cookie with antiforgery tokens and per-request revalidation, rather than JWT.

## Where It Fits

Documentation (docs/adr). Explains the design implemented in `src/TutoringCentre.Api/Auth/*`; detailed behaviour is in `docs/architecture/authentication.md`. Linked from the root `README.md` and `docs/architecture/overview.md`.

## Walkthrough

Status Accepted, 2026-10-05. **Context:** a same-origin SPA (Vite proxy today, a single deployable later); no third-party or mobile client; staff-only users; one user can belong to several centres, so a session carries 'who' and 'acting in which centre', and the centre changes mid-session. **Decision:** (1) an encrypted `HttpOnly` cookie `__Host-tcm.session` holds user id, security stamp and, once chosen, centre and role - JavaScript can never read it; (2) centre selection is server-side and validated (`POST /api/session/centre` re-issues the cookie only after confirming an active membership; `tenant.no_membership` is identical for unknown centre, non-member and inactive member); (3) because a cookie cannot be revoked by itself, every request revalidates against the database (`SessionRevalidationHandler`, cached 60 s on success, never on failure); (4) a double-submit antiforgery token (`X-XSRF-TOKEN`) on the whole `/api` group including login (login CSRF); (5) login is a port (`IAuthenticationService`) not a CQRS command because `UserManager` writes its own counters and has no use-case transaction. **Alternatives:** JWT in `localStorage` (readable by any script), JWT in a cookie (still needs CSRF and cannot be revoked without a blocklist), full `SignInManager` + Identity UI (built for Razor Pages; `UserManager` alone is the right-sized slice). **Consequences:** CSRF protection is mandatory on every unsafe `/api` request; revocation takes effect within the 60 s window; the client stays simple (no token storage); same-origin only.

## Concepts Used

### Cookie authentication and server-side sessions

#### What it means

After a successful login the server must remember *who* the browser is on later requests (HTTP itself is stateless). Cookie authentication does that with one cookie that the browser attaches automatically. Here the cookie holds an **encrypted, tamper-proof ticket** (the user's claims); only the server can read or create it, and flags such as `HttpOnly`, `Secure`, `SameSite` and the `__Host-` name prefix limit where and how browsers send it.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#624-cookie-authentication-and-server-side-sessions](../../../PROJECT_OVERVIEW2.md#624-cookie-authentication-and-server-side-sessions).)

#### Where it appears in this file

The decision recorded.

#### How it works here

Decision paragraphs 1-3.

#### Why it matters here

Every claim here is checked against code in `AuthenticationSetup.cs`, `SessionRevalidationHandler.cs` and `AuthEndpoints.cs` in the overview's authentication section.

### CSRF protection with antiforgery tokens

#### What it means

Because browsers attach cookies automatically, a malicious *other* site can make the victim's browser send a state-changing request. CSRF protection adds a second, explicit proof that the request came from the application's own script: a token that must be sent in a header, which a foreign site cannot read or set.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens](../../../PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens).)

#### Where it appears in this file

Why the filter covers the whole group.

#### How it works here

Decision paragraph 4.

#### Why it matters here

A cookie is sent automatically by the browser, so state-changing requests need a second proof.

### Session revalidation and revocation

#### What it means

A self-contained cookie cannot be cancelled by itself. Revalidation re-checks the cookie's claims against the database on requests (here via a *security stamp* and the active membership), so a changed password or revoked access ends the session; a short cache bounds the database cost.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#627-session-revalidation-and-revocation](../../../PROJECT_OVERVIEW2.md#627-session-revalidation-and-revocation).)

#### Where it appears in this file

Revocation window.

#### How it works here

Decision paragraph 3 and Consequences.

#### Why it matters here

Honest about the trade-off: revocation is not instant.

### Architecture Decision Records and documentation as code

#### What it means

An ADR records a decision, its context, alternatives and consequences, so the *why* survives the people who made it. Docs kept in the repo are versioned with the code.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

ADR format.

#### How it works here

Whole file.

#### Why it matters here

Context, decision, alternatives and consequences in plain prose.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The ADR says 'a single deployable in production'; no code in the repository serves the SPA from the API yet (the fallback in `Program.cs` is described as a placeholder for Month 2 static file serving).

## Related Files

- [`docs/architecture/authentication.md`](../architecture/authentication.md.md)
- [`src/TutoringCentre.Api/Auth/AuthenticationSetup.cs`](../../src/TutoringCentre.Api/Auth/AuthenticationSetup.cs.md)
- [`src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs`](../../src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs.md)
- [`src/TutoringCentre.Api/Auth/AuthEndpoints.cs`](../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs.md)
- [`src/TutoringCentre.Api/Auth/AntiforgerySetup.cs`](../../src/TutoringCentre.Api/Auth/AntiforgerySetup.cs.md)
