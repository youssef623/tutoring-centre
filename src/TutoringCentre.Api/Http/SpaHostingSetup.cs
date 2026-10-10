using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;

namespace TutoringCentre.Api.Http;

/// <summary>
/// Serves the frontend's production build from the web root, so the browser sees one origin (no CORS, and
/// `__Host-` cookies that simply work). A no-op in Development, where the Vite dev server serves the frontend
/// instead and no built <c>index.html</c> exists in the web root.
/// </summary>
public static class SpaHostingSetup
{
    private const string OneYear = "31536000";

    /// <summary>
    /// Serves hashed build assets (JS, CSS, fonts). Placed before authentication: a static file is served or
    /// it is not — it never needs a session, and running it earlier means it short-circuits before auth work
    /// happens for a request that was only ever asking for a `.js` file.
    /// </summary>
    public static WebApplication UseSpaStaticFiles(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = context =>
            {
                // Hashed by content (Vite's build output): safe to cache forever, because a changed file gets a
                // new name. index.html is the one unhashed file in the build (also reachable directly, not only
                // through the fallback below), so it keeps the opposite header instead.
                context.Context.Response.Headers[HeaderNames.CacheControl] = context.File.Name == "index.html"
                    ? "no-cache"
                    : $"public, max-age={OneYear}, immutable";
            },
        });

        return app;
    }

    /// <summary>
    /// Falls back to the app shell for any other GET, so deep links work — anonymous, because the shell itself
    /// needs no session (the frontend's own route guards decide what to show once it loads). A no-op when no
    /// build is present: every unmatched route keeps the plain JSON 404 instead of a broken HTML response.
    /// </summary>
    public static WebApplication MapSpaFallback(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var indexHtmlPath = Path.Combine(app.Environment.WebRootPath ?? string.Empty, "index.html");
        if (!File.Exists(indexHtmlPath))
        {
            return app;
        }

        // The default pattern, `{*path:nonfile}`, excludes file-looking paths (e.g. a missing /assets/*.js),
        // which fall through to the JSON 404 fallback registered after this one instead of getting the HTML shell.
        app.MapFallbackToFile("index.html", new StaticFileOptions
        {
            OnPrepareResponse = context =>
                context.Context.Response.Headers[HeaderNames.CacheControl] = "no-cache",
        }).AllowAnonymous();

        return app;
    }
}
