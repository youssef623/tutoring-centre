using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Execution;
using TutoringCentre.Application.Common.Security;
namespace TutoringCentre.Application.Tests.Common;
public sealed class ActorScopeRunnerTests
{
    [Fact] public async Task InnerActorAndCorrelationAreIsolatedAndExceptionsPropagate()
    {
        await using var services = new ServiceCollection().AddApplication().BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        await using var outer = services.CreateAsyncScope();
        var current = outer.ServiceProvider.GetRequiredService<CurrentActorContext>();
        current.Set(new SystemActor(Guid.NewGuid()));
        outer.ServiceProvider.GetRequiredService<CorrelationContext>().Set("test:actor");
        var inner = new CurrentActorContext(); inner.Set(new ParentActor(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid()));
        var runner = outer.ServiceProvider.GetRequiredService<IActorScopeRunner>();
        await runner.RunAsync(inner, provider => {
            Assert.NotSame(current,provider.GetRequiredService<ICurrentActor>());
            Assert.Equal(inner.Actor,provider.GetRequiredService<ICurrentActor>().Actor);
            Assert.Equal("test:actor",provider.GetRequiredService<ICorrelationContext>().CorrelationId);
            return Task.FromResult(0);
        });
        var error = new InvalidOperationException("probe");
        Assert.Same(error, await Assert.ThrowsAsync<InvalidOperationException>(() => runner.RunAsync<int>(inner,_ => throw error)));
        Assert.IsType<SystemActor>(current.Actor);
    }
}
