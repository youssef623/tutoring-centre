using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Identity;

public sealed class PortalScopeResolverTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task ParentStudentSetIsCachedOnlyForOneRequest()
    {
        var parent = await Fixture.SeedParentAsync(NileCentreId, "Nile Parent", "+201000000051");
        var first = await Fixture.SeedStudentAsync(NileCentreId, "First Child");
        await Fixture.SeedStudentParentLinkAsync(NileCentreId, first, parent, true);
        var actor = new ParentActor(Guid.NewGuid(), NileCentreId, parent);
        Guid second = default;
        await Fixture.ReadAsAsync(actor, async services =>
        {
            var resolver = services.GetRequiredService<IPortalScopeResolver>();
            var initial = await resolver.ResolveAsync(actor, CancellationToken.None);
            Assert.Equal(first, Assert.Single(initial));
            second = await Fixture.SeedStudentAsync(NileCentreId, "Second Child");
            await Fixture.SeedStudentParentLinkAsync(NileCentreId, second, parent, true);
            Assert.Same(initial, await resolver.ResolveAsync(actor, CancellationToken.None));
            Assert.Equal(first, Assert.Single(initial));
            Assert.Empty(await resolver.ResolveAsync(new ParentActor(actor.UserId, MaadiCentreId, parent), CancellationToken.None));
            Assert.Equal(TeachingScopeKind.None, (await services.GetRequiredService<ITeachingScope>().GetAsync(CancellationToken.None)).Kind);
            return true;
        });
        var refreshed = await Fixture.ReadAsAsync(actor, services => services.GetRequiredService<IPortalScopeResolver>().ResolveAsync(actor, CancellationToken.None));
        Assert.True(refreshed.SetEquals([first, second]));
    }

    [Fact]
    public async Task StudentHasExactlyItsOwnStudentIdAndCannotResolveAnotherActor()
    {
        var student = await Fixture.SeedStudentAsync(NileCentreId, "Student");
        var actor = new StudentActor(Guid.NewGuid(), NileCentreId, student);
        await Fixture.ReadAsAsync(actor, async services =>
        {
            var resolver = services.GetRequiredService<IPortalScopeResolver>();
            Assert.Equal(student, Assert.Single(await resolver.ResolveAsync(actor, CancellationToken.None)));
            Assert.Empty(await resolver.ResolveAsync(new StudentActor(Guid.NewGuid(), NileCentreId, student), CancellationToken.None));
            return true;
        });
    }
}
