# tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs

## Purpose

Tests the correlation-ID middleware's generation, validation and propagation.

## Where It Fits

Api.Tests/Http; collection `conventions`.

## Walkthrough

Header `X-Correlation-Id`. Tests: none supplied -> response header present, length 32; valid `client-supplied-id` -> echoed; 65-char header -> replaced (length 32); `bad id!` -> replaced; error body `correlationId` equals the header (`/api/test/notfound`); the id appears in a Serilog event (`WaitForAsync` on `CorrelationId` property for `/api/test/ok` with header `log-check-id`). Together they pin the middleware's contract (header, body, logs).

## Concepts Used

### Structured logging, Serilog and correlation IDs

#### What it means

Structured logging records a message *template* plus named properties, so logs are queryable by field. A *correlation ID* is attached to every log line and response of one request so they can be matched. *Destructuring* lets Serilog log an object's properties; a policy can mask sensitive ones.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#613-structured-logging-correlation-ids-and-redaction](../../../../PROJECT_OVERVIEW.md#613-structured-logging-correlation-ids-and-redaction).)

#### Where it appears in this file

Correlation IDs.

#### How it works here

All tests.

#### Why it matters here

Cross-cutting traceability.

### In-process hosting with WebApplicationFactory

#### What it means

`WebApplicationFactory<Program>` starts the real application inside the test process with an in-memory test server, so real middleware, DI and routing run without opening a network port. Tests can override configuration and services before the host is built.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

`factory.CreateClient()`.

#### How it works here

Constructor.

#### Why it matters here

In-memory HTTP client.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs`](../../../src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs.md)
- [`tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs`](../Fixtures/InMemoryLogSink.cs.md)
- [`tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs`](../Fixtures/ConventionsFactory.cs.md)
