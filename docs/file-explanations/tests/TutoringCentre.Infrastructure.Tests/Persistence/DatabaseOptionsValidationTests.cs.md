# `tests/TutoringCentre.Infrastructure.Tests/Persistence/DatabaseOptionsValidationTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Pure options-binding tests: no database is needed, since these assert on configuration shape, not connectivity.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/DatabaseOptionsValidationTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 64 lines and 2,516 bytes. Source hash: `4cb068faa5b4d9da4b9d6e887f34768fb4acc79fe568ea9dfbbe16ca0bd29316`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.Configuration;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Options;`
- Line 4: `using TutoringCentre.Application;`
- Line 5: `using TutoringCentre.Infrastructure;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/DatabaseOptionsValidationTests.cs#L13): `public sealed class DatabaseOptionsValidationTests`
- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/DatabaseOptionsValidationTests.cs#L16): `public void ResolvingDatabaseOptions_WithoutRuntimeConnectionString_ThrowsNamingTheKey()`
- [Line 27](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/DatabaseOptionsValidationTests.cs#L27): `public void ResolvingDatabaseOptions_WithOnlyRuntimeConnectionString_Succeeds()`
- [Line 38](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/DatabaseOptionsValidationTests.cs#L38): `public async Task ApplyMigrationsAsync_WithoutMigrationsConnectionString_ThrowsNamingTheKey()`
- [Line 47](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/DatabaseOptionsValidationTests.cs#L47): `private static ServiceProvider BuildProvider(string? postgres, string? postgresMigrations)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TutoringCentre.Application;
using TutoringCentre.Infrastructure;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

/// <summary>
/// Pure options-binding tests: no database is needed, since these assert on configuration shape, not connectivity.
/// </summary>
public sealed class DatabaseOptionsValidationTests
{
    [Fact]
    public void ResolvingDatabaseOptions_WithoutRuntimeConnectionString_ThrowsNamingTheKey()
    {
        var provider = BuildProvider(postgres: null, postgresMigrations: null);

        var exception = Assert.Throws<OptionsValidationException>(
            () => provider.GetRequiredService<IOptions<DatabaseOptions>>().Value);

        Assert.Contains("ConnectionString", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ResolvingDatabaseOptions_WithOnlyRuntimeConnectionString_Succeeds()
    {
        var provider = BuildProvider(postgres: "Host=localhost;Database=x;Username=x;Password=x", postgresMigrations: null);

        var options = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

        Assert.Equal("Host=localhost;Database=x;Username=x;Password=x", options.ConnectionString);
        Assert.Null(options.MigrationsConnectionString);
    }

    [Fact]
    public async Task ApplyMigrationsAsync_WithoutMigrationsConnectionString_ThrowsNamingTheKey()
    {
        var provider = BuildProvider(postgres: "Host=localhost;Database=x;Username=x;Password=x", postgresMigrations: null);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => provider.ApplyMigrationsAsync());

        Assert.Contains("ConnectionStrings:PostgresMigrations", exception.Message, StringComparison.Ordinal);
    }

    private static ServiceProvider BuildProvider(string? postgres, string? postgresMigrations)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = postgres,
                ["ConnectionStrings:PostgresMigrations"] = postgresMigrations,
            })
            .Build();

        var services = new ServiceCollection()
            .AddLogging()
            .AddApplication()
            .AddInfrastructure(configuration);

        return services.BuildServiceProvider();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
