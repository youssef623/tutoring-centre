using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Common.Messaging;

/// <summary>Only explicitly registered type names can become commands. Payloads never select CLR types.</summary>
public sealed class OutboxRegistry
{
    private readonly Dictionary<string, OutboxMapping> _mappings;

    internal OutboxRegistry(IEnumerable<OutboxMapping> mappings) =>
        _mappings = mappings.ToDictionary(mapping => mapping.TypeName, StringComparer.Ordinal);

    public bool TryResolve(string typeName, string payload, out OutboxDelivery? delivery)
    {
        if (_mappings.TryGetValue(typeName, out var mapping))
        {
            delivery = mapping.Resolve(payload);
            return true;
        }
        delivery = null;
        return false;
    }
}

/// <summary>A registered command and its statically typed dispatcher delegate.</summary>
public sealed class OutboxDelivery(object command, Func<IServiceProvider, CancellationToken, Task<Result>> dispatch)
{
    public object Command { get; } = command;
    public Task<Result> DispatchAsync(IServiceProvider services, CancellationToken cancellationToken) => dispatch(services, cancellationToken);
}

internal sealed record OutboxMapping(string TypeName, Func<string, OutboxDelivery> Resolve);

public static class OutboxRegistration
{
    public static IServiceCollection AddOutbox(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton(provider => new OutboxRegistry(provider.GetServices<OutboxMapping>()));
        return services;
    }

    public static IServiceCollection AddOutboxMessage<TMessage, TCommand>(this IServiceCollection services,
        Func<TMessage, TCommand> toCommand) where TMessage : class, IOutboxMessage
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(toCommand);
        if (string.IsNullOrWhiteSpace(TMessage.TypeName) || TMessage.TypeName.Length > 100)
            throw new InvalidOperationException("Outbox type names must contain 1 to 100 characters.");
        if (services.Any(descriptor => descriptor.ImplementationInstance is OutboxMapping mapping && mapping.TypeName == TMessage.TypeName))
            throw new InvalidOperationException($"Outbox type '{TMessage.TypeName}' is already registered.");

        var contracts = typeof(TCommand).GetInterfaces().Where(type => type.IsGenericType
            && type.GetGenericTypeDefinition() == typeof(ICommand<>)).ToArray();
        if (contracts.Length != 1) throw new InvalidOperationException("An outbox mapping must target exactly one command contract.");
        // Bind the response type once for this explicitly registered command. No payload-driven type lookup
        // or arbitrary command discovery occurs, and delivery itself uses the typed dispatcher delegate.
        var factory = typeof(OutboxRegistration).GetMethod(nameof(CreateMapping), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(typeof(TMessage), typeof(TCommand), contracts[0].GenericTypeArguments[0]);
        services.AddSingleton((OutboxMapping)factory.Invoke(null, [toCommand])!);
        return services.AddOutbox();
    }

    private static OutboxMapping CreateMapping<TMessage, TCommand, TResponse>(Func<TMessage, TCommand> toCommand)
        where TMessage : class, IOutboxMessage where TCommand : ICommand<TResponse> => new(TMessage.TypeName, payload =>
        {
            var message = JsonSerializer.Deserialize<TMessage>(payload, JsonSerializerOptions.Web)
                ?? throw new JsonException("An outbox payload cannot be null.");
            var command = toCommand(message);
            return new OutboxDelivery(command, async (services, ct) =>
                await services.GetRequiredService<Dispatcher>().SendAsync<TCommand, TResponse>(command, ct));
        });
}
