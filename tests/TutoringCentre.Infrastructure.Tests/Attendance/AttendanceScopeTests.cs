using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public sealed class AttendanceScopeTests(PostgresFixture fixture) : AttendanceCommandTestBase(fixture)
{
    [Fact]
    public async Task LinkedTeacher_MarksOwnSession_AnotherTeacherGetsNotFound()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "teacher-scope");
        var own = await LinkTeacherAsync(scenario, scenario.TeacherId, "own-attendance@example.test");
        var other = await LinkTeacherAsync(scenario, scenario.SubstituteId, "other-attendance@example.test");
        Time.SetUtcNow(scenario.StartsAt);
        var denied = await MarkAsync(other, scenario);
        Assert.Equal("attendance.session_not_found", denied.Error!.Code);
        Assert.Equal(0, await CountAsync("select count(*) from attendance.attendance_records"));
        Assert.True((await MarkAsync(own, scenario)).IsSuccess);
        Assert.Equal(own.UserId, await Fixture.ScalarAsync<Guid>($"select marked_by_user_id from attendance.attendance_records where session_id = '{scenario.SessionId}'"));
    }

    [Fact]
    public async Task SubstituteOwnsReassignedSession_WhileGroupKeepsItsRegularTeacher()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "substitute");
        var regular = await LinkTeacherAsync(scenario, scenario.TeacherId, "regular-attendance@example.test");
        var substitute = await LinkTeacherAsync(scenario, scenario.SubstituteId, "substitute-attendance@example.test");
        await ReassignSessionAsync(scenario, scenario.SubstituteId);
        Assert.Equal(scenario.TeacherId, await Fixture.ScalarAsync<Guid>($"select teacher_id from academics.groups where id = '{scenario.GroupId}'"));
        Time.SetUtcNow(scenario.StartsAt);
        Assert.Equal("attendance.session_not_found", (await MarkAsync(regular, scenario)).Error!.Code);
        Assert.True((await MarkAsync(substitute, scenario)).IsSuccess);
    }
}
