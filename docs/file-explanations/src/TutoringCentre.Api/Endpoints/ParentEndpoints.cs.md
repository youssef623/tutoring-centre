# `src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs`

## Purpose

Source summary (intent, not independent execution proof): Parent endpoints (Day 39 contract). Each endpoint: bind → dispatch → map the result. No rule, repository, entity or tenant logic — the tenant comes only from the actor, through the dispatcher's own layer 1 step. Protected by the authorization fallback policy; none is anonymous. No list and no delete: Month 3 reaches a parent through a student or through the phone lookup. The lookup is a POST (phone numbers never belong in a URL, proxy log or browser history) but still dispatches a query on the read-only pipeline; its response gets "Cache-Control: no-store" the same automatic way every /api response does (`TutoringCentre.Api.Http.SecurityHeadersSetup`), not from anything written here.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 103 lines and 4,971 bytes. Source hash: `afd7ed0664f8258b4d9c648ebdb18ccd81869eb07ec56c6faf1f09df404304a3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Http;`
- Line 2: `using TutoringCentre.Application.Academics.Parents;`
- Line 3: `using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;`
- Line 4: `using TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;`
- Line 5: `using TutoringCentre.Application.Academics.Parents.Queries.GetParent;`
- Line 6: `using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;`
- Line 7: `using TutoringCentre.Application.Common.Cqrs;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 20](../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L20): `public static class ParentEndpoints`
- [Line 22](../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L22): `public static RouteGroupBuilder MapParentEndpoints(this RouteGroupBuilder api)`
- [Line 56](../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L56): `private static async Task<IResult> CreateParentAsync(CreateParentRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 63](../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L63): `private static async Task<IResult> GetParentAsync(Guid id, Dispatcher dispatcher, CancellationToken ct)`
- [Line 69](../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L69): `private static async Task<IResult> UpdateParentAsync(Guid id, UpdateParentRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 76](../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L76): `private static async Task<IResult> LookupParentAsync(LookupParentRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 84](../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L84): `public sealed record CreateParentRequest(string FullName, string Phone);`
- [Line 86](../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L86): `public sealed record CreateParentResponse(Guid Id);`
- [Line 93](../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L93): `public sealed record UpdateParentRequest`
- [Line 103](../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L103): `public sealed record LookupParentRequest(string Phone);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 13 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
