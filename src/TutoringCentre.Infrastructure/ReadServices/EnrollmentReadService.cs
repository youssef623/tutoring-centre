using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of Enrollment: no-tracking projections joined to groups, students, subjects and teachers for their
/// names. Tenancy comes from the EF query filter and row-level security; ownership comes from
/// <see cref="TeacherScope"/>, applied as a predicate on the group row in the same SQL. A
/// <see cref="TeacherScope.None"/> scope reads no group.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class EnrollmentReadService(AppDbContext db) : IEnrollmentReadService
{
    public async Task<GroupRosterDto?> GetRosterAsync(Guid groupId, TeacherScope scope, CancellationToken ct)
    {
        var groups = ScopedGroups(scope);
        if (groups is null)
        {
            return null;
        }

        var group = await groups
            .Where(group => group.Id == groupId)
            .Select(group => new { group.Id, group.Name, group.Capacity })
            .SingleOrDefaultAsync(ct);
        if (group is null)
        {
            return null;
        }

        var items = await (
                from enrollment in db.Set<Enrollment>().AsNoTracking()
                join student in db.Set<Student>().AsNoTracking() on enrollment.StudentId equals student.Id
                where enrollment.GroupId == groupId && enrollment.Status == EnrollmentStatus.Active
                orderby student.SearchName, student.Id
                select new RosterEntryDto(
                    enrollment.Id,
                    student.Id,
                    student.FullName,
                    enrollment.EnrolledAt,
                    EF.Property<uint>(enrollment, "Version")))
            .Take(Group.MaxCapacity)
            .ToListAsync(ct);

        // The capacity invariant keeps active enrollments at or below MaxCapacity, so the capped list is the whole roster.
        return new GroupRosterDto(group.Id, group.Name, group.Capacity, items.Count, items);
    }

    public async Task<IReadOnlyList<StudentEnrollmentDto>?> ListForStudentAsync(Guid studentId, TeacherScope scope, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(scope);
        if (!await db.Set<Student>().AsNoTracking().AnyAsync(student => student.Id == studentId, ct))
        {
            return null;
        }

        var groups = ScopedGroups(scope);
        if (groups is null)
        {
            return [];
        }

        return await (
                from enrollment in db.Set<Enrollment>().AsNoTracking()
                join grp in groups on enrollment.GroupId equals grp.Id
                join subject in db.Set<Subject>().AsNoTracking() on grp.SubjectId equals subject.Id
                join teacher in db.Set<Teacher>().AsNoTracking() on grp.TeacherId equals teacher.Id
                where enrollment.StudentId == studentId
                orderby enrollment.Status == EnrollmentStatus.Active ? 0 : 1, enrollment.EnrolledAt descending, enrollment.Id descending
                select new StudentEnrollmentDto(
                    enrollment.Id,
                    grp.Id,
                    grp.Name,
                    subject.Name,
                    teacher.FullName,
                    enrollment.Status,
                    enrollment.EnrolledAt,
                    enrollment.EndedAt,
                    enrollment.EndReason,
                    EF.Property<uint>(enrollment, "Version")))
            .ToListAsync(ct);
    }

    /// <summary>The groups the scope allows, or null when it allows none. Fails closed on any unknown kind.</summary>
    private IQueryable<Group>? ScopedGroups(TeacherScope scope)
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
}
