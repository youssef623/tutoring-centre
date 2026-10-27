# `src/TutoringCentre.Api/Auth/AntiforgerySetup.cs`

## Purpose

Source summary (intent, not independent execution proof): Double-submit CSRF protection: the browser sends the antiforgery cookie automatically, but the header value must be read by our own script from our own origin (see AntiforgeryEndpointFilter for enforcement).

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/AntiforgerySetup.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 30 lines and 1,157 bytes. Source hash: `47ecea882f8778ec755222ac6c7ff714b794747ab8c41a486f2d8a0e64c9f0b2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Api/Auth/AntiforgerySetup.cs#L7): `public static class AntiforgerySetup`
- [Line 11](../../../../../src/TutoringCentre.Api/Auth/AntiforgerySetup.cs#L11): `public static IServiceCollection AddApiAntiforgery(this IServiceCollection services)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs:748](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L748)
- [tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs:92](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L92)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleDoubleBookingHttpTests.cs:60](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleDoubleBookingHttpTests.cs#L60)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs:625](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L625)
- [tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs:68](../../../../../tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs#L68)
- [tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs:32](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs#L32)
- [tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbe.cs:84](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbe.cs#L84)
- [tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs:96](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs#L96)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Api.Auth;

/// <summary>
/// Double-submit CSRF protection: the browser sends the antiforgery cookie automatically, but the header value
/// must be read by our own script from our own origin (see AntiforgeryEndpointFilter for enforcement).
/// </summary>
public static class AntiforgerySetup
{
    public const string HeaderName = "X-XSRF-TOKEN";

    public static IServiceCollection AddApiAntiforgery(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddAntiforgery(options =>
        {
            options.HeaderName = HeaderName;

            // __Host- forces Secure, Path "/" and no Domain. Strict (not Lax, unlike the session cookie): this
            // cookie is never needed on a top-level cross-site navigation, only on same-origin script requests.
            options.Cookie.Name = "__Host-tcm.xsrf";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.Path = "/";
        });

        return services;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
