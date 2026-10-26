using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Identity.Queries.ValidateStaffSession;

/// <summary>Does not depend on the current actor: called before one exists, with the user ID the session claims.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ValidateStaffSessionHandler(IMembershipReadService readService)
    : IQueryHandler<ValidateStaffSessionQuery, bool>
{
    public async Task<Result<bool>> HandleAsync(ValidateStaffSessionQuery query, CancellationToken cancellationToken)
    {
        var state = await readService.GetSessionStateAsync(query.UserId, query.CentreId, cancellationToken);

        // False for: unknown user (no state), a changed security stamp, (when a centre is set) an inactive
        // membership, or a role the session claims that no longer matches the membership's current one —
        // GetSessionStateAsync reports MembershipActive true and CurrentRole null when no centre is given,
        // matching a cookie that likewise carries no role without a centre (SessionPrincipalFactory).
        var isValid = state is not null
            && state.SecurityStamp == query.SecurityStamp
            && state.MembershipActive
            && state.CurrentRole == query.Role;

        return Result<bool>.Success(isValid);
    }
}
