using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Security;
namespace TutoringCentre.Application.Common.Execution;

/// <summary>Fresh actor, correlation and service scope; queries own their read-only dispatcher transaction.</summary>
public sealed class ActorScopeRunner(IServiceScopeFactory scopes, ICorrelationContext correlation) : IActorScopeRunner
{
    public async Task<T> RunAsync<T>(ICurrentActor actor, Func<IServiceProvider, Task<T>> work)
    {
        ArgumentNullException.ThrowIfNull(actor);
        ArgumentNullException.ThrowIfNull(work);
        await using var scope = scopes.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor.Actor);
        scope.ServiceProvider.GetRequiredService<CorrelationContext>().Set(correlation.CorrelationId ?? Guid.NewGuid().ToString("N"));
        return await work(scope.ServiceProvider);
    }
}
