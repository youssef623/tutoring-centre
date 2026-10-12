using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;

/// <summary>Orchestrates: scoped lookup → the state transition, which is behaviour on the entity, not a flag update.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ArchiveStudentHandler(IStudentRepository students) : ICommandHandler<ArchiveStudentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ArchiveStudentCommand command, CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(command.StudentId, command.Version, cancellationToken);
        if (student is null)
        {
            return Result<Unit>.Failure(StudentErrors.NotFound());
        }

        var archived = student.Archive();
        return archived.IsFailure ? Result<Unit>.Failure(archived.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
