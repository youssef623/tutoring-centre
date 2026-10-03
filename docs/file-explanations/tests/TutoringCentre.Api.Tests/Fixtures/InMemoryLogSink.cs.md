# tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs

## Purpose

Serilog sink that stores log events in memory so tests can assert on logging.

## Where It Fits

Api.Tests/Fixtures. Registered as a singleton `ILogEventSink` by `ConventionsFactory`.

## Walkthrough

`ConcurrentQueue<LogEvent>`; `Emit` enqueues; `Snapshot()`; `WaitForAsync(predicate, timeout=5s)` polls every 25 ms (request logging can occur just after the response is sent); static `Text(logEvent, propertyName)` returns a scalar string property or null. Thread-safe because requests run on multiple threads.

## Concepts Used

### Structured logging, Serilog and correlation IDs

#### What it means

Structured logging records a message *template* plus named properties, so logs are queryable by field. A *correlation ID* is attached to every log line and response of one request so they can be matched. *Destructuring* lets Serilog log an object's properties; a policy can mask sensitive ones.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#613-structured-logging-correlation-ids-and-redaction](../../../../PROJECT_OVERVIEW.md#613-structured-logging-correlation-ids-and-redaction).)

#### Where it appears in this file

Sink pattern.

#### How it works here

`Emit`.

#### Why it matters here

Observability tests.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs`](ConventionsFactory.cs.md)
- [`tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs`](../Http/CorrelationIdTests.cs.md)
- [`tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs`](../Http/LoggingConventionTests.cs.md)
