using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents.Commands.CreateParent;

/// <summary>Orchestrates: parse the phone → set-based uniqueness → Domain invariants → add. The dispatcher saves and commits.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CreateParentHandler(ICurrentActor currentActor, IParentRepository parents)
    : ICommandHandler<CreateParentCommand, CreateParentResult>
{
    public async Task<Result<CreateParentResult>> HandleAsync(CreateParentCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre (Day 22).
        var centreId = currentActor.Actor.CentreId!.Value;

        var phone = PhoneNumber.Create(command.Phone);
        if (phone.IsFailure)
        {
            return Result<CreateParentResult>.Failure(ParentErrors.PhoneInvalid(phone.Error!));
        }

        // Set-based rule: an invariant the entity alone cannot check, backed by the unique index.
        if (await parents.PhoneExistsAsync(phone.Value, excludingId: null, cancellationToken))
        {
            return Result<CreateParentResult>.Failure(ParentErrors.PhoneTaken());
        }

        var created = Parent.Create(centreId, command.FullName, phone.Value);
        if (created.IsFailure)
        {
            return Result<CreateParentResult>.Failure(created.Error!);
        }

        parents.Add(created.Value);
        return Result<CreateParentResult>.Success(new CreateParentResult(created.Value.Id));
    }
}
