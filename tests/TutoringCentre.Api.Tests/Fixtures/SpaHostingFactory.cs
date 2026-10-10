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
