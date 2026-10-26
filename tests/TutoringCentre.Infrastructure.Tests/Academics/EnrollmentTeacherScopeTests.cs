using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;
using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 48.10: ownership for derived resources. A roster and a student's enrollments belong to groups, so the
/// group's teacher scope applies to them too, through the real provider, read service and PostgreSQL. In Nile,
/// G1 belongs to teacher T1 and G2 to teacher T2; the actor is a Teacher-role member linked to T1.
/// </summary>
public sealed class EnrollmentTeacherScopeTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)
{
    [Fact]
    public async Task LinkedTeacher_ReadsTheOwnGroupsRosterAndGetsGroupNotFoundForAnothers()
    {
        var world = await ArrangeAsync("rscope1");

        var own = await Fixture.QueryAsAsync<GetGroupRosterQuery, GroupRosterDto>(world.T1Actor.Actor, new GetGroupRosterQuery(world.G1));
        var other = await Fixture.QueryAsAsync<GetGroupRosterQuery, GroupRosterDto>(world.T1Actor.Actor, new GetGroupRosterQuery(world.G2));

        Assert.True(own.IsSuccess, own.Error?.Message);
        Assert.Equal(world.StudentId, Assert.Single(own.Value.Items).StudentId);
        Assert.True(other.IsFailure);
        Assert.Equal("group.not_found", other.Error!.Code);
    }

    [Fact]
    public async Task LinkedTeacher_ListingAStudentsEnrollments_SeesOnlyThoseInOwnGroups()
    {
        var world = await ArrangeAsync("rscope2");

        var asTeacher = await Fixture.QueryAsAsync<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>(
            world.T1Actor.Actor, new ListStudentEnrollmentsQuery(world.StudentId));
        var asOwner = await Fixture.QueryAsAsync<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>(
            world.Owner.Actor, new ListStudentEnrollmentsQuery(world.StudentId));

        Assert.True(asTeacher.IsSuccess, asTeacher.Error?.Message);
        Assert.Equal(world.G1, Assert.Single(asTeacher.Value).GroupId);
        Assert.Equal([world.G1, world.G2], asOwner.Value.Select(enrollment => enrollment.GroupId).Order().ToArray());
    }

    private async Task<World> ArrangeAsync(string tag)
    {
        var owner = await CreateStaffAsync(NileCentreId, $"owner-{tag}@nile.test", StaffRole.Owner);
        var t1Member = await CreateStaffAsync(NileCentreId, $"teacher1-{tag}@nile.test", StaffRole.Teacher);
        var subjectId = await CreateSubjectAsync(owner.Actor, "Mathematics");
        var t1 = await CreateTeacherAsync(owner.Actor, "Amina Hassan", t1Member.UserId);
        var t2 = await CreateTeacherAsync(owner.Actor, "Youssef Kamal");
        var g1 = await CreateGroupAsync(owner.Actor, "Grade 10 A", subjectId, t1);
        var g2 = await CreateGroupAsync(owner.Actor, "Grade 10 B", subjectId, t2);
        var studentId = (await SeedStudentsAsync(NileCentreId, 1, $"Student {tag}"))[0];
        await EnrollOkAsync(owner.Actor, g1, studentId);
        await EnrollOkAsync(owner.Actor, g2, studentId);
        return new World(owner, t1Member, g1, g2, studentId);
    }

    private sealed record World(CreatedStaff Owner, CreatedStaff T1Actor, Guid G1, Guid G2, Guid StudentId);
}
