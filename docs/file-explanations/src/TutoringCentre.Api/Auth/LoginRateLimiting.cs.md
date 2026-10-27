# `src/TutoringCentre.Api/Auth/LoginRateLimiting.cs`

## Purpose

Source summary (intent, not independent execution proof): Caps login attempts per client IP: a source cannot try passwords against many accounts without limit. Lockout (Infrastructure/Identity) protects one account; this protects against spraying many accounts from one source. Forwarded-header-aware client IPs are Month 2.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/LoginRateLimiting.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 74 lines and 3,346 bytes. Source hash: `c282b79e6a045fe584522c06d310d41ad50bdf6124663d03ccb1b5789c03b778`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Threading.RateLimiting;`
- Line 2: `using Microsoft.AspNetCore.RateLimiting;`
- Line 3: `using Microsoft.Extensions.Hosting;`
- Line 4: `using Microsoft.Extensions.Configuration;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../src/TutoringCentre.Api/Auth/LoginRateLimiting.cs#L13): `public static class LoginRateLimiting`
- [Line 34](../../../../../src/TutoringCentre.Api/Auth/LoginRateLimiting.cs#L34): `public static IServiceCollection AddLoginRateLimiting(this IServiceCollection services, IConfiguration configuration)`
- [Line 60](../../../../../src/TutoringCentre.Api/Auth/LoginRateLimiting.cs#L60): `private static string GetPartitionKey(HttpContext httpContext)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:38](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L38)
- [src/TutoringCentre.Api/Auth/ChangePasswordRateLimiting.cs:54](../../../../../src/TutoringCentre.Api/Auth/ChangePasswordRateLimiting.cs#L54)
- [tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs:33](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs#L33)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// Caps login attempts per client IP: a source cannot try passwords against many accounts without limit.
/// Lockout (Infrastructure/Identity) protects one account; this protects against spraying many accounts
/// from one source. Forwarded-header-aware client IPs are Month 2.
/// </summary>
public static class LoginRateLimiting
{
    public const string PolicyName = "login";

    /// <summary>
    /// Testing-only escape hatch: lets the test suite vary the partition key per test so unrelated tests never
    /// share one exhausted window, without weakening the real per-IP limit. Honoured only when the host
    /// environment is exactly "Testing" (ApiFactory's environment) — Production and Development never read it,
    /// so a real caller cannot use this header to dodge the limit by rotating its value per request.
    /// </summary>
    public const string TestPartitionHeaderName = "X-Test-RateLimit-Partition";

    private const int DefaultPermitLimit = 10;
    private static readonly TimeSpan DefaultWindow = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Overridable via configuration (e.g. "Auth__LoginRateLimit__PermitLimit"/"...__WindowSeconds" env vars)
    /// so CI's e2e job — a single trusted IP running many fixture logins per run, not a brute-force source —
    /// can raise the budget without touching the Production/Development default or the Testing-environment
    /// suite's hardcoded 10/minute assertion (TutoringCentre.Api.Tests only ever sees the default).
    /// </summary>
    public static IServiceCollection AddLoginRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var permitLimit = configuration.GetValue("Auth:LoginRateLimit:PermitLimit", DefaultPermitLimit);
        var windowSeconds = configuration.GetValue("Auth:LoginRateLimit:WindowSeconds", (int)DefaultWindow.TotalSeconds);
        var window = TimeSpan.FromSeconds(windowSeconds);

        services.AddRateLimiter(options =>
        {
            options.AddPolicy(PolicyName, httpContext => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: GetPartitionKey(httpContext),
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = window,
                    QueueLimit = 0,
                }));

            options.OnRejected = RateLimitRejection.HandleAsync;
        });

        return services;
    }

    private static string GetPartitionKey(HttpContext httpContext)
    {
        var environment = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
        if (environment.IsEnvironment("Testing"))
        {
            var testPartition = httpContext.Request.Headers[TestPartitionHeaderName].FirstOrDefault();
            if (testPartition is not null)
            {
                return testPartition;
            }
        }

        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
