using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Identity;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of a staff member's own profile, memberships and session state. Non-tracking projections only;
/// every query filters by the given user ID, and profile/active-membership results include only active memberships.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class MembershipReadService(AppDbContext db) : IMembershipReadService
{
    public async Task<StaffProfileDto?> GetProfileAsync(Guid userId, CancellationToken ct)
    {
        var user = await db.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new { u.Id, u.DisplayName, u.Email, u.PreferredLocale })
            .SingleOrDefaultAsync(ct);

        if (user is null)
        {
            return null;
        }

        var memberships = await (
            from membership in db.Set<Membership>().AsNoTracking()
            where membership.UserId == userId && membership.Status == MembershipStatus.Active
            join centre in db.Set<Centre>().AsNoTracking() on membership.CentreId equals centre.Id
            select new MembershipDto(centre.Id, centre.Name, centre.Slug, membership.Role))
            .ToListAsync(ct);

        return new StaffProfileDto(user.Id, user.DisplayName, user.Email ?? string.Empty, user.PreferredLocale, memberships);
    }

    public async Task<ActiveMembershipDto?> GetActiveMembershipAsync(Guid userId, Guid centreId, CancellationToken ct) =>
        await (
            from membership in db.Set<Membership>().AsNoTracking()
            where membership.UserId == userId && membership.CentreId == centreId && membership.Status == MembershipStatus.Active
            join centre in db.Set<Centre>().AsNoTracking() on membership.CentreId equals centre.Id
            select new ActiveMembershipDto(centre.Id, centre.Name, membership.Role))
            .SingleOrDefaultAsync(ct);

    public async Task<SessionStateDto?> GetSessionStateAsync(Guid userId, Guid? centreId, CancellationToken ct)
    {
        var stamp = await db.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.SecurityStamp)
            .SingleOrDefaultAsync(ct);

        if (stamp is null)
        {
            return null;
        }

        if (centreId is null)
        {
            return new SessionStateDto(stamp, MembershipActive: true, CurrentRole: null);
        }

        var membership = await db.Set<Membership>()
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.CentreId == centreId)
            .Select(m => new { m.Status, m.Role })
            .SingleOrDefaultAsync(ct);

        var membershipActive = membership is not null && membership.Status == MembershipStatus.Active;
        return new SessionStateDto(stamp, membershipActive, membership?.Role);
    }
}
