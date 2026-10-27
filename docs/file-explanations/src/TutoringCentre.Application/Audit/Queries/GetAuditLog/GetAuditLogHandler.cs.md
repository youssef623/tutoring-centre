# `src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogHandler.cs`

## Purpose

Declares `GetAuditLogHandler`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogHandler.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 25 lines and 938 bytes. Source hash: `ecfa6ddc65097d390a5f664a907ccd9337ca437b09439573e0c9adf6cb4b41c3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Audit;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogHandler.cs#L9): `internal sealed class GetAuditLogHandler(IAuditReadService readService) : IQueryHandler<GetAuditLogQuery, AuditPageDto>`
- [Line 11](../../../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogHandler.cs#L11): `public async Task<Result<AuditPageDto>> HandleAsync(GetAuditLogQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs:62](../../../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs#L62)
- [tests/TutoringCentre.Application.Tests/Audit/GetAuditLogHandlerTests.cs:13](../../../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogHandlerTests.cs#L13)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Audit.Queries.GetAuditLog;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetAuditLogHandler(IAuditReadService readService) : IQueryHandler<GetAuditLogQuery, AuditPageDto>
{
    public async Task<Result<AuditPageDto>> HandleAsync(GetAuditLogQuery query, CancellationToken cancellationToken)
    {
        var page = await readService.GetPageAsync(
            query.EntityType,
            query.EntityId,
            query.ActorUserId,
            query.From,
            query.To,
            query.Cursor,
            query.PageSize,
            cancellationToken);

        return Result<AuditPageDto>.Success(page);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
