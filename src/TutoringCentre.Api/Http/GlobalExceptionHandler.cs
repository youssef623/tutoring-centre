using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TutoringCentre.Api.Http;

/// <summary>
/// The one place unexpected exceptions are logged and translated. Responses never contain exception text, types or stack
/// traces (information leakage); the full exception goes to the log only.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the exception-handling middleware via DI.")]
internal sealed partial class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        ProblemDetails problem;

        if (exception is BadHttpRequestException)
        {
            // Malformed JSON, unknown JSON field (strict JSON), bad route value: the client's fault.
            LogMalformedRequest(logger, httpContext.Request.Method, httpContext.Request.Path);
            problem = Create(StatusCodes.Status400BadRequest, "The request could not be read.", "request.malformed");
        }
        else
        {
            LogUnexpectedException(logger, exception, httpContext.Request.Method, httpContext.Request.Path);
            problem = Create(StatusCodes.Status500InternalServerError, "An unexpected error occurred.", "server.unexpected");
        }

        await new ProblemResult(problem).ExecuteAsync(httpContext);
        return true;
    }

    private static ProblemDetails Create(int status, string title, string code)
    {
        var problem = new ProblemDetails { Status = status, Title = title };
        problem.Extensions["code"] = code;
        return problem;
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Malformed request {RequestMethod} {RequestPath}")]
    private static partial void LogMalformedRequest(ILogger logger, string requestMethod, PathString requestPath);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception while processing {RequestMethod} {RequestPath}")]
    private static partial void LogUnexpectedException(ILogger logger, Exception exception, string requestMethod, PathString requestPath);
}
