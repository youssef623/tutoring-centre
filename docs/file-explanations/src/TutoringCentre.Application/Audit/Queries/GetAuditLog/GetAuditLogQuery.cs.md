# `src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): The audit log behind the audit page. Every filter is optional; the caller cannot pass a centre.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogQuery.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 18 lines and 628 bytes. Source hash: `463b33f2dd071ad45f799348ff60120d23b5b70db7ea966fc50560ff4180828e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Audit;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogQuery.cs#L8): `public sealed record GetAuditLogQuery(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/AuditEndpoints.cs:42](../../../../../../../src/TutoringCentre.Api/Endpoints/AuditEndpoints.cs#L42)
- [src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogHandler.cs:9](../../../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogHandler.cs#L9)
- [src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogValidator.cs:7](../../../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogValidator.cs#L7)
- [tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs:30](../../../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs#L30)
- [tests/TutoringCentre.Application.Tests/Audit/GetAuditLogHandlerTests.cs:18](../../../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs:13](../../../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L13)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs:35](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs#L35)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Audit.Queries.GetAuditLog;

/// <summary>The audit log behind the audit page. Every filter is optional; the caller cannot pass a centre.</summary>
public sealed record GetAuditLogQuery(
    string? EntityType,
    Guid? EntityId,
    Guid? ActorUserId,
    DateTimeOffset? From,
    DateTimeOffset? To,
    string? Cursor,
    int PageSize) : IQuery<AuditPageDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.AuditView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
