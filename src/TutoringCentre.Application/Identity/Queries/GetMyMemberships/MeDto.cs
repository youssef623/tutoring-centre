using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Queries.GetMyMemberships;

/// <summary>
/// The signed-in staff member's own profile. <see cref="ActiveCentreId"/> and <see cref="ActiveRole"/> are null
/// unless the actor's selected centre is still among the user's active memberships. <see cref="Permissions"/> is
/// computed fresh from <see cref="ActiveRole"/> on every call through the role map — advice for the UI, never
/// stored in the cookie and never the decision itself (the dispatcher's permission step is).
/// </summary>
public sealed record MeDto(
    Guid UserId,
    string DisplayName,
    string Email,
    string PreferredLocale,
    bool MustChangePassword,
    Guid? ActiveCentreId,
    StaffRole? ActiveRole,
    IReadOnlyList<MembershipDto> Memberships,
    IReadOnlyList<string> Permissions);
