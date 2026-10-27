# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 21.2: app.current_centre's lifetime — set per transaction, read back through a real dispatched request.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 135 lines and 6,830 bytes. Source hash: `5f3935627e5503ce87bfb3c7b7696ec2ee3e9d33ae3788a9ea74a1735d26e442`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 3: `using Npgsql;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Identity;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence;`
- Line 7: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L12): `public sealed class TenantContextLifetimeTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L15): `public async Task Query_AsNileActor_SeesNilesId()`
- [Line 25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L25): `public async Task Query_AsMaadiActor_SeesMaadisId()`
- [Line 35](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L35): `public async Task Query_AsActorWithoutACentre_SeesEmptyString()`
- [Line 45](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L45): `public async Task Dispatch_NileThenNoCentre_OnTheSameConnection_SecondSeesEmpty()`
- [Line 78](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L78): `public async Task AfterAFailedCommandRollsBack_ANewTransactionOnTheSameConnection_SeesNoValue()`
- [Line 102](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L102): `public async Task Dispatch_AsCommandOrQuery_BothSeeTheSetting()`
- [Line 113](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L113): `private static async Task<string> ReadSettingThroughUnitOfWorkAsync(NpgsqlConnection connection, Actor actor)`
- [Line 125](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L125): `private static AppDbContext OpenAppDbContext(NpgsqlConnection connection, Actor actor)`
- [Line 134](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs#L134): `private static StaffActor StaffActorIn(Guid centreId) => new(Guid.CreateVersion7(), centreId, StaffRole.Teacher);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>Task 21.2: app.current_centre's lifetime — set per transaction, read back through a real dispatched request.</summary>
public sealed class TenantContextLifetimeTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task Query_AsNileActor_SeesNilesId()
    {
        var result = await Fixture.QueryAsAsync<ReadCurrentCentreSettingQuery, string>(
            StaffActorIn(NileCentreId), new ReadCurrentCentreSettingQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal(NileCentreId.ToString(), result.Value);
    }

    [Fact]
    public async Task Query_AsMaadiActor_SeesMaadisId()
    {
        var result = await Fixture.QueryAsAsync<ReadCurrentCentreSettingQuery, string>(
            StaffActorIn(MaadiCentreId), new ReadCurrentCentreSettingQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal(MaadiCentreId.ToString(), result.Value);
    }

    [Fact]
    public async Task Query_AsActorWithoutACentre_SeesEmptyString()
    {
        var result = await Fixture.QueryAsAsync<ReadCurrentCentreSettingQuery, string>(
            new AnonymousActor(), new ReadCurrentCentreSettingQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, result.Value);
    }

    [Fact]
    public async Task Dispatch_NileThenNoCentre_OnTheSameConnection_SecondSeesEmpty()
    {
        // One already-open connection, shared (never closed) across both requests below, via UseNpgsql(DbConnection):
        // the one guarantee a pool size setting can only ever approximate. UnitOfWork is instantiated directly
        // (InternalsVisibleTo) rather than resolved through a pool, so "Maximum Pool Size=1" never has to be relied
        // on to force reuse — this is what "a large pool hides the bug" is really about.
        await using var connection = new NpgsqlConnection(Fixture.AppConnectionString);
        await connection.OpenAsync();

        var nileValue = await ReadSettingThroughUnitOfWorkAsync(connection, StaffActorIn(NileCentreId));
        Assert.Equal(NileCentreId.ToString(), nileValue);

        // A second request's own BeginAsync would always overwrite the setting with its own value before reading
        // it back, passing even if Nile's value had wrongly survived the first request's commit — so what actually
        // proves the setting is transaction-local (not session-local) is reading it raw first, before anything
        // explicitly sets it again.
        await using (var checkTransaction = await connection.BeginTransactionAsync())
        {
            await using var checkCommand = new NpgsqlCommand(
                "select nullif(current_setting('app.current_centre', true), '') is null", connection, checkTransaction);
            var selectsNoCentre = (bool)(await checkCommand.ExecuteScalarAsync())!;
            await checkTransaction.RollbackAsync();

            Assert.True(selectsNoCentre, "Expected Nile's committed transaction not to leave a value behind.");
        }

        // And the ordinary case the task describes: a second, real request for an actor without a centre, on the
        // same connection, still correctly sees empty.
        var noCentreValue = await ReadSettingThroughUnitOfWorkAsync(connection, new AnonymousActor());
        Assert.Equal(string.Empty, noCentreValue);
    }

    [Fact]
    public async Task AfterAFailedCommandRollsBack_ANewTransactionOnTheSameConnection_SeesNoValue()
    {
        await using var connection = new NpgsqlConnection(Fixture.AppConnectionString);
        await connection.OpenAsync();

        await using (var db = OpenAppDbContext(connection, StaffActorIn(NileCentreId)))
        {
            var unitOfWork = new UnitOfWork(db, NullLogger<UnitOfWork>.Instance);
            await unitOfWork.BeginAsync(readOnly: false, CancellationToken.None);
            var result = await new AlwaysFailingCommandHandler().HandleAsync(new AlwaysFailingCommand(), CancellationToken.None);
            Assert.True(result.IsFailure);
            await unitOfWork.RollbackAsync(CancellationToken.None); // what Dispatcher.SendAsync does on failure
        }
```

The excerpt stops at line 90; use the original file for the remaining 45 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
