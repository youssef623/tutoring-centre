# `src/TutoringCentre.Api/Http/SecurityHeadersSetup.cs`

## Purpose

Source summary (intent, not independent execution proof): Browser-enforced defence in depth: every response (HTML, hashed assets, `/api` responses, and error responses alike) carries the same headers, set as early as possible so nothing downstream can skip them.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Http/SecurityHeadersSetup.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 61 lines and 2,746 bytes. Source hash: `41bee8e2936d5d01694fad8c67be38aec461600302f678da04d4d0a4908bf64b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Api/Http/SecurityHeadersSetup.cs#L7): `public static class SecurityHeadersSetup`
- [Line 31](../../../../../src/TutoringCentre.Api/Http/SecurityHeadersSetup.cs#L31): `public static WebApplication UseSecurityHeaders(this WebApplication app)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Api.Http;

/// <summary>
/// Browser-enforced defence in depth: every response (HTML, hashed assets, `/api` responses, and error
/// responses alike) carries the same headers, set as early as possible so nothing downstream can skip them.
/// </summary>
public static class SecurityHeadersSetup
{
    // No 'unsafe-inline' or 'unsafe-eval' for script-src, ever — the whole point of this policy is that
    // injected markup still cannot run script, because the browser refuses anything not served from our origin.
    //
    // style-src carries 'unsafe-inline' (Task 34.8, confirmed against the local production stack behind
    // Caddy): Sonner's Toaster (frontend/src/routes/__root.tsx, mounted on every route) sets its runtime-
    // computed offsets as inline styles on its own container; the SPA is a pre-built static index.html with
    // no per-request templating, so neither a CSP nonce nor a style hash is available to allow just that.
    // Only style-src is relaxed; script-src keeps its full 'self'-only policy. Recorded for 35.9.
    private const string ContentSecurityPolicy =
        "default-src 'self'; " +
        "script-src 'self'; " +
        "style-src 'self' 'unsafe-inline'; " +
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
