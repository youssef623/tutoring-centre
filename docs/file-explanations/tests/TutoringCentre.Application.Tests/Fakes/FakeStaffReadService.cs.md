# `tests/TutoringCentre.Application.Tests/Fakes/FakeStaffReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): Records the centre and current-user arguments the handler passed, so a test can prove they came from the actor, not a parameter.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStaffReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 20 lines and 787 bytes. Source hash: `9d9965a69da306f3ec8b4925a2ec6c2ab1ac59ef9fd1afda23c12a607c35d86b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Staff;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStaffReadService.cs#L6): `public sealed class FakeStaffReadService : IStaffReadService`
- [Line 14](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStaffReadService.cs#L14): `public Task<IReadOnlyList<StaffMemberDto>> ListAsync(Guid centreId, Guid currentUserId, CancellationToken ct)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Staff/ListStaffHandlerTests.cs:17](../../../../../tests/TutoringCentre.Application.Tests/Staff/ListStaffHandlerTests.cs#L17)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:174](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L174)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Staff;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>Records the centre and current-user arguments the handler passed, so a test can prove they came from the actor, not a parameter.</summary>
public sealed class FakeStaffReadService : IStaffReadService
{
    public List<StaffMemberDto> Items { get; set; } = [];

    public Guid? LastCentreIdRequested { get; private set; }

    public Guid? LastCurrentUserIdRequested { get; private set; }

    public Task<IReadOnlyList<StaffMemberDto>> ListAsync(Guid centreId, Guid currentUserId, CancellationToken ct)
    {
        LastCentreIdRequested = centreId;
        LastCurrentUserIdRequested = currentUserId;
        return Task.FromResult<IReadOnlyList<StaffMemberDto>>(Items);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
