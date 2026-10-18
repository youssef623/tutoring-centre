using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Identity.Contracts;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Identity;

/// <summary>
/// EF Core implementation of <see cref="IStaffDirectory"/>. The centre condition is written by hand, like
/// <see cref="TutoringCentre.Infrastructure.ReadServices.StaffReadService"/>'s: identity.memberships has no EF
/// query filter and no row-level security (Day 29's documented exemption — a membership is read at login,
/// before a tenant is selected), so nothing underneath this query narrows it to one centre automatically.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class StaffDirectory(AppDbContext db) : IStaffDirectory
{
    public Task<bool> IsActiveMemberWithRoleAsync(Guid centreId, Guid userId, StaffRole role, CancellationToken ct) =>
        db.Set<Membership>()
            .AsNoTracking()
            .AnyAsync(
                membership => membership.CentreId == centreId
                    && membership.UserId == userId
                    && membership.Role == role
                    && membership.Status == MembershipStatus.Active,
                ct);
}
