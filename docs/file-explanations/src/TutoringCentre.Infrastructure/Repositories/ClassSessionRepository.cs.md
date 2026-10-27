# `src/TutoringCentre.Infrastructure/Repositories/ClassSessionRepository.cs`

## Purpose

The adapter tracks versioned sessions, projects existing generation dates, loads future or later scheduled occurrences, and takes sorted transaction-scoped advisory teacher locks. It avoids bulk updates because tracking drives auditing and tenant write guards. Existing rule dates provide generation idempotency independently of cancellation status.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Repositories/ClassSessionRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 75 lines and 3,908 bytes. Source hash: `01000020cd8df69581f8cf2eb50b4bf4e33a4ff40aab36ac2617d5e08147a63c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 4: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 5: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../src/TutoringCentre.Infrastructure/Repositories/ClassSessionRepository.cs#L15): `internal sealed class ClassSessionRepository(AppDbContext db) : IClassSessionRepository`
- [Line 17](../../../../../src/TutoringCentre.Infrastructure/Repositories/ClassSessionRepository.cs#L17): `public async Task<ClassSession?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)`
- [Line 31](../../../../../src/TutoringCentre.Infrastructure/Repositories/ClassSessionRepository.cs#L31): `public async Task<IReadOnlyList<DateOnly>> ListExistingDatesAsync(Guid ruleId, DateOnly from, DateOnly through, CancellationToken ct) =>`
- [Line 38](../../../../../src/TutoringCentre.Infrastructure/Repositories/ClassSessionRepository.cs#L38): `public async Task<IReadOnlyList<ClassSession>> ListScheduledAfterAsync(Guid ruleId, DateOnly afterDate, CancellationToken ct) =>`
- [Line 46](../../../../../src/TutoringCentre.Infrastructure/Repositories/ClassSessionRepository.cs#L46): `public async Task<IReadOnlyList<ClassSession>> ListFutureScheduledForGroupAsync(Guid groupId, DateTimeOffset now, CancellationToken ct) =>`
- [Line 54](../../../../../src/TutoringCentre.Infrastructure/Repositories/ClassSessionRepository.cs#L54): `public void AddRange(IEnumerable<ClassSession> sessions)`
- [Line 60](../../../../../src/TutoringCentre.Infrastructure/Repositories/ClassSessionRepository.cs#L60): `public async Task LockTeacherSchedulesAsync(IEnumerable<Guid> teacherIds, CancellationToken ct)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:146](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L146)
- [tests/TutoringCentre.Architecture.Tests/QueryFilterBypassTests.cs:43](../../../../../tests/TutoringCentre.Architecture.Tests/QueryFilterBypassTests.cs#L43)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
