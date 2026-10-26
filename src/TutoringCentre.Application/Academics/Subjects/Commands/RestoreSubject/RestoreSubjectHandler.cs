using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;

/// <summary>Orchestrates: scoped lookup → the state transition, the opposite of Archive. No uniqueness check needed — an archived subject keeps its name reserved by the unique index.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class RestoreSubjectHandler(ISubjectRepository subjects) : ICommandHandler<RestoreSubjectCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RestoreSubjectCommand command, CancellationToken cancellationToken)
    {
        var subject = await subjects.GetByIdAsync(command.SubjectId, command.Version, cancellationToken);
        if (subject is null)
        {
            return Result<Unit>.Failure(Error.NotFound("subject.not_found", "This subject could not be found."));
        }

        var restored = subject.Restore();
        return restored.IsFailure ? Result<Unit>.Failure(restored.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
