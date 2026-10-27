# `tests/TutoringCentre.Application.Tests/Staff/ListStaffHandlerTests.cs`

## Purpose

Declares `ListStaffHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Staff/ListStaffHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 1,012 bytes. Source hash: `d07c11e1e2f8cc061e4aea61520b468900465347451b4d8a27a1ab2b43a5ff75`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Application.Staff.Queries.ListStaff;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Staff/ListStaffHandlerTests.cs#L8): `public sealed class ListStaffHandlerTests`
- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Staff/ListStaffHandlerTests.cs#L11): `public async Task HandleAsync_PassesTheActorsCentreAndUserToTheReadService()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Staff;

public sealed class ListStaffHandlerTests
{
    [Fact]
    public async Task HandleAsync_PassesTheActorsCentreAndUserToTheReadService()
    {
        var centreId = Guid.CreateVersion7();
        var userId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(userId, centreId, StaffRole.Owner));
        var readService = new FakeStaffReadService();
        var handler = new ListStaffHandler(actorContext, readService);

        var result = await handler.HandleAsync(new ListStaffQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(centreId, readService.LastCentreIdRequested);
        Assert.Equal(userId, readService.LastCurrentUserIdRequested);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
