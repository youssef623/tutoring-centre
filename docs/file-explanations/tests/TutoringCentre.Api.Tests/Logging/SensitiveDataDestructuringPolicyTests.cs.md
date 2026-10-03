# tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs

## Purpose

Tests that the policy masks sensitive properties and leaves others alone.

## Where It Fits

Api.Tests/Logging; no host.

## Walkthrough

Builds a `LoggerConfiguration().Destructure.With<SensitiveDataDestructuringPolicy>().WriteTo.Sink(new DelegateSink(events.Add))`, logs `"Probe {@Request}"`, and inspects the `StructureValue`. Tests: `Password`, `ApiToken`, `PhoneNumber` are `"***"`; `Email` intact; an anonymous object without sensitive names is unaffected.

## Concepts Used

### Structured logging, Serilog and correlation IDs

#### What it means

Structured logging records a message *template* plus named properties, so logs are queryable by field. A *correlation ID* is attached to every log line and response of one request so they can be matched. *Destructuring* lets Serilog log an object's properties; a policy can mask sensitive ones.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#613-structured-logging-correlation-ids-and-redaction](../../../../PROJECT_OVERVIEW.md#613-structured-logging-correlation-ids-and-redaction).)

#### Where it appears in this file

Destructuring policy test.

#### How it works here

All three.

#### Why it matters here

Verifies the safety net.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs`](../../../src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs.md)
