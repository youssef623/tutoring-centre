# `tests/TutoringCentre.Application.Tests/Fakes/FakeAuditReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory stand-in for audit reads. Records every filter it was last asked for, so a handler test can assert they were passed through unchanged.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeAuditReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 43 lines and 1,376 bytes. Source hash: `16ba9a8019816c4422846f575d05dbddd9a9e378c9577bf7e3e1ed56136a454b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Audit;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeAuditReadService.cs#L6): `public sealed class FakeAuditReadService : IAuditReadService`
- [Line 24](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeAuditReadService.cs#L24): `public Task<AuditPageDto> GetPageAsync(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs:61](../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs#L61)
- [tests/TutoringCentre.Application.Tests/Audit/GetAuditLogHandlerTests.cs:12](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogHandlerTests.cs#L12)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Audit;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for audit reads. Records every filter it was last asked for, so a handler test can assert they were passed through unchanged.</summary>
public sealed class FakeAuditReadService : IAuditReadService
{
    public AuditPageDto PageToReturn { get; set; } = new([], null);

    public string? LastEntityType { get; private set; }

    public Guid? LastEntityId { get; private set; }

    public Guid? LastActorUserId { get; private set; }

    public DateTimeOffset? LastOccurredFrom { get; private set; }

    public DateTimeOffset? LastOccurredTo { get; private set; }

    public string? LastCursor { get; private set; }

    public int LastPageSize { get; private set; }

    public Task<AuditPageDto> GetPageAsync(
        string? entityType,
        Guid? entityId,
        Guid? actorUserId,
        DateTimeOffset? occurredFrom,
        DateTimeOffset? occurredTo,
        string? cursor,
        int pageSize,
        CancellationToken ct)
    {
        LastEntityType = entityType;
        LastEntityId = entityId;
        LastActorUserId = actorUserId;
        LastOccurredFrom = occurredFrom;
        LastOccurredTo = occurredTo;
        LastCursor = cursor;
        LastPageSize = pageSize;
        return Task.FromResult(PageToReturn);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
