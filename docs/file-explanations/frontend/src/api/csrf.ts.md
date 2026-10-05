# frontend/src/api/csrf.ts

## Purpose

In-memory cache of the antiforgery token, fetched through the generated client and refreshed on demand.

## Where It Fits

frontend/src/api. Used by `apiFetch.ts` (attach/refresh), by `routes/login.tsx` and `routes/select-centre.tsx` (refresh after the session changes) and by `features/session/useSignOut.ts`. Depends on the generated `getAntiforgeryToken`.

## Walkthrough

`cachedToken: Promise<string> | null` (5) - a module-level variable. The comment states why it is memory only: never `localStorage`, `sessionStorage` or a script-written cookie, so later-injected script cannot read a token that was never stored anywhere readable. `fetchToken()` (7-16) calls `getAntiforgeryToken()` and returns `response.token`; on error it sets `cachedToken = null` (a failure is not cached) and rethrows. `getCsrfToken()` (19-22): `cachedToken ??= fetchToken()` - fetch once, then reuse the same promise. `refreshCsrfToken()` (25-28): replaces the cache with a new fetch (the token is bound to the signed-in user).

## Concepts Used

### CSRF protection with antiforgery tokens

#### What it means

Because browsers attach cookies automatically, a malicious *other* site can make the victim's browser send a state-changing request. CSRF protection adds a second, explicit proof that the request came from the application's own script: a token that must be sent in a header, which a foreign site cannot read or set.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens](../../../../PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens).)

#### Where it appears in this file

Client half of double submit.

#### How it works here

Whole file.

#### Why it matters here

The server sets the `__Host-tcm.xsrf` cookie when this fetch hits `/api/auth/antiforgery`; this file returns the matching token for the header.

### async/await and cancellation

#### What it means

`async`/`await` lets a method wait for I/O (database, network) without blocking a thread: the method returns a `Task`, and execution resumes after the awaited operation completes. A `CancellationToken` is a cooperative signal (for example, the HTTP request was aborted) passed down so work can stop early.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Caching a promise, not a value.

#### How it works here

Line 5, `??=` at line 20.

#### Why it matters here

Concurrent callers share one in-flight request instead of starting several.

### Secrets and configuration layering

#### What it means

Configuration comes from layered sources (JSON files, user-secrets, environment variables, command line) where later layers override earlier ones. Secrets must stay out of git: this repo keeps the connection string out of `appsettings.json`, reads it from user-secrets or an environment variable, and scans history with gitleaks.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#11-configuration--environment--deployment](../../../../PROJECT_OVERVIEW2.md#11-configuration--environment--deployment).)

#### Where it appears in this file

Where a secret may live in the browser.

#### How it works here

Comment lines 3-4.

#### Why it matters here

Keeping it out of storage limits what a script injected later can steal.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

`cachedToken` is module state, so tests that need exact call counts must reset modules (`apiFetch.test.ts` does).

## Related Files

- [`frontend/src/api/apiFetch.ts`](apiFetch.ts.md)
- `frontend/src/api/generated/tutoring-centre.ts` (generated / lockfile / media: no separate explanation, see INDEX)
- [`frontend/src/features/session/useSignOut.ts`](../features/session/useSignOut.ts.md)
- [`src/TutoringCentre.Api/Auth/AuthEndpoints.cs`](../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs.md)
