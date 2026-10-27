# `tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs`

## Purpose

Source summary (intent, not independent execution proof): Hosts the real Api in-process against a throwaway PostgreSQL 17 container, on the same three-role split as everywhere else (Day 19): roles bootstrapped by the real db/bootstrap-roles.sh, migrated as tutoring_owner, the app itself running as tutoring_app. Environment "Testing" means no development auto-migration and no user-secrets: the test controls the database.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 126 lines and 6,173 bytes. Source hash: `97b457716515d51a0f960df91494bfe9c6f5ed5668678ff8408e2d800ee683e1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.Hosting;`
- Line 2: `using Microsoft.AspNetCore.Mvc.Testing;`
- Line 3: `using Microsoft.AspNetCore.TestHost;`
- Line 4: `using Microsoft.Extensions.DependencyInjection;`
- Line 5: `using Npgsql;`
- Line 6: `using Respawn;`
- Line 7: `using Respawn.Graph;`
- Line 8: `using Testcontainers.PostgreSql;`
- Line 9: `using TutoringCentre.Application.Common.Ports;`
- Line 10: `using TutoringCentre.Infrastructure.Persistence;`
- Line 11: `using TutoringCentre.Infrastructure.Time;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs#L21): `public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime`
- [Line 43](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs#L43): `public async Task InitializeAsync()`
- [Line 72](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs#L72): `public async Task ResetAsync()`
- [Line 80](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs#L80): `public Task<T> ScalarAsync<T>(string sql) => ScalarAsync<T>(SuperuserConnectionString, sql);`
- [Line 83](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs#L83): `public static async Task<T> ScalarAsync<T>(string connectionString, string sql)`
- [Line 92](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs#L92): `public async Task ExecuteAsync(string sql)`
- [Line 124](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs#L124): `private string WithCredentials(string username, string password) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs:18](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L18)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs:19](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L19)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs:16](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L16)
- [tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs:16](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L16)
- [tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs:15](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L15)
- [tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs:16](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L16)
- [tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs:21](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L21)
- [tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs:14](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs#L14)
- [tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs:10](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L10)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs:17](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L17)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleDoubleBookingHttpTests.cs:20](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleDoubleBookingHttpTests.cs#L20)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs:19](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L19)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs:16](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L16)
- [tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs:16](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L16)
- [tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs:10](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L10)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs:14](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L14)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs:10](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L10)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs:14](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L14)
- [tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs:16](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs#L16)
- [tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs:10](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L10)
- 22 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Time;

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

    /// <summary>"Now" for the application's IClock: real time unless a test fixes it (Day 53 schedule tests).</summary>
    public FixableTimeProvider Time { get; } = new();

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

```

The excerpt stops at line 90; use the original file for the remaining 36 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
