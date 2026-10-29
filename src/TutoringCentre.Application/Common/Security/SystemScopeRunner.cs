using Microsoft.Extensions.DependencyInjection;

namespace TutoringCentre.Application.Common.Security;

/// <summary>Owns the scope lifetime. Business results and exceptions pass through unchanged.</summary>
public sealed class SystemScopeRunner(IServiceScopeFactory scopeFactory) : ISystemScopeRunner
{
    public async Task<TResult> RunAsync<TResult>(Guid? centreId, string correlationId,
        Func<IServiceProvider, CancellationToken, Task<TResult>> work, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        ArgumentNullException.ThrowIfNull(work);
        cancellationToken.ThrowIfCancellationRequested();
        await using var scope = scopeFactory.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new SystemActor(centreId));
        scope.ServiceProvider.GetRequiredService<CorrelationContext>().Set(correlationId);
        return await work(scope.ServiceProvider, cancellationToken);
    }
}
