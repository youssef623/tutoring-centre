# `src/TutoringCentre.Api/Endpoints/AuditEndpoints.cs`

## Purpose

Source summary (intent, not independent execution proof): Audit endpoints (Day 32 contract: docs/architecture/api-conventions.md). Bind → dispatch → map the result. The centre never comes from the route or the query string — only from the actor. Read-only: there is no write endpoint for audit.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Endpoints/AuditEndpoints.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 46 lines and 1,646 bytes. Source hash: `4ee5e368a9206fc64544045ee03850bd6af087755022ef5bf6dc8a4eec974506`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Http;`
- Line 2: `using TutoringCentre.Application.Audit;`
- Line 3: `using TutoringCentre.Application.Audit.Queries.GetAuditLog;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../src/TutoringCentre.Api/Endpoints/AuditEndpoints.cs#L13): `public static class AuditEndpoints`
- [Line 17](../../../../../src/TutoringCentre.Api/Endpoints/AuditEndpoints.cs#L17): `public static RouteGroupBuilder MapAuditEndpoints(this RouteGroupBuilder api)`
- [Line 31](../../../../../src/TutoringCentre.Api/Endpoints/AuditEndpoints.cs#L31): `private static async Task<IResult> GetAuditLogAsync(`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Api.Http;
using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Audit.Queries.GetAuditLog;
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Api.Endpoints;

/// <summary>
/// Audit endpoints (Day 32 contract: docs/architecture/api-conventions.md). Bind → dispatch → map the result.
/// The centre never comes from the route or the query string — only from the actor. Read-only: there is no
/// write endpoint for audit.
/// </summary>
public static class AuditEndpoints
{
    private const int DefaultPageSize = 50;

    public static RouteGroupBuilder MapAuditEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);

        api.MapGet("/audit", GetAuditLogAsync)
            .WithName("GetAuditLog")
            .Produces<AuditPageDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        return api;
    }

    private static async Task<IResult> GetAuditLogAsync(
        Dispatcher dispatcher,
        CancellationToken ct,
        string? entityType = null,
        Guid? entityId = null,
        Guid? actorUserId = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        string? cursor = null,
        int pageSize = DefaultPageSize)
    {
        var result = await dispatcher.QueryAsync<GetAuditLogQuery, AuditPageDto>(
            new GetAuditLogQuery(entityType, entityId, actorUserId, from, to, cursor, pageSize), ct);
        return result.ToHttpResult(Results.Ok);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
