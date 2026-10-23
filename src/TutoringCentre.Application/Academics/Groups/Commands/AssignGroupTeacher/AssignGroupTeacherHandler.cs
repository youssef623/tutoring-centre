using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;

/// <summary>
/// Orchestrates: scoped group lookup → resolve the new teacher inside the acting centre → the entity's change.
/// No audit code: the audit policy records teacherId before and after.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class AssignGroupTeacherHandler(IGroupRepository groups, ITeacherRepository teachers)
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

        var assigned = group.AssignTeacher(teacher.Id);
        return assigned.IsFailure ? Result<Unit>.Failure(assigned.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
