using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;

/// <summary>
/// Orchestrates: parse the phone → scoped lookup → Domain invariants → set-based uniqueness, excluding the
/// parent itself and only run when the phone actually changed. A parent in another centre and one that does
/// not exist are indistinguishable: both parent.not_found. The handler never compares versions itself — the
/// save detects a stale one.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class UpdateParentHandler(IParentRepository parents) : ICommandHandler<UpdateParentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(UpdateParentCommand command, CancellationToken cancellationToken)
    {
        var phone = PhoneNumber.Create(command.Phone);
        if (phone.IsFailure)
        {
            return Result<Unit>.Failure(ParentErrors.PhoneInvalid(phone.Error!));
        }

        var parent = await parents.GetByIdAsync(command.ParentId, command.Version, cancellationToken);
        if (parent is null)
        {
            return Result<Unit>.Failure(ParentErrors.NotFound());
        }

        var previousPhone = parent.Phone;

        var updated = parent.Update(command.FullName, phone.Value);
        if (updated.IsFailure)
        {
            return Result<Unit>.Failure(updated.Error!);
        }

        if (!parent.Phone.Equals(previousPhone)
            && await parents.PhoneExistsAsync(parent.Phone, parent.Id, cancellationToken))
        {
            return Result<Unit>.Failure(ParentErrors.PhoneTaken());
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
