using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff.Commands.ChangeStaffRole;

/// <summary>
/// Decision order, exactly (Day 29 contract): scoped load → self check → last-owner lock, only when demoting
/// an active owner → the entity's own role-change rules. The lock never runs before the scoped load, and
/// never runs at all unless the target is an active owner losing that role.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ChangeStaffRoleHandler(ICurrentActor currentActor, IMembershipRepository memberships)
    : ICommandHandler<ChangeStaffRoleCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ChangeStaffRoleCommand command, CancellationToken cancellationToken)
    {
        var centreId = currentActor.Actor.CentreId!.Value;

        var membership = await memberships.GetForUpdateAsync(command.MembershipId, centreId, command.Version, cancellationToken);
        if (membership is null)
        {
            return Result<Unit>.Failure(StaffErrors.NotFound());
        }

        if (currentActor.Actor is StaffActor staffActor && staffActor.UserId == membership.UserId)
        {
            return Result<Unit>.Failure(StaffErrors.CannotChangeSelf());
        }

        if (membership.Status == MembershipStatus.Active && membership.Role == StaffRole.Owner && command.Role != StaffRole.Owner)
        {
            var activeOwnerCount = await memberships.LockActiveOwnersAsync(centreId, cancellationToken);
            if (activeOwnerCount <= 1)
            {
                return Result<Unit>.Failure(StaffErrors.LastOwner());
            }
        }

        var changed = membership.ChangeRole(command.Role);
        return changed.IsFailure ? Result<Unit>.Failure(changed.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
