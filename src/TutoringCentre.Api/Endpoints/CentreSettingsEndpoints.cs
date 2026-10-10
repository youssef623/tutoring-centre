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
