using Microsoft.Extensions.Hosting;

namespace TutoringCentre.Api.Cli;

/// <summary>
/// Whether the seed command may run here (Task 34.7): Development and Testing always allow it (so local setup
/// and the test suite are unaffected); Production allows it only for a demo instance that opted in explicitly.
/// Checked by Program's CLI dispatch before <see cref="SeedCommand.RunAsync"/>, not inside it — the command
/// itself keeps doing exactly what it always did, unconditionally, for every caller (including this project's
/// own `SeedCommandTests`, which exercise it directly and never want it gated).
/// </summary>
public static class SeedGate
{
    public static bool IsAllowed(IHostEnvironment environment, DemoOptions demoOptions)
    {
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(demoOptions);

        return environment.IsDevelopment() || environment.IsEnvironment("Testing") || demoOptions.Enabled;
    }
}
