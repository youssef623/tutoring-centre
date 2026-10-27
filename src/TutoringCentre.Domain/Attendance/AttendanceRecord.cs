using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Attendance;

/// <summary>A student's attendance in one session. References belong to the same centre and never change.</summary>
public sealed class AttendanceRecord : Entity, ITenantOwned
{
    private string? correctionReason;

    private AttendanceRecord()
    {
    }

    private AttendanceRecord(Guid centreId, Guid sessionId, Guid studentId, Guid enrollmentId,
        AttendanceStatus status, AttendanceSource source, Guid? byUserId, DateTimeOffset now)
    {
        CentreId = centreId;
        SessionId = sessionId;
        StudentId = studentId;
        EnrollmentId = enrollmentId;
        Status = status;
        Source = source;
        MarkedByUserId = byUserId;
        MarkedAt = now;
    }

    public Guid CentreId { get; }
    public Guid SessionId { get; }
    public Guid StudentId { get; }
    public Guid EnrollmentId { get; }
    public AttendanceStatus Status { get; private set; }
    public AttendanceSource Source { get; private set; }
    public DateTimeOffset MarkedAt { get; private set; }
    public Guid? MarkedByUserId { get; private set; }
    public int CorrectionCount { get; private set; }
    public string? LastCorrectionReason
    {
        get => correctionReason;
        private set => correctionReason = value;
    }
    public DateTimeOffset? LastCorrectedAt { get; private set; }
    public Guid? LastCorrectedByUserId { get; private set; }

    public static Result<AttendanceRecord> Create(Guid centreId, Guid sessionId, Guid studentId, Guid enrollmentId,
        AttendanceStatus status, AttendanceSource source, Guid? byUserId, AttendanceSessionTiming timing, DateTimeOffset now)
    {
        if (centreId == Guid.Empty || sessionId == Guid.Empty || studentId == Guid.Empty || enrollmentId == Guid.Empty || byUserId == Guid.Empty)
        {
            return Result<AttendanceRecord>.Failure(ReferenceRequired());
        }

        var error = MarkingError(timing, now);
        return error is not null
            ? Result<AttendanceRecord>.Failure(error)
            : Result<AttendanceRecord>.Success(new AttendanceRecord(centreId, sessionId, studentId, enrollmentId, status, source, byUserId, now));
    }

    public Result<MarkOutcome> Mark(AttendanceStatus status, AttendanceSource source, Guid? byUserId,
        AttendanceSessionTiming timing, DateTimeOffset now)
    {
        var error = MarkingError(timing, now);
        if (error is not null)
        {
            return Result<MarkOutcome>.Failure(error);
        }

        if (CorrectionCount > 0)
        {
            return Result<MarkOutcome>.Failure(Error.Rule("attendance.record_corrected", "A corrected record must be changed through a correction."));
        }

        if (Status == status)
        {
            return Result<MarkOutcome>.Success(MarkOutcome.Unchanged);
        }

        Status = status;
        Source = source;
        MarkedAt = now;
        MarkedByUserId = byUserId;
        return Result<MarkOutcome>.Success(MarkOutcome.Updated);
    }

    public Result Correct(AttendanceStatus newStatus, string? reason, Guid byUserId,
        AttendanceSessionTiming timing, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(timing);
        if (timing.IsCancelled)
        {
            return Result.Failure(Error.Rule("attendance.session_cancelled", "Attendance cannot be corrected for a cancelled session."));
        }

        if (!AttendanceWindow.HasStarted(timing, now))
        {
            return Result.Failure(Error.Rule("attendance.correction_too_early", "Attendance can only be corrected once the session has started."));
        }

        var normalisedReason = string.Join(" ", (reason ?? string.Empty).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        if (normalisedReason.Length is < 3 or > 200)
        {
            return Result.Failure(Error.Validation("attendance.correction_reason_invalid", "A correction reason must contain 3 to 200 characters.",
                new Dictionary<string, string[]> { ["reason"] = ["A correction reason must contain 3 to 200 characters."] }));
        }

        if (newStatus == Status)
        {
            return Result.Failure(Error.Rule("attendance.correction_unchanged", "A correction must change the attendance status."));
        }

        if (byUserId == Guid.Empty)
        {
            return Result.Failure(ReferenceRequired());
        }

        Status = newStatus;
        LastCorrectionReason = normalisedReason;
        LastCorrectedAt = now;
        LastCorrectedByUserId = byUserId;
        CorrectionCount++;
        return Result.Success();
    }

    private static Error ReferenceRequired() =>
        Error.Validation("attendance.reference_required", "A non-empty reference is required.");

    private static Error? MarkingError(AttendanceSessionTiming timing, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(timing);
        if (timing.IsCancelled)
        {
            return Error.Rule("attendance.session_cancelled", "Attendance cannot be marked for a cancelled session.");
        }

        return AttendanceWindow.StateAt(timing, now) switch
        {
            AttendanceWindowState.NotOpen => Error.Rule("attendance.session_not_open", "The attendance window has not opened yet."),
            AttendanceWindowState.Closed => Error.Rule("attendance.window_closed", "The attendance window has closed."),
            _ => null,
        };
    }
}
