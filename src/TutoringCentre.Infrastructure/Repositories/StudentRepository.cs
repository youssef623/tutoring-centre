using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Student write-side port. Always includes the parent links: without them,
/// a handler calling RemoveParent or SetPrimaryParent would see an empty collection and the aggregate would
/// wrongly report student.parent_not_linked. No centre predicate anywhere: the EF query filter and row-level
/// security already scope every query to the acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class StudentRepository(AppDbContext db) : IStudentRepository
{
    public async Task<Student?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var student = await db.Set<Student>().Include(student => student.Parents).SingleOrDefaultAsync(student => student.Id == id, ct);
        if (student is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time: a stale client is
        // refused by zero rows affected, not by a comparison here.
        db.Entry(student).Property("Version").OriginalValue = expectedVersion;
        return student;
    }

    public Task<Student?> FindAsync(Guid id, CancellationToken ct) =>
        db.Set<Student>().Include(student => student.Parents).SingleOrDefaultAsync(student => student.Id == id, ct);

    public void Add(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);
        db.Set<Student>().Add(student);
    }
}
