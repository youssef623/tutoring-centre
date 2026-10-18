using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Teacher write-side port. It tracks entities; the unit of work saves them. No
/// centre predicate anywhere: the EF query filter and row-level security already scope every query to the
/// acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class TeacherRepository(AppDbContext db) : ITeacherRepository
{
    public async Task<Teacher?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var teacher = await db.Set<Teacher>().SingleOrDefaultAsync(teacher => teacher.Id == id, ct);
        if (teacher is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time: a stale client is
        // refused by zero rows affected, not by a comparison here.
        db.Entry(teacher).Property("Version").OriginalValue = expectedVersion;
        return teacher;
    }

    public Task<Teacher?> FindAsync(Guid id, CancellationToken ct) =>
        db.Set<Teacher>().SingleOrDefaultAsync(teacher => teacher.Id == id, ct);

    public Task<bool> UserLinkedAsync(Guid userId, Guid? excludingTeacherId, CancellationToken ct) =>
        db.Set<Teacher>().AnyAsync(teacher => teacher.UserId == userId && teacher.Id != excludingTeacherId, ct);

    public void Add(Teacher teacher)
    {
        ArgumentNullException.ThrowIfNull(teacher);
        db.Set<Teacher>().Add(teacher);
    }
}
