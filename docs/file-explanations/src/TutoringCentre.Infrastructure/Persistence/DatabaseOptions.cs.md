# src/TutoringCentre.Infrastructure/Persistence/DatabaseOptions.cs

## Purpose

Typed settings object holding the database connection string, validated at startup.

## Where It Fits

Infrastructure/Persistence. Configured and validated in `DependencyInjection.cs`; read by the `AddDbContext` factory.

## Walkthrough

`public sealed class DatabaseOptions { [Required(AllowEmptyStrings = false)] public string ConnectionString { get; set; } = string.Empty; }`. Doc: 'running without a database is a misconfiguration, not a degraded state'. The XML comment says it is 'Bound from configuration key ConnectionStrings:Postgres', but the code actually assigns the captured string in `Configure(...)` rather than binding a section.

## Concepts Used

### Options pattern and ValidateOnStart

#### What it means

The options pattern binds configuration into a typed class and can validate it. `ValidateOnStart` runs that validation when the host starts, so a bad configuration stops the app immediately instead of failing on first use.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#614-options-validation-and-health-checks](../../../../PROJECT_OVERVIEW2.md#614-options-validation-and-health-checks).)

#### Where it appears in this file

The class.

#### How it works here

`[Required]` attribute.

#### Why it matters here

Empty string -> `OptionsValidationException` at host start.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Holds the value of `ConnectionStrings:Postgres` (secret).

## Gotchas and Issues

Comment vs code wording difference only.

## Related Files

- [`src/TutoringCentre.Infrastructure/DependencyInjection.cs`](../DependencyInjection.cs.md)
- [`src/TutoringCentre.Api/appsettings.json`](../../TutoringCentre.Api/appsettings.json.md)
