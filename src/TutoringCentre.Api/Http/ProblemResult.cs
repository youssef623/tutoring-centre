using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TutoringCentre.Api.Http;

/// <summary>Writes a Problem Details body (RFC 9457) with the standard extensions. The single writer for every error response.</summary>
internal sealed class ProblemResult(ProblemDetails problem) : IResult
{
    public const string ContentType = "application/problem+json";

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        Enrich(problem, httpContext);
        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

        return httpContext.Response.WriteAsJsonAsync(
            problem,
            problem.GetType(),
            options: null,
            contentType: ContentType,
            cancellationToken: httpContext.RequestAborted);
    }

    /// <summary>Adds <c>traceId</c> and <c>correlationId</c> (null only when the correlation middleware did not run).</summary>
    internal static void Enrich(ProblemDetails problem, HttpContext httpContext)
    {
        problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        problem.Extensions["correlationId"] = httpContext.Items[HttpContextKeys.CorrelationId];
    }
}
