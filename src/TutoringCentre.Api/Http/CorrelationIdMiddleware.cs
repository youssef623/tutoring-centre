using System.Diagnostics.CodeAnalysis;
using Serilog.Context;

namespace TutoringCentre.Api.Http;

/// <summary>
/// Resolves or creates the request's correlation ID, exposes it on the response and in every log line.
/// A client-supplied value is accepted only when it is 1–64 characters of letters, digits, '.', '_' or '-':
/// an unvalidated header written into logs could inject fake log lines or huge values.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the ASP.NET Core middleware pipeline.")]
internal sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-Id";
    private const int MaxLength = 64;

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var supplied = context.Request.Headers[HeaderName].ToString();
        var correlationId = IsValid(supplied) ? supplied : Guid.NewGuid().ToString("N");

        context.Items[HttpContextKeys.CorrelationId] = correlationId;

        // OnStarting, not an immediate header write: the exception handler clears the response before writing a 500.
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }

    private static bool IsValid(string value) =>
        value.Length is > 0 and <= MaxLength
        && value.All(character => char.IsAsciiLetterOrDigit(character) || character is '.' or '_' or '-');
}
