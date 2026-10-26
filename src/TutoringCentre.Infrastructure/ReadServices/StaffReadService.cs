using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of staff listing. Unlike <see cref="SubjectReadService"/>, the centre predicate is written by
/// hand below: identity.memberships has no EF query filter and no row-level security (the Day 29 contract's
/// documented exemption — a membership is read at login, before a tenant is selected).
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class StaffReadService(AppDbContext db) : IStaffReadService
{
    public async Task<IReadOnlyList<StaffMemberDto>> ListAsync(Guid centreId, Guid currentUserId, CancellationToken ct) =>
        await Query(centreId, currentUserId).ToListAsync(ct);

    private IQueryable<StaffMemberDto> Query(Guid centreId, Guid currentUserId) =>
        from membership in db.Set<Membership>().AsNoTracking()
        where membership.CentreId == centreId
        join user in db.Set<ApplicationUser>().AsNoTracking() on membership.UserId equals user.Id
        orderby user.DisplayName
        select new StaffMemberDto(
            membership.Id,
            user.Id,
            user.DisplayName,
            user.Email!,
            membership.Role,
            membership.Status,
            EF.Property<uint>(membership, "Version"),
            user.Id == currentUserId);
}
