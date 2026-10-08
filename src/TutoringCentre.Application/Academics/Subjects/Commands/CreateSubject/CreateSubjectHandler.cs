using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;

/// <summary>Orchestrates: Domain invariants → set-based uniqueness → add. The dispatcher saves and commits.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CreateSubjectHandler(ICurrentActor currentActor, ISubjectRepository subjects)
    : ICommandHandler<CreateSubjectCommand, CreateSubjectResult>
{
    public async Task<Result<CreateSubjectResult>> HandleAsync(CreateSubjectCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre (Day 22).
        var centreId = currentActor.Actor.CentreId!.Value;

        var created = Subject.Create(centreId, command.Name);
        if (created.IsFailure)
        {
            return Result<CreateSubjectResult>.Failure(created.Error!);
        }

        // Set-based rule: an invariant the entity alone cannot check, backed by the unique index (Day 23).
        if (await subjects.NameExistsAsync(created.Value.NormalizedName, excludingId: null, cancellationToken))
        {
            var fields = new Dictionary<string, string[]> { ["name"] = ["A subject with this name already exists."] };
            return Result<CreateSubjectResult>.Failure(
                new Error("subject.name_taken", "A subject with this name already exists.", ErrorKind.Conflict, fields));
        }

        subjects.Add(created.Value);
        return Result<CreateSubjectResult>.Success(new CreateSubjectResult(created.Value.Id));
    }
}
