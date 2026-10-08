using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;

/// <summary>Orchestrates: scoped lookup → the state transition, which is behaviour on the entity, not a flag update.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ArchiveSubjectHandler(ISubjectRepository subjects) : ICommandHandler<ArchiveSubjectCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ArchiveSubjectCommand command, CancellationToken cancellationToken)
    {
        var subject = await subjects.GetByIdAsync(command.SubjectId, command.Version, cancellationToken);
        if (subject is null)
        {
            return Result<Unit>.Failure(Error.NotFound("subject.not_found", "This subject could not be found."));
        }

        var archived = subject.Archive();
        return archived.IsFailure ? Result<Unit>.Failure(archived.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
