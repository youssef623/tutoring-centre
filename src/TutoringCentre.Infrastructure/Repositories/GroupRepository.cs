using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Group write-side port. It tracks entities; the unit of work saves them. No
/// centre predicate anywhere: the EF query filter and row-level security already scope every query to the
/// acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GroupRepository(AppDbContext db) : IGroupRepository
{
    public async Task<Group?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var group = await db.Set<Group>().Include(group => group.MonthlyFees).SingleOrDefaultAsync(group => group.Id == id, ct);
        if (group is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time: a stale client is
        // refused by zero rows affected, not by a comparison here.
        db.Entry(group).Property("Version").OriginalValue = expectedVersion;
        return group;
    }

    public Task<Group?> FindAsync(Guid id, CancellationToken ct) =>
        db.Set<Group>().SingleOrDefaultAsync(group => group.Id == id, ct);

    public async Task<GroupForEnrollment?> GetForEnrollmentAsync(Guid groupId, CancellationToken ct)
    {
        // Allow-listed raw SQL (QueryFilterBypassTests): LINQ cannot express FOR UPDATE. Interpolated, so groupId
        // is a parameter, never SQL text. xmin is named explicitly because SELECT * omits system columns and the
        // entity maps its Version to it. EF composes the tenant filter over this as a sub-select, and row-level
        // security applies to it as to any query; PostgreSQL still locks the row the sub-select returns. Tracked,
        // not AsNoTracking: the handler may change the group. The lock waits for any other holder and is released
        // by the dispatcher's commit or rollback.
        var group = await db.Set<Group>()
            .FromSql($"SELECT *, xmin FROM academics.groups WHERE id = {groupId} FOR UPDATE")
            .SingleOrDefaultAsync(ct);
        if (group is null)
        {
            return null;
        }

        // Counted only now, with the lock held: under READ COMMITTED this statement sees every enrollment
        // committed by the transaction that held the lock before us.
        var activeCount = await db.Set<Enrollment>()
            .CountAsync(enrollment => enrollment.GroupId == groupId && enrollment.Status == EnrollmentStatus.Active, ct);
        return new GroupForEnrollment(group, activeCount);
    }

    public bool IsAtVersion(Group group, uint expectedVersion)
    {
        ArgumentNullException.ThrowIfNull(group);
        return db.Entry(group).Property<uint>("Version").CurrentValue == expectedVersion;
    }

    public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>
        db.Set<Group>().AnyAsync(group => group.NormalizedName == normalizedName && group.Id != excludingId, ct);

    public Task<bool> AnyActiveForSubjectAsync(Guid subjectId, CancellationToken ct) =>
        db.Set<Group>().AnyAsync(group => group.SubjectId == subjectId && group.Status == GroupStatus.Active, ct);

    public Task<bool> AnyActiveForTeacherAsync(Guid teacherId, CancellationToken ct) =>
        db.Set<Group>().AnyAsync(group => group.TeacherId == teacherId && group.Status == GroupStatus.Active, ct);

    public void Add(Group group)
    {
        ArgumentNullException.ThrowIfNull(group);
        db.Set<Group>().Add(group);
    }
}
