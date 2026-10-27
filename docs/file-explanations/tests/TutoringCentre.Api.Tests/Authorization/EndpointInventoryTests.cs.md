# `tests/TutoringCentre.Api.Tests/Authorization/EndpointInventoryTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 30.8: enumerates the running application's own endpoints and demands that every one of them has been explicitly classified into one of three lists — anonymous, any authenticated user, or the permission matrix (`PermissionMatrixTests`). An endpoint nobody classified fails the build, so it cannot ship unprotected by accident. Expectations are explicit routes, never wildcards or generated from the role map.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Authorization/EndpointInventoryTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 100 lines and 4,230 bytes. Source hash: `68b994768e71e89ca8e54edb1458a0fa244c9be20009466edaf1296152b772eb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.Http.Metadata;`
- Line 2: `using Microsoft.AspNetCore.Routing;`
- Line 3: `using Microsoft.Extensions.DependencyInjection;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../tests/TutoringCentre.Api.Tests/Authorization/EndpointInventoryTests.cs#L15): `public sealed class EndpointInventoryTests(ApiFactory factory)`
- [Line 38](../../../../../tests/TutoringCentre.Api.Tests/Authorization/EndpointInventoryTests.cs#L38): `public void EveryRunningEndpoint_IsClassifiedExactlyOnce()`
- [Line 64](../../../../../tests/TutoringCentre.Api.Tests/Authorization/EndpointInventoryTests.cs#L64): `internal static HashSet<string> GetRouteKeys(ApiFactory factory)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Authorization;

/// <summary>
/// Task 30.8: enumerates the running application's own endpoints and demands that every one of them has been
/// explicitly classified into one of three lists — anonymous, any authenticated user, or the permission matrix
/// (<see cref="PermissionMatrixTests"/>). An endpoint nobody classified fails the build, so it cannot ship
/// unprotected by accident. Expectations are explicit routes, never wildcards or generated from the role map.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class EndpointInventoryTests(ApiFactory factory)
{
    /// <summary>No session required at all.</summary>
    private static readonly HashSet<string> AnonymousRoutes = new(StringComparer.Ordinal)
    {
        // MapHealthChecks maps a pattern with no HTTP method restriction (ANY, not GET specifically).
        "ANY /health",
        "ANY /health/ready",
        "GET /api/system/info",
        "GET /api/auth/antiforgery",
        "POST /api/auth/login",
    };

    /// <summary>Any signed-in user, regardless of permission or selected centre.</summary>
    private static readonly HashSet<string> AnyAuthenticatedRoutes = new(StringComparer.Ordinal)
    {
        "POST /api/auth/logout",
        "GET /api/me",
        "POST /api/session/centre",
        "POST /api/auth/change-password",
    };

    [Fact]
    public void EveryRunningEndpoint_IsClassifiedExactlyOnce()
    {
        var actualRoutes = GetRouteKeys(factory);
        var expectedRoutes = AnonymousRoutes
            .Concat(AnyAuthenticatedRoutes)
            .Concat(PermissionMatrixTests.MatrixRoutes)
            .ToList();

        var duplicates = expectedRoutes.GroupBy(route => route, StringComparer.Ordinal).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
        Assert.Empty(duplicates);

        var expectedSet = new HashSet<string>(expectedRoutes, StringComparer.Ordinal);
        var unclassified = actualRoutes.Except(expectedSet, StringComparer.Ordinal).ToList();
        var missing = expectedSet.Except(actualRoutes, StringComparer.Ordinal).ToList();

        Assert.True(
            unclassified.Count == 0 && missing.Count == 0,
            $"Unclassified (running but not in any list): [{string.Join(", ", unclassified)}]. "
            + $"Missing (classified but not running): [{string.Join(", ", missing)}].");
    }

    /// <summary>
    /// Every endpoint's route key, as "METHOD /path", in the real raw route pattern. Fallback routes
    /// (<c>ExcludeFromDescription</c>, e.g. the catch-all 404 handlers) are not addressable business endpoints
    /// and are excluded the same way OpenAPI generation excludes them — they are not something to classify.
    /// </summary>
    internal static HashSet<string> GetRouteKeys(ApiFactory factory)
    {
        var dataSources = factory.Services.GetServices<EndpointDataSource>();
        var keys = new HashSet<string>(StringComparer.Ordinal);

        foreach (var dataSource in dataSources)
        {
            foreach (var endpoint in dataSource.Endpoints)
            {
                if (endpoint is not RouteEndpoint routeEndpoint)
                {
                    continue;
                }

                if (routeEndpoint.Metadata.GetMetadata<IExcludeFromDescriptionMetadata>() is { ExcludeFromDescription: true })
                {
                    continue;
                }

                var methods = routeEndpoint.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods;
                if (methods is null || methods.Count == 0)
                {
                    // No method restriction at all (e.g. MapHealthChecks): matches any verb.
                    keys.Add($"ANY {routeEndpoint.RoutePattern.RawText}");
                    continue;
                }

```

The excerpt stops at line 90; use the original file for the remaining 10 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
