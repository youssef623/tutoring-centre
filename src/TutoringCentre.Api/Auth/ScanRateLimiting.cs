using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace TutoringCentre.Api.Auth;

public static class ScanRateLimiting
{
    public const string PolicyName = "scan";

    public static IServiceCollection AddScanRateLimiting(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddRateLimiter(options =>
        {
            options.AddPolicy(PolicyName, context => RateLimitPartition.GetSlidingWindowLimiter(
                context.User.FindFirst(SessionClaimNames.UserId)?.Value ?? "anonymous",
                _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 60,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 6,
                    QueueLimit = 0,
                }));
            options.OnRejected = RateLimitRejection.HandleAsync;
        });
        return services;
    }
}
