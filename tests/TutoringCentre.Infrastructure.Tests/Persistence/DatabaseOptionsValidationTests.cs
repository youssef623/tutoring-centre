using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TutoringCentre.Application;
using TutoringCentre.Infrastructure;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

/// <summary>
/// Pure options-binding tests: no database is needed, since these assert on configuration shape, not connectivity.
/// </summary>
public sealed class DatabaseOptionsValidationTests
{
    [Fact]
    public void ResolvingDatabaseOptions_WithoutRuntimeConnectionString_ThrowsNamingTheKey()
    {
        var provider = BuildProvider(postgres: null, postgresMigrations: null);

        var exception = Assert.Throws<OptionsValidationException>(
            () => provider.GetRequiredService<IOptions<DatabaseOptions>>().Value);

        Assert.Contains("ConnectionString", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ResolvingDatabaseOptions_WithOnlyRuntimeConnectionString_Succeeds()
    {
        var provider = BuildProvider(postgres: "Host=localhost;Database=x;Username=x;Password=x", postgresMigrations: null);

        var options = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

        Assert.Equal("Host=localhost;Database=x;Username=x;Password=x", options.ConnectionString);
        Assert.Null(options.MigrationsConnectionString);
    }

    [Fact]
    public async Task ApplyMigrationsAsync_WithoutMigrationsConnectionString_ThrowsNamingTheKey()
    {
        var provider = BuildProvider(postgres: "Host=localhost;Database=x;Username=x;Password=x", postgresMigrations: null);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => provider.ApplyMigrationsAsync());

        Assert.Contains("ConnectionStrings:PostgresMigrations", exception.Message, StringComparison.Ordinal);
    }

    private static ServiceProvider BuildProvider(string? postgres, string? postgresMigrations)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = postgres,
                ["ConnectionStrings:PostgresMigrations"] = postgresMigrations,
            })
            .Build();

        var services = new ServiceCollection()
            .AddLogging()
            .AddApplication()
            .AddInfrastructure(configuration);

        return services.BuildServiceProvider();
    }
}
