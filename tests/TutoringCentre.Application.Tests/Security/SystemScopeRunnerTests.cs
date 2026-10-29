using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Security;

public sealed class SystemScopeRunnerTests
{
    [Fact]
    public async Task RunAsync_SetsSystemActorCorrelationAndToken_WithoutChangingCaller()
    {
        await using var services = CreateServices();
        await using var caller = services.CreateAsyncScope();
        var user = new StaffActor(Guid.NewGuid(), Guid.NewGuid(), StaffRole.Teacher);
        caller.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(user);
        caller.ServiceProvider.GetRequiredService<CorrelationContext>().Set("http:caller");
        var centre = Guid.NewGuid();
        using var cancellation = new CancellationTokenSource();
        var value = await caller.ServiceProvider.GetRequiredService<ISystemScopeRunner>()
            .RunAsync(centre, "job:42", (scopedServices, ct) =>
            {
                Assert.Equal(new SystemActor(centre), scopedServices.GetRequiredService<ICurrentActor>().Actor);
                Assert.Equal("job:42", scopedServices.GetRequiredService<ICorrelationContext>().CorrelationId);
                Assert.Equal(cancellation.Token, ct);
                return Task.FromResult(42);
            }, cancellation.Token);
        Assert.Equal(42, value);
        Assert.Same(user, caller.ServiceProvider.GetRequiredService<ICurrentActor>().Actor);
        Assert.Equal("http:caller", caller.ServiceProvider.GetRequiredService<ICorrelationContext>().CorrelationId);
    }

    [Fact]
    public async Task RunAsync_NullCentre_SetsPlatformSystemActor()
    {
        await using var services = CreateServices();
        var actor = await services.GetRequiredService<ISystemScopeRunner>().RunAsync(null, "job:platform",
            (scopedServices, _) => Task.FromResult(scopedServices.GetRequiredService<ICurrentActor>().Actor));
        Assert.Equal(new SystemActor(null), actor);
    }

    [Fact]
    public async Task RunAsync_TwoCalls_HaveDifferentScopesDisposedBeforeReturn()
    {
        await using var services = CreateServices();
        var runner = services.GetRequiredService<ISystemScopeRunner>();
        var first = await runner.RunAsync(null, "job:one", (scopedServices, _) => Task.FromResult(scopedServices.GetRequiredService<ScopeProbe>()));
        var second = await runner.RunAsync(null, "job:two", (scopedServices, _) => Task.FromResult(scopedServices.GetRequiredService<ScopeProbe>()));
        Assert.NotSame(first, second);
        Assert.True(first.Disposed);
        Assert.True(second.Disposed);
    }

    [Fact]
    public async Task RunAsync_ExceptionPropagatesUnchanged_AndScopeIsDisposed()
    {
        await using var services = CreateServices();
        var expected = new InvalidOperationException("test failure");
        ScopeProbe? probe = null;
        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            services.GetRequiredService<ISystemScopeRunner>().RunAsync<int>(null, "job:fails", (scopedServices, _) =>
            {
                probe = scopedServices.GetRequiredService<ScopeProbe>();
                throw expected;
            }));
        Assert.Same(expected, actual);
        Assert.True(probe?.Disposed);
    }

    [Fact]
    public async Task RunAsync_CancelledToken_DoesNotRunWork()
    {
        await using var services = CreateServices();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var called = false;
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => services.GetRequiredService<ISystemScopeRunner>()
            .RunAsync(null, "job:cancelled", (_, _) => { called = true; return Task.FromResult(0); }, cancellation.Token));
        Assert.False(called);
    }

    private static ServiceProvider CreateServices() => new ServiceCollection().AddApplication()
        .AddScoped<ScopeProbe>().BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

    private sealed class ScopeProbe : IAsyncDisposable
    {
        public bool Disposed { get; private set; }
        public ValueTask DisposeAsync() { Disposed = true; return ValueTask.CompletedTask; }
    }
}
