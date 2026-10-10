using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Centres;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity;
using TutoringCentre.Application.Platform;
using TutoringCentre.Application.Staff;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Persistence.Interceptors;
using TutoringCentre.Infrastructure.ReadServices;
using TutoringCentre.Infrastructure.Repositories;
using TutoringCentre.Infrastructure.Time;

namespace TutoringCentre.Infrastructure;

/// <summary>Registers Infrastructure implementations of Application ports. Called once from the Api's composition root.</summary>
public static class DependencyInjection
{
    private const string PostgresConnectionStringName = "Postgres";
    private const string PostgresMigrationsConnectionStringName = "PostgresMigrations";
    private const string PostgresHealthCheckName = "postgres";
    private const string PostgresRolePrivilegeHealthCheckName = "postgres-runtime-role";

    // A static array avoids allocating a new one per call (analyzer CA1861 under latest-recommended).
    private static readonly string[] ReadinessTags = ["ready"];

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // The host (WebApplicationBuilder) already registers this in production; registering it here too means
        // services that need IConfiguration directly (the development seeder's Seed:Password lookup) also resolve
        // in test harnesses that build a plain ServiceCollection instead of a full host.
        services.AddSingleton(configuration);

        // Time: stateless and thread-safe, so one instance for the app.
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IClock, SystemClock>();

        var connectionString = configuration.GetConnectionString(PostgresConnectionStringName);
        var migrationsConnectionString = configuration.GetConnectionString(PostgresMigrationsConnectionStringName);
        var healthChecks = services.AddHealthChecks();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            // Missing configuration must not crash startup; readiness reports it instead.
            healthChecks.AddCheck(
                PostgresHealthCheckName,
                () => HealthCheckResult.Unhealthy("ConnectionStrings:Postgres is not configured."),
                ReadinessTags);
        }
        else
        {
            healthChecks.AddNpgSql(connectionString, name: PostgresHealthCheckName, tags: ReadinessTags);

            // A tripwire, not a connectivity check: fails readiness if the runtime connection ever turns out to
            // be a superuser, BYPASSRLS, or the owner role, any of which would make RLS unenforceable.
            healthChecks.AddCheck(
                PostgresRolePrivilegeHealthCheckName,
                new RuntimeRolePrivilegeHealthCheck(connectionString),
                tags: ReadinessTags);
        }

        // Persistence. Timestamps and the tenant guard are stateless (the guard reads AppDbContext.CurrentCentreId
        // off the context being saved rather than holding a dependency of its own), so one instance each is enough.
        // Audit depends on the per-scope actor and correlation context, so it is scoped, not singleton.
        services.AddSingleton<TimestampInterceptor>();
        services.AddScoped<AuditInterceptor>();
        services.AddSingleton<TenantWriteGuardInterceptor>();

        services
            .AddOptions<DatabaseOptions>()
            .Configure(options =>
            {
                options.ConnectionString = connectionString ?? string.Empty;
                options.MigrationsConnectionString = migrationsConnectionString;
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Runtime registration: every request resolves AppDbContext through this, on the tutoring_app connection
        // only. MigrationRunner and the design-time factory build their own short-lived context on the owner
        // connection instead of resolving this one, so the owner connection is never available through DI.
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            AppDbContextOptionsConfigurator.Configure(
                options,
                serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString,
                serviceProvider.GetRequiredService<TimestampInterceptor>(),
                serviceProvider.GetRequiredService<AuditInterceptor>(),
                serviceProvider.GetRequiredService<TenantWriteGuardInterceptor>());
        });

        // The key ring that encrypts the session and antiforgery cookies, persisted in the same database
        // (Task 34.6) instead of the container's own file system, so every instance reads the same keys and a
        // cookie survives a restart or a redeploy. Registered here, not in the Api's AuthenticationSetup,
        // because only Program and the CLI may reference Infrastructure (ApiRuleTests) — AppDbContext is an
        // Infrastructure type. The application name is unchanged from Month 1: changing it would invalidate
        // every existing cookie.
        services.AddDataProtection().SetApplicationName("TutoringCentre").PersistKeysToDbContext<AppDbContext>();

        // One unit of work per scope: the dispatcher begins, saves and commits through it.
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICentreRepository, CentreRepository>();
        services.AddScoped<ICentreReadService, CentreReadService>();

        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<ISubjectReadService, SubjectReadService>();

        services.AddScoped<IMembershipRepository, MembershipRepository>();
        services.AddScoped<IStaffReadService, StaffReadService>();

        services.AddScoped<ISystemInfoReadService, SystemInfoReadService>();
        services.AddScoped<IMembershipReadService, MembershipReadService>();

        services.AddScoped<IAuditReadService, AuditReadService>();

        // Identity core only: no SignInManager, no Identity UI, no role services (there are no role tables —
        // a role belongs to a user in a centre, not globally). Cookie/sign-in wiring is Day 15.
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                // Modern guidance favours password length over composition rules.
                options.Password.RequiredLength = 10;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>();
        services.AddScoped<DevelopmentIdentitySeeder>();
        services.AddScoped<IAuthenticationService, IdentityAuthenticationService>();
        services.AddScoped<IStaffAccountService, StaffAccountService>();

        return services;
    }
}
