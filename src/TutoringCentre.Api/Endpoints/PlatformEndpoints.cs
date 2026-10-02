using TutoringCentre.Api.Http;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Platform;
using TutoringCentre.Application.Platform.Queries.GetSystemInfo;

namespace TutoringCentre.Api.Endpoints;

/// <summary>Platform-level endpoints. Each endpoint: bind → dispatch → map. No logic, no database access, no outcome `if`s.</summary>
public static class PlatformEndpoints
{
    public static RouteGroupBuilder MapPlatformEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);

        api.MapGet("/system/info", GetSystemInfoAsync)
            .WithName("GetSystemInfo")
            .Produces<SystemInfoDto>()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .AllowAnonymous();

        return api;
    }

    private static async Task<IResult> GetSystemInfoAsync(Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<GetSystemInfoQuery, SystemInfoDto>(new GetSystemInfoQuery(), ct);
        return result.ToHttpResult(dto => Results.Ok(dto));
    }
}
