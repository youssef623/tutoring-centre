# `src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs`

## Purpose

Migration application requires a separate owner connection and constructs a short-lived anonymous context outside runtime DI. It applies pending schema changes and disposes that context. Its summary caller list is stale because MigrateCommand now invokes it in addition to development startup and seeding.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 38 lines and 1,921 bytes. Source hash: `d07be826a75f7c27ebc41d70c8a4202ca79fc651d11f368c147cfae3c65c8187`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Options;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Infrastructure.Persistence.Interceptors;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs#L15): `public static class MigrationRunner`
- [Line 17](../../../../../src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs#L17): `public static async Task ApplyMigrationsAsync(this IServiceProvider services, CancellationToken ct = default)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Architecture.Tests/QueryFilterBypassTests.cs:40](../../../../../tests/TutoringCentre.Architecture.Tests/QueryFilterBypassTests.cs#L40)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Persistence.Interceptors;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// Applies pending EF Core migrations through a short-lived <see cref="AppDbContext"/> built directly on the
/// migrations (tutoring_owner) connection, then disposes it. The app's own DI-registered AppDbContext never runs
/// migrations and stays on the runtime (tutoring_app) connection throughout. Called by the Api in Development and
/// by the seed command only.
/// </summary>
public static class MigrationRunner
{
    public static async Task ApplyMigrationsAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(services);

        // IOptions<T> resolves fine without a scope; building AppDbContext by hand means nothing scoped is needed.
        var migrationsConnectionString = services
            .GetRequiredService<IOptions<DatabaseOptions>>().Value.MigrationsConnectionString;

        if (string.IsNullOrWhiteSpace(migrationsConnectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:PostgresMigrations is required to apply migrations. Set it alongside ConnectionStrings:Postgres.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        AppDbContextOptionsConfigurator.Configure(optionsBuilder, migrationsConnectionString, new TenantWriteGuardInterceptor());

        // No request or job is running this: the actor stays anonymous, same as the design-time factory.
        await using var migrationContext = new AppDbContext(optionsBuilder.Options, new CurrentActorContext());
        await migrationContext.Database.MigrateAsync(ct);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
