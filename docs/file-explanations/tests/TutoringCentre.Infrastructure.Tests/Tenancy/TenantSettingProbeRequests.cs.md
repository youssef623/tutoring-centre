# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 21.2: test-only requests that read app.current_centre back from inside a real dispatched request, proving what UnitOfWork.BeginAsync (Task 21.1) actually set for that transaction — not a stand-in assertion.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 45 lines and 2,263 bytes. Source hash: `c956859098c8131375114bdbe3b41d5b5d6175f97c2cb7cb74f99eda2092c77c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`
- Line 4: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs#L10): `internal sealed record ReadCurrentCentreSettingQuery : IQuery<string>;`
- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs#L12): `internal sealed record ReadCurrentCentreSettingCommand : ICommand<string>;`
- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs#L16): `internal sealed record AlwaysFailingCommand : ICommand<string>;`
- [Line 18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs#L18): `internal sealed class ReadCurrentCentreSettingQueryHandler(AppDbContext db) : IQueryHandler<ReadCurrentCentreSettingQuery, string>`
- [Line 20](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs#L20): `public async Task<Result<string>> HandleAsync(ReadCurrentCentreSettingQuery query, CancellationToken cancellationToken) =>`
- [Line 24](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs#L24): `internal sealed class ReadCurrentCentreSettingCommandHandler(AppDbContext db) : ICommandHandler<ReadCurrentCentreSettingCommand, string>`
- [Line 26](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs#L26): `public async Task<Result<string>> HandleAsync(ReadCurrentCentreSettingCommand command, CancellationToken cancellationToken) =>`
- [Line 30](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs#L30): `internal sealed class AlwaysFailingCommandHandler : ICommandHandler<AlwaysFailingCommand, string>`
- [Line 32](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs#L32): `public Task<Result<string>> HandleAsync(AlwaysFailingCommand command, CancellationToken cancellationToken) =>`
- [Line 36](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs#L36): `internal static class TenantSettingReader`
- [Line 38](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs#L38): `public static async Task<string> ReadAsync(AppDbContext db, CancellationToken ct)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:176](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L176)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:177](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L177)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:178](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L178)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs:17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L17)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs:87](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L87)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs:104](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L104)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs:118](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L118)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>Task 21.2: test-only requests that read app.current_centre back from inside a real dispatched request,
/// proving what UnitOfWork.BeginAsync (Task 21.1) actually set for that transaction — not a stand-in assertion.</summary>
internal sealed record ReadCurrentCentreSettingQuery : IQuery<string>;

internal sealed record ReadCurrentCentreSettingCommand : ICommand<string>;

/// <summary>Always fails, without touching the database: proves a transaction's app.current_centre setting does
/// not survive a rollback (Task 21.2, lifetime test 5).</summary>
internal sealed record AlwaysFailingCommand : ICommand<string>;

internal sealed class ReadCurrentCentreSettingQueryHandler(AppDbContext db) : IQueryHandler<ReadCurrentCentreSettingQuery, string>
{
    public async Task<Result<string>> HandleAsync(ReadCurrentCentreSettingQuery query, CancellationToken cancellationToken) =>
        Result<string>.Success(await TenantSettingReader.ReadAsync(db, cancellationToken));
}

internal sealed class ReadCurrentCentreSettingCommandHandler(AppDbContext db) : ICommandHandler<ReadCurrentCentreSettingCommand, string>
{
    public async Task<Result<string>> HandleAsync(ReadCurrentCentreSettingCommand command, CancellationToken cancellationToken) =>
        Result<string>.Success(await TenantSettingReader.ReadAsync(db, cancellationToken));
}

internal sealed class AlwaysFailingCommandHandler : ICommandHandler<AlwaysFailingCommand, string>
{
    public Task<Result<string>> HandleAsync(AlwaysFailingCommand command, CancellationToken cancellationToken) =>
        Task.FromResult(Result<string>.Failure(Error.Rule("test.always_fails", "Fails on purpose; writes nothing.")));
}

internal static class TenantSettingReader
{
    public static async Task<string> ReadAsync(AppDbContext db, CancellationToken ct)
    {
        var rows = await db.Database
            .SqlQueryRaw<string?>("select current_setting('app.current_centre', true) as value")
            .ToListAsync(ct);
        return rows[0] ?? string.Empty;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
