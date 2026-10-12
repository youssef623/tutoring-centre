using TutoringCentre.Api.Http;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;
using TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;
using TutoringCentre.Application.Academics.Parents.Queries.GetParent;
using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Api.Endpoints;

/// <summary>
/// Parent endpoints (Day 39 contract). Each endpoint: bind → dispatch → map the result. No rule, repository,
/// entity or tenant logic — the tenant comes only from the actor, through the dispatcher's own layer 1 step.
/// Protected by the authorization fallback policy; none is anonymous. No list and no delete: Month 3 reaches
/// a parent through a student or through the phone lookup. The lookup is a POST (phone numbers never belong
/// in a URL, proxy log or browser history) but still dispatches a query on the read-only pipeline; its
/// response gets "Cache-Control: no-store" the same automatic way every /api response does
/// (<see cref="TutoringCentre.Api.Http.SecurityHeadersSetup"/>), not from anything written here.
/// </summary>
public static class ParentEndpoints
{
    public static RouteGroupBuilder MapParentEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);

        api.MapPost("/parents", CreateParentAsync)
            .WithName("CreateParent")
            .Produces<CreateParentResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status409Conflict);

        api.MapGet("/parents/{id:guid}", GetParentAsync)
            .WithName("GetParent")
            .Produces<ParentDetailDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        api.MapPut("/parents/{id:guid}", UpdateParentAsync)
            .WithName("UpdateParent")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        api.MapPost("/parents/lookup", LookupParentAsync)
            .WithName("LookupParent")
            .Produces<ParentLookupDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden);

        return api;
    }

    private static async Task<IResult> CreateParentAsync(CreateParentRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<CreateParentCommand, CreateParentResult>(
            new CreateParentCommand(request.FullName, request.Phone), ct);
        return result.ToCreatedHttpResult(value => ($"/api/parents/{value.ParentId}", new CreateParentResponse(value.ParentId)));
    }

    private static async Task<IResult> GetParentAsync(Guid id, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<GetParentQuery, ParentDetailDto>(new GetParentQuery(id), ct);
        return result.ToHttpResult(Results.Ok);
    }

    private static async Task<IResult> UpdateParentAsync(Guid id, UpdateParentRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<UpdateParentCommand, Unit>(
            new UpdateParentCommand(id, request.FullName, request.Phone, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> LookupParentAsync(LookupParentRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<LookupParentByPhoneQuery, ParentLookupDto>(new LookupParentByPhoneQuery(request.Phone), ct);
        return result.ToHttpResult(Results.Ok);
    }
}

/// <summary>Exactly the two properties CreateParentCommand needs: no id, no centreId.</summary>
public sealed record CreateParentRequest(string FullName, string Phone);

public sealed record CreateParentResponse(Guid Id);

/// <summary>
/// The ID comes only from the route; this body carries no ID. Version is `required`: a positional-record
/// `uint` silently defaults to 0 when the property is absent from the JSON, which would masquerade as a (very)
/// stale version instead of the 400 a missing field should produce.
/// </summary>
public sealed record UpdateParentRequest
{
    public required string FullName { get; init; }

    public required string Phone { get; init; }

    public required uint Version { get; init; }
}

/// <summary>Exactly one property, so a phone number never needs to travel in a URL.</summary>
public sealed record LookupParentRequest(string Phone);
