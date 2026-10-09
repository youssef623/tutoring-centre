using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Staff.Commands.ReactivateStaff;

/// <summary>Scoped load → the entity's existing Activate rule. No self or last-owner guard: a deactivated person cannot be the acting actor.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ReactivateStaffHandler(ICurrentActor currentActor, IMembershipRepository memberships)
    : ICommandHandler<ReactivateStaffCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ReactivateStaffCommand command, CancellationToken cancellationToken)
    {
        var centreId = currentActor.Actor.CentreId!.Value;

        var membership = await memberships.GetForUpdateAsync(command.MembershipId, centreId, command.Version, cancellationToken);
        if (membership is null)
        {
            return Result<Unit>.Failure(StaffErrors.NotFound());
        }

        var activated = membership.Activate();
        return activated.IsFailure ? Result<Unit>.Failure(activated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
