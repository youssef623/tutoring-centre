using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of Parent: no-tracking projections straight to DTOs. No tenant predicate written by hand — the
/// EF query filter and row-level security scope <c>parents</c>, <c>student_parents</c> and <c>students</c> the
/// same way, so the detail join can never reach another centre's row on any of the three tables. The phone
/// column is selected as the mapped <see cref="PhoneNumber"/> and turned into its E.164 string only after
/// materialisation, never inside the SQL-translated part of a projection.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ParentReadService(AppDbContext db) : IParentReadService
{
    private const int ChildrenCap = 50;

    public async Task<ParentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct)
    {
        var parent = await db.Set<Parent>()
            .AsNoTracking()
            .Where(parent => parent.Id == id)
            .Select(parent => new { parent.Id, parent.FullName, parent.Phone, Version = EF.Property<uint>(parent, "Version") })
            .SingleOrDefaultAsync(ct);

        if (parent is null)
        {
            return null;
        }

        var children = await db.Set<StudentParentLink>()
            .AsNoTracking()
            .Where(link => link.ParentId == id)
            .Join(
                db.Set<Student>().AsNoTracking(),
                link => link.StudentId,
                student => student.Id,
                (link, student) => new { link, student })
            .OrderBy(joined => joined.student.FullName)
            .Take(ChildrenCap)
            .Select(joined => new ParentChildDto(
                joined.student.Id, joined.student.FullName, joined.student.Status, joined.link.Relationship, joined.link.IsPrimary))
            .ToListAsync(ct);

        return new ParentDetailDto(parent.Id, parent.FullName, parent.Phone.Value, parent.Version, children);
    }

    public async Task<ParentDto?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct)
    {
        var parent = await db.Set<Parent>()
            .AsNoTracking()
            .Where(parent => parent.Phone == phone)
            .Select(parent => new { parent.Id, parent.FullName, parent.Phone, Version = EF.Property<uint>(parent, "Version") })
            .SingleOrDefaultAsync(ct);

        return parent is null ? null : new ParentDto(parent.Id, parent.FullName, parent.Phone.Value, parent.Version);
    }
}
