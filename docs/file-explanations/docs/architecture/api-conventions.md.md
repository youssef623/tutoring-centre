# docs/architecture/api-conventions.md

## Purpose

Authoritative description of the HTTP conventions: error shape, Result-to-status mapping, exception handling, strict JSON, correlation IDs, log redaction, routing and the first endpoint.

## Where It Fits

Documentation for `src/TutoringCentre.Api/Http/*`, `Logging/*`, `Auth/*`, `Program.cs` and `Endpoints/PlatformEndpoints.cs`. I verified each rule against those files and `tests/TutoringCentre.Api.Tests/Http/*`.

## Walkthrough

Sections: *Error shape* (RFC 9457 written only by `ProblemResult`; extensions `traceId`, `correlationId`; `errors` for validation); *Result -> HTTP mapping* table (Validation 400, NotFound 404, Conflict 409, Rule 422, Forbidden 403, Unauthenticated 401; non-generic success -> 204); *Exceptions* (`GlobalExceptionHandler`: `BadHttpRequestException` -> 400 `request.malformed` logged Warning, else 500 `server.unexpected` logged Error; no message/type/stack in body); *Strict JSON* (`UnmappedMemberHandling.Disallow`, camelCase enum converter, `ThrowOnBadRequest`); *Correlation IDs* (first middleware, 1-64 chars of letters/digits/`.`/`_`/`-`, Serilog `LogContext`, `OnStarting`); *Log redaction*; *Routing* (`/api` group, `route.not_found` fallback); *`GET /api/system/info`* (anonymous explicitly via `.AllowAnonymous()`); new section *Authentication and authorization*: the rule that the authorization fallback policy requires an authenticated user (a new endpoint is protected the moment it is mapped, forgetting an attribute fails closed with 401), a table of exactly six explicitly anonymous routes (`GET /health`, `GET /health/ready`, `GET /api/system/info`, `GET /api/auth/antiforgery`, `POST /api/auth/login`, the `/api/{**path}` fallback), the explanation of the second pattern-less anonymous fallback (the authorization fallback policy would otherwise turn 'no endpoint matched' into 401), the note that `/api/auth/logout`, `/api/me` and `/api/session/centre` rely on the fallback policy instead of repeating `.RequireAuthorization()`, and that the Development-only `/openapi/v1.json` is `.AllowAnonymous()` too.

## Concepts Used

### Problem Details (RFC 9457) and centralised error handling

#### What it means

Problem Details is a standard JSON error shape (`title`, `status`, `detail`, extensions) served as `application/problem+json`. Centralising error writing in one place keeps every error response uniform and prevents leaking internals.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling](../../../PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling).)

#### Where it appears in this file

Whole document.

#### How it works here

Specification that the Api tests pin.

#### Why it matters here

Read before adding an endpoint.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Minor: says correlationId is `null` only if the middleware did not run; true per `ProblemResult.Enrich`.

## Related Files

- [`src/TutoringCentre.Api/Http/ProblemResult.cs`](../../src/TutoringCentre.Api/Http/ProblemResult.cs.md)
- [`src/TutoringCentre.Api/Http/ResultHttpExtensions.cs`](../../src/TutoringCentre.Api/Http/ResultHttpExtensions.cs.md)
- [`src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs`](../../src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs.md)
- [`src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs`](../../src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs.md)
- [`src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs`](../../src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs.md)
- [`src/TutoringCentre.Api/Program.cs`](../../src/TutoringCentre.Api/Program.cs.md)
- [`docs/architecture/authentication.md`](authentication.md.md)
- [`src/TutoringCentre.Api/Auth/AuthenticationSetup.cs`](../../src/TutoringCentre.Api/Auth/AuthenticationSetup.cs.md)
