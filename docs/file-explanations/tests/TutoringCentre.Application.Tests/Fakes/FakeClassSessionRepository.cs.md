# `tests/TutoringCentre.Application.Tests/Fakes/FakeClassSessionRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory class-session repository. `Existing` simulates stored rows (built with the real SessionGenerator, the only way to obtain a session); `Added` records AddRange calls. No overlap check: the exclusion constraints are proven against a real database, not here.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClassSessionRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 48 lines and 2,512 bytes. Source hash: `5e1bf808645f543e1a6d1586e5243ac19f159144bdda714f0739a490778d0cee`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 2: `using TutoringCentre.Domain.Academics.Scheduling;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClassSessionRepository.cs#L11): `public sealed class FakeClassSessionRepository : IClassSessionRepository`
- [Line 17](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClassSessionRepository.cs#L17): `public Task<ClassSession?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>`
- [Line 20](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClassSessionRepository.cs#L20): `public Task<IReadOnlyList<DateOnly>> ListExistingDatesAsync(Guid ruleId, DateOnly from, DateOnly through, CancellationToken ct) =>`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClassSessionRepository.cs#L26): `public Task<IReadOnlyList<ClassSession>> ListScheduledAfterAsync(Guid ruleId, DateOnly afterDate, CancellationToken ct) =>`
- [Line 33](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClassSessionRepository.cs#L33): `public Task<IReadOnlyList<ClassSession>> ListFutureScheduledForGroupAsync(Guid groupId, DateTimeOffset now, CancellationToken ct) =>`
- [Line 38](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClassSessionRepository.cs#L38): `public void AddRange(IEnumerable<ClassSession> sessions) => Added.AddRange(sessions);`
- [Line 43](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClassSessionRepository.cs#L43): `public Task LockTeacherSchedulesAsync(IEnumerable<Guid> teacherIds, CancellationToken ct)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs:150](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L150)
- [tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs:103](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L103)
- [tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs:63](../../../../../tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs#L63)
- [tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs:85](../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L85)
- [tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs:111](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L111)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:136](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L136)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory class-session repository. <see cref="Existing"/> simulates stored rows (built with the real
/// SessionGenerator, the only way to obtain a session); <see cref="Added"/> records AddRange calls. No overlap
/// check: the exclusion constraints are proven against a real database, not here.
/// </summary>
public sealed class FakeClassSessionRepository : IClassSessionRepository
{
    public List<ClassSession> Existing { get; } = [];

    public List<ClassSession> Added { get; } = [];

    public Task<ClassSession?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(session => session.Id == id));

    public Task<IReadOnlyList<DateOnly>> ListExistingDatesAsync(Guid ruleId, DateOnly from, DateOnly through, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<DateOnly>>(Existing
            .Where(session => session.ScheduleRuleId == ruleId && session.LocalDate >= from && session.LocalDate <= through)
            .Select(session => session.LocalDate)
            .ToList());

    public Task<IReadOnlyList<ClassSession>> ListScheduledAfterAsync(Guid ruleId, DateOnly afterDate, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<ClassSession>>(Existing
            .Where(session => session.ScheduleRuleId == ruleId
                && session.LocalDate > afterDate
                && session.Status == ClassSessionStatus.Scheduled)
            .ToList());

    public Task<IReadOnlyList<ClassSession>> ListFutureScheduledForGroupAsync(Guid groupId, DateTimeOffset now, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<ClassSession>>(Existing
            .Where(session => session.GroupId == groupId && session.StartsAt > now && session.Status == ClassSessionStatus.Scheduled)
            .ToList());

    public void AddRange(IEnumerable<ClassSession> sessions) => Added.AddRange(sessions);

    /// <summary>The teachers whose schedule lock was taken, in call order, and how many sessions were added by then.</summary>
    public List<(Guid TeacherId, int AddedBefore)> Locked { get; } = [];

    public Task LockTeacherSchedulesAsync(IEnumerable<Guid> teacherIds, CancellationToken ct)
    {
        Locked.AddRange(teacherIds.Select(teacherId => (teacherId, Added.Count)));
        return Task.CompletedTask;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
