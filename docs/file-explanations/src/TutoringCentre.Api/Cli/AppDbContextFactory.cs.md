# `src/TutoringCentre.Api/Cli/AppDbContextFactory.cs`

## Purpose

Source summary (intent, not independent execution proof): Lets `dotnet ef` create `AppDbContext` at design time, from ConnectionStrings:PostgresMigrations (the tutoring_owner login) through the same `AppDbContextOptionsConfigurator` the runtime migration path uses. EF Core tooling finds this automatically and skips building the full host for it. Tooling, like the seed command alongside it: the one other place in Api allowed to reference Infrastructure directly (S7).

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Cli/AppDbContextFactory.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 42 lines and 2,147 bytes. Source hash: `c2d90e4794dcb6a148894d856ecc0577551b6c75d750178d5a8a955b39ae80c7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Design;`
- Line 3: `using Microsoft.Extensions.Configuration;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../src/TutoringCentre.Api/Cli/AppDbContextFactory.cs#L15): `public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>`
- [Line 21](../../../../../src/TutoringCentre.Api/Cli/AppDbContextFactory.cs#L21): `public AppDbContext CreateDbContext(string[] args)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Api.Cli;

/// <summary>
/// Lets `dotnet ef` create <see cref="AppDbContext"/> at design time, from ConnectionStrings:PostgresMigrations
/// (the tutoring_owner login) through the same <see cref="AppDbContextOptionsConfigurator"/> the runtime migration
/// path uses. EF Core tooling finds this automatically and skips building the full host for it. Tooling, like the
/// seed command alongside it: the one other place in Api allowed to reference Infrastructure directly (S7).
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    // Matches TutoringCentre.Api.csproj's UserSecretsId: a design-time factory bypasses WebApplication.CreateBuilder,
    // which is what would otherwise wire user-secrets up automatically in Development.
    private const string ApiUserSecretsId = "a0748b74-413e-4737-b25c-2a2e46b2d4e6";

    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(ApiUserSecretsId)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("PostgresMigrations");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:PostgresMigrations is required to create the design-time context. Set it via "
                + "user-secrets locally, or the ConnectionStrings__PostgresMigrations environment variable in CI.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        AppDbContextOptionsConfigurator.Configure(optionsBuilder, connectionString);

        // No request is running this tool: the actor stays anonymous, same as the runtime migration path.
        return new AppDbContext(optionsBuilder.Options, new CurrentActorContext());
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
