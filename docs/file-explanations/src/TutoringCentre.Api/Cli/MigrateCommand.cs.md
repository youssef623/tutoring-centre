# `src/TutoringCentre.Api/Cli/MigrateCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Applies pending migrations using ConnectionStrings:PostgresMigrations (tutoring_owner) and exits. A deploy pipeline step, not something the long-running web process ever does itself (Task 34.7): owner credentials belong to a short-lived job, not a process that serves the internet. Idempotent — a database already up to date is a no-op, since `MigrationRunner.ApplyMigrationsAsync` only ever applies what is pending.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Cli/MigrateCommand.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 38 lines and 1,549 bytes. Source hash: `b4c9865a97375d51bc0ddbc5ab5ad45edaff8ebb9f0fcfe30c4a6d0ecd36d25e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging;`
- Line 4: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../src/TutoringCentre.Api/Cli/MigrateCommand.cs#L14): `public static class MigrateCommand`
- [Line 20](../../../../../src/TutoringCentre.Api/Cli/MigrateCommand.cs#L20): `public static async Task<int> RunAsync(IServiceProvider services)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Program.cs:125](../../../../../src/TutoringCentre.Api/Program.cs#L125)
- [tests/TutoringCentre.Api.Tests/Cli/MigrateCommandTests.cs:14](../../../../../tests/TutoringCentre.Api.Tests/Cli/MigrateCommandTests.cs#L14)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Api.Cli;

/// <summary>
/// Applies pending migrations using ConnectionStrings:PostgresMigrations (tutoring_owner) and exits. A deploy
/// pipeline step, not something the long-running web process ever does itself (Task 34.7): owner credentials
/// belong to a short-lived job, not a process that serves the internet. Idempotent — a database already up to
/// date is a no-op, since <see cref="MigrationRunner.ApplyMigrationsAsync"/> only ever applies what is pending.
/// </summary>
public static class MigrateCommand
{
    [SuppressMessage(
        "Performance",
        "CA1848:Use the LoggerMessage delegates",
        Justification = "A handful of one-off log lines in a short-lived CLI command; a source-generated delegate is unwarranted here.")]
    public static async Task<int> RunAsync(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("TutoringCentre.Api.Cli.MigrateCommand");

        try
        {
            await services.ApplyMigrationsAsync();
            logger.LogInformation("Migrate: database is up to date.");
            return 0;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Migrate: applying migrations failed.");
            return 1;
        }
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
