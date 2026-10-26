using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;

/// <summary>Orchestrates: scoped lookup → the reverse state transition, a separate intention from archive with its own rule and error.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class RestoreTeacherHandler(ITeacherRepository teachers) : ICommandHandler<RestoreTeacherCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RestoreTeacherCommand command, CancellationToken cancellationToken)
    {
        var teacher = await teachers.GetByIdAsync(command.TeacherId, command.Version, cancellationToken);
        if (teacher is null)
        {
            return Result<Unit>.Failure(TeacherErrors.NotFound());
        }

        var restored = teacher.Restore();
        return restored.IsFailure ? Result<Unit>.Failure(restored.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
