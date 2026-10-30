using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class PortalLinkRepository(AppDbContext db) : IPortalLinkRepository
{
    public Task<bool> SubjectExistsAsync(Guid centreId, PortalSubjectKind kind, Guid subjectId, CancellationToken ct) =>
        db.Set<PortalLink>().AnyAsync(link => link.CentreId == centreId
            && ((kind == PortalSubjectKind.Parent && link.ParentId == subjectId)
                || (kind == PortalSubjectKind.Student && link.StudentId == subjectId)), ct);

    public Task<PortalLink?> GetAsync(Guid linkId, Guid centreId, CancellationToken ct) =>
        db.Set<PortalLink>().SingleOrDefaultAsync(link => link.CentreId == centreId && link.Id == linkId, ct);

    public async Task<Membership?> GetMembershipForUpdateAsync(PortalLink link, uint expectedVersion, CancellationToken ct)
    {
        var role = Enum.Parse<StaffRole>(link.RoleKey, true);
        var membership = await db.Set<Membership>().SingleOrDefaultAsync(member => member.CentreId == link.CentreId
            && member.UserId == link.UserId && member.Role == role, ct);
        if (membership is not null)
            db.Entry(membership).Property("Version").OriginalValue = expectedVersion;
        return membership;
    }

    public void Add(PortalLink link) => db.Set<PortalLink>().Add(link);
}
