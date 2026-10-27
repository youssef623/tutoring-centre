# `src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs`

## Purpose

Source summary (intent, not independent execution proof): Centre settings endpoints (Day 32 contract: docs/architecture/api-conventions.md). Bind → dispatch → map the result. Neither route carries an id: the centre is always the actor's own.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 58 lines and 2,483 bytes. Source hash: `86ba8df7f7aa9360126acf80093e91c419f449201ed12ec5b351723e4809f5fc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Http;`
- Line 2: `using TutoringCentre.Application.Centres;`
- Line 3: `using TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;`
- Line 4: `using TutoringCentre.Application.Centres.Queries.GetCentreSettings;`
- Line 5: `using TutoringCentre.Application.Common.Cqrs;`
- Line 6: `using TutoringCentre.Domain.Centres;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs#L14): `public static class CentreSettingsEndpoints`
- [Line 16](../../../../../src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs#L16): `public static RouteGroupBuilder MapCentreSettingsEndpoints(this RouteGroupBuilder api)`
- [Line 36](../../../../../src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs#L36): `private static async Task<IResult> GetCentreSettingsAsync(Dispatcher dispatcher, CancellationToken ct)`
- [Line 42](../../../../../src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs#L42): `private static async Task<IResult> UpdateCentreSettingsAsync(UpdateCentreSettingsRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 51](../../../../../src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs#L51): `public sealed record UpdateCentreSettingsRequest`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Api.Http;
using TutoringCentre.Application.Centres;
using TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;
using TutoringCentre.Application.Centres.Queries.GetCentreSettings;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Api.Endpoints;

/// <summary>
/// Centre settings endpoints (Day 32 contract: docs/architecture/api-conventions.md). Bind → dispatch → map
/// the result. Neither route carries an id: the centre is always the actor's own.
/// </summary>
public static class CentreSettingsEndpoints
{
    public static RouteGroupBuilder MapCentreSettingsEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);

        api.MapGet("/centre/settings", GetCentreSettingsAsync)
            .WithName("GetCentreSettings")
            .Produces<CentreSettingsDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        api.MapPut("/centre/settings", UpdateCentreSettingsAsync)
            .WithName("UpdateCentreSettings")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return api;
    }

    private static async Task<IResult> GetCentreSettingsAsync(Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<GetCentreSettingsQuery, CentreSettingsDto>(new GetCentreSettingsQuery(), ct);
        return result.ToHttpResult(Results.Ok);
    }

    private static async Task<IResult> UpdateCentreSettingsAsync(UpdateCentreSettingsRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<UpdateCentreSettingsCommand, Unit>(
            new UpdateCentreSettingsCommand(request.Name, request.DefaultLocale, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }
}

/// <summary>Exactly the contract's three editable fields: a client over-posting "slug" or "timeZoneId" is rejected with 400 by strict JSON, not silently ignored.</summary>
public sealed record UpdateCentreSettingsRequest
{
    public required string Name { get; init; }

    public required SupportedLocale DefaultLocale { get; init; }

    public required uint Version { get; init; }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
