# `src/TutoringCentre.Infrastructure/DependencyInjection.cs`

## Purpose

The infrastructure composition method binds runtime and migration database options, registers clocks and interceptor lifetimes, creates the runtime AppDbContext, persists Data Protection keys to PostgreSQL, wires repository/read-service ports, and configures Identity. Runtime DI uses the application connection; migration construction is separate. Interceptor lifetimes differ because audit reads scoped identity while timestamp and tenant guard are stateless.

## Where it fits

Category: **source**. [Open current file](../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 183 lines and 9,676 bytes. Source hash: `cbaa54589b6b8c372d9fddddd8cf8a25d40286894b0d77afa4a62e02cd1c4977`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.DataProtection;`
- Line 2: `using Microsoft.AspNetCore.Identity;`
- Line 3: `using Microsoft.EntityFrameworkCore;`
- Line 4: `using Microsoft.Extensions.Configuration;`
- Line 5: `using Microsoft.Extensions.DependencyInjection;`
- Line 6: `using Microsoft.Extensions.Diagnostics.HealthChecks;`
- Line 7: `using Microsoft.Extensions.Options;`
- Line 8: `using TutoringCentre.Application.Academics.Enrollments;`
- Line 9: `using TutoringCentre.Application.Academics.Parents;`
- Line 10: `using TutoringCentre.Application.Academics.Students;`
- Line 11: `using TutoringCentre.Application.Academics.Groups;`
- Line 12: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 13: `using TutoringCentre.Application.Academics.Subjects;`
- Line 14: `using TutoringCentre.Application.Academics.Teachers;`
- Line 15: `using TutoringCentre.Application.Audit;`
- Line 16: `using TutoringCentre.Application.Centres;`
- Line 17: `using TutoringCentre.Application.Centres.Contracts;`
- Line 18: `using TutoringCentre.Application.Common.Ports;`
- Line 19: `using TutoringCentre.Application.Common.Security;`
- Line 20: `using TutoringCentre.Application.Identity;`
- Line 21: `using TutoringCentre.Application.Identity.Contracts;`
- Line 22: `using TutoringCentre.Application.Platform;`
- Line 23: `using TutoringCentre.Application.Staff;`
- Line 24: `using TutoringCentre.Infrastructure.Centres;`
- Line 25: `using TutoringCentre.Infrastructure.Identity;`
- Line 26: `using TutoringCentre.Infrastructure.Persistence;`
- Line 27: `using TutoringCentre.Infrastructure.Persistence.Interceptors;`
- Line 28: `using TutoringCentre.Infrastructure.ReadServices;`
- Line 29: `using TutoringCentre.Infrastructure.Repositories;`
- Line 30: `using TutoringCentre.Infrastructure.Time;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 35](../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L35): `public static class DependencyInjection`
- [Line 45](../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L45): `public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Centres;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity;
using TutoringCentre.Application.Identity.Contracts;
using TutoringCentre.Application.Platform;
using TutoringCentre.Application.Staff;
using TutoringCentre.Infrastructure.Centres;
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
```

The excerpt stops at line 90; use the original file for the remaining 93 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
