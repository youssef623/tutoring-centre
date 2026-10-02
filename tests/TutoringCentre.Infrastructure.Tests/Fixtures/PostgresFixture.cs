using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;
using TutoringCentre.Application;
using TutoringCentre.Infrastructure;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Fixtures;

/// <summary>
/// One throwaway PostgreSQL 17 container per test run, migrated with the REAL migrations, reset by Respawn between tests.
/// The container's connection string is the only one ever used — never the development database.
/// </summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17").Build();
    private Respawner? _respawner;

    public ServiceProvider Services { get; private set; } = null!;

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        Services = CreateServiceProvider();
        await Services.ApplyMigrationsAsync();

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["platform", "identity"],
            TablesToIgnore = [new Table("platform", "__ef_migrations_history")],
        });
    }

    public async Task DisposeAsync()
    {
        await Services.DisposeAsync();
        await _container.DisposeAsync();
    }

    /// <summary>Builds a container identical to production's, optionally with extra test-only registrations (handlers).</summary>
    public ServiceProvider CreateServiceProvider(Action<IServiceCollection>? configure = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:Postgres"] = ConnectionString })
            .Build();

        var services = new ServiceCollection()
            .AddLogging()
            .AddApplication()
            .AddInfrastructure(configuration);
        configure?.Invoke(services);

        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    public IServiceScope CreateScope() => Services.CreateScope();

    /// <summary>Deletes all rows in the platform and identity schemas except the migrations history.</summary>
    public async Task ResetAsync()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await _respawner!.ResetAsync(connection);
    }

    /// <summary>Runs a scalar SQL statement against the test database (assertions on persisted state).</summary>
    public async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync())!;
    }

    public Task<long> CountCentresAsync() => ScalarAsync<long>("select count(*) from platform.centres");
}
