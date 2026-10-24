using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Enrollment write-side port. It tracks entities; the unit of work saves them. No
/// centre predicate anywhere: the EF query filter and row-level security already scope every query to the
/// acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class EnrollmentRepository(AppDbContext db) : IEnrollmentRepository
{
    public async Task<Enrollment?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var enrollment = await db.Set<Enrollment>().SingleOrDefaultAsync(enrollment => enrollment.Id == id, ct);
        if (enrollment is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time: a stale client is
        // refused by zero rows affected, not by a comparison here.
        db.Entry(enrollment).Property("Version").OriginalValue = expectedVersion;
        return enrollment;
    }

    public Task<bool> ActiveExistsAsync(Guid studentId, Guid groupId, CancellationToken ct) =>
        db.Set<Enrollment>().AnyAsync(
            enrollment => enrollment.StudentId == studentId
                && enrollment.GroupId == groupId
                && enrollment.Status == EnrollmentStatus.Active,
            ct);

    public Task<bool> AnyActiveForGroupAsync(Guid groupId, CancellationToken ct) =>
        db.Set<Enrollment>().AnyAsync(enrollment => enrollment.GroupId == groupId && enrollment.Status == EnrollmentStatus.Active, ct);

    public Task<bool> AnyActiveForStudentAsync(Guid studentId, CancellationToken ct) =>
        db.Set<Enrollment>().AnyAsync(enrollment => enrollment.StudentId == studentId && enrollment.Status == EnrollmentStatus.Active, ct);

    public void Add(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        db.Set<Enrollment>().Add(enrollment);
    }
}
