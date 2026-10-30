using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Models;

namespace TutoringCentre.Infrastructure.Assistant;

public static class AssistantRegistration
{
    public static IServiceCollection AddAssistant(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        services.AddScoped(provider => provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AssistantOptions>>().Value);
        services.AddScoped<AgentLoop>();
        services.AddOptions<AssistantOptions>().Bind(configuration.GetSection("Assistant")).ValidateDataAnnotations()
            .Validate(options => options.CallTimeout > TimeSpan.Zero && options.CallTimeout <= TimeSpan.FromMinutes(5), "CallTimeout must be positive and at most five minutes.")
            .Validate(options => options.TotalRunTimeout > TimeSpan.Zero && options.TotalRunTimeout <= TimeSpan.FromSeconds(60), "TotalRunTimeout must be positive and at most sixty seconds.")
            .ValidateOnStart();
        services.AddOptions<ModelProviderOptions>().Bind(configuration.GetSection("Assistant"))
            .Validate(options => options.Provider is null or "live" or "scripted", "Provider must be live or scripted.")
            .Validate(options => options.Provider != "live" || (!string.IsNullOrWhiteSpace(options.ApiKey) && !string.IsNullOrWhiteSpace(options.Model)), "Live provider requires ApiKey and Model.")
            .Validate<IHostEnvironment>((options, environment) => options.Provider != "scripted"
                || (environment.IsEnvironment("Testing") && File.Exists(options.ScriptPath)),
                "Scripted provider requires Testing and an existing script file.")
            .ValidateOnStart();
        services.AddSingleton<IChatClient>(provider =>
        {
            var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ModelProviderOptions>>().Value;
            if (options.Provider != "live") throw new InvalidOperationException("The live model client is not selected.");
            return new OpenAIClient(options.ApiKey!).GetChatClient(options.Model!).AsIChatClient();
        });
        services.AddSingleton<ILanguageModel>(provider =>
        {
            var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ModelProviderOptions>>().Value;
            if (options.Provider == "scripted") return new ScriptedLanguageModel(options.ScriptPath!);
            if (options.Provider != "live") throw new InvalidOperationException("Select Assistant:Provider before using the assistant.");
            return new ProviderLanguageModel(provider.GetRequiredService<IChatClient>(),
                provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AssistantOptions>>(),
                provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<ProviderLanguageModel>>(), options.Model);
        });
        return services;
    }
}

public sealed class ModelProviderOptions
{
    public string? Provider { get; set; }
    public string? Model { get; set; }
    public string? ApiKey { get; set; }
    public string? ScriptPath { get; set; }
}
