# `tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs`

## Purpose

Source summary (intent, not independent execution proof): One throwaway PostgreSQL 17 container per test run, on the same three-role split as everywhere else (Day 19): roles bootstrapped by the real db/bootstrap-roles.sh, migrated with the REAL migrations as tutoring_owner, application services registered on tutoring_app. The container's superuser connection is never used by the app under test — only to bootstrap roles and to reset between tests.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 478 lines and 26,338 bytes. Source hash: `afff31097e0ff1d8b4e4c4f126a232b8977a4c9baa0e83148597ed478adfcaf4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.Extensions.Configuration;`
- Line 3: `using Microsoft.Extensions.DependencyInjection;`
- Line 4: `using Npgsql;`
- Line 5: `using Respawn;`
- Line 6: `using Respawn.Graph;`
- Line 7: `using Testcontainers.PostgreSql;`
- Line 8: `using TutoringCentre.Application;`
- Line 9: `using TutoringCentre.Application.Common.Cqrs;`
- Line 10: `using TutoringCentre.Application.Common.Security;`
- Line 11: `using TutoringCentre.Application.Platform;`
- Line 12: `using TutoringCentre.Domain.Common;`
- Line 13: `using TutoringCentre.Infrastructure;`
- Line 14: `using TutoringCentre.Infrastructure.Persistence;`
- Line 15: `using TutoringCentre.Infrastructure.Persistence.Interceptors;`
- Line 16: `using TutoringCentre.Infrastructure.Persistence.Migrations;`
- Line 17: `using TutoringCentre.Infrastructure.Tests.Academics;`
- Line 18: `using TutoringCentre.Infrastructure.Tests.Identity;`
- Line 19: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 29](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L29): `public sealed class PostgresFixture : IAsyncLifetime`
- [Line 61](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L61): `public async Task InitializeAsync()`
- [Line 87](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L87): `private async Task CreateProbeTableAsync()`
- [Line 128](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L128): `public async Task DisposeAsync()`
- [Line 143](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L143): `public ServiceProvider CreateServiceProvider(Action<IServiceCollection>? configure = null, string? appConnectionString = null)`
- [Line 219](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L219): `public IServiceScope CreateScope() => Services.CreateScope();`
- [Line 222](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L222): `public async Task ResetAsync()`
- [Line 230](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L230): `public Task<T> ScalarAsync<T>(string sql) => ScalarAsync<T>(SuperuserConnectionString, sql);`
- [Line 233](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L233): `public static async Task<T> ScalarAsync<T>(string connectionString, string sql)`
- [Line 241](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L241): `public Task<long> CountCentresAsync() => ScalarAsync<long>("select count(*) from platform.centres");`
- [Line 248](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L248): `public async Task<Guid> SeedProbeAsync(Guid centreId, string label)`
- [Line 267](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L267): `public async Task<Guid> SeedSubjectAsync(Guid centreId, string name)`
- [Line 291](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L291): `public async Task<Guid> SeedParentAsync(Guid centreId, string fullName, string phone)`
- [Line 312](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L312): `public async Task<Guid> SeedStudentAsync(Guid centreId, string fullName, string status = "active")`
- [Line 333](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L333): `public async Task<Guid> SeedStudentParentLinkAsync(Guid centreId, Guid studentId, Guid parentId, bool isPrimary, string relationship = "father")`
- [Line 359](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L359): `public async Task<Guid> SeedUserAsync(string displayName = "Test Actor")`
- [Line 383](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L383): `public async Task<Guid> SeedAuditEntryAsync(`
- [Line 424](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L424): `public async Task<Guid> SeedTeacherAsync(Guid centreId, string fullName, Guid? userId = null)`
- [Line 449](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L449): `public async Task<Guid> SeedGroupAsync(Guid centreId, string name, Guid subjectId, Guid teacherId, int capacity = 20)`
- [Line 471](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L471): `private string WithCredentials(string username, string password) =>`
- [Line 474](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L474): `private sealed class AlwaysDisabledDemoModeProvider : IDemoModeProvider`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs:31](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L31)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs:13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs#L13)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs:12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs:14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs#L14)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs:16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L16)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs:18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L18)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs:13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs#L13)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs:24](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L24)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupIntegrityTests.cs:12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupIntegrityTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs:12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs:19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L19)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs:22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L22)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs:22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L22)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs:25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L25)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs:17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs#L17)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs:17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs#L17)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs:21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L21)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentRaceTests.cs:15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentRaceTests.cs#L15)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/RowLevelSecurityCoverageTests.cs:12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/RowLevelSecurityCoverageTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleDstPersistenceTests.cs:9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleDstPersistenceTests.cs#L9)
- 56 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
using TutoringCentre.Application.Platform;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Persistence.Interceptors;
using TutoringCentre.Infrastructure.Persistence.Migrations;
using TutoringCentre.Infrastructure.Tests.Academics;
using TutoringCentre.Infrastructure.Tests.Identity;
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
            SchemasToInclude = ["platform", "identity", "academics", "probe", "audit"],
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
```

The excerpt stops at line 90; use the original file for the remaining 388 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
