# `src/TutoringCentre.Api/Http/ProblemResult.cs`

## Purpose

Source summary (intent, not independent execution proof): Writes a Problem Details body (RFC 9457) with the standard extensions. The single writer for every error response.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Http/ProblemResult.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 32 lines and 1,281 bytes. Source hash: `66a08d54111d4e0b08b5e2d7a1ab8d6821e233eac4738a795ed46acb968181df`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics;`
- Line 2: `using Microsoft.AspNetCore.Mvc;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Api/Http/ProblemResult.cs#L7): `internal sealed class ProblemResult(ProblemDetails problem) : IResult`
- [Line 11](../../../../../src/TutoringCentre.Api/Http/ProblemResult.cs#L11): `public Task ExecuteAsync(HttpContext httpContext)`
- [Line 27](../../../../../src/TutoringCentre.Api/Http/ProblemResult.cs#L27): `internal static void Enrich(ProblemDetails problem, HttpContext httpContext)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/RateLimitRejection.cs:34](../../../../../src/TutoringCentre.Api/Auth/RateLimitRejection.cs#L34)
- [src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs:33](../../../../../src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs#L33)
- [src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs:11](../../../../../src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs#L11)
- [src/TutoringCentre.Api/Http/ResultHttpExtensions.cs:77](../../../../../src/TutoringCentre.Api/Http/ResultHttpExtensions.cs#L77)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
