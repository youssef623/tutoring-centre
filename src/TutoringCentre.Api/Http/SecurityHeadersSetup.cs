namespace TutoringCentre.Api.Http;

/// <summary>
/// Browser-enforced defence in depth: every response (HTML, hashed assets, `/api` responses, and error
/// responses alike) carries the same headers, set as early as possible so nothing downstream can skip them.
/// </summary>
public static class SecurityHeadersSetup
{
    // No 'unsafe-inline' or 'unsafe-eval' for script-src, ever — the whole point of this policy is that
    // injected markup still cannot run script, because the browser refuses anything not served from our origin.
    private const string ContentSecurityPolicy =
        "default-src 'self'; " +
        "script-src 'self'; " +
        "style-src 'self'; " +
        "img-src 'self' data:; " +
        "font-src 'self'; " +
        "connect-src 'self'; " +
        "frame-ancestors 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self'; " +
        "object-src 'none'";

    private const string PermissionsPolicy = "camera=(), microphone=(), geolocation=()";

    public static WebApplication UseSecurityHeaders(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.Use((context, next) =>
        {
            context.Response.OnStarting(() =>
            {
                var headers = context.Response.Headers;
                headers["Content-Security-Policy"] = ContentSecurityPolicy;
                headers["X-Content-Type-Options"] = "nosniff";
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                headers["Cross-Origin-Opener-Policy"] = "same-origin";
                headers["Permissions-Policy"] = PermissionsPolicy;

                // Only where the response itself hasn't already decided its own caching (the SPA's static
                // files and index.html set their own Cache-Control, deliberately left alone here).
                if (context.Request.Path.StartsWithSegments("/api") && !headers.ContainsKey("Cache-Control"))
                {
                    headers["Cache-Control"] = "no-store";
                }

                return Task.CompletedTask;
            });

            return next();
        });

        return app;
    }
}
