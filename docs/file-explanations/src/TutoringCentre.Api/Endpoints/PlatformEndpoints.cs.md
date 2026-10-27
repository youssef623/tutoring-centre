# `src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs`

## Purpose

Source summary (intent, not independent execution proof): Platform-level endpoints. Each endpoint: bind → dispatch → map. No logic, no database access, no outcome `if`s.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 29 lines and 1,101 bytes. Source hash: `fb5f2b1fd74cd8fe08201c9fa478fcaf9041a8e5a72a279f7c6a0318202710f9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Http;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Platform;`
- Line 4: `using TutoringCentre.Application.Platform.Queries.GetSystemInfo;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs#L9): `public static class PlatformEndpoints`
- [Line 11](../../../../../src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs#L11): `public static RouteGroupBuilder MapPlatformEndpoints(this RouteGroupBuilder api)`
- [Line 24](../../../../../src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs#L24): `private static async Task<IResult> GetSystemInfoAsync(Dispatcher dispatcher, CancellationToken ct)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
