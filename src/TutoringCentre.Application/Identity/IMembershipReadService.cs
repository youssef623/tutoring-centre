using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity;

/// <summary>
/// Read-side port for a staff member's own profile, memberships and session state. Every operation takes the
/// user ID explicitly and returns only that user's data — never another user's, never an entity, never IQueryable.
/// </summary>
public interface IMembershipReadService
{
    Task<StaffProfileDto?> GetProfileAsync(Guid userId, CancellationToken ct);

    Task<ActiveMembershipDto?> GetActiveMembershipAsync(Guid userId, Guid centreId, CancellationToken ct);

    Task<SessionStateDto?> GetSessionStateAsync(Guid userId, Guid? centreId, CancellationToken ct);
}

/// <summary>A staff member's profile and their active memberships only.</summary>
public sealed record StaffProfileDto(
    Guid UserId,
    string DisplayName,
    string Email,
    string PreferredLocale,
    IReadOnlyList<MembershipDto> Memberships);

/// <summary>One active membership, as seen from the owning user's profile.</summary>
public sealed record MembershipDto(Guid CentreId, string CentreName, string CentreSlug, StaffRole Role);

/// <summary>The user's active membership in one specific centre — the tenant gate's answer.</summary>
public sealed record ActiveMembershipDto(Guid CentreId, string CentreName, StaffRole Role);

/// <summary>What the authentication pipeline needs to decide whether an existing session is still valid.</summary>
public sealed record SessionStateDto(string SecurityStamp, bool MembershipActive);
