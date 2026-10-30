using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

public sealed class PortalDirectoryTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task ParentsChildrenIncludeOnlyLinkedActiveStudentsInCurrentCentre()
    {
        var parent = await Fixture.SeedParentAsync(NileCentreId, "Nile Parent", "+201000000041");
        var active = await Fixture.SeedStudentAsync(NileCentreId, "Active Child");
        var inactive = await Fixture.SeedStudentAsync(NileCentreId, "Inactive Child", "archived");
        await Fixture.SeedStudentAsync(NileCentreId, "Unlinked Child");
        await Fixture.SeedStudentParentLinkAsync(NileCentreId, active, parent, true);
        await Fixture.SeedStudentParentLinkAsync(NileCentreId, inactive, parent, true);
        var children = await Fixture.ReadAsAsync(new SystemActor(NileCentreId), services =>
            services.GetRequiredService<IPortalDirectory>().GetChildrenAsync(parent, CancellationToken.None));
        var child = Assert.Single(children);
        Assert.Equal(active, child.Id);
        Assert.Equal("Active Child", child.Name);
    }

    [Fact]
    public async Task ForeignParentAndStudentAreUniformlyMissing()
    {
        var parent = await Fixture.SeedParentAsync(MaadiCentreId, "Maadi Parent", "+201000000042");
        var student = await Fixture.SeedStudentAsync(MaadiCentreId, "Maadi Student");
        await Fixture.SeedStudentParentLinkAsync(MaadiCentreId, student, parent, true);
        await Fixture.ReadAsAsync(new SystemActor(NileCentreId), async services =>
        {
            var directory = services.GetRequiredService<IPortalDirectory>();
            Assert.Null(await directory.GetParentAsync(parent, CancellationToken.None));
            Assert.Null(await directory.GetStudentAsync(student, CancellationToken.None));
            Assert.Empty(await directory.GetChildrenAsync(parent, CancellationToken.None));
            return true;
        });
    }

    [Fact]
    public async Task DirectoryReturnsFactsWithoutEntitiesAndReportsStudentActivity()
    {
        var parent = await Fixture.SeedParentAsync(NileCentreId, "Nile Parent", "+201000000043");
        var student = await Fixture.SeedStudentAsync(NileCentreId, "Inactive Student", "archived");
        await Fixture.ReadAsAsync(new SystemActor(NileCentreId), async services =>
        {
            var directory = services.GetRequiredService<IPortalDirectory>();
            Assert.Equal(new PortalSubjectFacts(parent, "Nile Parent", true), await directory.GetParentAsync(parent, CancellationToken.None));
            Assert.Equal(new PortalSubjectFacts(student, "Inactive Student", false), await directory.GetStudentAsync(student, CancellationToken.None));
            return true;
        });
    }
}
