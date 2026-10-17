using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Contracts;

/// <summary>
/// What another module may ask Identity about a staff member, published instead of letting that module reach
/// into Identity's own repositories or read services directly (<see cref="TutoringCentre.Architecture.Tests"/>'s
/// module-boundary rule allows a <c>Contracts</c> namespace specifically for this). Returns a bool and nothing
/// else — never a <c>Membership</c> or a DTO — so a caller cannot learn more about the membership than the one
/// fact it asked for.
/// </summary>
public interface IStaffDirectory
{
    /// <summary>Whether <paramref name="userId"/> is an active member of <paramref name="centreId"/> holding exactly <paramref name="role"/>.</summary>
    Task<bool> IsActiveMemberWithRoleAsync(Guid centreId, Guid userId, StaffRole role, CancellationToken ct);
}
