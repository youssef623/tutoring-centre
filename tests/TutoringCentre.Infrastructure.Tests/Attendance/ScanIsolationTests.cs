using TutoringCentre.Domain.Attendance;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public sealed class ScanIsolationTests(PostgresFixture fixture) : ScanTestBase(fixture)
{
    [Fact]
    public async Task MaadiCardInNile_IdenticalToUnknown_NoRowsInEitherCentre()
    {
        var nile = await ArrangeAttendanceAsync(NileCentreId, "scan-nile");
        var maadi = await ArrangeAttendanceAsync(MaadiCentreId, "scan-maadi");
        var card = await IssueAsync(maadi);
        Time.SetUtcNow(nile.StartsAt);
        var foreign = await ScanAsync(nile, card.Token);
        var unknown = await ScanAsync(nile, CardToken.Generate().Reveal());
        Assert.Equal("card.not_recognised", foreign.Error!.Code);
        Assert.Equal(unknown.Error, foreign.Error);
        Assert.Equal(0, await CountAsync("select count(*) from attendance.attendance_records"));
    }

    [Fact]
    public async Task OtherTeacherSession_HiddenBeforeCardLookup()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "scan-teacher");
        var teacher = await LinkTeacherAsync(scenario, scenario.SubstituteId, "scan-teacher@example.test");
        var card = await IssueAsync(scenario);
        Time.SetUtcNow(scenario.StartsAt);
        Assert.Equal("attendance.session_not_found", (await ScanAsync(scenario, card.Token, teacher)).Error!.Code);
        Assert.Equal(0, await CountAsync("select count(*) from attendance.attendance_records"));
    }

    [Fact]
    public async Task OtherGroupStudent_NotExpected_NoRecord()
    {
        var target = await ArrangeAttendanceAsync(NileCentreId, "scan-target");
        var otherGroup = await Fixture.ScalarAsync<Guid>($"select id from academics.groups where centre_id = '{NileCentreId}' and name = 'Grade 10 B'");
        var otherStudent = await Fixture.SeedStudentAsync(NileCentreId, "Other group student");
        var enrollment = await Clocked.SendAsAsync<EnrollStudentCommand, EnrollStudentResult>(target.Owner, new(otherGroup, otherStudent));
        Assert.True(enrollment.IsSuccess, enrollment.Error?.Code);
        var card = await IssueAsync(target with { StudentId = otherStudent });
        Time.SetUtcNow(target.StartsAt);
        Assert.Equal("attendance.student_not_in_session", (await ScanAsync(target, card.Token)).Error!.Code);
        Assert.Equal(0, await CountAsync("select count(*) from attendance.attendance_records"));
    }
}
