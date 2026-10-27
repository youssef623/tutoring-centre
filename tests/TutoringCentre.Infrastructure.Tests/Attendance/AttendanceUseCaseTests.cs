using TutoringCentre.Application.Attendance.Commands.MarkAttendance;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public sealed class AttendanceUseCaseTests(PostgresFixture fixture) : AttendanceCommandTestBase(fixture)
{
    [Fact]
    public async Task MarkReMarkAndCorrection_PersistAndAuditReason()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "use-cases");
        Time.SetUtcNow(scenario.StartsAt);
        var created = await MarkAsync(scenario.Owner, scenario);
        Assert.Equal(new MarkAttendanceResult(1, 0, 0), created.Value);
        var id = await RecordIdAsync(scenario);
        var markedAt = await Fixture.ScalarAsync<DateTime>($"select marked_at from attendance.attendance_records where id = '{id}'");
        Assert.Equal(scenario.StartsAt.UtcDateTime, markedAt);
        Assert.Equal(0, await CountAsync($"select count(*) from audit.audit_entries where entity_id = '{id}'"));
        Time.Advance(TimeSpan.FromMinutes(1));
        Assert.Equal(new MarkAttendanceResult(0, 0, 1), (await MarkAsync(scenario.Owner, scenario)).Value);
        Assert.Equal(markedAt, await Fixture.ScalarAsync<DateTime>($"select marked_at from attendance.attendance_records where id = '{id}'"));
        Assert.Equal(new MarkAttendanceResult(0, 1, 0), (await MarkAsync(scenario.Owner, scenario, AttendanceStatus.Late)).Value);
        var lastMark = await Fixture.ScalarAsync<DateTime>($"select marked_at from attendance.attendance_records where id = '{id}'");
        var corrected = await CorrectAsync(scenario.Owner, id, await RecordVersionAsync(id));
        Assert.True(corrected.IsSuccess, corrected.Error?.Message);
        Assert.Equal("excused", await Fixture.ScalarAsync<string>($"select status from attendance.attendance_records where id = '{id}'"));
        Assert.Equal(lastMark, await Fixture.ScalarAsync<DateTime>($"select marked_at from attendance.attendance_records where id = '{id}'"));
        Assert.Equal("parent called", await Fixture.ScalarAsync<string>($"select changes->'correctionReason'->>'after' from audit.audit_entries where entity_id = '{id}' and changes ? 'correctionReason'"));
        Assert.Equal("late", await Fixture.ScalarAsync<string>($"select changes->'status'->>'before' from audit.audit_entries where entity_id = '{id}' and changes ? 'correctionReason'"));
        Assert.Equal(2, await CountAsync($"select count(*) from audit.audit_entries where entity_id = '{id}' and action = 'updated'"));
    }

    [Fact]
    public async Task StaleCorrection_ReturnsConflictWithoutChangingRecordOrAudit()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "stale");
        Time.SetUtcNow(scenario.StartsAt);
        Assert.True((await MarkAsync(scenario.Owner, scenario)).IsSuccess);
        var id = await RecordIdAsync(scenario);
        var staleVersion = await RecordVersionAsync(id);
        Assert.True((await MarkAsync(scenario.Owner, scenario, AttendanceStatus.Late)).IsSuccess);
        var result = await CorrectAsync(scenario.Owner, id, staleVersion);
        Assert.Equal("concurrency.stale", result.Error!.Code);
        Assert.Equal("late", await Fixture.ScalarAsync<string>($"select status from attendance.attendance_records where id = '{id}'"));
        Assert.Equal(0, await Fixture.ScalarAsync<int>($"select correction_count from attendance.attendance_records where id = '{id}'"));
        Assert.Equal(1, await CountAsync($"select count(*) from audit.audit_entries where entity_id = '{id}'"));
    }

    [Fact]
    public async Task BatchWithCorrectedRecord_RollsBackEarlierNewMark()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "batch-rollback");
        Time.SetUtcNow(scenario.StartsAt);
        Assert.True((await MarkAsync(scenario.Owner, scenario)).IsSuccess);
        var id = await RecordIdAsync(scenario);
        Assert.True((await CorrectAsync(scenario.Owner, id, await RecordVersionAsync(id))).IsSuccess);
        var secondStudent = await Fixture.SeedStudentAsync(NileCentreId, "Second Student");
        var enrolled = await Clocked.SendAsAsync<EnrollStudentCommand, EnrollStudentResult>(scenario.Owner, new(scenario.GroupId, secondStudent));
        Assert.True(enrolled.IsSuccess, enrolled.Error?.Message);
        var result = await Clocked.SendAsAsync<MarkAttendanceCommand, MarkAttendanceResult>(scenario.Owner,
            new(scenario.SessionId, [new(secondStudent, AttendanceStatus.Present), new(scenario.StudentId, AttendanceStatus.Present)]));
        Assert.Equal("attendance.record_corrected", result.Error!.Code);
        Assert.Contains("marks[1].status", result.Error.Fields!.Keys);
        Assert.Equal(1, await CountAsync("select count(*) from attendance.attendance_records"));
        Assert.Equal("excused", await Fixture.ScalarAsync<string>($"select status from attendance.attendance_records where id = '{id}'"));
    }
}
