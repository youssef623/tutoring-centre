using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>Scans an assembly for command/query handlers and validators and registers them.</summary>
internal static class HandlerRegistration
{
    internal static IServiceCollection AddCqrsHandlers(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
            {
                continue;
            }

            foreach (var handlerInterface in type.GetInterfaces())
            {
                if (!handlerInterface.IsGenericType)
                {
                    continue;
                }

                var definition = handlerInterface.GetGenericTypeDefinition();
                if (definition == typeof(ICommandHandler<,>) || definition == typeof(IQueryHandler<,>))
                {
                    services.AddScoped(handlerInterface, type);
                }
            }
        }

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        return services;
    }
}
