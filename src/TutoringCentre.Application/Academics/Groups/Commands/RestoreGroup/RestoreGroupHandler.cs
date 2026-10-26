using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;

/// <summary>
/// Orchestrates: scoped lookup → re-validate both references → the reverse transition. A group never comes
/// back under an archived subject or teacher, since either may have been archived while it was closed.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class RestoreGroupHandler(IGroupRepository groups, ISubjectRepository subjects, ITeacherRepository teachers)
    : ICommandHandler<RestoreGroupCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RestoreGroupCommand command, CancellationToken cancellationToken)
    {
        var group = await groups.GetByIdAsync(command.GroupId, command.Version, cancellationToken);
        if (group is null)
        {
            return Result<Unit>.Failure(GroupErrors.NotFound());
        }

        var subject = await subjects.FindAsync(group.SubjectId, cancellationToken);
        if (subject is not { Status: SubjectStatus.Active })
        {
            return Result<Unit>.Failure(GroupErrors.SubjectArchived());
        }

        var teacher = await teachers.FindAsync(group.TeacherId, cancellationToken);
        if (teacher is not { Status: TeacherStatus.Active })
        {
            return Result<Unit>.Failure(GroupErrors.TeacherArchived());
        }

        var restored = group.Restore();
        return restored.IsFailure ? Result<Unit>.Failure(restored.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
