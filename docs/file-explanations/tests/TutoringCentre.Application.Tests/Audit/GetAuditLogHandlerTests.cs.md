# `tests/TutoringCentre.Application.Tests/Audit/GetAuditLogHandlerTests.cs`

## Purpose

Declares `GetAuditLogHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 48 lines and 2,008 bytes. Source hash: `893e60e6797b9f891970330c87754a42fa74ecaf86c8dc40c989dbfd17bc6f6b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Audit;`
- Line 2: `using TutoringCentre.Application.Audit.Queries.GetAuditLog;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogHandlerTests.cs#L7): `public sealed class GetAuditLogHandlerTests`
- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogHandlerTests.cs#L10): `public async Task HandleAsync_PassesEveryFilterThroughUnchanged()`
- [Line 33](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogHandlerTests.cs#L33): `public async Task HandleAsync_ReturnsThePageFromTheReadService()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Audit.Queries.GetAuditLog;
using TutoringCentre.Application.Tests.Fakes;

namespace TutoringCentre.Application.Tests.Audit;

public sealed class GetAuditLogHandlerTests
{
    [Fact]
    public async Task HandleAsync_PassesEveryFilterThroughUnchanged()
    {
        var readService = new FakeAuditReadService();
        var handler = new GetAuditLogHandler(readService);
        var entityId = Guid.CreateVersion7();
        var actorUserId = Guid.CreateVersion7();
        var from = DateTimeOffset.UtcNow.AddDays(-7);
        var to = DateTimeOffset.UtcNow;
        var query = new GetAuditLogQuery("membership", entityId, actorUserId, from, to, "some-cursor", 25);

        var result = await handler.HandleAsync(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("membership", readService.LastEntityType);
        Assert.Equal(entityId, readService.LastEntityId);
        Assert.Equal(actorUserId, readService.LastActorUserId);
        Assert.Equal(from, readService.LastOccurredFrom);
        Assert.Equal(to, readService.LastOccurredTo);
        Assert.Equal("some-cursor", readService.LastCursor);
        Assert.Equal(25, readService.LastPageSize);
    }

    [Fact]
    public async Task HandleAsync_ReturnsThePageFromTheReadService()
    {
        var readService = new FakeAuditReadService
        {
            PageToReturn = new AuditPageDto(
                [new AuditEntryDto(Guid.CreateVersion7(), DateTimeOffset.UtcNow, "staff", Guid.CreateVersion7(), "Nile Owner", "updated", "membership", Guid.CreateVersion7(), [], "corr-1")],
                "next-cursor"),
        };
        var handler = new GetAuditLogHandler(readService);

        var result = await handler.HandleAsync(new GetAuditLogQuery(null, null, null, null, null, null, 50), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(readService.PageToReturn, result.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
