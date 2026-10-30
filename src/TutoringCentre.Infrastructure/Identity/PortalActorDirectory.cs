using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Identity.Contracts;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;
namespace TutoringCentre.Infrastructure.Identity;
public sealed class PortalActorDirectory(AppDbContext db) : IPortalActorDirectory
{
    public Task<bool> IsActiveAsync(Guid userId, Guid subjectId, bool isStudent, CancellationToken ct) =>
        (from link in db.Set<PortalLink>()
         join member in db.Set<Membership>() on new { link.CentreId, link.UserId } equals new { member.CentreId, member.UserId }
         where link.UserId == userId && member.Status == MembershipStatus.Active
            && member.Role == (isStudent ? StaffRole.Student : StaffRole.Parent)
            && (isStudent ? link.StudentId == subjectId : link.ParentId == subjectId)
         select link.Id).AnyAsync(ct);
}
