using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;
using TutoringCentre.Application;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Persistence.Interceptors;
using TutoringCentre.Infrastructure.Persistence.Migrations;
using TutoringCentre.Infrastructure.Tests.Tenancy;

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

    private ServiceProvider? _probeServices;

    public ServiceProvider Services { get; private set; } = null!;

    /// <summary>
    /// A separate container, built only on first use, where AppDbContext resolves to the SAME instance as
    /// TenantProbeDbContext. That lets probe tests (Day 20/21) go through the real IUnitOfWork — whose
    /// constructor asks for AppDbContext — instead of touching TenantProbeDbContext directly and skipping
    /// UnitOfWork.BeginAsync (and, since Day 21, the app.current_centre setting row-level security depends on).
    /// Kept separate from <see cref="Services"/> so every other test's AppDbContext resolution is unaffected.
    /// </summary>
    internal ServiceProvider ProbeServices => _probeServices ??=
        CreateServiceProvider(services => services.AddScoped<AppDbContext>(provider => provider.GetRequiredService<TenantProbeDbContext>()));

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
        await CreateProbeTableAsync();

        // Forced row-level security arrives Day 21, under which even tutoring_owner's delete without a tenant
        // setting removes nothing — so the reset between tests runs as the actual Postgres superuser instead.
        await using var connection = new NpgsqlConnection(SuperuserConnectionString);
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["platform", "identity", "probe"],
            TablesToIgnore = [new Table("platform", "__ef_migrations_history")],
        });
    }

    /// <summary>
    /// Creates probe.tenant_probes (Task 20.3) as tutoring_owner, outside any migration — it never ships to
    /// production. Granted DML (not DDL) to tutoring_app, same as every real table the app writes through.
    /// </summary>
    private async Task CreateProbeTableAsync()
    {
        await using var connection = new NpgsqlConnection(OwnerConnectionString);
        await connection.OpenAsync();
        await using (var command = new NpgsqlCommand(
            """
            create schema if not exists probe;
            create table if not exists probe.tenant_probes (
                id uuid primary key,
                centre_id uuid not null references platform.centres(id),
                label varchar(50) not null
            );
            grant usage on schema probe to tutoring_app;
            grant select, insert, update, delete on probe.tenant_probes to tutoring_app;
            """,
            connection))
        {
            await command.ExecuteNonQueryAsync();
        }

        // Task 21.3: the exact SQL a real migration would run (TenantRowLevelSecurity.BuildEnableStatements),
        // reused rather than duplicated, so the probe table proves the production helper, not a stand-in for it.
        foreach (var statement in TenantRowLevelSecurity.BuildEnableStatements("probe", "tenant_probes"))
        {
            await using var command = new NpgsqlCommand(statement, connection);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DisposeAsync()
    {
        if (_probeServices is not null)
        {
            await _probeServices.DisposeAsync();
        }

        await Services.DisposeAsync();
        await _container.DisposeAsync();
    }

    /// <summary>
    /// Builds a container identical to production's, optionally with extra test-only registrations (handlers) or
    /// a non-default app connection string (Task 21.2's single-connection pool tests).
    /// </summary>
    public ServiceProvider CreateServiceProvider(Action<IServiceCollection>? configure = null, string? appConnectionString = null)
    {
        var connectionString = appConnectionString ?? AppConnectionString;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = connectionString,
                ["ConnectionStrings:PostgresMigrations"] = OwnerConnectionString,
            })
            .Build();

        var services = new ServiceCollection()
            .AddLogging()
            .AddApplication()
            .AddInfrastructure(configuration);

        // Task 20.3: same connection, same interceptors as AppDbContext, never pooled — the only difference is the
        // extra test-only TenantProbe mapping (AppDbContext.ExtendModel). Built by hand rather than AddDbContext:
        // EF's DI activation refuses a non-generic DbContextOptions constructor parameter (AppDbContext's own)
        // once a second DbContext type is registered in the same container, so there's no AddDbContext<TOther> to
        // reuse here — this context is scoped exactly like AppDbContext just without that helper.
        services.AddScoped(provider =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            AppDbContextOptionsConfigurator.Configure(
                optionsBuilder,
                connectionString,
                provider.GetRequiredService<TimestampInterceptor>(),
                provider.GetRequiredService<TenantWriteGuardInterceptor>());
            return new TenantProbeDbContext(optionsBuilder.Options, provider.GetRequiredService<ICurrentActor>());
        });

        // Task 21.2: lifetime probes for app.current_centre, dispatched like any real request.
        services.AddScoped<IQueryHandler<ReadCurrentCentreSettingQuery, string>, ReadCurrentCentreSettingQueryHandler>();
        services.AddScoped<ICommandHandler<ReadCurrentCentreSettingCommand, string>, ReadCurrentCentreSettingCommandHandler>();
        services.AddScoped<ICommandHandler<AlwaysFailingCommand, string>, AlwaysFailingCommandHandler>();

        // Task 21.5: each depends on exactly one isolation layer (the EF filter or row-level security), never both.
        services.AddScoped<IQueryHandler<ListProbeCentresIgnoringEfFilterQuery, List<Guid>>, ListProbeCentresIgnoringEfFilterQueryHandler>();
        services.AddScoped<IQueryHandler<ListProbeCentresQuery, List<Guid>>, ListProbeCentresQueryHandler>();
        services.AddScoped<IQueryHandler<ListProbeCentresByRawSqlQuery, List<Guid>>, ListProbeCentresByRawSqlQueryHandler>();

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

    /// <summary>
    /// Inserts one probe row for the given centre directly as the superuser — bypassing the filter, the guard and
    /// EF entirely, on purpose, so filter/guard tests seed known rows that the layer under test cannot have biased.
    /// Returns the generated id for tests that look a specific row up.
    /// </summary>
    public async Task<Guid> SeedProbeAsync(Guid centreId, string label)
    {
        var id = Guid.CreateVersion7();
        await using var connection = new NpgsqlConnection(SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "insert into probe.tenant_probes (id, centre_id, label) values (@id, @centre_id, @label)", connection);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("centre_id", centreId);
        command.Parameters.AddWithValue("label", label);
        await command.ExecuteNonQueryAsync();
        return id;
    }

    private string WithCredentials(string username, string password) =>
        new NpgsqlConnectionStringBuilder(SuperuserConnectionString) { Username = username, Password = password }.ConnectionString;
}
