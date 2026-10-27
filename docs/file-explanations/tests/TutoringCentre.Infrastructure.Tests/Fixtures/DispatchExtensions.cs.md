# `tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs`

## Purpose

Source summary (intent, not independent execution proof): Dispatches a use case in a new scope as the given actor — a scope per call, like a request or job.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 55 lines and 2,672 bytes. Source hash: `de24e3141fccb969095d7105a22d99d6c966c0d48283a05f70856193f9de4433`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.DependencyInjection;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs#L9): `public static class DispatchExtensions`
- [Line 11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs#L11): `public static Task<Result<TResponse>> SendAsAsync<TCommand, TResponse>(`
- [Line 19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs#L19): `public static async Task<Result<TResponse>> SendAsAsync<TCommand, TResponse>(`
- [Line 37](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs#L37): `public static Task<Result<TResponse>> QueryAsAsync<TQuery, TResponse>(`
- [Line 45](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs#L45): `public static async Task<Result<TResponse>> QueryAsAsync<TQuery, TResponse>(`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Tests.Fixtures;

/// <summary>Dispatches a use case in a new scope as the given actor — a scope per call, like a request or job.</summary>
public static class DispatchExtensions
{
    public static Task<Result<TResponse>> SendAsAsync<TCommand, TResponse>(
        this PostgresFixture fixture, Actor actor, TCommand command, string? correlationId = null)
        where TCommand : ICommand<TResponse>
    {
        ArgumentNullException.ThrowIfNull(fixture);
        return fixture.Services.SendAsAsync<TCommand, TResponse>(actor, command, correlationId);
    }

    public static async Task<Result<TResponse>> SendAsAsync<TCommand, TResponse>(
        this IServiceProvider provider, Actor actor, TCommand command, string? correlationId = null)
        where TCommand : ICommand<TResponse>
    {
        ArgumentNullException.ThrowIfNull(provider);
        await using var scope = provider.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor);
        if (correlationId is not null)
        {
            // Not an HTTP request: no CorrelationIdMiddleware to set this, so a test that needs one (Task 31.7's
            // correlation_id assertion) sets it directly on the same scope the dispatcher runs in.
            scope.ServiceProvider.GetRequiredService<CorrelationContext>().Set(correlationId);
        }

        var dispatcher = scope.ServiceProvider.GetRequiredService<Dispatcher>();
        return await dispatcher.SendAsync<TCommand, TResponse>(command, CancellationToken.None);
    }

    public static Task<Result<TResponse>> QueryAsAsync<TQuery, TResponse>(
        this PostgresFixture fixture, Actor actor, TQuery query)
        where TQuery : IQuery<TResponse>
    {
        ArgumentNullException.ThrowIfNull(fixture);
        return fixture.Services.QueryAsAsync<TQuery, TResponse>(actor, query);
    }

    public static async Task<Result<TResponse>> QueryAsAsync<TQuery, TResponse>(
        this IServiceProvider provider, Actor actor, TQuery query)
        where TQuery : IQuery<TResponse>
    {
        ArgumentNullException.ThrowIfNull(provider);
        await using var scope = provider.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor);
        var dispatcher = scope.ServiceProvider.GetRequiredService<Dispatcher>();
        return await dispatcher.QueryAsync<TQuery, TResponse>(query, CancellationToken.None);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
