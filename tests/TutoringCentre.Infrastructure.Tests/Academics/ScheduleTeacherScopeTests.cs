using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 52.11: teacher ownership through the real provider, read service and PostgreSQL. In Nile, G1 and G2 belong
/// to T1 and G3 to T2; the actor is a Teacher-role member linked to T1, and every group has a rule.
/// </summary>
public sealed class ScheduleTeacherScopeTests(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    [Fact]
    public async Task LinkedTeacher_TimetableContainsOnlyTheirOwnSessions()
    {
        var (world, teacherMember) = await ArrangeAsync();

        var asTeacher = await GetTimetableAsync(teacherMember.Actor, new DateOnly(2027, 3, 6));
        var asOwner = await GetTimetableAsync(world.Owner.Actor, new DateOnly(2027, 3, 6));

        Assert.Equal(2, asTeacher.Sessions.Count);
        Assert.All(asTeacher.Sessions, session => Assert.Equal(world.T1, session.TeacherId));
        Assert.Equal(3, asOwner.Sessions.Count);
    }

    [Fact]
    public async Task LinkedTeacher_RulesOfAnotherTeachersGroup_IsGroupNotFound()
    {
        var (world, teacherMember) = await ArrangeAsync();

        var own = await Clocked.QueryAsAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(
            teacherMember.Actor, new ListGroupScheduleRulesQuery(world.G1));
        var other = await Clocked.QueryAsAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(
            teacherMember.Actor, new ListGroupScheduleRulesQuery(world.G3));

        Assert.True(own.IsSuccess, own.Error?.Message);
        Assert.Single(own.Value);
        Assert.True(other.IsFailure);
        Assert.Equal("group.not_found", other.Error!.Code);
    }

    private async Task<(World World, CreatedStaff TeacherMember)> ArrangeAsync()
    {
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher1@nile.test", StaffRole.Teacher);
        var world = await ArrangeWorldAsync(NileCentreId, "nile", teacherMember.UserId);
        await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, new TimeOnly(10, 0));
        await AddRuleOkAsync(world.Owner.Actor, world.G2, DayOfWeek.Saturday, new TimeOnly(12, 0));
        await AddRuleOkAsync(world.Owner.Actor, world.G3, DayOfWeek.Saturday, new TimeOnly(10, 0));
        return (world, teacherMember);
    }
}
