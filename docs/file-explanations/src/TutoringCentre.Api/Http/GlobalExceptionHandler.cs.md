# `src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): The one place unexpected exceptions are logged and translated. Responses never contain exception text, types or stack traces (information leakage); the full exception goes to the log only.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 49 lines and 2,403 bytes. Source hash: `f93372990a8ee777c2c8ff61a38d72e3aee73b7d39a60dee98831d2ab1c5d7ea`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.AspNetCore.Diagnostics;`
- Line 3: `using Microsoft.AspNetCore.Mvc;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs#L12): `internal sealed partial class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler`
- [Line 14](../../../../../src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs#L14): `public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)`
- [Line 37](../../../../../src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs#L37): `private static ProblemDetails Create(int status, string title, string code)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs:13](../../../../../src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs#L13)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
