# tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs

## Purpose

Tests the HTTP error conventions end to end through the real pipeline.

## Where It Fits

Api.Tests/Http; collection `conventions`. Uses test endpoints from `ConventionEndpoints`.

## Walkthrough

Helper `ReadProblemAsync(response)` asserts content type `application/problem+json` and parses JSON. Tests (Act = HTTP call; Assert = status + `code`):
- `/api/test/{ok,validation,notfound,conflict,rule,forbidden}` -> 200, 400/`test.invalid`, 404, 409, 422, 403.
- `throws` -> 500, code `server.unexpected`, body does not contain `hunter2` or `InvalidOperationException`.
- malformed JSON body and unknown JSON field -> 400 `request.malformed`.
- empty name -> 400 problem+json (validator).  valid name -> 200 `"Hello Sara"`.
- unknown `/api/does-not-exist` GET and POST -> 404 `route.not_found`; unknown non-API path -> 404 problem+json.
- `[Theory]` three paths -> `traceId` non-empty.
- request header `test-corr-1` -> `correlationId` equals it; error content type; validation includes `errors.name[0] == "Name is required."`; non-validation errors have no `errors` property.
19 test methods in total.

## Concepts Used

### Problem Details (RFC 9457) and centralised error handling

#### What it means

Problem Details is a standard JSON error shape (`title`, `status`, `detail`, extensions) served as `application/problem+json`. Centralising error writing in one place keeps every error response uniform and prevents leaking internals.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling](../../../../PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling).)

#### Where it appears in this file

The whole contract.

#### How it works here

All tests.

#### Why it matters here

Locks status mapping and body shape.

### Middleware and the request pipeline

#### What it means

Middleware components form a chain; each receives the request, may act before calling `next`, and may act again after it returns. Order is behaviour: a component only sees what earlier components set up and only wraps what comes after it.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#8-routing-and-middleware](../../../../PROJECT_OVERVIEW2.md#8-routing-and-middleware).)

#### Where it appears in this file

Real pipeline including exception handler.

#### How it works here

`throws` and malformed JSON tests.

#### Why it matters here

Verifies handler wiring.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Api/Http/ProblemResult.cs`](../../../src/TutoringCentre.Api/Http/ProblemResult.cs.md)
- [`src/TutoringCentre.Api/Http/ResultHttpExtensions.cs`](../../../src/TutoringCentre.Api/Http/ResultHttpExtensions.cs.md)
- [`src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs`](../../../src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs.md)
- [`tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs`](../Fixtures/ConventionEndpoints.cs.md)
- [`docs/architecture/api-conventions.md`](../../../docs/architecture/api-conventions.md.md)
