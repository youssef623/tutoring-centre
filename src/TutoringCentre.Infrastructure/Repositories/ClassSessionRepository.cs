using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the class-session write-side port. It tracks entities; the unit of work saves them.
/// No centre predicate anywhere, and no ExecuteUpdate: a bulk update would skip the change tracker and with it
/// the audit interceptor and the tenant write guard. The counts involved are a few weeks of one rule or group.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ClassSessionRepository(AppDbContext db) : IClassSessionRepository
{
    public async Task<ClassSession?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var session = await db.Set<ClassSession>().SingleOrDefaultAsync(session => session.Id == id, ct);
        if (session is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time: a stale client is
        // refused by zero rows affected, not by a comparison here.
        db.Entry(session).Property("Version").OriginalValue = expectedVersion;
        return session;
    }

    public async Task<IReadOnlyList<DateOnly>> ListExistingDatesAsync(Guid ruleId, DateOnly from, DateOnly through, CancellationToken ct) =>
        await db.Set<ClassSession>()
            .AsNoTracking()
            .Where(session => session.ScheduleRuleId == ruleId && session.LocalDate >= from && session.LocalDate <= through)
            .Select(session => session.LocalDate)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ClassSession>> ListScheduledAfterAsync(Guid ruleId, DateOnly afterDate, CancellationToken ct) =>
        await db.Set<ClassSession>()
            .Where(session => session.ScheduleRuleId == ruleId
                && session.LocalDate > afterDate
                && session.Status == ClassSessionStatus.Scheduled)
            .OrderBy(session => session.StartsAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ClassSession>> ListFutureScheduledForGroupAsync(Guid groupId, DateTimeOffset now, CancellationToken ct) =>
        await db.Set<ClassSession>()
            .Where(session => session.GroupId == groupId
                && session.StartsAt > now
                && session.Status == ClassSessionStatus.Scheduled)
            .OrderBy(session => session.StartsAt)
            .ToListAsync(ct);

    public void AddRange(IEnumerable<ClassSession> sessions)
    {
        ArgumentNullException.ThrowIfNull(sessions);
        db.Set<ClassSession>().AddRange(sessions);
    }

    public async Task LockTeacherSchedulesAsync(IEnumerable<Guid> teacherIds, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(teacherIds);

        // Allow-listed raw SQL (QueryFilterBypassTests): an advisory lock has no LINQ form. Transaction-scoped, so the
        // dispatcher's commit or rollback releases it and nothing can leak to the next request on this connection.
        // The key is a 64-bit hash of a namespaced teacher ID: a collision between two teachers only makes their
        // writes wait for each other. One statement per teacher, in ascending order, so the acquisition order is
        // fixed by this loop rather than left to the planner. Interpolated, so the ID is a parameter, never SQL text.
        foreach (var teacherId in teacherIds.Distinct().Order())
        {
            var key = $"teacher-schedule:{teacherId}";
            await db.Database.ExecuteSqlAsync($"SELECT pg_advisory_xact_lock(hashtextextended({key}, 0))", ct);
        }
    }
}
