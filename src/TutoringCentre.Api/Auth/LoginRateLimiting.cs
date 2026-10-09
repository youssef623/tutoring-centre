using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Hosting;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// Caps login attempts per client IP: a source cannot try passwords against many accounts without limit.
/// Lockout (Infrastructure/Identity) protects one account; this protects against spraying many accounts
/// from one source. Forwarded-header-aware client IPs are Month 2.
/// </summary>
public static class LoginRateLimiting
{
    public const string PolicyName = "login";

    /// <summary>
    /// Testing-only escape hatch: lets the test suite vary the partition key per test so unrelated tests never
    /// share one exhausted window, without weakening the real per-IP limit. Honoured only when the host
    /// environment is exactly "Testing" (ApiFactory's environment) — Production and Development never read it,
    /// so a real caller cannot use this header to dodge the limit by rotating its value per request.
    /// </summary>
    public const string TestPartitionHeaderName = "X-Test-RateLimit-Partition";

    private const int PermitLimit = 10;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public static IServiceCollection AddLoginRateLimiting(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddRateLimiter(options =>
        {
            options.AddPolicy(PolicyName, httpContext => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: GetPartitionKey(httpContext),
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = PermitLimit,
                    Window = Window,
                    QueueLimit = 0,
                }));

            options.OnRejected = RateLimitRejection.HandleAsync;
        });

        return services;
    }

    private static string GetPartitionKey(HttpContext httpContext)
    {
        var environment = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
        if (environment.IsEnvironment("Testing"))
        {
            var testPartition = httpContext.Request.Headers[TestPartitionHeaderName].FirstOrDefault();
            if (testPartition is not null)
            {
                return testPartition;
            }
        }

        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
