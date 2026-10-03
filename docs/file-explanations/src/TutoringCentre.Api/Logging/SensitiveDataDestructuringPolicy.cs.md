# src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs

## Purpose

Serilog safety net that masks properties named like Password/Token/Secret/ConnectionString/Phone* when an object is logged with `{@Obj}`.

## Where It Fits

Api/Logging, `public sealed`. Installed in `Program.cs:23`. Tested by `SensitiveDataDestructuringPolicyTests`.

## Walkthrough

`IDestructuringPolicy.TryDestructure` (19-): returns `false` (use Serilog's default) for `IEnumerable` values; reflects public readable instance non-indexer properties; if none is sensitive returns `false` (40); else builds a `StructureValue` where sensitive properties are `ScalarValue("***")` (48-49) and others use `propertyValueFactory.CreatePropertyValue(value, destructureObjects: true)`; type name preserved. `IsSensitive` (59): name starts with `Phone` (case-insensitive) or contains any of `Password`, `Token`, `Secret`, `ConnectionString`. Doc: the primary rule remains 'never log request objects'.

## Concepts Used

### Structured logging, Serilog and correlation IDs

#### What it means

Structured logging records a message *template* plus named properties, so logs are queryable by field. A *correlation ID* is attached to every log line and response of one request so they can be matched. *Destructuring* lets Serilog log an object's properties; a policy can mask sensitive ones.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#613-structured-logging-correlation-ids-and-redaction](../../../../PROJECT_OVERVIEW.md#613-structured-logging-correlation-ids-and-redaction).)

#### Where it appears in this file

Destructuring policy.

#### How it works here

Whole class.

#### Why it matters here

Limits damage if someone logs a command object.

### Generics and constraints

#### What it means

Generics let one definition work for many types (`Result<T>`, `ICommandHandler<TCommand,TResponse>`). Constraints (`where TCommand : ICommand<TResponse>`) restrict which types are legal so the compiler can check them; the `in` modifier (contravariance) lets a handler of a base type satisfy a derived one.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Reflection use.

#### How it works here

`GetProperties`.

#### Why it matters here

Works for any type; costs reflection per call.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Fields are not inspected; names outside the list (e.g. `Pin`, `Email`) are not masked. Properties of nested collections use Serilog defaults.

## Related Files

- [`tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs`](../../../tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs.md)
- [`src/TutoringCentre.Api/Program.cs`](../Program.cs.md)
- [`docs/architecture/api-conventions.md`](../../../docs/architecture/api-conventions.md.md)
