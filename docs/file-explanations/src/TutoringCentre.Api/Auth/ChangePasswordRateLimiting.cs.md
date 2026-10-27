# `src/TutoringCentre.Api/Auth/ChangePasswordRateLimiting.cs`

## Purpose

Source summary (intent, not independent execution proof): Caps password-change attempts per signed-in user (Day 30 contract): unlike `LoginRateLimiting`, which guards against spraying many accounts from one source before any session exists, this request always carries a session — the right key is who is acting, not where from. Forwarded-header-aware per-IP limiting is Month 2 (Day 34); this does not need it.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/ChangePasswordRateLimiting.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 63 lines and 2,741 bytes. Source hash: `710dd1faf1a372fced6d3cfd61cfed07dfe5d51d78b91580f863cf12bb8069f1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Security.Claims;`
- Line 2: `using System.Threading.RateLimiting;`
- Line 3: `using Microsoft.AspNetCore.RateLimiting;`
- Line 4: `using Microsoft.Extensions.DependencyInjection;`
- Line 5: `using Microsoft.Extensions.Hosting;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../src/TutoringCentre.Api/Auth/ChangePasswordRateLimiting.cs#L15): `public static class ChangePasswordRateLimiting`
- [Line 22](../../../../../src/TutoringCentre.Api/Auth/ChangePasswordRateLimiting.cs#L22): `public static IServiceCollection AddChangePasswordRateLimiting(this IServiceCollection services)`
- [Line 46](../../../../../src/TutoringCentre.Api/Auth/ChangePasswordRateLimiting.cs#L46): `private static string GetPartitionKey(HttpContext httpContext)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:68](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L68)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// Caps password-change attempts per signed-in user (Day 30 contract): unlike <see cref="LoginRateLimiting"/>,
/// which guards against spraying many accounts from one source before any session exists, this request always
/// carries a session — the right key is who is acting, not where from. Forwarded-header-aware per-IP limiting
/// is Month 2 (Day 34); this does not need it.
/// </summary>
public static class ChangePasswordRateLimiting
{
    public const string PolicyName = "change-password";

    private const int PermitLimit = 5;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public static IServiceCollection AddChangePasswordRateLimiting(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddRateLimiter(options =>
        {
            options.AddPolicy(PolicyName, httpContext => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: GetPartitionKey(httpContext),
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = PermitLimit,
                    Window = Window,
                    QueueLimit = 0,
                }));

            // RateLimiterOptions.OnRejected is a single delegate shared by every policy on this options
            // instance (see RateLimitRejection); setting it again here is redundant with LoginRateLimiting's
            // registration, not an override, since both point at the same static handler.
            options.OnRejected = RateLimitRejection.HandleAsync;
        });

        return services;
    }

    private static string GetPartitionKey(HttpContext httpContext)
    {
        // Same escape hatch as LoginRateLimiting, honoured only in the "Testing" host environment: lets
        // AntiforgeryTestHelper give every ordinary test call its own window, so unrelated tests for the same
        // signed-in user never share one, without weakening the real per-user limit.
        var environment = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
        if (environment.IsEnvironment("Testing"))
        {
            var testPartition = httpContext.Request.Headers[LoginRateLimiting.TestPartitionHeaderName].FirstOrDefault();
            if (testPartition is not null)
            {
                return testPartition;
            }
        }

        return httpContext.User.FindFirst(SessionClaimNames.UserId)?.Value ?? "anonymous";
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
