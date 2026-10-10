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
