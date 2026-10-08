using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>
/// Hosts the real Api in-process against a throwaway PostgreSQL 17 container, on the same three-role split as
/// everywhere else (Day 19): roles bootstrapped by the real db/bootstrap-roles.sh, migrated as tutoring_owner,
/// the app itself running as tutoring_app. Environment "Testing" means no development auto-migration and no
/// user-secrets: the test controls the database.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Throwaway values for the seed and role-split tests; never the real development or any production password.
    internal const string TestSeedPassword = "Test-Only-Seed-Password-1!";
    private const string OwnerPassword = "test-only-owner-password";
    private const string AppPassword = "test-only-app-password";

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17").Build();
    private Respawner? _respawner;

    /// <summary>The container's own default role: a superuser. Used only to bootstrap roles and reset between tests.</summary>
    public string SuperuserConnectionString => _container.GetConnectionString();

    /// <summary>tutoring_owner: owns the schemas, runs migrations. Never resolved through the app's DI container.</summary>
    public string OwnerConnectionString => WithCredentials(DatabaseRoles.Owner, OwnerPassword);

    /// <summary>tutoring_app: what the running Api under test actually connects as.</summary>
    public string AppConnectionString => WithCredentials(DatabaseRoles.App, AppPassword);

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await RoleBootstrap.RunAsync(_container, OwnerPassword, AppPassword);

        // Accessing Services builds the host (ConfigureWebHost runs now that the container has connection strings).
        await Services.ApplyMigrationsAsync();

        // Forced row-level security arrives Day 21, under which even tutoring_owner's delete without a tenant
        // setting removes nothing — so the reset between tests runs as the actual Postgres superuser instead.
        await using var connection = new NpgsqlConnection(SuperuserConnectionString);
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["platform", "identity", "academics"],
            TablesToIgnore = [new Table("platform", "__ef_migrations_history")],
        });
    }

    Task IAsyncLifetime.DisposeAsync() => DisposeAsync().AsTask();

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _container.DisposeAsync();
        GC.SuppressFinalize(this);
    }

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

    /// <summary>Runs a non-query statement as the superuser (e.g. revoking a membership out from under a live session).</summary>
    public async Task ExecuteAsync(string sql)
    {
        await using var connection = new NpgsqlConnection(SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Postgres", AppConnectionString);
        builder.UseSetting("ConnectionStrings:PostgresMigrations", OwnerConnectionString);
        builder.UseSetting("Seed:Password", TestSeedPassword);

        // Zero: every test re-checks the session instead of racing a background cache window (Day 16).
        builder.UseSetting("SessionValidation:CacheDuration", "00:00:00");

        // Development turns these on by default; Testing does not. Missing registrations must fail the tests.
        builder.UseDefaultServiceProvider(options =>
        {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });
    }

    private string WithCredentials(string username, string password) =>
        new NpgsqlConnectionStringBuilder(SuperuserConnectionString) { Username = username, Password = password }.ConnectionString;
}
