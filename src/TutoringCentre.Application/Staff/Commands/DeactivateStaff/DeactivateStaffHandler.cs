using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff.Commands.DeactivateStaff;

/// <summary>
/// Decision order, exactly as ChangeStaffRole's (Day 29 contract): scoped load → self check → last-owner
/// lock, only when the target is an active owner → the entity's existing Deactivate rule. Never deletes a
/// row, never touches the login account or the user's memberships in other centres.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class DeactivateStaffHandler(ICurrentActor currentActor, IMembershipRepository memberships)
    : ICommandHandler<DeactivateStaffCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(DeactivateStaffCommand command, CancellationToken cancellationToken)
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

        if (membership.Status == MembershipStatus.Active && membership.Role == StaffRole.Owner)
        {
            var activeOwnerCount = await memberships.LockActiveOwnersAsync(centreId, cancellationToken);
            if (activeOwnerCount <= 1)
            {
                return Result<Unit>.Failure(StaffErrors.LastOwner());
            }
        }

        var deactivated = membership.Deactivate();
        return deactivated.IsFailure ? Result<Unit>.Failure(deactivated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
