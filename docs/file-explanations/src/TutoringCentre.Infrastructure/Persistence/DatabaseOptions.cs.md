# `src/TutoringCentre.Infrastructure/Persistence/DatabaseOptions.cs`

## Purpose

Source summary (intent, not independent execution proof): Database settings. Validated at startup: running without a database is a misconfiguration, not a degraded state.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Persistence/DatabaseOptions.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 1,170 bytes. Source hash: `e16a7ea7a84b6839fc2b76569711fb3045aecbbbf6bc25ca0704a167fa149000`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.ComponentModel.DataAnnotations;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../src/TutoringCentre.Infrastructure/Persistence/DatabaseOptions.cs#L6): `public sealed class DatabaseOptions`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:91](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L91)
- [src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs:23](../../../../../src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs#L23)
- [tests/TutoringCentre.Infrastructure.Tests/Persistence/DatabaseOptionsValidationTests.cs:21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/DatabaseOptionsValidationTests.cs#L21)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.ComponentModel.DataAnnotations;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>Database settings. Validated at startup: running without a database is a misconfiguration, not a degraded state.</summary>
public sealed class DatabaseOptions
{
    /// <summary>
    /// Bound from configuration key ConnectionStrings:Postgres (user-secrets in development). The tutoring_app
    /// runtime login: required for every process, since the running app always needs to read and write data.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Bound from configuration key ConnectionStrings:PostgresMigrations (user-secrets in development). The
    /// tutoring_owner login, used only to apply migrations. Not required here: a compromised running web process
    /// must never hold schema-owning credentials. <see cref="MigrationRunner"/> requires it instead, at the point
    /// it is actually asked to migrate (Development startup, the seed CLI, test fixtures).
    /// </summary>
    public string? MigrationsConnectionString { get; set; }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
