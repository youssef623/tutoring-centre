using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Enrollments;

/// <summary>
/// Write-side persistence port for the Enrollment aggregate. Methods are named by use; nothing here saves — the
/// dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query filter
/// and row-level security. Deliberately no way to count active enrollments: a count read without the group's row
/// lock would reopen the overbooking race, so the only count lives in
/// <see cref="Groups.IGroupRepository.GetForEnrollmentAsync"/>.
/// </summary>
public interface IEnrollmentRepository
{
    /// <summary>
    /// Loads one enrollment for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version. Returns null when no such enrollment is visible to the
    /// acting actor's centre.
    /// </summary>
    Task<Enrollment?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>Whether this student already holds an active enrollment in this group.</summary>
    Task<bool> ActiveExistsAsync(Guid studentId, Guid groupId, CancellationToken ct);

    /// <summary>Whether this group has any active enrollment.</summary>
    Task<bool> AnyActiveForGroupAsync(Guid groupId, CancellationToken ct);

    /// <summary>Whether this student has any active enrollment.</summary>
    Task<bool> AnyActiveForStudentAsync(Guid studentId, CancellationToken ct);

    /// <summary>Tracks a new enrollment; the dispatcher saves it.</summary>
    void Add(Enrollment enrollment);
}
