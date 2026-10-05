# src/TutoringCentre.Api/Auth/AntiforgerySetup.cs

## Purpose

Configures the ASP.NET Core antiforgery system: header name and the hardened `__Host-tcm.xsrf` cookie.

## Where It Fits

Api/Auth. `AddApiAntiforgery` is called at `Program.cs:48`; `HeaderName` is also used by tests (`AntiforgeryTestHelper`) and mirrored by the frontend (`apiFetch.ts`: `CsrfHeaderName`).

## Walkthrough

`HeaderName = "X-XSRF-TOKEN"` (9). `AddAntiforgery(options => ...)` (15-26): `HeaderName`; cookie `Name = "__Host-tcm.xsrf"`, `HttpOnly = true`, `SecurePolicy = Always`, `SameSite = Strict`, `Path = "/"`. Comment: `__Host-` forces Secure, Path `/` and no Domain; `Strict` rather than `Lax` (unlike the session cookie) because this cookie is never needed on a top-level cross-site navigation, only on same-origin script requests.

## Concepts Used

### CSRF protection with antiforgery tokens

#### What it means

Because browsers attach cookies automatically, a malicious *other* site can make the victim's browser send a state-changing request. CSRF protection adds a second, explicit proof that the request came from the application's own script: a token that must be sent in a header, which a foreign site cannot read or set.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens](../../../../PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens).)

#### Where it appears in this file

Antiforgery configuration.

#### How it works here

Lines 15-26.

#### Why it matters here

The cookie half of the token pair is never script-readable (`HttpOnly`).

### Cookie authentication and server-side sessions

#### What it means

After a successful login the server must remember *who* the browser is on later requests (HTTP itself is stateless). Cookie authentication does that with one cookie that the browser attaches automatically. Here the cookie holds an **encrypted, tamper-proof ticket** (the user's claims); only the server can read or create it, and flags such as `HttpOnly`, `Secure`, `SameSite` and the `__Host-` name prefix limit where and how browsers send it.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#624-cookie-authentication-and-server-side-sessions](../../../../PROJECT_OVERVIEW2.md#624-cookie-authentication-and-server-side-sessions).)

#### Where it appears in this file

Second cookie, stricter flags.

#### How it works here

`SameSite = Strict`.

#### Why it matters here

Defence in depth next to the session cookie.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Cookie `__Host-tcm.xsrf`; header `X-XSRF-TOKEN`.

## Gotchas and Issues

Because `SecurePolicy = Always`, antiforgery hard-checks that the request the backend receives is HTTPS (see `frontend/playwright.config.ts` comment) - this is why local development runs the API on its https profile.

## Related Files

- [`src/TutoringCentre.Api/Auth/AntiforgeryEndpointFilter.cs`](AntiforgeryEndpointFilter.cs.md)
- [`src/TutoringCentre.Api/Auth/AntiforgeryTokenResponse.cs`](AntiforgeryTokenResponse.cs.md)
- [`frontend/src/api/apiFetch.ts`](../../../frontend/src/api/apiFetch.ts.md)
- [`frontend/src/api/csrf.ts`](../../../frontend/src/api/csrf.ts.md)
