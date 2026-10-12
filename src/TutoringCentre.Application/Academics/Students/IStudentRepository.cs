using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Students;

/// <summary>
/// Write-side persistence port for the Student aggregate. Every load includes the parent links — the
/// invariant can only be checked on the whole aggregate, never on the root alone. Methods are named by use;
/// nothing here saves — the dispatcher does. No centre parameter anywhere: scoping is applied underneath this
/// port by the EF query filter and row-level security, so this interface cannot be called with the wrong
/// tenant. A student of another centre is simply not found by either load.
/// </summary>
public interface IStudentRepository
{
    /// <summary>
    /// Loads one student with its parent links for modification, expecting it to still be at
    /// <paramref name="expectedVersion"/>; the save, not the handler, detects a stale version.
    /// </summary>
    Task<Student?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>Loads one student with its parent links for modification with no version expectation — enrollment uses this on Day 48.</summary>
    Task<Student?> FindAsync(Guid id, CancellationToken ct);

    /// <summary>Tracks a new student; the dispatcher saves it.</summary>
    void Add(Student student);
}
