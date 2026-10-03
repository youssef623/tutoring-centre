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

The first JSON endpoint, added to prove the conventions above end-to-end. Returns `applicationVersion`, `latestMigration` and `databaseUpToDate`. No authentication (Month 2 adds it); anonymous access is explicit via `.AllowAnonymous()`, not an oversight.
