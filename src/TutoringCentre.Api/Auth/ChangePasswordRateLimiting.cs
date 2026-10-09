using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// Caps password-change attempts per signed-in user (Day 30 contract): unlike <see cref="LoginRateLimiting"/>,
/// which guards against spraying many accounts from one source before any session exists, this request always
/// carries a session — the right key is who is acting, not where from. Forwarded-header-aware per-IP limiting
/// is Month 2 (Day 34); this does not need it.
/// </summary>
public static class ChangePasswordRateLimiting
{
    public const string PolicyName = "change-password";

    private const int PermitLimit = 5;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public static IServiceCollection AddChangePasswordRateLimiting(this IServiceCollection services)
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

            // RateLimiterOptions.OnRejected is a single delegate shared by every policy on this options
            // instance (see RateLimitRejection); setting it again here is redundant with LoginRateLimiting's
            // registration, not an override, since both point at the same static handler.
            options.OnRejected = RateLimitRejection.HandleAsync;
        });

        return services;
    }

    private static string GetPartitionKey(HttpContext httpContext)
    {
        // Same escape hatch as LoginRateLimiting, honoured only in the "Testing" host environment: lets
        // AntiforgeryTestHelper give every ordinary test call its own window, so unrelated tests for the same
        // signed-in user never share one, without weakening the real per-user limit.
        var environment = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
        if (environment.IsEnvironment("Testing"))
        {
            var testPartition = httpContext.Request.Headers[LoginRateLimiting.TestPartitionHeaderName].FirstOrDefault();
            if (testPartition is not null)
            {
                return testPartition;
            }
        }

        return httpContext.User.FindFirst(SessionClaimNames.UserId)?.Value ?? "anonymous";
    }
}
