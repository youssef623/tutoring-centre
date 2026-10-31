using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Api.Tests.Fixtures;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Application.Assistant.Tools;
using TutoringCentre.Evaluation.Runner;
using TutoringCentre.Infrastructure.Persistence;
using Npgsql;
namespace TutoringCentre.Evaluation.Host;

public sealed class EvalHost(ILanguageModel model) : SharedApiFactory
{
    public ToolOutputCapture ToolOutputs { get; } = new();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "TutoringCentre.slnx"))) root = root.Parent;
        builder.UseContentRoot(Path.Combine(root?.FullName ?? throw new InvalidOperationException("eval.repository_missing"), "src", "TutoringCentre.Api"));
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Postgres"] = AppConnectionString,
            ["ConnectionStrings:PostgresMigrations"] = OwnerConnectionString
        }));

        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(model).PostConfigure<TutoringCentre.Infrastructure.Assistant.ModelProviderOptions>(options => { options.Provider = null; options.ApiKey = null; });
            foreach (var registration in services.Where(s => s.ServiceType == typeof(ITool)).ToArray())
            {
                services.Remove(registration);
                services.Add(new ServiceDescriptor(typeof(ITool), provider =>
                {
                    var real = registration.ImplementationInstance as ITool
                        ?? registration.ImplementationFactory?.Invoke(provider) as ITool
                        ?? (ITool)ActivatorUtilities.CreateInstance(provider, registration.ImplementationType!);
                    return new CapturingTool(real, ToolOutputs);
                }, registration.Lifetime));
            }
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var runtime = scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.GetConnectionString();
        if (!SameDatabase(runtime, AppConnectionString) || !SameDatabase(configuration.GetConnectionString("PostgresMigrations"), OwnerConnectionString))
        {
            host.Dispose();
            throw new InvalidOperationException("eval.container_required");
        }
        // SharedApiFactory invokes migrations only after this host has returned successfully.
        return host;
    }

    private static bool SameDatabase(string? actual, string expected)
    {
        if (actual is null) return false;
        var left = new NpgsqlConnectionStringBuilder(actual);
        var right = new NpgsqlConnectionStringBuilder(expected);
        // Production adds Application Name; compare the actual target/role/credential instead of formatting.
        return left.Host == right.Host && left.Port == right.Port && left.Database == right.Database
            && left.Username == right.Username && left.Password == right.Password;
    }
    // Connection strings cannot be supplied by CLI/config. Only this owned Testcontainer is accepted.
    public void RequireOwnedContainer(string connectionString)
    {
        if (!string.Equals(connectionString, SuperuserConnectionString, StringComparison.Ordinal)) throw new InvalidOperationException("eval.container_required");
    }
    public override Task ResetAsync() { RequireOwnedContainer(SuperuserConnectionString); return base.ResetAsync(); }
}
