using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application;

/// <summary>Registers the Application layer's services. Called once from the Api's composition root.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // One actor context per scope (HTTP request or job). ICurrentActor resolves to the SAME instance, read-only.
        services.AddScoped<CurrentActorContext>();
        services.AddScoped<ICurrentActor>(provider => provider.GetRequiredService<CurrentActorContext>());

        return services;
    }
}
