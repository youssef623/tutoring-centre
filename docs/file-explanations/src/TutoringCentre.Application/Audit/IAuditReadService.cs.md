# `src/TutoringCentre.Application/Audit/IAuditReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-side port for the audit log. Audit is read-only from the application's point of view: no audit command, repository or Domain type exists. No centre parameter: scoping comes from the acting actor, the same as ISubjectReadService relies on the EF query filter and row-level security underneath.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Application/Audit/IAuditReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 43 lines and 1,866 bytes. Source hash: `6bfeecbd0621d5f555fc0c7873dec0a2e852165147a72dce2e3855dc1b826e33`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../src/TutoringCentre.Application/Audit/IAuditReadService.cs#L8): `public interface IAuditReadService`
- [Line 27](../../../../../src/TutoringCentre.Application/Audit/IAuditReadService.cs#L27): `public sealed record AuditPageDto(IReadOnlyList<AuditEntryDto> Items, string? NextCursor);`
- [Line 30](../../../../../src/TutoringCentre.Application/Audit/IAuditReadService.cs#L30): `public sealed record AuditEntryDto(`
- [Line 43](../../../../../src/TutoringCentre.Application/Audit/IAuditReadService.cs#L43): `public sealed record AuditChangeDto(string Field, string? Before, string? After);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogHandler.cs:9](../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogHandler.cs#L9)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:157](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L157)
- [src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs:19](../../../../../src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs#L19)
- [tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs:61](../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs#L61)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeAuditReadService.cs:6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeAuditReadService.cs#L6)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Audit;

/// <summary>
/// Read-side port for the audit log. Audit is read-only from the application's point of view: no audit
/// command, repository or Domain type exists. No centre parameter: scoping comes from the acting actor, the
/// same as <c>ISubjectReadService</c> relies on the EF query filter and row-level security underneath.
/// </summary>
public interface IAuditReadService
{
    /// <summary>
    /// Newest first by (occurredAt, id), both descending. Every filter is optional and applied in SQL.
    /// <paramref name="cursor"/>, when given, resumes strictly after the last row of the page it came from.
    /// Fetches <paramref name="pageSize"/> + 1 rows internally to decide whether a next page exists.
    /// </summary>
    Task<AuditPageDto> GetPageAsync(
        string? entityType,
        Guid? entityId,
        Guid? actorUserId,
        DateTimeOffset? occurredFrom,
        DateTimeOffset? occurredTo,
        string? cursor,
        int pageSize,
        CancellationToken ct);
}

/// <summary>One page of the audit log. <see cref="NextCursor"/> is null when there is no further page.</summary>
public sealed record AuditPageDto(IReadOnlyList<AuditEntryDto> Items, string? NextCursor);

/// <summary>One audit row, shaped for the API contract. Never carries the actor's email.</summary>
public sealed record AuditEntryDto(
    Guid Id,
    DateTimeOffset OccurredAt,
    string ActorType,
    Guid? ActorUserId,
    string? ActorDisplayName,
    string Action,
    string EntityType,
    Guid EntityId,
    IReadOnlyList<AuditChangeDto> Changes,
    string? CorrelationId);

/// <summary>One changed field. Values are strings or null — the "changes" document never carries any other audited type today.</summary>
public sealed record AuditChangeDto(string Field, string? Before, string? After);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
