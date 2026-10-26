using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;

/// <summary>
/// Orchestrates: scoped lookup → Domain invariants → set-based uniqueness, excluding the subject itself. A
/// subject in another centre and one that does not exist are indistinguishable: both subject.not_found. The
/// handler never compares versions itself — the save detects a stale one (Day 25).
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class RenameSubjectHandler(ISubjectRepository subjects) : ICommandHandler<RenameSubjectCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RenameSubjectCommand command, CancellationToken cancellationToken)
    {
        var subject = await subjects.GetByIdAsync(command.SubjectId, command.Version, cancellationToken);
        if (subject is null)
        {
            return Result<Unit>.Failure(Error.NotFound("subject.not_found", "This subject could not be found."));
        }

        var previousNormalizedName = subject.NormalizedName;

        var renamed = subject.Rename(command.Name);
        if (renamed.IsFailure)
        {
            return Result<Unit>.Failure(renamed.Error!);
        }

        if (subject.NormalizedName != previousNormalizedName
            && await subjects.NameExistsAsync(subject.NormalizedName, subject.Id, cancellationToken))
        {
            var fields = new Dictionary<string, string[]> { ["name"] = ["A subject with this name already exists."] };
            return Result<Unit>.Failure(
                new Error("subject.name_taken", "A subject with this name already exists.", ErrorKind.Conflict, fields));
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
