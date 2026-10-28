using TutoringCentre.Application.Attendance;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public sealed class RecordScanUseCaseTests(PostgresFixture fixture) : ScanTestBase(fixture)
{
    [Fact]
    public async Task ScanTwice_OneRow_FirstTimeStatusAndVersion_NoAudit()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "scan-repeat");
        var card = await IssueAsync(scenario);
        Time.SetUtcNow(scenario.StartsAt);
        var first = await ScanAsync(scenario, card.Token);
        Assert.True(first.IsSuccess, first.Error?.Code);
        var id = await RecordIdAsync(scenario);
        var version = await RecordVersionAsync(id);
        Time.Advance(TimeSpan.FromMinutes(16));
        var repeat = await ScanAsync(scenario, card.Token);
        Assert.Equal(ScanOutcome.Recorded, first.Value.Outcome);
        Assert.Equal(ScanOutcome.AlreadyRecorded, repeat.Value.Outcome);
        Assert.Equal(AttendanceStatus.Present, repeat.Value.Status);
        Assert.Equal(first.Value.RecordedAt, repeat.Value.RecordedAt);
        Assert.Equal(version, await RecordVersionAsync(id));
        Assert.Equal(1, await CountAsync("select count(*) from attendance.attendance_records"));
        Assert.Equal(0, await CountAsync($"select count(*) from audit.audit_entries where entity_id = '{id}'"));
    }

    [Fact]
    public async Task ReissuedCard_OldRevoked_NewRecordsLateArrival()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "scan-reissue");
        var old = await IssueAsync(scenario);
        var current = await IssueAsync(scenario);
        Time.SetUtcNow(scenario.StartsAt.AddMinutes(16));
        Assert.Equal("card.revoked", (await ScanAsync(scenario, old.Token)).Error!.Code);
        var result = await ScanAsync(scenario, current.Token);
        Assert.Equal(ScanOutcome.Recorded, result.Value.Outcome);
        Assert.Equal(AttendanceStatus.Late, result.Value.Status);
        Assert.Equal(1, await CountAsync("select count(*) from attendance.attendance_records"));
    }

    [Theory]
    [InlineData(AttendanceStatus.Absent)]
    [InlineData(AttendanceStatus.Excused)]
    public async Task ManualNonArrival_ScanRecordsQrArrival(AttendanceStatus status)
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "scan-manual");
        var card = await IssueAsync(scenario);
        Time.SetUtcNow(scenario.StartsAt);
        Assert.True((await MarkAsync(scenario.Owner, scenario, status)).IsSuccess);
        var result = await ScanAsync(scenario, card.Token);
        Assert.Equal(ScanOutcome.Recorded, result.Value.Outcome);
        var id = await RecordIdAsync(scenario);
        Assert.Equal("qr", await Fixture.ScalarAsync<string>($"select source from attendance.attendance_records where id = '{id}'"));
        Assert.Equal("present", await Fixture.ScalarAsync<string>($"select status from attendance.attendance_records where id = '{id}'"));
    }

    [Fact]
    public async Task CorrectedRecord_ScanCannotOverwriteOrAudit()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "scan-corrected");
        var card = await IssueAsync(scenario);
        Time.SetUtcNow(scenario.StartsAt);
        Assert.True((await MarkAsync(scenario.Owner, scenario)).IsSuccess);
        var id = await RecordIdAsync(scenario);
        Assert.True((await CorrectAsync(scenario.Owner, id, await RecordVersionAsync(id))).IsSuccess);
        var version = await RecordVersionAsync(id);
        var auditCount = await CountAsync($"select count(*) from audit.audit_entries where entity_id = '{id}'");
        Assert.Equal("attendance.record_corrected", (await ScanAsync(scenario, card.Token)).Error!.Code);
        Assert.Equal(version, await RecordVersionAsync(id));
        Assert.Equal(auditCount, await CountAsync($"select count(*) from audit.audit_entries where entity_id = '{id}'"));
    }
}
