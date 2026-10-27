# `tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleRuleRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory schedule-rule repository. `Existing` simulates stored rows; `Added` records Add calls. `ActiveGroupIds` stands in for the join to active groups that `ListActiveAsync` makes; a rule whose group is not listed there is treated as belonging to an archived group.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleRuleRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 31 lines and 1,532 bytes. Source hash: `abe39b5f0d5085a55010af523de6a1615c2d5707c9f3efaad7682ec88ef8fb95`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 2: `using TutoringCentre.Domain.Academics.Scheduling;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleRuleRepository.cs#L11): `public sealed class FakeScheduleRuleRepository : IScheduleRuleRepository`
- [Line 19](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleRuleRepository.cs#L19): `public Task<ScheduleRule?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>`
- [Line 22](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleRuleRepository.cs#L22): `public Task<IReadOnlyList<ScheduleRule>> ListActiveAsync(DateOnly from, CancellationToken ct) =>`
- [Line 27](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleRuleRepository.cs#L27): `public Task<bool> AnyActiveForGroupAsync(Guid groupId, DateOnly today, CancellationToken ct) =>`
- [Line 30](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleRuleRepository.cs#L30): `public void Add(ScheduleRule rule) => Added.Add(rule);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs:149](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L149)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs:93](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L93)
- [tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs:83](../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L83)
- [tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs:108](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L108)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:135](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L135)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory schedule-rule repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add
/// calls. <see cref="ActiveGroupIds"/> stands in for the join to active groups that <see cref="ListActiveAsync"/>
/// makes; a rule whose group is not listed there is treated as belonging to an archived group.
/// </summary>
public sealed class FakeScheduleRuleRepository : IScheduleRuleRepository
{
    public List<ScheduleRule> Existing { get; } = [];

    public List<ScheduleRule> Added { get; } = [];

    public HashSet<Guid> ActiveGroupIds { get; } = [];

    public Task<ScheduleRule?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(rule => rule.Id == id));

    public Task<IReadOnlyList<ScheduleRule>> ListActiveAsync(DateOnly from, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<ScheduleRule>>(Existing
            .Where(rule => (rule.EffectiveTo is null || rule.EffectiveTo >= from) && ActiveGroupIds.Contains(rule.GroupId))
            .ToList());

    public Task<bool> AnyActiveForGroupAsync(Guid groupId, DateOnly today, CancellationToken ct) =>
        Task.FromResult(Existing.Any(rule => rule.GroupId == groupId && (rule.EffectiveTo is null || rule.EffectiveTo >= today)));

    public void Add(ScheduleRule rule) => Added.Add(rule);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
