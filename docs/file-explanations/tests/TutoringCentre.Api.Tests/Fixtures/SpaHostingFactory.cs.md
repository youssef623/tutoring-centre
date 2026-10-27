# `tests/TutoringCentre.Api.Tests/Fixtures/SpaHostingFactory.cs`

## Purpose

Source summary (intent, not independent execution proof): The real API with its web root pointed at a temporary directory holding a marker index.html and one hashed asset — standing in for a real frontend build, without needing one on disk for this test run.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SpaHostingFactory.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 38 lines and 1,611 bytes. Source hash: `5e952e3add71e2562cda3ae0039a90ea22fb82cdacd270dbe0a4418c34a047f4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.Hosting;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SpaHostingFactory.cs#L9): `public sealed class SpaHostingFactory : ApiFactory`
- [Line 18](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SpaHostingFactory.cs#L18): `public SpaHostingFactory()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Fixtures/SpaHostingCollection.cs:11](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SpaHostingCollection.cs#L11)
- [tests/TutoringCentre.Api.Tests/Http/SecurityHeadersTests.cs:6](../../../../../tests/TutoringCentre.Api.Tests/Http/SecurityHeadersTests.cs#L6)
- [tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs:7](../../../../../tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs#L7)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.AspNetCore.Hosting;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>
/// The real API with its web root pointed at a temporary directory holding a marker <c>index.html</c> and one
/// hashed asset — standing in for a real frontend build, without needing one on disk for this test run.
/// </summary>
public sealed class SpaHostingFactory : ApiFactory
{
    public const string IndexHtmlMarker = "<!doctype html><html><body>spa-hosting-test-marker</body></html>";
    public const string AssetContent = "console.log('spa-hosting-test-asset');";

    // Content root (not web root) directly: matches the real shape (wwwroot as a subfolder of the app's own
    // directory, exactly as the Dockerfile — Task 34.2 — copies the frontend build into the publish output).
    private readonly string _contentRootPath = Directory.CreateTempSubdirectory("spa-hosting-tests-").FullName;

    public SpaHostingFactory()
    {
        var webRootPath = Path.Combine(_contentRootPath, "wwwroot");
        Directory.CreateDirectory(Path.Combine(webRootPath, "assets"));
        File.WriteAllText(Path.Combine(webRootPath, "index.html"), IndexHtmlMarker);
        File.WriteAllText(Path.Combine(webRootPath, "assets", "app.abc123.js"), AssetContent);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseContentRoot(_contentRootPath);
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        Directory.Delete(_contentRootPath, recursive: true);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
