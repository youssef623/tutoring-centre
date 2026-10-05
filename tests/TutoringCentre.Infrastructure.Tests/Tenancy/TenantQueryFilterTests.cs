using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>Task 20.5: the tenant query filter convention (Task 20.2), proven on the probe entity against real PostgreSQL.</summary>
public sealed class TenantQueryFilterTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task List_AsNileStaffActor_ReturnsOnlyNileRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(NileCentreId, "nile-2");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var probe = await CreateProbeScope(StaffActorIn(NileCentreId));
        var rows = await probe.Context.Set<TenantProbe>().ToListAsync();

        Assert.Equal(2, rows.Count);
        Assert.All(rows, row => Assert.Equal(NileCentreId, row.CentreId));
    }

    [Fact]
    public async Task List_AsMaadiStaffActor_ReturnsOnlyMaadiRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-2");

        await using var probe = await CreateProbeScope(StaffActorIn(MaadiCentreId));
        var rows = await probe.Context.Set<TenantProbe>().ToListAsync();

        Assert.Equal(2, rows.Count);
        Assert.All(rows, row => Assert.Equal(MaadiCentreId, row.CentreId));
    }

    [Fact]
    public async Task List_AsStaffActorWithNoCentre_ReturnsZeroRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var probe = await CreateProbeScope(new StaffActor(Guid.CreateVersion7(), null, null));
        var rows = await probe.Context.Set<TenantProbe>().ToListAsync();

        Assert.Empty(rows);
    }

    [Fact]
    public async Task List_AsAnonymousActor_ReturnsZeroRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var probe = await CreateProbeScope(new AnonymousActor());
        var rows = await probe.Context.Set<TenantProbe>().ToListAsync();

        Assert.Empty(rows);
    }

    [Fact]
    public async Task FindById_MaadiProbeAsNileActor_FindsNothing()
    {
        var maadiProbeId = await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var probe = await CreateProbeScope(StaffActorIn(NileCentreId));
        var found = await probe.Context.Set<TenantProbe>().SingleOrDefaultAsync(row => row.Id == maadiProbeId);

        Assert.Null(found);
    }

    [Fact]
    public async Task CountAndAny_AsNileActor_RespectTheFilter()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(NileCentreId, "nile-2");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var probe = await CreateProbeScope(StaffActorIn(NileCentreId));

        Assert.Equal(2, await probe.Context.Set<TenantProbe>().CountAsync());
        Assert.True(await probe.Context.Set<TenantProbe>().AnyAsync());
        Assert.False(await probe.Context.Set<TenantProbe>().AnyAsync(row => row.CentreId == MaadiCentreId));
    }

    [Fact]
    public async Task List_TwoScopesDifferentActorsInParallel_EachSeesOnlyItsOwnRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-2");

        await using var nileProbe = await CreateProbeScope(StaffActorIn(NileCentreId));
        await using var maadiProbe = await CreateProbeScope(StaffActorIn(MaadiCentreId));

        var nileTask = nileProbe.Context.Set<TenantProbe>().ToListAsync();
        var maadiTask = maadiProbe.Context.Set<TenantProbe>().ToListAsync();
        var results = await Task.WhenAll(nileTask, maadiTask);
        var nileRows = results[0];
        var maadiRows = results[1];

        Assert.Single(nileRows);
        Assert.Equal(NileCentreId, nileRows[0].CentreId);
        Assert.Equal(2, maadiRows.Count);
        Assert.All(maadiRows, row => Assert.Equal(MaadiCentreId, row.CentreId));
    }

    private static StaffActor StaffActorIn(Guid centreId) => new(Guid.CreateVersion7(), centreId, StaffRole.Teacher);
}
