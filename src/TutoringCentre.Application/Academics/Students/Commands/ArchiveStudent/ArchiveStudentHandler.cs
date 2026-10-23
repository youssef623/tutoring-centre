using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;

/// <summary>Orchestrates: scoped lookup → no active enrollment → the state transition, which is behaviour on the entity, not a flag update.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ArchiveStudentHandler(IStudentRepository students, IEnrollmentRepository enrollments)
    : ICommandHandler<ArchiveStudentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ArchiveStudentCommand command, CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(command.StudentId, command.Version, cancellationToken);
        if (student is null)
        {
            return Result<Unit>.Failure(StudentErrors.NotFound());
        }

        if (await enrollments.AnyActiveForStudentAsync(student.Id, cancellationToken))
        {
            return Result<Unit>.Failure(Error.Rule("student.has_active_enrollments", "This student is still enrolled in a group."));
        }

        var archived = student.Archive();
        return archived.IsFailure ? Result<Unit>.Failure(archived.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
