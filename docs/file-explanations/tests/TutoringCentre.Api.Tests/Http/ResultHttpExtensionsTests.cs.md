# tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs

## Purpose

Unit tests of the `Result` -> HTTP mapping without starting a host.

## Where It Fits

Api.Tests/Http. Executes `IResult` objects against a `DefaultHttpContext` with a `MemoryStream` body.

## Walkthrough

Helper `ExecuteAsync(IResult)`: builds a service provider (`AddLogging().AddOptions()`), a `DefaultHttpContext` with `TraceIdentifier = "trace-1"` and `Items["CorrelationId"] = "corr-1"`, runs the result, reads status, content type and body. Tests: `[Theory]` each non-success kind (now including `ErrorKind.Unauthenticated` -> 401 `test.unauthenticated`) -> expected status, content type problem+json, `code`, `detail`, `status`; validation with fields -> `errors.name[0]`; failure includes `traceId`/`correlationId`; success -> 200 `"hello"`; non-generic success -> 204 empty; non-generic failure -> 409 with code.

## Concepts Used

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Fake HttpContext.

#### How it works here

`DefaultHttpContext`.

#### Why it matters here

Fast, host-free tests.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Api/Http/ResultHttpExtensions.cs`](../../../src/TutoringCentre.Api/Http/ResultHttpExtensions.cs.md)
- [`src/TutoringCentre.Api/Http/HttpContextKeys.cs`](../../../src/TutoringCentre.Api/Http/HttpContextKeys.cs.md)
