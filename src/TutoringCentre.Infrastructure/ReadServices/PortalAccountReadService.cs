using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class PortalAccountReadService(AppDbContext db) : IPortalAccountReadService
{
    public async Task<IReadOnlyList<PortalAccountDto>> ListAsync(Guid centreId, PortalSubjectKind kind, Guid subjectId, CancellationToken ct) =>
        await (from link in db.Set<PortalLink>().AsNoTracking()
               where link.CentreId == centreId && ((kind == PortalSubjectKind.Parent && link.ParentId == subjectId)
                   || (kind == PortalSubjectKind.Student && link.StudentId == subjectId))
               join membership in db.Set<Membership>().AsNoTracking() on new { link.CentreId, link.UserId }
                   equals new { membership.CentreId, membership.UserId }
               // Memberships are deliberately exempt from RLS; this predicate is mandatory.
               where membership.CentreId == centreId
               join user in db.Set<ApplicationUser>().AsNoTracking() on link.UserId equals user.Id
               select new PortalAccountDto(link.Id, membership.Id, user.Email ?? string.Empty, membership.Status,
                   EF.Property<uint>(membership, "Version"), EF.Property<DateTimeOffset>(link, "CreatedAt"))).ToListAsync(ct);
}
