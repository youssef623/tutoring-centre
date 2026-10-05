using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Fixtures;

/// <summary>
/// A DI scope holding a TenantProbeDbContext for one actor, with a real transaction already begun through the
/// real IUnitOfWork — exactly the production pipeline, so row-level security (Task 21.3) sees the same
/// app.current_centre setting a real request would set. Disposing commits (or rolls back, if the transaction was
/// left aborted by a thrown exception) and ends the scope.
/// </summary>
internal sealed class ProbeScope : IAsyncDisposable
{
    private readonly AsyncServiceScope _scope;
    private readonly IUnitOfWork _unitOfWork;

    private ProbeScope(AsyncServiceScope scope, IUnitOfWork unitOfWork, TenantProbeDbContext context)
    {
        _scope = scope;
        _unitOfWork = unitOfWork;
        Context = context;
    }

    public TenantProbeDbContext Context { get; }

    internal static async Task<ProbeScope> CreateAsync(AsyncServiceScope scope, Actor actor)
    {
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor);
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var context = scope.ServiceProvider.GetRequiredService<TenantProbeDbContext>();

        // Read-write for every probe scope: filter tests only read, but sharing one begin mode keeps this one
        // helper usable by both, and a read-only transaction still gets app.current_centre set the same way.
        await unitOfWork.BeginAsync(readOnly: false, CancellationToken.None);
        return new ProbeScope(scope, unitOfWork, context);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            // PostgreSQL treats COMMIT on an already-aborted transaction as ROLLBACK, so this is safe even after
            // a test asserted a thrown exception from a failed SaveChangesAsync.
            await _unitOfWork.CommitAsync(CancellationToken.None);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(CancellationToken.None);
        }
        finally
        {
            await _scope.DisposeAsync();
        }
    }
}

/// <summary>Opens a probe scope as a given actor — the one place every Task 20.5/20.6/21.x probe test sets its actor.</summary>
internal static class ProbeScopeExtensions
{
    public static Task<ProbeScope> CreateProbeScope(this PostgresFixture fixture, Actor actor)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        ArgumentNullException.ThrowIfNull(actor);

        var scope = fixture.ProbeServices.CreateAsyncScope();
        return ProbeScope.CreateAsync(scope, actor);
    }
}
