using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Attendance;
using Microsoft.Extensions.Logging;

namespace TutoringCentre.Application.Tests.Attendance;

public sealed class RecordScanHandlerTests
{
    private static readonly string[] LogKeys = ["Outcome", "SessionId", "CardId"];
    [Theory]
    [InlineData(0, AttendanceStatus.Present)]
    [InlineData(15, AttendanceStatus.Present)]
    [InlineData(16, AttendanceStatus.Late)]
    public async Task NewArrival_UsesScanPolicy(int minutes, AttendanceStatus status)
    {
        var f = new ScanFixture();
        f.Attendance.Clock.UtcNow += TimeSpan.FromMinutes(minutes);
        var result = await f.Scan();
        Assert.Equal(ScanOutcome.Recorded, result.Value.Outcome);
        Assert.Equal(status, result.Value.Status);
        Assert.Equal(f.Attendance.Clock.UtcNow, result.Value.RecordedAt);
        var record = Assert.Single(f.Attendance.Records.Added);
        Assert.Equal(AttendanceSource.Qr, record.Source);
        Assert.Equal(f.Attendance.Staff.UserId, record.MarkedByUserId);
        Assert.Equal(f.Card.StudentId, result.Value.StudentId);
        Assert.Equal(f.Attendance.Students[0].FullName, result.Value.StudentName);
        Assert.Equal(1, f.Cards.Reads);
    }

    [Theory]
    [InlineData(AttendanceStatus.Present, AttendanceSource.Manual)]
    [InlineData(AttendanceStatus.Late, AttendanceSource.Manual)]
    [InlineData(AttendanceStatus.Present, AttendanceSource.Qr)]
    [InlineData(AttendanceStatus.Late, AttendanceSource.Qr)]
    public async Task ExistingArrival_PreservesFirstStatusTimeAndSource(AttendanceStatus status, AttendanceSource source)
    {
        var f = new ScanFixture();
        var record = f.Existing(AttendanceStatus.Absent);
        record.Mark(status, source, f.Attendance.Staff.UserId,
            new(f.Attendance.Session.StartsAt, f.Attendance.Session.EndsAt, false), f.Attendance.Clock.UtcNow);
        var time = record.MarkedAt;
        f.Attendance.Clock.UtcNow += TimeSpan.FromMinutes(16);
        var result = await f.Scan();
        Assert.Equal(ScanOutcome.AlreadyRecorded, result.Value.Outcome);
        Assert.Equal(status, result.Value.Status);
        Assert.Equal(time, result.Value.RecordedAt);
        Assert.Equal(source, record.Source);
        Assert.Empty(f.Attendance.Records.Added);
        Assert.Equal(0, record.CorrectionCount);
    }

    [Theory]
    [InlineData(AttendanceStatus.Absent)]
    [InlineData(AttendanceStatus.Excused)]
    public async Task NonArrival_ScanWins(AttendanceStatus status)
    {
        var f = new ScanFixture();
        var record = f.Existing(status);
        Assert.Equal(ScanOutcome.Recorded, (await f.Scan()).Value.Outcome);
        Assert.Equal(AttendanceStatus.Present, record.Status);
        Assert.Equal(AttendanceSource.Qr, record.Source);
        Assert.Empty(f.Attendance.Records.Added);
    }

    [Theory]
    [InlineData(AttendanceStatus.Excused)]
    [InlineData(AttendanceStatus.Present)]
    public async Task CorrectedRecord_OfficeDecisionStands(AttendanceStatus status)
    {
        var f = new ScanFixture();
        var record = f.Existing(AttendanceStatus.Absent);
        Assert.True(record.Correct(status, "office decision", f.Attendance.Staff.UserId,
            new(f.Attendance.Session.StartsAt, f.Attendance.Session.EndsAt, false), f.Attendance.Clock.UtcNow).IsSuccess);
        Assert.Equal("attendance.record_corrected", (await f.Scan()).Error!.Code);
        Assert.Equal(status, record.Status);
        Assert.Equal(1, record.CorrectionCount);
    }

    [Fact]
    public async Task SystemActor_HasNoMarkingUser()
    {
        var f = new ScanFixture();
        f.Attendance.Actor.Actor = new SystemActor(f.Attendance.Staff.CentreId!.Value);
        Assert.True((await f.Scan()).IsSuccess);
        Assert.Null(Assert.Single(f.Attendance.Records.Added).MarkedByUserId);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ScanLog_ContainsOnlyOutcomeSessionAndCard(bool unknown)
    {
        var f = new ScanFixture();
        if (unknown) f.Cards.Items.Clear();
        await f.Scan();
        var entry = Assert.Single(f.Log.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Equal(LogKeys, entry.State.Where(pair => pair.Key != "{OriginalFormat}").Select(pair => pair.Key));
        Assert.Equal(unknown ? "card.not_recognised" : "Recorded", entry.State.Single(pair => pair.Key == "Outcome").Value);
        Assert.Equal(unknown ? null : (Guid?)f.Card.Id, entry.State.Single(pair => pair.Key == "CardId").Value);
    }
}
