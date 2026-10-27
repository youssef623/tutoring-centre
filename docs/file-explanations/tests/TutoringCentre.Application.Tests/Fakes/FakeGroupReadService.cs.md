# `tests/TutoringCentre.Application.Tests/Fakes/FakeGroupReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory stand-in for group reads. Applies the scope the way the real adapter does (rows outside it are invisible) and records the last criteria and scope it was asked for, so a handler test can assert both were passed through.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 31 lines and 1,214 bytes. Source hash: `6408534e95da47b645c73b183b9d96f574e324d530091300ed7eee8562754ff6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups;`
- Line 2: `using TutoringCentre.Application.Common.Security;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupReadService.cs#L11): `public sealed class FakeGroupReadService : IGroupReadService`
- [Line 19](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupReadService.cs#L19): `public Task<IReadOnlyList<GroupDto>> ListAsync(GroupListCriteria criteria, TeacherScope scope, CancellationToken ct)`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupReadService.cs#L26): `public Task<GroupDto?> GetAsync(Guid id, TeacherScope scope, CancellationToken ct)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:158](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L158)
- [tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs:19](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L19)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory stand-in for group reads. Applies the scope the way the real adapter does (rows outside it are
/// invisible) and records the last criteria and scope it was asked for, so a handler test can assert both were
/// passed through.
/// </summary>
public sealed class FakeGroupReadService : IGroupReadService
{
    public List<GroupDto> Groups { get; } = [];

    public GroupListCriteria? LastCriteria { get; private set; }

    public TeacherScope? LastScope { get; private set; }

    public Task<IReadOnlyList<GroupDto>> ListAsync(GroupListCriteria criteria, TeacherScope scope, CancellationToken ct)
    {
        LastCriteria = criteria;
        LastScope = scope;
        return Task.FromResult<IReadOnlyList<GroupDto>>(Groups.Where(group => scope.Allows(group.TeacherId)).ToList());
    }

    public Task<GroupDto?> GetAsync(Guid id, TeacherScope scope, CancellationToken ct)
    {
        LastScope = scope;
        return Task.FromResult(Groups.FirstOrDefault(group => group.Id == id && scope.Allows(group.TeacherId)));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
