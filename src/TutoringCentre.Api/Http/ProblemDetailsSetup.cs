namespace TutoringCentre.Api.Http;

/// <summary>Registers Problem Details for framework-generated errors (empty 404/405, exception handler fallbacks).</summary>
public static class ProblemDetailsSetup
{
    public static IServiceCollection AddApiProblemDetails(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = context => ProblemResult.Enrich(context.ProblemDetails, context.HttpContext));

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}
