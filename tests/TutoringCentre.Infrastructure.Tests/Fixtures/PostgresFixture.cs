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
/// One throwaway PostgreSQL 17 container per test run, on the same three-role split as everywhere else
/// (Day 19): roles bootstrapped by the real db/bootstrap-roles.sh, migrated with the REAL migrations as
/// tutoring_owner, application services registered on tutoring_app. The container's superuser connection is
/// never used by the app under test — only to bootstrap roles and to reset between tests.
/// </summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    // Test-only; match no real environment and never outlive this one throwaway container.
    private const string OwnerPassword = "test-only-owner-password";
    private const string AppPassword = "test-only-app-password";

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17").Build();
    private Respawner? _respawner;

    public ServiceProvider Services { get; private set; } = null!;

    /// <summary>The container's own default role: a superuser. Used only to bootstrap roles and reset between tests.</summary>
    public string SuperuserConnectionString => _container.GetConnectionString();

    /// <summary>tutoring_owner: owns the schemas, runs migrations. Never resolved through the app's DI container.</summary>
    public string OwnerConnectionString => WithCredentials(DatabaseRoles.Owner, OwnerPassword);

    /// <summary>tutoring_app: what the application under test actually connects as.</summary>
    public string AppConnectionString => WithCredentials(DatabaseRoles.App, AppPassword);

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await RoleBootstrap.RunAsync(_container, OwnerPassword, AppPassword);

        // Accessing Services builds the host (ConfigureWebHost runs now that the container has a connection string).
        Services = CreateServiceProvider();
        await Services.ApplyMigrationsAsync();

        // Forced row-level security arrives Day 21, under which even tutoring_owner's delete without a tenant
        // setting removes nothing — so the reset between tests runs as the actual Postgres superuser instead.
        await using var connection = new NpgsqlConnection(SuperuserConnectionString);
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
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = AppConnectionString,
                ["ConnectionStrings:PostgresMigrations"] = OwnerConnectionString,
            })
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
        await using var connection = new NpgsqlConnection(SuperuserConnectionString);
        await connection.OpenAsync();
        await _respawner!.ResetAsync(connection);
    }

    /// <summary>Runs a scalar SQL statement against the test database as the superuser (assertions on persisted state).</summary>
    public Task<T> ScalarAsync<T>(string sql) => ScalarAsync<T>(SuperuserConnectionString, sql);

    /// <summary>Runs a scalar SQL statement over a specific connection — e.g. <see cref="AppConnectionString"/>, to assert what tutoring_app itself can see or do.</summary>
    public static async Task<T> ScalarAsync<T>(string connectionString, string sql)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync())!;
    }

    public Task<long> CountCentresAsync() => ScalarAsync<long>("select count(*) from platform.centres");

    private string WithCredentials(string username, string password) =>
        new NpgsqlConnectionStringBuilder(SuperuserConnectionString) { Username = username, Password = password }.ConnectionString;
}
