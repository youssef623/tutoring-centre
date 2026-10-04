using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Queries.GetMyMemberships;

/// <summary>
/// The signed-in staff member's own profile. <see cref="ActiveCentreId"/> and <see cref="ActiveRole"/> are null
/// unless the actor's selected centre is still among the user's active memberships.
/// </summary>
public sealed record MeDto(
    Guid UserId,
    string DisplayName,
    string Email,
    string PreferredLocale,
    Guid? ActiveCentreId,
    StaffRole? ActiveRole,
    IReadOnlyList<MembershipDto> Memberships);
