# src/TutoringCentre.Api/Http/ProblemResult.cs

## Purpose

The single writer of error bodies: serialises a `ProblemDetails` as `application/problem+json` with `traceId` and `correlationId`.

## Where It Fits

Api/Http, `internal`. Used by `ResultHttpExtensions.ToProblemResult`, `GlobalExceptionHandler`, and (via `Enrich`) `ProblemDetailsSetup`.

## Walkthrough

`ProblemResult(ProblemDetails problem) : IResult` (7). `ExecuteAsync(HttpContext)` (11-): null-check; `Enrich(problem, httpContext)`; `StatusCode = problem.Status ?? 500`; `WriteAsJsonAsync(problem, problem.GetType(), options: null, contentType: "application/problem+json", ct)` - passing the runtime type makes derived types (`ValidationProblemDetails`) serialise their extra `errors` member; uses `RequestAborted`. `Enrich` (27-31, `internal static`): `Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier`; `Extensions["correlationId"] = httpContext.Items[HttpContextKeys.CorrelationId]` (null when the middleware did not run).

## Concepts Used

### Problem Details (RFC 9457) and centralised error handling

#### What it means

Problem Details is a standard JSON error shape (`title`, `status`, `detail`, extensions) served as `application/problem+json`. Centralising error writing in one place keeps every error response uniform and prevents leaking internals.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling](../../../../PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling).)

#### Where it appears in this file

The writer.

#### How it works here

Class.

#### Why it matters here

Every error body has the same shape.

### Minimal APIs, routing and route groups

#### What it means

Minimal APIs map a URL pattern and HTTP verb straight to a delegate (`MapGet("/x", handler)`). The framework binds delegate parameters from DI (services), the route, query, body or `CancellationToken`. A *route group* shares a prefix and metadata across endpoints.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling](../../../../PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling).)

#### Where it appears in this file

`IResult`.

#### How it works here

Interface.

#### Why it matters here

Endpoints return it like any built-in result.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Because everything funnels through here, a bug here affects all errors.

## Related Files

- [`src/TutoringCentre.Api/Http/ResultHttpExtensions.cs`](ResultHttpExtensions.cs.md)
- [`src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs`](GlobalExceptionHandler.cs.md)
- [`src/TutoringCentre.Api/Http/HttpContextKeys.cs`](HttpContextKeys.cs.md)
- [`tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs`](../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs.md)
