# tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs

## Purpose

Tests log levels for successful and failing requests.

## Where It Fits

Api.Tests/Http; collection `conventions`.

## Walkthrough

`SuccessfulRequest_LogsOneCompletionLineAtInformation`: `/api/test/ok` with id `logging-ok-check`; waits for an event with that `CorrelationId` and level Information. `UnhandledException_LogsAtErrorLevel`: `/api/test/throws` with `logging-error-check` -> an Error event. Does not assert that exactly one line is logged despite the name.

## Concepts Used

### Structured logging, Serilog and correlation IDs

#### What it means

Structured logging records a message *template* plus named properties, so logs are queryable by field. A *correlation ID* is attached to every log line and response of one request so they can be matched. *Destructuring* lets Serilog log an object's properties; a policy can mask sensitive ones.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#613-structured-logging-correlation-ids-and-redaction](../../../../PROJECT_OVERVIEW2.md#613-structured-logging-correlation-ids-and-redaction).)

#### Where it appears in this file

Level policy.

#### How it works here

Both tests.

#### Why it matters here

Confirms request logging level function and exception logging.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Test name says 'one completion line' but only existence is asserted.

## Related Files

- [`src/TutoringCentre.Api/Program.cs`](../../../src/TutoringCentre.Api/Program.cs.md)
- [`tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs`](../Fixtures/InMemoryLogSink.cs.md)
