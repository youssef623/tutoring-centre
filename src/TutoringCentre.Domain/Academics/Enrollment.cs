using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>
/// "This student holds a seat in this group since this moment." Refers to the student and the group by ID only.
/// Ending is a state transition that keeps the row: history is never edited, so re-enrolling after a withdrawal
/// starts a new enrollment and a transfer is an end plus a new start — there is no Reactivate. Timestamps are
/// passed in by the caller (from the application's clock); this type never reads a clock.
/// Not this type's concern: capacity (a rule about the group and a count of other rows, Group.EnsureCanEnroll),
/// "not already enrolled" (other rows: the handler and a partial unique index), and whether the student or the
/// group is active (other aggregates: the handler). The only way to obtain one is <see cref="Start"/>.
/// </summary>
public sealed class Enrollment : Entity, ITenantOwned
{
    private Enrollment(Guid centreId, Guid studentId, Guid groupId, DateTimeOffset enrolledAt)
    {
        CentreId = centreId;
        StudentId = studentId;
        GroupId = groupId;
        EnrolledAt = enrolledAt;
        Status = EnrollmentStatus.Active;
    }

    // For EF Core materialisation only; EF then sets every property from the row.
    private Enrollment()
    {
    }

    /// <summary>The centre this enrollment belongs to; fixed at start and never changeable.</summary>
    public Guid CentreId { get; private set; }

    /// <summary>The enrolled student; fixed at start and never changeable.</summary>
    public Guid StudentId { get; private set; }

    /// <summary>The group the seat is in; fixed at start and never changeable (a transfer is a new enrollment).</summary>
    public Guid GroupId { get; private set; }

    public EnrollmentStatus Status { get; private set; }

    public DateTimeOffset EnrolledAt { get; private set; }

    /// <summary>When the enrollment ended; null while active.</summary>
    public DateTimeOffset? EndedAt { get; private set; }

    /// <summary>Why the enrollment ended; null while active.</summary>
    public EnrollmentEndReason? EndReason { get; private set; }

    public static Result<Enrollment> Start(Guid centreId, Guid studentId, Guid groupId, DateTimeOffset enrolledAt)
    {
        if (centreId == Guid.Empty)
        {
            return Result<Enrollment>.Failure(Error.Validation("enrollment.centre_required", "A centre is required."));
        }

        if (studentId == Guid.Empty)
        {
            return Result<Enrollment>.Failure(Error.Validation("enrollment.student_required", "A student is required."));
        }

        if (groupId == Guid.Empty)
        {
            return Result<Enrollment>.Failure(Error.Validation("enrollment.group_required", "A group is required."));
        }

        return Result<Enrollment>.Success(new Enrollment(centreId, studentId, groupId, enrolledAt));
    }

    /// <summary>Ends the enrollment once. A second end fails and keeps the first end's time and reason.</summary>
    public Result End(DateTimeOffset endedAt, EnrollmentEndReason reason)
    {
        if (Status == EnrollmentStatus.Ended)
        {
            return Result.Failure(Error.Rule("enrollment.already_ended", "This enrollment has already ended."));
        }

        if (endedAt < EnrolledAt)
        {
            return Result.Failure(Error.Rule("enrollment.end_before_start", "An enrollment cannot end before it started."));
        }

        Status = EnrollmentStatus.Ended;
        EndedAt = endedAt;
        EndReason = reason;
        return Result.Success();
    }
}
