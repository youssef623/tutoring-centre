using TutoringCentre.Infrastructure.Messaging;
namespace TutoringCentre.Api.Cli;

/// <summary>Bounded local verification through the real relay; never an HTTP endpoint.</summary>
public static class RelayCommand
{
    public static async Task<int> RunAsync(IServiceProvider services, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services); ArgumentNullException.ThrowIfNull(environment);
        if (!environment.IsDevelopment() && !environment.IsEnvironment("Testing")) return 1;
        var relay = services.GetRequiredService<OutboxRelayService>();
        for (var stage = 0; stage < 3; stage++) await relay.ProcessBatchAsync();
        return 0;
    }
}
