using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

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

        services.AddAuthorization();

        services.AddMemoryCache();
        services.AddOptions<SessionValidationOptions>().BindConfiguration("SessionValidation");

        // Local key ring in Development (the framework default); persisted keys are Month 2.
        services.AddDataProtection().SetApplicationName("TutoringCentre");

        return services;
    }
}
