# `src/TutoringCentre.Api/Http/SpaHostingSetup.cs`

## Purpose

Source summary (intent, not independent execution proof): Serves the frontend's production build from the web root, so the browser sees one origin (no CORS, and `__Host-` cookies that simply work). A no-op in Development, where the Vite dev server serves the frontend instead and no built index.html exists in the web root.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Http/SpaHostingSetup.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 65 lines and 2,844 bytes. Source hash: `61fc8563ca9feba9ed16b99297eca28a667d5f32fb66ae10896d213125edce62`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.StaticFiles;`
- Line 2: `using Microsoft.Net.Http.Headers;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../src/TutoringCentre.Api/Http/SpaHostingSetup.cs#L11): `public static class SpaHostingSetup`
- [Line 20](../../../../../src/TutoringCentre.Api/Http/SpaHostingSetup.cs#L20): `public static WebApplication UseSpaStaticFiles(this WebApplication app)`
- [Line 45](../../../../../src/TutoringCentre.Api/Http/SpaHostingSetup.cs#L45): `public static WebApplication MapSpaFallback(this WebApplication app)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
