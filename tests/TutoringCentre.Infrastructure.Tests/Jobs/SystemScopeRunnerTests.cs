using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Jobs;

public sealed class SystemScopeRunnerTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task RunAsync_DispatchedQuery_UsesNileTransactionSettingAndCorrelation()
    {
        var result = await Fixture.Services.GetRequiredService<ISystemScopeRunner>().RunAsync(NileCentreId, "job:tenant-proof", async (scopedServices, ct) =>
        {
            Assert.Equal(new SystemActor(NileCentreId), scopedServices.GetRequiredService<ICurrentActor>().Actor);
            Assert.Equal("job:tenant-proof", scopedServices.GetRequiredService<ICorrelationContext>().CorrelationId);
            return await scopedServices.GetRequiredService<Dispatcher>().QueryAsync<ReadCurrentCentreSettingQuery, string>(new ReadCurrentCentreSettingQuery(), ct);
        });
        Assert.True(result.IsSuccess);
        Assert.Equal(NileCentreId.ToString(), result.Value);
    }
}
