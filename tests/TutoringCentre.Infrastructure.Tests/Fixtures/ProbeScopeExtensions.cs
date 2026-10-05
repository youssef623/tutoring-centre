using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Fixtures;

/// <summary>A DI scope holding a TenantProbeDbContext for one actor, like a request scope. Disposing it ends the scope.</summary>
internal sealed class ProbeScope : IAsyncDisposable
{
    private readonly AsyncServiceScope _scope;

    internal ProbeScope(AsyncServiceScope scope, TenantProbeDbContext context)
    {
        _scope = scope;
        Context = context;
    }

    public TenantProbeDbContext Context { get; }

    public ValueTask DisposeAsync() => _scope.DisposeAsync();
}

/// <summary>Opens a probe scope as a given actor — the one place every Task 20.5/20.6 test sets its actor.</summary>
internal static class ProbeScopeExtensions
{
    public static ProbeScope CreateProbeScope(this PostgresFixture fixture, Actor actor)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        ArgumentNullException.ThrowIfNull(actor);

        var scope = fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor);
        return new ProbeScope(scope, scope.ServiceProvider.GetRequiredService<TenantProbeDbContext>());
    }
}
