using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Academics.Scheduling;

/// <summary>
/// Write-side persistence port for class sessions. Methods are named by use; nothing here saves — the dispatcher
/// does. No centre parameter anywhere. Deliberately no bulk update and no overlap query: sessions change only as
/// tracked entities (so the audit interceptor and the tenant write guard see every change), and overlap is decided
/// by the database's exclusion constraints at save time.
/// </summary>
public interface IClassSessionRepository
{
    /// <summary>
    /// Loads one session for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version. Returns null when no such session is visible to the acting
    /// centre.
    /// </summary>
    Task<ClassSession?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>
    /// The local dates from <paramref name="from"/> to <paramref name="through"/> (both inclusive) that already have a
    /// session of this rule, scheduled or cancelled — generation skips them, so a cancelled date is never refilled.
    /// </summary>
    Task<IReadOnlyList<DateOnly>> ListExistingDatesAsync(Guid ruleId, DateOnly from, DateOnly through, CancellationToken ct);

    /// <summary>The rule's still-scheduled sessions on local dates after <paramref name="afterDate"/>, tracked for modification.</summary>
    Task<IReadOnlyList<ClassSession>> ListScheduledAfterAsync(Guid ruleId, DateOnly afterDate, CancellationToken ct);

    /// <summary>The group's still-scheduled sessions that start after <paramref name="now"/>, tracked for modification.</summary>
    Task<IReadOnlyList<ClassSession>> ListFutureScheduledForGroupAsync(Guid groupId, DateTimeOffset now, CancellationToken ct);

    /// <summary>Tracks new sessions; the dispatcher saves them.</summary>
    void AddRange(IEnumerable<ClassSession> sessions);

    /// <summary>
    /// Holds each teacher's schedule lock for the rest of the current transaction, waiting for any other holder
    /// (Day 53). Every handler that gives a teacher sessions takes it first, so two colliding writes for one teacher
    /// run one after the other: the second meets the first's committed sessions and the exclusion constraint answers
    /// it with a double-booking conflict, instead of both inserting at once and PostgreSQL ending one in a deadlock.
    /// Taken in ascending ID order whatever order is given, so two writers needing the same teachers cannot deadlock
    /// on the locks themselves. Released by the dispatcher's commit or rollback.
    /// </summary>
    Task LockTeacherSchedulesAsync(IEnumerable<Guid> teacherIds, CancellationToken ct);
}
