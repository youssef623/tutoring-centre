using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Queries.GetMyMemberships;

/// <summary>
/// Returns the actor's own profile. Takes no user ID from the caller — "my" queries take identity from the actor,
/// so there is no ID to tamper with.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetMyMembershipsHandler(ICurrentActor currentActor, IMembershipReadService readService)
    : IQueryHandler<GetMyMembershipsQuery, MeDto>
{
    public async Task<Result<MeDto>> HandleAsync(GetMyMembershipsQuery query, CancellationToken cancellationToken)
    {
        if (currentActor.Actor is not StaffActor staffActor)
        {
            return Result<MeDto>.Failure(Error.Unauthenticated("auth.not_authenticated", "Sign in to continue."));
        }

        var profile = await readService.GetProfileAsync(staffActor.UserId, cancellationToken);
        if (profile is null)
        {
            return Result<MeDto>.Failure(Error.Unauthenticated("auth.not_authenticated", "Sign in to continue."));
        }

        // The actor's selected centre only counts if it is still an active membership in the freshly read profile.
        var activeMembership = staffActor.CentreId is { } centreId
            ? profile.Memberships.FirstOrDefault(membership => membership.CentreId == centreId)
            : null;

        var permissions = activeMembership is null
            ? Array.Empty<string>()
            : RolePermissions.For(activeMembership.Role).Order(StringComparer.Ordinal).ToArray();

        var dto = new MeDto(
            profile.UserId,
            profile.DisplayName,
            profile.Email,
            profile.PreferredLocale,
            activeMembership?.CentreId,
            activeMembership?.Role,
            profile.Memberships,
            permissions);

        return Result<MeDto>.Success(dto);
    }
}
