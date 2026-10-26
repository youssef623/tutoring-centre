using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;

/// <summary>
/// Orchestrates: locked lookup with the active count → version check → the range and shrink rules on the entity.
/// Takes the same group lock as enrolling (ADR 0011): an unlocked load would let a concurrent enrollment slip in
/// between the count and the shrink and leave more students than seats.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ChangeGroupCapacityHandler(IGroupRepository groups) : ICommandHandler<ChangeGroupCapacityCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ChangeGroupCapacityCommand command, CancellationToken cancellationToken)
    {
        var locked = await groups.GetForEnrollmentAsync(command.GroupId, cancellationToken);
        if (locked is null)
        {
            return Result<Unit>.Failure(GroupErrors.NotFound());
        }

        if (!groups.IsAtVersion(locked.Group, command.Version))
        {
            return Result<Unit>.Failure(GroupErrors.Stale());
        }

        var changed = locked.Group.ChangeCapacity(command.Capacity, locked.ActiveEnrollmentCount);
        return changed.IsFailure ? Result<Unit>.Failure(changed.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
