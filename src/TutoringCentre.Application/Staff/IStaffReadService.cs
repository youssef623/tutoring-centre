using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff;

/// <summary>
/// Read-side port for staff listing. identity.memberships carries no automatic tenant scoping (the Day 29
/// contract's documented exemption), so the implementation writes the centre predicate itself — never
/// delegated to an EF query filter the way <c>ISubjectReadService</c> can rely on.
/// </summary>
public interface IStaffReadService
{
    /// <summary>Both active and inactive members of the centre, ordered by display name. Never a password hash, security stamp, lockout field, or a membership in another centre.</summary>
    Task<IReadOnlyList<StaffMemberDto>> ListAsync(Guid centreId, Guid currentUserId, CancellationToken ct);
}

public sealed record StaffMemberDto(
    Guid MembershipId,
    Guid UserId,
    string DisplayName,
    string Email,
    StaffRole Role,
    MembershipStatus Status,
    uint Version,
    bool IsCurrentUser);
