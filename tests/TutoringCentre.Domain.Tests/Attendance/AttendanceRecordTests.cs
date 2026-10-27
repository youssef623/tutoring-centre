using System.Globalization;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Attendance;

public sealed class AttendanceRecordTests
{
    private static readonly Guid Centre = Guid.NewGuid();
    private static readonly Guid Session = Guid.NewGuid();
    private static readonly Guid Student = Guid.NewGuid();
    private static readonly Guid Enrollment = Guid.NewGuid();
    private static readonly Guid User = Guid.NewGuid();
    private static readonly AttendanceSessionTiming Timing = new(At("2026-12-05T16:00Z"), At("2026-12-05T17:30Z"), false);
    private static DateTimeOffset At(string value) => DateTimeOffset.Parse(value, CultureInfo.InvariantCulture);
    private static AttendanceRecord Record() => AttendanceRecord.Create(Centre, Session, Student, Enrollment,
        AttendanceStatus.Present, AttendanceSource.Manual, User, Timing, At("2026-12-05T15:30Z")).Value;

    private static object?[] Snapshot(AttendanceRecord record) =>
    [
        record.Id, record.CentreId, record.SessionId, record.StudentId, record.EnrollmentId,
        record.Status, record.Source, record.MarkedAt, record.MarkedByUserId, record.CorrectionCount,
        record.LastCorrectionReason, record.LastCorrectedAt, record.LastCorrectedByUserId,
    ];

    private static void Failure(Result result, string code, ErrorKind kind = ErrorKind.Rule)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(code, result.Error!.Code);
        Assert.Equal(kind, result.Error.Kind);
    }

    [Fact]
    public void Create_InWindow_SetsEveryField()
    {
        var record = Record();
        Assert.NotEqual(Guid.Empty, record.Id);
        Assert.Equal(7, record.Id.Version);
        Assert.Equal(new object?[] { record.Id, Centre, Session, Student, Enrollment, AttendanceStatus.Present,
            AttendanceSource.Manual, At("2026-12-05T15:30Z"), User, 0, null, null, null }, Snapshot(record));
        Assert.IsAssignableFrom<ITenantOwned>(record);
    }

    [Fact]
    public void Create_NullMarkingUser_Succeeds() =>
        Assert.True(AttendanceRecord.Create(Centre, Session, Student, Enrollment, AttendanceStatus.Absent,
            AttendanceSource.Qr, null, Timing, At("2026-12-05T16:00Z")).IsSuccess);

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Create_EmptyReference_Fails(int emptyIndex)
    {
        Guid[] ids = [Centre, Session, Student, Enrollment, User];
        ids[emptyIndex] = Guid.Empty;
        var result = AttendanceRecord.Create(ids[0], ids[1], ids[2], ids[3], AttendanceStatus.Present,
            AttendanceSource.Manual, ids[4], Timing, At("2026-12-05T16:00Z"));
        Failure(result, "attendance.reference_required", ErrorKind.Validation);
    }

    [Theory]
    [InlineData(false, "2026-12-05T15:29:59Z", "attendance.session_not_open")]
    [InlineData(false, "2026-12-05T19:30:01Z", "attendance.window_closed")]
    [InlineData(true, "2026-12-05T15:29:59Z", "attendance.session_cancelled")]
    [InlineData(true, "2026-12-05T19:30:01Z", "attendance.session_cancelled")]
    public void Create_AndMark_RejectSessionRulesInOrder(bool cancelled, string now, string code)
    {
        var timing = new AttendanceSessionTiming(Timing.StartsAt, Timing.EndsAt, cancelled);
        Failure(AttendanceRecord.Create(Centre, Session, Student, Enrollment, AttendanceStatus.Present,
            AttendanceSource.Manual, User, timing, At(now)), code);
        var record = Record();
        var before = Snapshot(record);
        Failure(record.Mark(AttendanceStatus.Present, AttendanceSource.Qr, null, timing, At(now)), code);
        Assert.Equal(before, Snapshot(record));
    }

    [Fact]
    public void Mark_SameStatus_LeavesEveryFieldUnchanged()
    {
        var record = Record();
        var before = Snapshot(record);
        var result = record.Mark(AttendanceStatus.Present, AttendanceSource.Qr, null, Timing, At("2026-12-05T16:01Z"));
        Assert.True(result.IsSuccess);
        Assert.Equal(MarkOutcome.Unchanged, result.Value);
        Assert.Equal(before, Snapshot(record));
    }

    [Fact]
    public void Mark_NewStatus_UpdatesMarkFieldsOnly()
    {
        var record = Record();
        var user = Guid.NewGuid();
        var result = record.Mark(AttendanceStatus.Late, AttendanceSource.Qr, user, Timing, At("2026-12-05T19:30Z"));
        Assert.True(result.IsSuccess);
        Assert.Equal(MarkOutcome.Updated, result.Value);
        Assert.Equal(new object?[] { record.Id, Centre, Session, Student, Enrollment, AttendanceStatus.Late,
            AttendanceSource.Qr, At("2026-12-05T19:30Z"), user, 0, null, null, null }, Snapshot(record));
    }

    [Theory]
    [InlineData(false, "2026-12-05T16:02Z", "attendance.record_corrected")]
    [InlineData(false, "2026-12-06T16:00Z", "attendance.window_closed")]
    [InlineData(false, "2026-12-05T15:29Z", "attendance.session_not_open")]
    [InlineData(true, "2026-12-05T16:02Z", "attendance.session_cancelled")]
    public void Mark_CorrectedRecord_ChecksWindowBeforeLockAndStatus(bool cancelled, string now, string code)
    {
        var record = Record();
        Assert.True(record.Correct(AttendanceStatus.Absent, "wrong student", User, Timing, At("2026-12-05T16:01Z")).IsSuccess);
        var before = Snapshot(record);
        Failure(record.Mark(AttendanceStatus.Absent, AttendanceSource.Qr, null,
            new(Timing.StartsAt, Timing.EndsAt, cancelled), At(now)), code);
        Assert.Equal(before, Snapshot(record));
    }

    [Fact]
    public void Correct_InsideWindowAndNextDay_UpdatesCorrectionFieldsOnly()
    {
        var record = Record();
        var user = Guid.NewGuid();
        Assert.True(record.Correct(AttendanceStatus.Absent, "  wrong   student ", user, Timing, At("2026-12-05T16:01Z")).IsSuccess);
        Assert.Equal(new object?[] { record.Id, Centre, Session, Student, Enrollment, AttendanceStatus.Absent,
            AttendanceSource.Manual, At("2026-12-05T15:30Z"), User, 1, "wrong student", At("2026-12-05T16:01Z"), user }, Snapshot(record));
        Assert.True(record.Correct(AttendanceStatus.Excused, "parent called", User, Timing, At("2026-12-06T16:00Z")).IsSuccess);
        Assert.Equal(new object?[] { record.Id, Centre, Session, Student, Enrollment, AttendanceStatus.Excused,
            AttendanceSource.Manual, At("2026-12-05T15:30Z"), User, 2, "parent called", At("2026-12-06T16:00Z"), User }, Snapshot(record));
    }

    [Theory]
    [InlineData(true, "2026-12-05T15:59Z", "ok", AttendanceStatus.Present, "attendance.session_cancelled")]
    [InlineData(false, "2026-12-05T15:59Z", "ok", AttendanceStatus.Present, "attendance.correction_too_early")]
    [InlineData(false, "2026-12-05T16:01Z", "ok", AttendanceStatus.Present, "attendance.correction_reason_invalid")]
    [InlineData(false, "2026-12-05T16:01Z", "wrong student", AttendanceStatus.Present, "attendance.correction_unchanged")]
    [InlineData(true, "2026-12-06T16:00Z", "wrong student", AttendanceStatus.Absent, "attendance.session_cancelled")]
    public void Correct_Failure_UsesCheckOrderAndChangesNothing(bool cancelled, string now, string reason, AttendanceStatus status, string code)
    {
        var record = Record();
        var before = Snapshot(record);
        var result = record.Correct(status, reason, User, new(Timing.StartsAt, Timing.EndsAt, cancelled), At(now));
        Failure(result, code, code == "attendance.correction_reason_invalid" ? ErrorKind.Validation : ErrorKind.Rule);
        Assert.Equal(before, Snapshot(record));
        if (result.Error!.Kind == ErrorKind.Validation)
        {
            Assert.True(result.Error.Fields!.ContainsKey("reason"));
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t\n ")]
    [InlineData("ok")]
    public void Correct_InvalidReason_ChangesNothing(string? reason)
    {
        var record = Record();
        var before = Snapshot(record);
        var result = record.Correct(AttendanceStatus.Absent, reason, User, Timing, At("2026-12-05T16:01Z"));
        Failure(result, "attendance.correction_reason_invalid", ErrorKind.Validation);
        Assert.True(result.Error!.Fields!.ContainsKey("reason"));
        Assert.Equal(before, Snapshot(record));
    }

    [Theory]
    [InlineData(3, true)]
    [InlineData(200, true)]
    [InlineData(201, false)]
    public void Correct_ReasonLength_CheckedAfterNormalisation(int length, bool succeeds)
    {
        var record = Record();
        var before = Snapshot(record);
        var result = record.Correct(AttendanceStatus.Absent, " \t" + new string('x', length) + "\n ", User, Timing, At("2026-12-05T16:00Z"));
        Assert.Equal(succeeds, result.IsSuccess);
        if (!succeeds)
        {
            Failure(result, "attendance.correction_reason_invalid", ErrorKind.Validation);
            Assert.True(result.Error!.Fields!.ContainsKey("reason"));
            Assert.Equal(before, Snapshot(record));
        }
        else
        {
            Assert.Equal(new string('x', length), record.LastCorrectionReason);
        }
    }

    [Fact]
    public void Correct_EmptyUser_ChangesNothing()
    {
        var record = Record();
        var before = Snapshot(record);
        Failure(record.Correct(AttendanceStatus.Absent, "wrong student", Guid.Empty, Timing, At("2026-12-05T16:01Z")),
            "attendance.reference_required", ErrorKind.Validation);
        Assert.Equal(before, Snapshot(record));
    }
}
