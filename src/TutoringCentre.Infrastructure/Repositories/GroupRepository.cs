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
        var group = await db.Set<Group>().SingleOrDefaultAsync(group => group.Id == id, ct);
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
