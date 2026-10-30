using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TutoringCentre.Application.Assistant.Tools;
namespace TutoringCentre.Infrastructure.Assistant;
public sealed class ToolRegistrationValidator(IServiceScopeFactory scopes) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopes.CreateScope();
        _ = scope.ServiceProvider.GetRequiredService<ToolRegistry>();
        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
