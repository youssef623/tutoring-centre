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

                foreach (var method in methods)
                {
                    keys.Add($"{method} {routeEndpoint.RoutePattern.RawText}");
                }
            }
        }

        return keys;
    }
}
