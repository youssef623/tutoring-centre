using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;

/// <summary>
/// Orchestrates, in this order (ADR 0011): lock the group and count its seats → resolve the student → not already
/// enrolled → the capacity rule on the group → start and add. The lock comes first so every later step is
/// serialized per group; the group comes before the student so a foreign group ID answers group.not_found before
/// anything about the student is revealed. The dispatcher's commit releases the lock.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class EnrollStudentHandler(
    ICurrentActor currentActor,
    IClock clock,
    IGroupRepository groups,
    IStudentRepository students,
    IEnrollmentRepository enrollments)
    : ICommandHandler<EnrollStudentCommand, EnrollStudentResult>
{
    public async Task<Result<EnrollStudentResult>> HandleAsync(EnrollStudentCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre.
        var centreId = currentActor.Actor.CentreId!.Value;

        var locked = await groups.GetForEnrollmentAsync(command.GroupId, cancellationToken);
        if (locked is null)
        {
            return Result<EnrollStudentResult>.Failure(GroupErrors.NotFound());
        }

        var student = await students.FindAsync(command.StudentId, cancellationToken);
        if (student is null)
        {
            return Result<EnrollStudentResult>.Failure(StudentErrors.NotFound());
        }

        if (student.Status == StudentStatus.Archived)
        {
            return Result<EnrollStudentResult>.Failure(EnrollmentErrors.StudentArchived());
        }

        if (await enrollments.ActiveExistsAsync(student.Id, locked.Group.Id, cancellationToken))
        {
            return Result<EnrollStudentResult>.Failure(EnrollmentErrors.AlreadyEnrolled());
        }

        var canEnroll = locked.Group.EnsureCanEnroll(locked.ActiveEnrollmentCount);
        if (canEnroll.IsFailure)
        {
            return Result<EnrollStudentResult>.Failure(canEnroll.Error!);
        }

        var started = Enrollment.Start(centreId, student.Id, locked.Group.Id, clock.UtcNow);
        if (started.IsFailure)
        {
            return Result<EnrollStudentResult>.Failure(started.Error!);
        }

        enrollments.Add(started.Value);
        return Result<EnrollStudentResult>.Success(new EnrollStudentResult(started.Value.Id));
    }
}
