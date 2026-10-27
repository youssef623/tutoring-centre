# `src/TutoringCentre.Infrastructure/Persistence/AppDbContextOptionsConfigurator.cs`

## Purpose

Source summary (intent, not independent execution proof): The one place Npgsql, the snake_case naming convention, the migrations history table and the save interceptors are configured, so the runtime registration (tutoring_app, DependencyInjection.cs) and the short-lived migration context (tutoring_owner, `MigrationRunner` and the design-time factory) can never drift apart — including which interceptors run on each.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Persistence/AppDbContextOptionsConfigurator.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 34 lines and 1,569 bytes. Source hash: `d4a64f5861aec0e1167eb820bd2b6951b95df435ee3dfca2a28635b50da56fd8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Diagnostics;`
- Line 3: `using Npgsql;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../src/TutoringCentre.Infrastructure/Persistence/AppDbContextOptionsConfigurator.cs#L13): `public static class AppDbContextOptionsConfigurator`
- [Line 19](../../../../../src/TutoringCentre.Infrastructure/Persistence/AppDbContextOptionsConfigurator.cs#L19): `public static void Configure(DbContextOptionsBuilder builder, string connectionString, params IInterceptor[] interceptors)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/AppDbContextFactory.cs:37](../../../../../src/TutoringCentre.Api/Cli/AppDbContextFactory.cs#L37)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:105](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L105)
- [src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs:32](../../../../../src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs#L32)
- [tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs:82](../../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L82)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:167](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L167)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// The one place Npgsql, the snake_case naming convention, the migrations history table and the save interceptors
/// are configured, so the runtime registration (tutoring_app, DependencyInjection.cs) and the short-lived
/// migration context (tutoring_owner, <see cref="MigrationRunner"/> and the design-time factory) can never drift
/// apart — including which interceptors run on each.
/// </summary>
public static class AppDbContextOptionsConfigurator
{
    // Lets an operator find this app's connections in pg_stat_activity regardless of which role they connect as.
    // Not "TutoringCentre.Api" verbatim: that string collides with the architecture tests' namespace-dependency scan.
    private const string ApplicationName = "TutoringCentreApi";

    public static void Configure(DbContextOptionsBuilder builder, string connectionString, params IInterceptor[] interceptors)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var namedConnectionString = new NpgsqlConnectionStringBuilder(connectionString) { ApplicationName = ApplicationName }.ConnectionString;

        builder
            .UseNpgsql(namedConnectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", Schemas.Platform))
            .UseSnakeCaseNamingConvention();

        if (interceptors.Length > 0)
        {
            builder.AddInterceptors(interceptors);
        }
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
