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
