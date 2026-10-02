using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Tests.Fixtures;

/// <summary>Dispatches a use case in a new scope as the given actor — a scope per call, like a request or job.</summary>
public static class DispatchExtensions
{
    public static Task<Result<TResponse>> SendAsAsync<TCommand, TResponse>(
        this PostgresFixture fixture, Actor actor, TCommand command)
        where TCommand : ICommand<TResponse>
    {
        ArgumentNullException.ThrowIfNull(fixture);
        return fixture.Services.SendAsAsync<TCommand, TResponse>(actor, command);
    }

    public static async Task<Result<TResponse>> SendAsAsync<TCommand, TResponse>(
        this IServiceProvider provider, Actor actor, TCommand command)
        where TCommand : ICommand<TResponse>
    {
        ArgumentNullException.ThrowIfNull(provider);
        await using var scope = provider.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor);
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
