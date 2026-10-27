# `tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 32.10: proves updating centre settings against real PostgreSQL — isolation, the audit row it produces, and concurrency.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 86 lines and 4,986 bytes. Source hash: `1f63255dcc855996b1ce52acceecfad259f2527c44d9d04cb8a76bede59557ce`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Centres;`
- Line 2: `using TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;`
- Line 3: `using TutoringCentre.Application.Centres.Queries.GetCentreSettings;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Domain.Centres;`
- Line 6: `using TutoringCentre.Domain.Common;`
- Line 7: `using TutoringCentre.Domain.Identity;`
- Line 8: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 9: `using TutoringCentre.Infrastructure.Tests.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs#L14): `public sealed class CentreSettingsTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)`
- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs#L17): `public async Task UpdateCentreSettings_AsNileOwner_ChangesNileOnly_MaadiRowUnchanged()`
- [Line 37](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs#L37): `public async Task UpdateCentreSettings_WritesExactlyOneCentreUpdatedAuditRowWithNameBeforeAndAfter()`
- [Line 67](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs#L67): `public async Task UpdateCentreSettings_WithStaleVersion_ReturnsConcurrencyStale()`
- [Line 84](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs#L84): `private Task<string> SnapshotCentreAsync(Guid centreId) =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Centres;
using TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;
using TutoringCentre.Application.Centres.Queries.GetCentreSettings;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Identity;

namespace TutoringCentre.Infrastructure.Tests.Centres;

/// <summary>Task 32.10: proves updating centre settings against real PostgreSQL — isolation, the audit row it produces, and concurrency.</summary>
public sealed class CentreSettingsTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    [Fact]
    public async Task UpdateCentreSettings_AsNileOwner_ChangesNileOnly_MaadiRowUnchanged()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-settings-1@nile.test", StaffRole.Owner);
        var maadiBefore = await SnapshotCentreAsync(MaadiCentreId);
        var before = await Fixture.QueryAsAsync<GetCentreSettingsQuery, CentreSettingsDto>(owner.Actor, new GetCentreSettingsQuery());
        Assert.True(before.IsSuccess, before.Error?.Message);

        var result = await Fixture.SendAsAsync<UpdateCentreSettingsCommand, Unit>(
            owner.Actor, new UpdateCentreSettingsCommand("Nile Learning Centre", SupportedLocale.En, before.Value.Version));

        Assert.True(result.IsSuccess, result.Error?.Message);
        var after = await Fixture.QueryAsAsync<GetCentreSettingsQuery, CentreSettingsDto>(owner.Actor, new GetCentreSettingsQuery());
        Assert.True(after.IsSuccess, after.Error?.Message);
        Assert.Equal("Nile Learning Centre", after.Value.Name);
        Assert.Equal(SupportedLocale.En, after.Value.DefaultLocale);
        Assert.NotEqual(before.Value.Version, after.Value.Version);
        Assert.Equal(maadiBefore, await SnapshotCentreAsync(MaadiCentreId));
    }

    [Fact]
    public async Task UpdateCentreSettings_WritesExactlyOneCentreUpdatedAuditRowWithNameBeforeAndAfter()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-settings-2@nile.test", StaffRole.Owner);
        var before = await Fixture.QueryAsAsync<GetCentreSettingsQuery, CentreSettingsDto>(owner.Actor, new GetCentreSettingsQuery());
        var countBefore = await Fixture.ScalarAsync<long>(
            $"select count(*) from audit.audit_entries where entity_type = 'centre' and entity_id = '{NileCentreId}'");

        var result = await Fixture.SendAsAsync<UpdateCentreSettingsCommand, Unit>(
            owner.Actor, new UpdateCentreSettingsCommand("Renamed Nile Centre", SupportedLocale.En, before.Value.Version));
        Assert.True(result.IsSuccess, result.Error?.Message);

        Assert.Equal(
            countBefore + 1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from audit.audit_entries where entity_type = 'centre' and entity_id = '{NileCentreId}'"));
        Assert.Equal(
            "updated",
            await Fixture.ScalarAsync<string>(
                $"select action from audit.audit_entries where entity_type = 'centre' and entity_id = '{NileCentreId}' order by occurred_at desc limit 1"));
        Assert.Equal(
            before.Value.Name,
            await Fixture.ScalarAsync<string>(
                $"select changes->'name'->>'before' from audit.audit_entries where entity_type = 'centre' and entity_id = '{NileCentreId}' order by occurred_at desc limit 1"));
        Assert.Equal(
            "Renamed Nile Centre",
            await Fixture.ScalarAsync<string>(
                $"select changes->'name'->>'after' from audit.audit_entries where entity_type = 'centre' and entity_id = '{NileCentreId}' order by occurred_at desc limit 1"));
    }

    [Fact]
    public async Task UpdateCentreSettings_WithStaleVersion_ReturnsConcurrencyStale()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-settings-3@nile.test", StaffRole.Owner);
        var before = await Fixture.QueryAsAsync<GetCentreSettingsQuery, CentreSettingsDto>(owner.Actor, new GetCentreSettingsQuery());
        var staleVersion = before.Value.Version;

        var first = await Fixture.SendAsAsync<UpdateCentreSettingsCommand, Unit>(
            owner.Actor, new UpdateCentreSettingsCommand("First Rename", SupportedLocale.En, staleVersion));
        Assert.True(first.IsSuccess, first.Error?.Message);

        var staleRetry = await Fixture.SendAsAsync<UpdateCentreSettingsCommand, Unit>(
            owner.Actor, new UpdateCentreSettingsCommand("Second Rename", SupportedLocale.Ar, staleVersion));

        Assert.True(staleRetry.IsFailure);
        Assert.Equal("concurrency.stale", staleRetry.Error!.Code);
    }

    private Task<string> SnapshotCentreAsync(Guid centreId) =>
        Fixture.ScalarAsync<string>($"select name || '|' || default_locale || '|' || xmin from platform.centres where id = '{centreId}'");
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
