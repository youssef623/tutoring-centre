# `src/TutoringCentre.Api/Auth/AuthenticationSetup.cs`

## Purpose

The setup configures the secure HttpOnly host-prefixed session cookie, eight-hour sliding ticket lifetime, per-request validation hook, API-style 401/403 behavior, and authenticated fallback policy. Cache options are bound here; database key persistence is registered in Infrastructure. Cookie setting changes must respect browser prefix rules and existing client behavior.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/AuthenticationSetup.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 59 lines and 2,504 bytes. Source hash: `340eb3e2b77a9ef302bbc2ecf4051f05e4d82e40307bdb5067a86f730180594f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.Authentication.Cookies;`
- Line 2: `using Microsoft.AspNetCore.Authorization;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../src/TutoringCentre.Api/Auth/AuthenticationSetup.cs#L12): `public static class AuthenticationSetup`
- [Line 14](../../../../../src/TutoringCentre.Api/Auth/AuthenticationSetup.cs#L14): `public static IServiceCollection AddApiAuthentication(this IServiceCollection services)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// Cookie authentication for the Api: a same-origin SPA session, not a website. An unauthenticated or
/// access-denied request gets a plain status code — never a redirect to a login page — and
/// <c>UseStatusCodePages</c> (Day 11) turns that bare status into the project's Problem Details shape,
/// the same path every other error takes.
/// </summary>
public static class AuthenticationSetup
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                // The __Host- prefix forces Secure, Path "/" and no Domain — the browser rejects the cookie otherwise.
                options.Cookie.Name = "__Host-tcm.session";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.Path = "/";

                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;

                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = SessionRevalidationHandler.ValidateAsync,
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    },
                };
            });

        // Fail-closed: a new endpoint is protected unless it explicitly opts out with .AllowAnonymous().
        services.AddAuthorization(options =>
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        services.AddMemoryCache();
        services.AddOptions<SessionValidationOptions>().BindConfiguration("SessionValidation");

        return services;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
