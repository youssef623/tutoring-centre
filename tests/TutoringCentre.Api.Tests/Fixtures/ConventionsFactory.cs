using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>The real API plus test-only endpoints/handlers and an in-memory log sink. Own container, own collection.</summary>
public sealed class ConventionsFactory : ApiFactory
{
    public InMemoryLogSink Logs { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.AddScoped<ICommandHandler<ConventionCommand, string>, ConventionCommandHandler>();
            services.AddScoped<ICommandHandler<TestNameCommand, string>, TestNameHandler>();
            services.AddScoped<IValidator<TestNameCommand>, TestNameValidator>();
            services.AddScoped<IQueryHandler<CurrentActorQuery, CurrentActorDto>, CurrentActorQueryHandler>();
            services.AddSingleton<ILogEventSink>(Logs);
            services.AddSingleton<IStartupFilter, ConventionEndpointsStartupFilter>();
        });
    }
}
