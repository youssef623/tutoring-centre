using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class PortalDirectory(AppDbContext db, ICurrentActor actor) : IPortalDirectory
{
    public async Task<IReadOnlyList<string>> ListStudentNamesAsync(CancellationToken ct) =>
        await db.Set<Student>().AsNoTracking().Where(student => student.CentreId == actor.CentreId)
            .Select(student => student.FullName).Distinct().ToListAsync(ct);

    public async Task<PortalSubjectFacts?> GetParentAsync(Guid parentId, CancellationToken ct) =>
        await db.Set<Parent>().AsNoTracking()
            .Where(parent => parent.CentreId == actor.CentreId && parent.Id == parentId)
            .Select(parent => new PortalSubjectFacts(parent.Id, parent.FullName, true)).SingleOrDefaultAsync(ct);

    public async Task<PortalSubjectFacts?> GetStudentAsync(Guid studentId, CancellationToken ct) =>
        await db.Set<Student>().AsNoTracking()
            .Where(student => student.CentreId == actor.CentreId && student.Id == studentId)
            .Select(student => new PortalSubjectFacts(student.Id, student.FullName, student.Status == StudentStatus.Active))
            .SingleOrDefaultAsync(ct);

    public async Task<IReadOnlyList<PortalChild>> GetChildrenAsync(Guid parentId, CancellationToken ct) =>
        await (from link in db.Set<StudentParentLink>().AsNoTracking()
               where link.CentreId == actor.CentreId && link.ParentId == parentId
               join student in db.Set<Student>().AsNoTracking() on new { link.CentreId, Id = link.StudentId }
                   equals new { student.CentreId, student.Id }
               where student.CentreId == actor.CentreId && student.Status == StudentStatus.Active
               orderby student.FullName, student.Id
               select new PortalChild(student.Id, student.FullName)).ToListAsync(ct);
}
