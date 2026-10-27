# `tests/TutoringCentre.Infrastructure.Tests/Fixtures/ProbeScopeExtensions.cs`

## Purpose

Source summary (intent, not independent execution proof): A DI scope holding a TenantProbeDbContext for one actor, with a real transaction already begun through the real IUnitOfWork — exactly the production pipeline, so row-level security (Task 21.3) sees the same app.current_centre setting a real request would set. Disposing commits (or rolls back, if the transaction was left aborted by a thrown exception) and ends the scope.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/ProbeScopeExtensions.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 70 lines and 2,826 bytes. Source hash: `13c8a2b03e0f917cf41061304cf65b713a99bc11f57cecf5f80e78c7529b5d8a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.DependencyInjection;`
- Line 2: `using TutoringCentre.Application.Common.Ports;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/ProbeScopeExtensions.cs#L14): `internal sealed class ProbeScope : IAsyncDisposable`
- [Line 19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/ProbeScopeExtensions.cs#L19): `private ProbeScope(AsyncServiceScope scope, IUnitOfWork unitOfWork, TenantProbeDbContext context)`
- [Line 28](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/ProbeScopeExtensions.cs#L28): `internal static async Task<ProbeScope> CreateAsync(AsyncServiceScope scope, Actor actor)`
- [Line 40](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/ProbeScopeExtensions.cs#L40): `public async ValueTask DisposeAsync()`
- [Line 60](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/ProbeScopeExtensions.cs#L60): `internal static class ProbeScopeExtensions`
- [Line 62](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/ProbeScopeExtensions.cs#L62): `public static Task<ProbeScope> CreateProbeScope(this PostgresFixture fixture, Actor actor)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs:27](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs#L27)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
