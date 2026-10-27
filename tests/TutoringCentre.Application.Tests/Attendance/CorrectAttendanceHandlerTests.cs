using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Attendance.Commands.CorrectAttendance;
using TutoringCentre.Application.Attendance.Commands.MarkAttendance;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Attendance;

public sealed class CorrectAttendanceHandlerTests
{
    [Fact]
    public async Task UnknownRecord_ReturnsNotFoundBeforeSessionLookup()
    {
        var fixture = new AttendanceHandlerFixture();
        var result = await fixture.CorrectHandler.HandleAsync(new(Guid.NewGuid(), AttendanceStatus.Absent, "wrong student", 42), CancellationToken.None);
        Assert.Equal("attendance.not_found", result.Error!.Code);
        Assert.Empty(fixture.Roster.Calls);
    }

    [Fact]
    public async Task Correction_UsesClientVersionAndNormalisesReason()
    {
        var fixture = new AttendanceHandlerFixture();
        var record = fixture.AddExisting();
        var result = await fixture.CorrectHandler.HandleAsync(new(record.Id, AttendanceStatus.Absent, "  wrong   student ", 42), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(42u, fixture.Records.ExpectedVersion);
        Assert.Equal("wrong student", record.LastCorrectionReason);
        Assert.Equal(fixture.Staff.UserId, record.LastCorrectedByUserId);
    }

    [Fact]
    public async Task BeforeStart_ReturnsTooEarly()
    {
        var fixture = new AttendanceHandlerFixture();
        var record = fixture.AddExisting();
        fixture.Clock.UtcNow = fixture.Session.StartsAt.AddSeconds(-1);
        var result = await fixture.CorrectHandler.HandleAsync(new(record.Id, AttendanceStatus.Absent, "wrong student", 42), CancellationToken.None);
        Assert.Equal("attendance.correction_too_early", result.Error!.Code);
        Assert.Equal(0, record.CorrectionCount);
    }

    [Theory]
    [InlineData("missingSession")]
    [InlineData("otherTeacher")]
    [InlineData("none")]
    public async Task SessionMissingOrOutsideScope_IsAttendanceNotFound(string scenario)
    {
        var fixture = new AttendanceHandlerFixture();
        var record = fixture.AddExisting();
        if (scenario == "missingSession") fixture.Roster.Sessions.Clear();
        if (scenario == "otherTeacher") fixture.Scope.Scope = TeachingScope.Own(fixture.OtherSession.TeacherId);
        if (scenario == "none") fixture.Scope.Scope = TeachingScope.None;
        var result = await fixture.CorrectHandler.HandleAsync(new(record.Id, AttendanceStatus.Absent, "wrong student", 42), CancellationToken.None);
        Assert.Equal("attendance.not_found", result.Error!.Code);
        Assert.Equal(0, record.CorrectionCount);
    }

    [Fact]
    public async Task SystemActor_IsDeniedEvenWhenScopeAllows()
    {
        var fixture = new AttendanceHandlerFixture();
        var record = fixture.AddExisting();
        var result = await fixture.DispatchAsync<CorrectAttendanceCommand, Unit>(new SystemActor(fixture.Staff.CentreId),
            new(record.Id, AttendanceStatus.Absent, "wrong student", 42));
        Assert.Equal("auth.permission_denied", result.Error!.Code);
        Assert.Equal(0, record.CorrectionCount);
    }

    [Fact]
    public async Task Teacher_IsDeniedByDispatcherBeforeHandler()
    {
        var fixture = new AttendanceHandlerFixture();
        var result = await fixture.DispatchAsync<CorrectAttendanceCommand, Unit>(new StaffActor(Guid.NewGuid(), fixture.Staff.CentreId, StaffRole.Teacher),
            new(Guid.NewGuid(), AttendanceStatus.Absent, "wrong student", 42));
        Assert.Equal("auth.permission_denied", result.Error!.Code);
        Assert.Empty(fixture.UnitOfWork.Calls);
        Assert.Null(fixture.Records.ExpectedVersion);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task NoSelectedCentre_RefusesBothCommands(bool correction)
    {
        var fixture = new AttendanceHandlerFixture();
        var actor = new StaffActor(Guid.NewGuid(), null, null);
        var error = correction
            ? (await fixture.DispatchAsync<CorrectAttendanceCommand, Unit>(actor, new(Guid.NewGuid(), AttendanceStatus.Absent, "wrong student", 42))).Error
            : (await fixture.DispatchAsync<MarkAttendanceCommand, MarkAttendanceResult>(actor, fixture.Marks())).Error;
        Assert.Equal("tenant.not_selected", error!.Code);
        Assert.Empty(fixture.UnitOfWork.Calls);
        Assert.Empty(fixture.Roster.Calls);
    }
}
