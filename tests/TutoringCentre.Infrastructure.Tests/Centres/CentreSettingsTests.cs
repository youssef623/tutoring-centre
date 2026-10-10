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
