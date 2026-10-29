using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Centres;

public sealed class CentreDirectoryTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task ListCentreIdsAsync_ReturnsOrderedIdsRegardlessOfActor()
    {
        Actor[] actors = [new AnonymousActor(), new SystemActor(null), new SystemActor(NileCentreId), new SystemActor(MaadiCentreId)];
        foreach (var actor in actors)
        {
            await using var scope = Fixture.Services.CreateAsyncScope();
            scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor);
            var ids = await scope.ServiceProvider.GetRequiredService<ICentreDirectory>().ListCentreIdsAsync(CancellationToken.None);
            Assert.Equal(new[] { NileCentreId, MaadiCentreId }.Order(), ids);
        }
    }
}
