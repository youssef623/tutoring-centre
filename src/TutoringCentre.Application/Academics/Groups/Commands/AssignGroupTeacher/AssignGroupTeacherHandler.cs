using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;

/// <summary>
/// Orchestrates: scoped group lookup → resolve the new teacher inside the acting centre → the entity's change →
/// move the group's future scheduled sessions to the new teacher in the same transaction, so the old teacher is
/// not left booked. If the new teacher is busy at one of those times, the exclusion constraint refuses the save
/// and the whole reassignment rolls back. Past and cancelled sessions keep their teacher. No audit code: the audit
/// policy records teacherId before and after, on the group and on each moved session.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class AssignGroupTeacherHandler(
    IClock clock,
    IGroupRepository groups,
    ITeacherRepository teachers,
    IClassSessionRepository sessions)
    : ICommandHandler<AssignGroupTeacherCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(AssignGroupTeacherCommand command, CancellationToken cancellationToken)
    {
        var group = await groups.GetByIdAsync(command.GroupId, command.Version, cancellationToken);
        if (group is null)
        {
            return Result<Unit>.Failure(GroupErrors.NotFound());
        }

        var teacher = await teachers.FindAsync(command.TeacherId, cancellationToken);
        if (teacher is null)
        {
            return Result<Unit>.Failure(GroupErrors.TeacherNotFound());
        }

        if (teacher.Status == TeacherStatus.Archived)
        {
            return Result<Unit>.Failure(GroupErrors.TeacherArchived());
        }

        // Both teachers, before either's sessions change: a concurrent schedule write for the new teacher is then
        // refused by the constraint, not ended in a deadlock.
        await sessions.LockTeacherSchedulesAsync([group.TeacherId, teacher.Id], cancellationToken);

        var assigned = group.AssignTeacher(teacher.Id);
        if (assigned.IsFailure)
        {
            return Result<Unit>.Failure(assigned.Error!);
        }

        var now = clock.UtcNow;
        foreach (var session in await sessions.ListFutureScheduledForGroupAsync(group.Id, now, cancellationToken))
        {
            var reassigned = session.ReassignTeacher(teacher.Id, now);
            if (reassigned.IsFailure)
            {
                return Result<Unit>.Failure(reassigned.Error!);
            }
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
