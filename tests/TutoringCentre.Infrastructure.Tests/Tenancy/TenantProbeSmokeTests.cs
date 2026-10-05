using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using Xunit.Abstractions;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Task 20.3's closing proof: the probe table is reachable end to end, and the tenant query filter convention
/// (Task 20.2) really does translate to a parameterized WHERE clause on centre_id, not a client-side filter.
/// </summary>
public sealed class TenantProbeSmokeTests(PostgresFixture fixture, ITestOutputHelper output) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task SeedProbeAsync_TwoRowsPerCentre_CountsFourAsSuperuser()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(NileCentreId, "nile-2");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-2");

        var count = await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes");

        Assert.Equal(4, count);
    }

    [Fact]
    public async Task ListQuery_AsNileActor_FiltersCentreIdWithAParameter()
    {
        var nileActor = new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Teacher);

        await using var probe = CreateProbeScope(nileActor);
        var sql = probe.Context.Set<TenantProbe>().ToQueryString();
        output.WriteLine(sql);

        Assert.Contains("centre_id", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("WHERE t.centre_id = @ef_filter__CurrentCentreId", sql, StringComparison.Ordinal);
        Assert.Matches(@"@ef_filter__CurrentCentreId='[0-9a-fA-F-]{36}'", sql);
    }
}
