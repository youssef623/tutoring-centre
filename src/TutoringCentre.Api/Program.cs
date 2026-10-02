using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using TutoringCentre.Api.Cli;
using TutoringCentre.Application;
using TutoringCentre.Infrastructure;
using TutoringCentre.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(new RenderedCompactJsonFormatter()));

builder.Services.AddHealthChecks();
builder.Services.AddApplication().AddInfrastructure(builder.Configuration);

var app = builder.Build();

// CLI mode: `dotnet run --project src/TutoringCentre.Api -- seed`
if (args is ["seed"])
{
    await app.Services.ApplyMigrationsAsync();
    return await SeedCommand.RunAsync(app.Services);
}

// Development convenience only. Production migrations run from the deployment pipeline (Month 2), never at app startup:
// auto-migrating there is risky (several instances racing, no review, long locks).
if (app.Environment.IsDevelopment())
{
    await app.Services.ApplyMigrationsAsync();
}

// One line per HTTP request; health probes are noise at Information level.
app.UseSerilogRequestLogging(options => options.GetLevel = (httpContext, _, exception) =>
    exception is not null || httpContext.Response.StatusCode >= 500
        ? LogEventLevel.Error
        : httpContext.Request.Path.StartsWithSegments("/health")
            ? LogEventLevel.Verbose
            : LogEventLevel.Information);

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false,
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = c => c.Tags.Contains("ready"),
});

app.Run();
return 0;

public partial class Program;
