using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TutoringCentre.Api.Http;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// The one 429 response every rate-limited policy produces. <c>RateLimiterOptions.OnRejected</c> is a single
/// delegate, not a per-policy list: two policies calling <c>AddRateLimiter</c> each with their own handler
/// would have the later registration silently overwrite the earlier one, so both share this instead.
/// </summary>
internal static class RateLimitRejection
{
    public static async ValueTask HandleAsync(OnRejectedContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);

        var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var metadataRetryAfter)
            ? metadataRetryAfter
            : TimeSpan.FromMinutes(1);

        context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status429TooManyRequests,
            Title = "Too many requests.",
            Detail = "Too many attempts. Try again later.",
        };
        problem.Extensions["code"] = "auth.rate_limited";

        await new ProblemResult(problem).ExecuteAsync(context.HttpContext);
    }
}
