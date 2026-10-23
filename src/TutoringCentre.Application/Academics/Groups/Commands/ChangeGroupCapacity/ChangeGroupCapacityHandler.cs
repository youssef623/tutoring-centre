using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;

/// <summary>Orchestrates: scoped lookup → the range rule on the entity. Its own command so a later lock stays local to it.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ChangeGroupCapacityHandler(IGroupRepository groups) : ICommandHandler<ChangeGroupCapacityCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ChangeGroupCapacityCommand command, CancellationToken cancellationToken)
    {
        var group = await groups.GetByIdAsync(command.GroupId, command.Version, cancellationToken);
        if (group is null)
        {
            return Result<Unit>.Failure(GroupErrors.NotFound());
        }

        // No enrollments can exist yet: Task 48.8 replaces this 0 with the real active count, read under the
        // group's row lock.
        var changed = group.ChangeCapacity(command.Capacity, activeEnrollmentCount: 0);
        return changed.IsFailure ? Result<Unit>.Failure(changed.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
