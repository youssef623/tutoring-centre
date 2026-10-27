# `tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs`

## Purpose

Declares `ReadOnlyQueryTests`, `SneakyWriteQuery`, `SneakyWriteQueryHandler`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 58 lines and 2,365 bytes. Source hash: `72fa61a76e7386180343637760baef0b1c42cea94f5e21640186d0515b53b18d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using Microsoft.Extensions.DependencyInjection;`
- Line 4: `using Npgsql;`
- Line 5: `using TutoringCentre.Application.Common.Cqrs;`
- Line 6: `using TutoringCentre.Application.Common.Security;`
- Line 7: `using TutoringCentre.Domain.Common;`
- Line 8: `using TutoringCentre.Infrastructure.Persistence;`
- Line 9: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs#L13): `public sealed class ReadOnlyQueryTests(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs#L16): `public async Task QueryAsync_HandlerAttemptsInsert_PostgresRejectsItAndNoRowExists()`
- [Line 30](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs#L30): `private static PostgresException? FindPostgresException(Exception? exception)`
- [Line 44](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs#L44): `internal sealed record SneakyWriteQuery : IQuery<string>;`
- [Line 47](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs#L47): `internal sealed class SneakyWriteQueryHandler(AppDbContext db) : IQueryHandler<SneakyWriteQuery, string>`
- [Line 49](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs#L49): `public async Task<Result<string>> HandleAsync(SneakyWriteQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

public sealed class ReadOnlyQueryTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task QueryAsync_HandlerAttemptsInsert_PostgresRejectsItAndNoRowExists()
    {
        await using var provider = Fixture.CreateServiceProvider(
            services => services.AddScoped<IQueryHandler<SneakyWriteQuery, string>, SneakyWriteQueryHandler>());

        var exception = await Record.ExceptionAsync(() =>
            provider.QueryAsAsync<SneakyWriteQuery, string>(new SystemActor(null), new SneakyWriteQuery()));

        var postgresException = FindPostgresException(exception);
        Assert.NotNull(postgresException);
        Assert.Equal("25006", postgresException.SqlState);
        Assert.Equal(0, await Fixture.CountCentresAsync());
    }

    private static PostgresException? FindPostgresException(Exception? exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is PostgresException postgres)
            {
                return postgres;
            }
        }

        return null;
    }
}

internal sealed record SneakyWriteQuery : IQuery<string>;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class SneakyWriteQueryHandler(AppDbContext db) : IQueryHandler<SneakyWriteQuery, string>
{
    public async Task<Result<string>> HandleAsync(SneakyWriteQuery query, CancellationToken cancellationToken)
    {
        await db.Database.ExecuteSqlRawAsync(
            "insert into platform.centres (id, name, slug, time_zone_id, default_locale, created_at) " +
            "values (gen_random_uuid(), 'Sneaky', 'sneaky', 'Africa/Cairo', 'en', now())",
            cancellationToken);

        return Result<string>.Success("a write succeeded inside a query — this must never happen");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
