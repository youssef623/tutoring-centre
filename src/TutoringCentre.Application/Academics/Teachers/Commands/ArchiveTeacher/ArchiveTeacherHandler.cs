using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;

/// <summary>Orchestrates: scoped lookup → the state transition, which is behaviour on the entity, not a flag update. The account link is untouched.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ArchiveTeacherHandler(ITeacherRepository teachers) : ICommandHandler<ArchiveTeacherCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ArchiveTeacherCommand command, CancellationToken cancellationToken)
    {
        var teacher = await teachers.GetByIdAsync(command.TeacherId, command.Version, cancellationToken);
        if (teacher is null)
        {
            return Result<Unit>.Failure(TeacherErrors.NotFound());
        }

        var archived = teacher.Archive();
        return archived.IsFailure ? Result<Unit>.Failure(archived.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
