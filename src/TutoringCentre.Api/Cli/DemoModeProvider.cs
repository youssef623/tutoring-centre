using Microsoft.Extensions.Options;
using TutoringCentre.Application.Platform;

namespace TutoringCentre.Api.Cli;

/// <summary>Adapts the bound <see cref="DemoOptions"/> to the Application layer's <see cref="IDemoModeProvider"/> port.</summary>
public sealed class DemoModeProvider(IOptions<DemoOptions> options) : IDemoModeProvider
{
    public bool IsEnabled => options.Value.Enabled;
}
