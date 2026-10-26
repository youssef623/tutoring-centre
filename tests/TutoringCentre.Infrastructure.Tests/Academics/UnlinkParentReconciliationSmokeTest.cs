using System.Globalization;
using TutoringCentre.Application.Academics.Students.Commands.LinkParent;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;
using RealRegisterStudentCommand = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentCommand;
using RealRegisterStudentParent = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentParent;
using RealRegisterStudentResult = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentResult;
using RealUnlinkParentCommand = TutoringCentre.Application.Academics.Students.Commands.UnlinkParent.UnlinkParentCommand;
using Unit = TutoringCentre.Application.Common.Cqrs.Unit;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Proves UnitOfWork.ReconcileOrphanedStudentParentLinks through the real production handlers, not the
/// Day 38 test-only ones in StudentParentTestRequests.cs: a link removed through UnlinkParentCommand must
/// actually disappear from the database, not just from the aggregate's in-memory collection.
/// </summary>
public sealed class UnlinkParentReconciliationSmokeTest(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task UnlinkParent_ThroughRealHandler_RemovesTheLinkRow()
    {
        var actor = await StaffActorInAsync(NileCentreId);

        var registered = await Fixture.SendAsAsync<RealRegisterStudentCommand, RealRegisterStudentResult>(
            actor,
            new RealRegisterStudentCommand(
                "Youssef Omar", null, null, new RealRegisterStudentParent(null, "Omar Hassan", "01012345678"), ParentRelationship.Father));
        Assert.True(registered.IsSuccess);

        var secondParentId = await Fixture.SeedParentAsync(NileCentreId, "Amal Fathy", "+201212345678");
        var versionAfterRegister = await ReadVersionAsync(registered.Value.StudentId);
        var linked = await Fixture.SendAsAsync<LinkParentCommand, Unit>(
            actor, new LinkParentCommand(registered.Value.StudentId, secondParentId, ParentRelationship.Mother, versionAfterRegister));
        Assert.True(linked.IsSuccess, linked.Error?.Message);

        var versionAfterLink = await ReadVersionAsync(registered.Value.StudentId);
        var unlinked = await Fixture.SendAsAsync<RealUnlinkParentCommand, Unit>(
            actor, new RealUnlinkParentCommand(registered.Value.StudentId, secondParentId, versionAfterLink));
        Assert.True(unlinked.IsSuccess, unlinked.Error?.Message);

        Assert.Equal(
            0,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.student_parents where student_id = '{registered.Value.StudentId}' and parent_id = '{secondParentId}'"));
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.student_parents where student_id = '{registered.Value.StudentId}'"));
    }

    private async Task<StaffActor> StaffActorInAsync(Guid centreId) => new(await Fixture.SeedUserAsync(), centreId, StaffRole.Owner);

    private async Task<uint> ReadVersionAsync(Guid studentId) =>
        uint.Parse(await Fixture.ScalarAsync<string>($"select xmin::text from academics.students where id = '{studentId}'"), CultureInfo.InvariantCulture);
}
