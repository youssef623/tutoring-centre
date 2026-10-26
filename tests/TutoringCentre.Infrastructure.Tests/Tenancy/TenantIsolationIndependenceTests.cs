using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Task 21.5: the EF query filter (Day 20) and row-level security (Task 21.3) are each a complete control on
/// their own — each test here removes or bypasses the OTHER layer and proves the one layer under test still holds.
/// </summary>
public sealed class TenantIsolationIndependenceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task BypassingTheEfFilter_AsNileActor_StillReturnsOnlyNileRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        var result = await Fixture.ProbeServices.QueryAsAsync<ListProbeCentresIgnoringEfFilterQuery, List<Guid>>(
            StaffActorIn(NileCentreId), new ListProbeCentresIgnoringEfFilterQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal([NileCentreId], result.Value);
    }

    [Fact]
    public async Task EfFilterAlone_WithSuperuserConnectionInPlaceOfApp_StillReturnsOnlyNileRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        // Superuser bypasses row-level security entirely, so only the EF filter (Task 20.2) is left standing.
        await using var provider = Fixture.CreateServiceProvider(
            services => services.AddScoped<AppDbContext>(sp => sp.GetRequiredService<TenantProbeDbContext>()),
            appConnectionString: Fixture.SuperuserConnectionString);

        var result = await provider.QueryAsAsync<ListProbeCentresQuery, List<Guid>>(
            StaffActorIn(NileCentreId), new ListProbeCentresQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal([NileCentreId], result.Value);
    }

    [Fact]
    public async Task RawSqlInsideADispatchedQuery_AsNileActor_StillReturnsOnlyNileRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        var result = await Fixture.QueryAsAsync<ListProbeCentresByRawSqlQuery, List<Guid>>(
            StaffActorIn(NileCentreId), new ListProbeCentresByRawSqlQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal([NileCentreId], result.Value);
    }

    private static StaffActor StaffActorIn(Guid centreId) => new(Guid.CreateVersion7(), centreId, StaffRole.Teacher);
}
