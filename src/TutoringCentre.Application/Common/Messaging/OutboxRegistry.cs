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
    private readonly HashSet<string> _deferred;

    internal OutboxRegistry(IEnumerable<OutboxMapping> mappings, IEnumerable<DeferredOutboxType>? deferred = null)
    {
        _mappings = mappings.ToDictionary(mapping => mapping.TypeName, StringComparer.Ordinal);
        _deferred = (deferred ?? []).Select(message => message.TypeName).ToHashSet(StringComparer.Ordinal);
        if (_deferred.Overlaps(_mappings.Keys))
            throw new InvalidOperationException("An outbox type cannot be both deferred and mapped to a command.");
    }

    /// <summary>Known work awaiting its consumer; it must remain pending rather than be acknowledged or discarded.</summary>
    public bool IsRetryable(string typeName, string code) => _mappings.TryGetValue(typeName, out var mapping) && mapping.RetryableCodes.Contains(code, StringComparer.Ordinal);

    public bool IsDeferred(string typeName) => _deferred.Contains(typeName);

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

internal sealed record OutboxMapping(string TypeName, Func<string, OutboxDelivery> Resolve, IReadOnlyList<string> RetryableCodes);
internal sealed record DeferredOutboxType(string TypeName);

public static class OutboxRegistration
{
    public static IServiceCollection AddOutbox(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton(provider => new OutboxRegistry(provider.GetServices<OutboxMapping>(), provider.GetServices<DeferredOutboxType>()));
        return services;
    }

    /// <summary>Explicitly preserves a stable message type until its actual consumer is installed.</summary>
    public static IServiceCollection AddDeferredOutboxMessage<TMessage>(this IServiceCollection services)
        where TMessage : class, IOutboxMessage
    {
        ArgumentNullException.ThrowIfNull(services);
        ValidateRegistration(services, TMessage.TypeName);
        services.AddSingleton(new DeferredOutboxType(TMessage.TypeName));
        return services.AddOutbox();
    }

    /// <summary>Registers one stable message type to one idempotent command. Delivery may repeat after commit.</summary>
    public static IServiceCollection AddOutboxMessage<TMessage, TCommand>(this IServiceCollection services,
        Func<TMessage, TCommand> toCommand, params string[] retryableCodes) where TMessage : class, IOutboxMessage
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(toCommand);
        ValidateRegistration(services, TMessage.TypeName, allowDeferred: true);

        var contracts = typeof(TCommand).GetInterfaces().Where(type => type.IsGenericType
            && type.GetGenericTypeDefinition() == typeof(ICommand<>)).ToArray();
        if (contracts.Length != 1) throw new InvalidOperationException("An outbox mapping must target exactly one command contract.");
        // Bind the response type once for this explicitly registered command. No payload-driven type lookup
        // or arbitrary command discovery occurs, and delivery itself uses the typed dispatcher delegate.
        var factory = typeof(OutboxRegistration).GetMethod(nameof(CreateMapping), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(typeof(TMessage), typeof(TCommand), contracts[0].GenericTypeArguments[0]);
        var mapping = (OutboxMapping)factory.Invoke(null, [toCommand, retryableCodes])!;
        // Installing the real consumer explicitly promotes pending work; the type is never both active and deferred.
        foreach (var descriptor in services.Where(descriptor => descriptor.ImplementationInstance is DeferredOutboxType deferred
            && deferred.TypeName == TMessage.TypeName).ToArray())
            services.Remove(descriptor);
        services.AddSingleton(mapping);
        return services.AddOutbox();
    }

    private static void ValidateRegistration(IServiceCollection services, string typeName, bool allowDeferred = false)
    {
        if (string.IsNullOrWhiteSpace(typeName) || typeName.Length > 100)
            throw new InvalidOperationException("Outbox type names must contain 1 to 100 characters.");
        if (services.Any(descriptor => descriptor.ImplementationInstance is OutboxMapping mapping && mapping.TypeName == typeName
            || !allowDeferred && descriptor.ImplementationInstance is DeferredOutboxType deferred && deferred.TypeName == typeName))
            throw new InvalidOperationException($"Outbox type '{typeName}' is already registered.");
    }

    private static OutboxMapping CreateMapping<TMessage, TCommand, TResponse>(Func<TMessage, TCommand> toCommand, string[] retryableCodes)
        where TMessage : class, IOutboxMessage where TCommand : ICommand<TResponse> => new(TMessage.TypeName, payload =>
        {
            var message = JsonSerializer.Deserialize<TMessage>(payload, JsonSerializerOptions.Web)
                ?? throw new JsonException("An outbox payload cannot be null.");
            var command = toCommand(message);
            return new OutboxDelivery(command, async (services, ct) =>
                await services.GetRequiredService<Dispatcher>().SendAsync<TCommand, TResponse>(command, ct));
        }, Array.AsReadOnly(retryableCodes.ToArray()));
}
