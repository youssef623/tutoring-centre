# `src/TutoringCentre.Api/Auth/AntiforgeryEndpointFilter.cs`

## Purpose

Source summary (intent, not independent execution proof): Applied once to the whole /api group: every request whose method is not GET, HEAD or OPTIONS must carry a valid antiforgery token, login included — this is what stops login CSRF (an attacker signing a victim into the attacker's own account). Minimal APIs only validate antiforgery automatically for form-bound endpoints; this JSON API needs the explicit check.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/AntiforgeryEndpointFilter.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 42 lines and 1,769 bytes. Source hash: `2f3f8756e4f4584ff5d26a7d6fb33b17d72b1e4ddfc955a641c1fdaea960c4d8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.AspNetCore.Antiforgery;`
- Line 3: `using TutoringCentre.Api.Http;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../src/TutoringCentre.Api/Auth/AntiforgeryEndpointFilter.cs#L15): `internal sealed class AntiforgeryEndpointFilter(IAntiforgery antiforgery) : IEndpointFilter`
- [Line 19](../../../../../src/TutoringCentre.Api/Auth/AntiforgeryEndpointFilter.cs#L19): `public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Program.cs:214](../../../../../src/TutoringCentre.Api/Program.cs#L214)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Antiforgery;
using TutoringCentre.Api.Http;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// Applied once to the whole /api group: every request whose method is not GET, HEAD or OPTIONS must carry a
/// valid antiforgery token, login included — this is what stops login CSRF (an attacker signing a victim into
/// the attacker's own account). Minimal APIs only validate antiforgery automatically for form-bound endpoints;
/// this JSON API needs the explicit check.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the endpoint filter pipeline via DI.")]
internal sealed class AntiforgeryEndpointFilter(IAntiforgery antiforgery) : IEndpointFilter
{
    private static readonly HashSet<string> SafeMethods = new(StringComparer.OrdinalIgnoreCase) { "GET", "HEAD", "OPTIONS" };

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var httpContext = context.HttpContext;
        if (SafeMethods.Contains(httpContext.Request.Method))
        {
            return await next(context);
        }

        try
        {
            await antiforgery.ValidateRequestAsync(httpContext);
        }
        catch (AntiforgeryValidationException)
        {
            // No side effects: the endpoint handler never runs.
            return Error.Forbidden("auth.csrf_invalid", "A valid antiforgery token is required for this request.").ToProblemResult();
        }

        return await next(context);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
