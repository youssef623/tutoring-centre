using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>Task 20.6: the tenant write guard interceptor (Task 20.4), proven on the probe entity against real PostgreSQL.</summary>
public sealed class TenantWriteGuardTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task Add_NileActorAddsNileProbe_Saved()
    {
        var entity = new TenantProbe { Id = Guid.CreateVersion7(), CentreId = NileCentreId, Label = "ok" };

        await using (var probe = await CreateProbeScope(StaffActorIn(NileCentreId)))
        {
            probe.Context.Add(entity);
            await probe.Context.SaveChangesAsync();
        } // commits here (Task 21.1): the scalar check below reads through a separate connection, so it must run after.

        Assert.Equal(1, await Fixture.ScalarAsync<long>($"select count(*) from probe.tenant_probes where id = '{entity.Id}'"));
    }

    [Fact]
    public async Task Add_NileActorAddsProbeCarryingMaadiCentre_ThrowsAndWritesNothing()
    {
        await using var probe = await CreateProbeScope(StaffActorIn(NileCentreId));
        probe.Context.Add(new TenantProbe { Id = Guid.CreateVersion7(), CentreId = MaadiCentreId, Label = "smuggled" });

        await Assert.ThrowsAsync<TenantViolationException>(() => probe.Context.SaveChangesAsync());

        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes"));
    }

    [Fact]
    public async Task Add_ActorWithoutCentre_Throws()
    {
        await using var probe = await CreateProbeScope(new StaffActor(Guid.CreateVersion7(), null, null));
        probe.Context.Add(new TenantProbe { Id = Guid.CreateVersion7(), CentreId = NileCentreId, Label = "x" });

        await Assert.ThrowsAsync<TenantViolationException>(() => probe.Context.SaveChangesAsync());

        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes"));
    }

    [Fact]
    public async Task Modify_TrackedNileProbeCentreChangedToMaadi_ThrowsAndLeavesRowUnchanged()
    {
        var probeId = await Fixture.SeedProbeAsync(NileCentreId, "nile-1");

        await using var probe = await CreateProbeScope(StaffActorIn(NileCentreId));
        var entry = probe.Context.Attach(new TenantProbe { Id = probeId, CentreId = NileCentreId, Label = "nile-1" });
        entry.Property(nameof(TenantProbe.CentreId)).CurrentValue = MaadiCentreId;

        await Assert.ThrowsAsync<TenantViolationException>(() => probe.Context.SaveChangesAsync());

        Assert.Equal(NileCentreId, await Fixture.ScalarAsync<Guid>($"select centre_id from probe.tenant_probes where id = '{probeId}'"));
    }

    [Fact]
    public async Task Delete_MaadiProbeAttachedAndDeletedByNileActor_ThrowsAndLeavesRowPresent()
    {
        var probeId = await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var probe = await CreateProbeScope(StaffActorIn(NileCentreId));
        probe.Context.Remove(new TenantProbe { Id = probeId, CentreId = MaadiCentreId, Label = "maadi-1" });

        await Assert.ThrowsAsync<TenantViolationException>(() => probe.Context.SaveChangesAsync());

        Assert.Equal(1, await Fixture.ScalarAsync<long>($"select count(*) from probe.tenant_probes where id = '{probeId}'"));
    }

    [Fact]
    public async Task Add_SystemActorWithCentreSucceeds_SystemActorWithoutCentreThrows()
    {
        await using (var probe = await CreateProbeScope(new SystemActor(NileCentreId)))
        {
            probe.Context.Add(new TenantProbe { Id = Guid.CreateVersion7(), CentreId = NileCentreId, Label = "system-nile" });
            await probe.Context.SaveChangesAsync();
        }

        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes"));

        await using var probeNoCentre = await CreateProbeScope(new SystemActor(null));
        probeNoCentre.Context.Add(new TenantProbe { Id = Guid.CreateVersion7(), CentreId = NileCentreId, Label = "system-no-centre" });

        await Assert.ThrowsAsync<TenantViolationException>(() => probeNoCentre.Context.SaveChangesAsync());

        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes"));
    }

    private static StaffActor StaffActorIn(Guid centreId) => new(Guid.CreateVersion7(), centreId, StaffRole.Teacher);
}
