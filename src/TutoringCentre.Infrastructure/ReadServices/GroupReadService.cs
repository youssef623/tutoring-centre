using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of Group: no-tracking projections joined to subjects and teachers for their names. Tenancy comes
/// from the EF query filter and row-level security; ownership comes from <see cref="TeacherScope"/>, applied as
/// a predicate in the same SQL so out-of-scope rows never leave the database. A <see cref="TeacherScope.None"/>
/// scope returns nothing without running a query.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GroupReadService(AppDbContext db) : IGroupReadService
{
    private const int ListCap = 500;

    public async Task<IReadOnlyList<GroupDto>> ListAsync(GroupListCriteria criteria, TeacherScope scope, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        var groups = Scoped(scope);
        if (groups is null)
        {
            return [];
        }

        if (!criteria.IncludeArchived)
        {
            groups = groups.Where(group => group.Status == GroupStatus.Active);
        }

        if (criteria.SubjectId is { } subjectId)
        {
            groups = groups.Where(group => group.SubjectId == subjectId);
        }

        if (criteria.TeacherId is { } teacherId)
        {
            groups = groups.Where(group => group.TeacherId == teacherId);
        }

        return await Project(groups.OrderBy(group => group.NormalizedName).Take(ListCap)).ToListAsync(ct);
    }

    public async Task<GroupDto?> GetAsync(Guid id, TeacherScope scope, CancellationToken ct)
    {
        var groups = Scoped(scope);
        if (groups is null)
        {
            return null;
        }

        return await Project(groups.Where(group => group.Id == id)).SingleOrDefaultAsync(ct);
    }

    /// <summary>The groups the scope allows, or null when it allows none. Fails closed on any unknown kind.</summary>
    private IQueryable<Group>? Scoped(TeacherScope scope)
    {
        ArgumentNullException.ThrowIfNull(scope);
        var groups = db.Set<Group>().AsNoTracking();
        return scope.Kind switch
        {
            TeacherScopeKind.All => groups,
            TeacherScopeKind.Own when scope.TeacherId is { } ownTeacherId => groups.Where(group => group.TeacherId == ownTeacherId),
            _ => null,
        };
    }

    private IQueryable<GroupDto> Project(IQueryable<Group> groups) =>
        from grp in groups
        join subject in db.Set<Subject>().AsNoTracking() on grp.SubjectId equals subject.Id
        join teacher in db.Set<Teacher>().AsNoTracking() on grp.TeacherId equals teacher.Id
        select new GroupDto(
            grp.Id,
            grp.Name,
            grp.SubjectId,
            subject.Name,
            grp.TeacherId,
            teacher.FullName,
            grp.Capacity,
            0,
            grp.Status,
            EF.Property<uint>(grp, "Version"));
}
