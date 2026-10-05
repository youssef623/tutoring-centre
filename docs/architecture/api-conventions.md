# API conventions

## Error shape

**Rule.** Every error response is RFC 9457 Problem Details, `application/problem+json`, written by exactly one type: `ProblemResult` (`src/TutoringCentre.Api/Http/ProblemResult.cs`). No other code writes an error body.

Every error carries two extensions beyond the RFC fields:

| Extension | Source | Purpose |
| --- | --- | --- |
| `traceId` | `Activity.Current?.Id ?? HttpContext.TraceIdentifier` | Correlate with distributed tracing. |
| `correlationId` | `HttpContext.Items["CorrelationId"]`, set by `CorrelationIdMiddleware` | Correlate with the client's own logs; `null` only if the middleware did not run. |

Validation failures also carry `errors`: a field name (camelCase) → message list, built from `Error.Fields`.

## Result → HTTP mapping

**Rule.** `ResultHttpExtensions` (`src/TutoringCentre.Api/Http/ResultHttpExtensions.cs`) is the single place a domain `Result`/`Error` becomes an HTTP response. Endpoints call `.ToHttpResult(...)` and never branch on `ErrorKind` themselves.

| `ErrorKind` | HTTP status | Example |
| --- | --- | --- |
| `Validation` | 400 | bad input shape |
| `NotFound` | 404 | resource doesn't exist |
| `Conflict` | 409 | duplicate slug |
| `Rule` | 422 | business rule says no |
| `Forbidden` | 403 | caller not allowed |
| `Unauthenticated` | 401 | caller's identity is unknown (Day 14) |

A successful `Result` (non-generic) maps to `204 No Content`; a successful `Result<T>` is handed to the caller-supplied `onSuccess` function (usually `Results.Ok(value)`).

## Exceptions

**Rule.** Exceptions never reach the client as-is. `GlobalExceptionHandler` (`src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs`) is the one place that logs an exception and turns it into Problem Details.

| Exception | Status | `code` | Logged at |
| --- | --- | --- | --- |
| `BadHttpRequestException` (malformed JSON, unknown field, bad route value) | 400 | `request.malformed` | Warning (no exception object) |
| anything else | 500 | `server.unexpected` | Error (with the exception object) |

The 500 body never contains the exception's message, type or stack trace — only the log does. This is why the dispatcher rolls back and rethrows on handler exceptions instead of swallowing them: the handler here is the single place that decides what the client sees.

## Strict JSON

`ConfigureHttpJsonOptions` sets `UnmappedMemberHandling.Disallow` (an unknown field in a request body is a 400, not silently ignored) and a camelCase `JsonStringEnumConverter`. `RouteHandlerOptions.ThrowOnBadRequest = true` makes a malformed body throw `BadHttpRequestException` in every environment, so it always reaches `GlobalExceptionHandler` and comes back as `request.malformed` rather than an empty, unexplained 400.

## Correlation IDs

`CorrelationIdMiddleware` runs first in the pipeline. A client-supplied `X-Correlation-Id` header is accepted only if it is 1–64 characters of letters, digits, `.`, `_` or `-`; otherwise a new one is generated. Either way, the value is: pushed onto the Serilog `LogContext` (so every log line for the request carries it), stored on `HttpContext.Items` (so `ProblemResult` can put it in error bodies), and written back on the response header in `OnStarting` (so it survives a response that `GlobalExceptionHandler` clears and rewrites).

## Log redaction

`SensitiveDataDestructuringPolicy` is a safety net: when an object is destructured with `{@Object}`, any property named like `Password`, `Token`, `Secret`, `ConnectionString` or `Phone*` is replaced with `***` before it reaches the sink. It does not replace the primary rule from Day 7 — never log request/command objects in the first place — only limits the damage if that rule is broken later.

## Routing

All product HTTP endpoints live under the `/api` group (`app.MapGroup("/api")`). An unmatched `/api/*` route returns Problem Details 404 with `code: "route.not_found"` via `app.MapFallback`, instead of the framework's empty body. Routes outside `/api` (health checks today, the SPA shell from Month 2) are unaffected.

## `GET /api/system/info`

The first JSON endpoint, added to prove the conventions above end-to-end. Returns `applicationVersion`, `latestMigration` and `databaseUpToDate`. Anonymous access is explicit via `.AllowAnonymous()` (see Authentication and authorization below), not an oversight.

## Authentication and authorization (Day 16)

**Rule.** The authorization fallback policy requires an authenticated user. A new endpoint is protected the moment it is mapped, with no attribute needed — forgetting one fails closed (401) instead of silently exposing the endpoint.

Exactly six routes are explicitly anonymous, each with its own `.AllowAnonymous()` (or equivalent) call, never by omission:

| Route | Why |
| --- | --- |
| `GET /health` | Liveness probe; no caller is signed in yet when checking the process is up. |
| `GET /health/ready` | Readiness probe; same reason. |
| `GET /api/system/info` | Public version/migration status, proven anonymous since Day 11. |
| `GET /api/auth/antiforgery` | Bootstraps the CSRF token pair before any session exists. |
| `POST /api/auth/login` | The one way to start a session; can't require being already signed in. |
| `/api/{**path}` fallback | The uniform 404 for an unknown route must not leak whether a route exists behind a login wall. |

A second, pattern-less `app.MapFallback(...)` is also anonymous, for a subtler reason: ASP.NET Core's authorization fallback policy applies not only to mapped endpoints without `.AllowAnonymous()`, but also to requests that match *no* endpoint at all — without a catch-all, an unknown route outside `/api` (today just `/does-not-exist`-style probes; from Month 2, the SPA shell's own routes) would 401 instead of 404. This fallback stays anonymous because the SPA shell it will eventually serve is itself public; auth-gating happens client-side within the app, same as any other static asset.

Every other endpoint — `/api/auth/logout`, `/api/me`, `/api/session/centre`, and anything added later — relies on the fallback policy rather than its own `.RequireAuthorization()` call. That redundant call was removed where it previously existed: once the default is "authenticated," repeating it per endpoint reads as if it might mean something stricter than the default, when it does not. The Development-only `/openapi/v1.json` document (`app.MapOpenApi()`) is also marked `.AllowAnonymous()` so it stays reachable for local tooling; it never exists outside Development.
