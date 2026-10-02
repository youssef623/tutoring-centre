using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>Scans an assembly once and registers handlers (scoped) and validators.</summary>
internal static class HandlerRegistration
{
    internal static IServiceCollection AddCqrsHandlers(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        var implementations = assembly
            .GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false });

        foreach (var implementation in implementations)
        {
            foreach (var handlerInterface in implementation.GetInterfaces().Where(IsHandlerInterface))
            {
                // Scoped: one instance per request/job scope, matching the scoped DbContext and actor.
                services.AddScoped(handlerInterface, implementation);
            }
        }

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        return services;
    }

    private static bool IsHandlerInterface(Type type) =>
        type.IsGenericType
        && (type.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
            || type.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
}
