# `src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs`

## Purpose

Source summary (intent, not independent execution proof): Resolves or creates the request's correlation ID, exposes it on the response and in every log line. A client-supplied value is accepted only when it is 1–64 characters of letters, digits, '.', '_' or '-': an unvalidated header written into logs could inject fake log lines or huge values.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 49 lines and 2,170 bytes. Source hash: `757b9d9731282b638a75e199874f21f93b6616f0f39930aa63f6922f9386a413`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics;`
- Line 2: `using System.Diagnostics.CodeAnalysis;`
- Line 3: `using Serilog.Context;`
- Line 4: `using TutoringCentre.Application.Common.Security;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs#L14): `internal sealed class CorrelationIdMiddleware(RequestDelegate next)`
- [Line 19](../../../../../src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs#L19): `public async Task InvokeAsync(HttpContext context, CorrelationContext correlationContext)`
- [Line 46](../../../../../src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs#L46): `private static bool IsValid(string value) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Program.cs:171](../../../../../src/TutoringCentre.Api/Program.cs#L171)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Serilog.Context;
using TutoringCentre.Application.Common.Security;

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

    public async Task InvokeAsync(HttpContext context, CorrelationContext correlationContext)
    {
        ArgumentNullException.ThrowIfNull(context);

        var supplied = context.Request.Headers[HeaderName].ToString();
        var correlationId = IsValid(supplied) ? supplied : Guid.NewGuid().ToString("N");

        context.Items[HttpContextKeys.CorrelationId] = correlationId;
        correlationContext.Set(correlationId);

        // Task 35.6: the same reference shown to the user in a toast, findable on the request's trace when
        // telemetry is enabled. A no-op (no listener) when telemetry is off, so this costs nothing by default.
        Activity.Current?.SetTag("correlation.id", correlationId);

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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
