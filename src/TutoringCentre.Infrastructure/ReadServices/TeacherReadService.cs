using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of Teacher: no-tracking projections, left-joined to <c>identity.users</c> only for the linked
/// account's display name (never its email — that stays behind <c>staff.view</c>). No tenant predicate written
/// by hand — the EF query filter and row-level security scope <c>teachers</c> the same way they scope
/// <c>subjects</c>; the join to <c>identity.users</c> exposes one column and nothing else.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class TeacherReadService(AppDbContext db) : ITeacherReadService
{
    private const int ListCap = 500;

    public async Task<IReadOnlyList<TeacherDto>> ListAsync(bool includeArchived, CancellationToken ct)
    {
        var rows = await (
            from teacher in db.Set<Teacher>().AsNoTracking()
            join user in db.Set<ApplicationUser>().AsNoTracking() on teacher.UserId equals user.Id into linkedUsers
            from linkedUser in linkedUsers.DefaultIfEmpty()
            where includeArchived || teacher.Status == TeacherStatus.Active
            orderby teacher.SearchName
            select new
            {
                teacher,
                UserDisplayName = (string?)linkedUser.DisplayName,
                ActiveGroupCount = db.Set<Group>().Count(taught => taught.TeacherId == teacher.Id && taught.Status == GroupStatus.Active),
                Version = EF.Property<uint>(teacher, "Version"),
            })
            .Take(ListCap)
            .ToListAsync(ct);

        return rows.Select(row => ToDto(row.teacher, row.UserDisplayName, row.ActiveGroupCount, row.Version)).ToList();
    }

    public async Task<TeacherDto?> GetAsync(Guid id, CancellationToken ct)
    {
        var row = await (
            from teacher in db.Set<Teacher>().AsNoTracking()
            join user in db.Set<ApplicationUser>().AsNoTracking() on teacher.UserId equals user.Id into linkedUsers
            from linkedUser in linkedUsers.DefaultIfEmpty()
            where teacher.Id == id
            select new
            {
                teacher,
                UserDisplayName = (string?)linkedUser.DisplayName,
                ActiveGroupCount = db.Set<Group>().Count(taught => taught.TeacherId == teacher.Id && taught.Status == GroupStatus.Active),
                Version = EF.Property<uint>(teacher, "Version"),
            })
            .SingleOrDefaultAsync(ct);

        return row is null ? null : ToDto(row.teacher, row.UserDisplayName, row.ActiveGroupCount, row.Version);
    }

    public Task<Guid?> FindIdByUserAsync(Guid userId, CancellationToken ct) =>
        db.Set<Teacher>()
            .AsNoTracking()
            .Where(teacher => teacher.UserId == userId && teacher.Status == TeacherStatus.Active)
            .Select(teacher => (Guid?)teacher.Id)
            .SingleOrDefaultAsync(ct);

    private static TeacherDto ToDto(Teacher teacher, string? userDisplayName, int activeGroupCount, uint version) => new(
        teacher.Id,
        teacher.FullName,
        teacher.Phone?.Value,
        teacher.Status,
        teacher.UserId,
        userDisplayName,
        activeGroupCount,
        version);
}
