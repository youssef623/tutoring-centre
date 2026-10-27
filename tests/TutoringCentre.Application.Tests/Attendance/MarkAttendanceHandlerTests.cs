using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Attendance.Commands.MarkAttendance;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Application.Tests.Attendance;

public sealed class MarkAttendanceHandlerTests
{
    [Fact]
    public async Task NewRepeatAndChangedBatches_ReturnExactCountsAndServerReferences()
    {
        var fixture = new AttendanceHandlerFixture();
        var first = await fixture.MarkHandler.HandleAsync(fixture.Marks(), CancellationToken.None);
        Assert.Equal(new MarkAttendanceResult(3, 0, 0), first.Value);
        Assert.All(fixture.Records.Added, record =>
        {
            Assert.Equal(fixture.Students.Single(student => student.StudentId == record.StudentId).EnrollmentId, record.EnrollmentId);
            Assert.Equal(fixture.Staff.CentreId, record.CentreId);
            Assert.Equal(fixture.Staff.UserId, record.MarkedByUserId);
            Assert.Equal(AttendanceSource.Manual, record.Source);
        });
        fixture.Clock.UtcNow = fixture.Clock.UtcNow.AddMinutes(1);
        var repeat = await fixture.MarkHandler.HandleAsync(fixture.Marks(), CancellationToken.None);
        Assert.Equal(new MarkAttendanceResult(0, 0, 3), repeat.Value);
        var marks = fixture.Marks().Marks.ToArray();
        marks[1] = marks[1] with { Status = AttendanceStatus.Late };
        var changed = await fixture.MarkHandler.HandleAsync(new(fixture.Session.SessionId, marks), CancellationToken.None);
        Assert.Equal(new MarkAttendanceResult(0, 1, 2), changed.Value);
    }

    [Theory]
    [InlineData("unknown")]
    [InlineData("otherTeacher")]
    [InlineData("none")]
    public async Task UnknownOrOutsideScope_ReturnsSessionNotFoundBeforeRosterOrWrites(string scenario)
    {
        var fixture = new AttendanceHandlerFixture();
        var id = scenario == "unknown" ? Guid.NewGuid() : scenario == "otherTeacher" ? fixture.OtherSession.SessionId : fixture.Session.SessionId;
        if (scenario == "none") fixture.Scope.Scope = TeachingScope.None;
        var result = await fixture.MarkHandler.HandleAsync(fixture.Marks() with { SessionId = id }, CancellationToken.None);
        Assert.Equal("attendance.session_not_found", result.Error!.Code);
        Assert.Equal(["session"], fixture.Roster.Calls);
        Assert.Empty(fixture.Records.Added);
        Assert.Equal(0, fixture.Records.SessionReads);
    }

    [Theory]
    [InlineData("before", "attendance.session_not_open")]
    [InlineData("after", "attendance.window_closed")]
    [InlineData("cancelled", "attendance.session_cancelled")]
    public async Task InvalidWindow_ReturnsBeforeExpectedStudentsOrWrites(string scenario, string code)
    {
        var fixture = new AttendanceHandlerFixture();
        fixture.Clock.UtcNow = scenario == "before" ? fixture.Session.StartsAt.AddMinutes(-31) : fixture.Session.EndsAt.AddHours(3);
        if (scenario == "cancelled") fixture.Roster.Sessions[fixture.Session.SessionId] = fixture.Session with { IsCancelled = true };
        var result = await fixture.MarkHandler.HandleAsync(fixture.Marks(), CancellationToken.None);
        Assert.Equal(code, result.Error!.Code);
        Assert.Equal(["session"], fixture.Roster.Calls);
        Assert.Empty(fixture.Records.Added);
    }

    [Fact]
    public async Task UnexpectedStudentInMiddle_HasIndexedFieldAndAddsNothing()
    {
        var fixture = new AttendanceHandlerFixture();
        var marks = fixture.Marks().Marks.ToArray();
        marks[1] = marks[1] with { StudentId = Guid.NewGuid() };
        var result = await fixture.MarkHandler.HandleAsync(new(fixture.Session.SessionId, marks), CancellationToken.None);
        Assert.Equal("attendance.student_not_in_session", result.Error!.Code);
        Assert.Contains("marks[1].studentId", result.Error.Fields!.Keys);
        Assert.Empty(fixture.Records.Added);
        Assert.Equal(0, fixture.Records.SessionReads);
    }

    [Fact]
    public async Task CorrectedRecord_RejectsBatchWithIndexedStatus()
    {
        var fixture = new AttendanceHandlerFixture();
        var record = fixture.AddExisting();
        Assert.True(record.Correct(AttendanceStatus.Absent, "wrong student", fixture.Staff.UserId,
            new(fixture.Session.StartsAt, fixture.Session.EndsAt, false), fixture.Clock.UtcNow).IsSuccess);
        var result = await fixture.MarkHandler.HandleAsync(fixture.Marks(), CancellationToken.None);
        Assert.Equal("attendance.record_corrected", result.Error!.Code);
        Assert.Contains("marks[0].status", result.Error.Fields!.Keys);
        Assert.Empty(fixture.Records.Added);
    }

    [Fact]
    public async Task SystemMark_HasNoStaffUser()
    {
        var fixture = new AttendanceHandlerFixture();
        fixture.Actor.Actor = new SystemActor(fixture.Staff.CentreId);
        var result = await fixture.MarkHandler.HandleAsync(fixture.Marks(), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.All(fixture.Records.Added, record => Assert.Null(record.MarkedByUserId));
    }
}
