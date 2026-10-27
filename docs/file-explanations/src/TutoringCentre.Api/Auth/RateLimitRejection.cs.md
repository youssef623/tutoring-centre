# `src/TutoringCentre.Api/Auth/RateLimitRejection.cs`

## Purpose

Source summary (intent, not independent execution proof): The one 429 response every rate-limited policy produces. RateLimiterOptions.OnRejected is a single delegate, not a per-policy list: two policies calling AddRateLimiter each with their own handler would have the later registration silently overwrite the earlier one, so both share this instead.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/RateLimitRejection.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 36 lines and 1,443 bytes. Source hash: `a1e9e30461804e07a11629fc6fe214881c48a9df5a3b779c8344b2171f8a5d1a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Globalization;`
- Line 2: `using System.Threading.RateLimiting;`
- Line 3: `using Microsoft.AspNetCore.Mvc;`
- Line 4: `using Microsoft.AspNetCore.RateLimiting;`
- Line 5: `using TutoringCentre.Api.Http;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../src/TutoringCentre.Api/Auth/RateLimitRejection.cs#L14): `internal static class RateLimitRejection`
- [Line 16](../../../../../src/TutoringCentre.Api/Auth/RateLimitRejection.cs#L16): `public static async ValueTask HandleAsync(OnRejectedContext context, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/ChangePasswordRateLimiting.cs:40](../../../../../src/TutoringCentre.Api/Auth/ChangePasswordRateLimiting.cs#L40)
- [src/TutoringCentre.Api/Auth/LoginRateLimiting.cs:54](../../../../../src/TutoringCentre.Api/Auth/LoginRateLimiting.cs#L54)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
