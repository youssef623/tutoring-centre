using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using TutoringCentre.Application;
using TutoringCentre.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(new RenderedCompactJsonFormatter()));

builder.Services.AddHealthChecks();
builder.Services.AddApplication().AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsedMs, ex) => ex is not null
        ? LogEventLevel.Error
        : httpContext.Response.StatusCode >= 500
            ? LogEventLevel.Error
            : httpContext.Request.Path.StartsWithSegments("/health")
                ? LogEventLevel.Verbose
                : LogEventLevel.Information;
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false,
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = c => c.Tags.Contains("ready"),
});

app.Run();

public partial class Program;
