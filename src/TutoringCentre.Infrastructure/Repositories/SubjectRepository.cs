using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Subject write-side port. It tracks entities; the unit of work saves them. No
/// centre predicate anywhere: the EF query filter and row-level security (Week 1, Days 20-21) already scope
/// every query to the acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class SubjectRepository(AppDbContext db) : ISubjectRepository
{
    public async Task<Subject?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var subject = await db.Set<Subject>().SingleOrDefaultAsync(subject => subject.Id == id, ct);
        if (subject is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time (Task 23.2's shadow
        // "Version" property, mapped to xmin): a stale client is refused by zero rows affected, not by a
        // comparison here.
        db.Entry(subject).Property("Version").OriginalValue = expectedVersion;
        return subject;
    }

    public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>
        db.Set<Subject>().AnyAsync(subject => subject.NormalizedName == normalizedName && subject.Id != excludingId, ct);

    public void Add(Subject subject)
    {
        ArgumentNullException.ThrowIfNull(subject);
        db.Set<Subject>().Add(subject);
    }
}
