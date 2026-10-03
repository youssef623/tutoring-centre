# tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs

## Purpose

The real Api plus test-only endpoints, handlers and an in-memory log sink, used to test HTTP conventions.

## Where It Fits

Api.Tests/Fixtures; subclass of `ApiFactory`.

## Walkthrough

`Logs = new InMemoryLogSink()`. `ConfigureWebHost` calls the base then `ConfigureTestServices`: registers `ICommandHandler<ConventionCommand,string>` -> `ConventionCommandHandler`, `ICommandHandler<TestNameCommand,string>` -> `TestNameHandler`, `IValidator<TestNameCommand>` -> `TestNameValidator`, `ILogEventSink` singleton = `Logs` (picked up by `.ReadFrom.Services(services)` in `Program.cs`), and `IStartupFilter` `ConventionEndpointsStartupFilter` which maps `/api/test/*` routes. Test handlers are registered *after* the app's scan, so they exist only in this host.

## Concepts Used

### In-process hosting with WebApplicationFactory

#### What it means

`WebApplicationFactory<Program>` starts the real application inside the test process with an in-memory test server, so real middleware, DI and routing run without opening a network port. Tests can override configuration and services before the host is built.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

`ConfigureTestServices`.

#### How it works here

Method body.

#### Why it matters here

Override/add services for the test host only.

### Structured logging, Serilog and correlation IDs

#### What it means

Structured logging records a message *template* plus named properties, so logs are queryable by field. A *correlation ID* is attached to every log line and response of one request so they can be matched. *Destructuring* lets Serilog log an object's properties; a policy can mask sensitive ones.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#613-structured-logging-correlation-ids-and-redaction](../../../../PROJECT_OVERVIEW.md#613-structured-logging-correlation-ids-and-redaction).)

#### Where it appears in this file

Sink injected via DI.

#### How it works here

`ILogEventSink` registration.

#### Why it matters here

Lets tests assert on what was logged.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs`](ApiFactory.cs.md)
- [`tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs`](ConventionEndpoints.cs.md)
- [`tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpointsStartupFilter.cs`](ConventionEndpointsStartupFilter.cs.md)
- [`tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs`](ConventionTestTypes.cs.md)
- [`tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs`](InMemoryLogSink.cs.md)
