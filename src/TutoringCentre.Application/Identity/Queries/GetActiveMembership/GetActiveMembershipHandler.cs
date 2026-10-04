using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Identity.Queries.GetActiveMembership;

/// <summary>
/// The only way a centre ID enters a session. Returns the identical failure (tenant.no_membership) whether the
/// centre does not exist, the user is not a member, or the membership is inactive — NotFound would let a caller
/// probe which centre IDs exist.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetActiveMembershipHandler(ICurrentActor currentActor, IMembershipReadService readService)
    : IQueryHandler<GetActiveMembershipQuery, ActiveMembershipDto>
{
    public async Task<Result<ActiveMembershipDto>> HandleAsync(GetActiveMembershipQuery query, CancellationToken cancellationToken)
    {
        if (currentActor.Actor is not StaffActor staffActor)
        {
            return Result<ActiveMembershipDto>.Failure(Error.Unauthenticated("auth.not_authenticated", "Sign in to continue."));
        }

        var membership = await readService.GetActiveMembershipAsync(staffActor.UserId, query.CentreId, cancellationToken);
        if (membership is null)
        {
            return Result<ActiveMembershipDto>.Failure(
                Error.Forbidden("tenant.no_membership", "You do not have an active membership in this centre."));
        }

        return Result<ActiveMembershipDto>.Success(membership);
    }
}
