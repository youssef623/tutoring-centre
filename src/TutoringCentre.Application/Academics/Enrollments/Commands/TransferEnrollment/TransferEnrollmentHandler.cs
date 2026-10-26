using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;

/// <summary>
/// Orchestrates: scoped lookup of the enrollment → still active → a different group → lock and count the target
/// → same subject as the source → not already in the target → the capacity rule on the target → end the old
/// enrollment as Transferred and start a new one. Both changes are saved in the dispatcher's one transaction, so
/// they commit together or not at all. Only the target is locked: only its capacity is at risk, and taking one
/// lock per command means two opposite transfers can never wait on each other in a cycle. The old enrollment's
/// GroupId is never changed; history stays as it was.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class TransferEnrollmentHandler(
    ICurrentActor currentActor,
    IClock clock,
    IGroupRepository groups,
    IEnrollmentRepository enrollments)
    : ICommandHandler<TransferEnrollmentCommand, TransferEnrollmentResult>
{
    public async Task<Result<TransferEnrollmentResult>> HandleAsync(TransferEnrollmentCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre.
        var centreId = currentActor.Actor.CentreId!.Value;

        // A stale version is detected by the save (zero rows affected), which rolls back both changes.
        var current = await enrollments.GetByIdAsync(command.EnrollmentId, command.Version, cancellationToken);
        if (current is null)
        {
            return Result<TransferEnrollmentResult>.Failure(EnrollmentErrors.NotFound());
        }

        if (current.Status != EnrollmentStatus.Active)
        {
            return Result<TransferEnrollmentResult>.Failure(EnrollmentErrors.AlreadyEnded());
        }

        if (current.GroupId == command.TargetGroupId)
        {
            return Result<TransferEnrollmentResult>.Failure(EnrollmentErrors.SameGroup());
        }

        var target = await groups.GetForEnrollmentAsync(command.TargetGroupId, cancellationToken);
        if (target is null)
        {
            return Result<TransferEnrollmentResult>.Failure(GroupErrors.NotFound());
        }

        // Read without a lock: the source only loses a seat, which can never break its capacity.
        var source = await groups.FindAsync(current.GroupId, cancellationToken);
        if (source is null)
        {
            return Result<TransferEnrollmentResult>.Failure(GroupErrors.NotFound());
        }

        if (source.SubjectId != target.Group.SubjectId)
        {
            return Result<TransferEnrollmentResult>.Failure(EnrollmentErrors.SubjectMismatch());
        }

        if (await enrollments.ActiveExistsAsync(current.StudentId, target.Group.Id, cancellationToken))
        {
            return Result<TransferEnrollmentResult>.Failure(EnrollmentErrors.AlreadyEnrolled());
        }

        var canEnroll = target.Group.EnsureCanEnroll(target.ActiveEnrollmentCount);
        if (canEnroll.IsFailure)
        {
            return Result<TransferEnrollmentResult>.Failure(canEnroll.Error!);
        }

        var now = clock.UtcNow;
        var ended = current.End(now, EnrollmentEndReason.Transferred);
        if (ended.IsFailure)
        {
            return Result<TransferEnrollmentResult>.Failure(ended.Error!);
        }

        var started = Enrollment.Start(centreId, current.StudentId, target.Group.Id, now);
        if (started.IsFailure)
        {
            return Result<TransferEnrollmentResult>.Failure(started.Error!);
        }

        enrollments.Add(started.Value);
        return Result<TransferEnrollmentResult>.Success(new TransferEnrollmentResult(started.Value.Id));
    }
}
