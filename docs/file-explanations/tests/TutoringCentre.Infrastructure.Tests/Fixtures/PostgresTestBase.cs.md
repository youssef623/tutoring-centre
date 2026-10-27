# `tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs`

## Purpose

Source summary (intent, not independent execution proof): Base class for real-database tests: every test starts with empty tables.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 12 lines and 457 bytes. Source hash: `5770f2aa99c2600867acc8a7e9d047696f142c5bb93d487859901760905b94b8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs#L3): `/// <summary>Base class for real-database tests: every test starts with empty tables.</summary>`
- [Line 5](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs#L5): `public abstract class PostgresTestBase(PostgresFixture fixture) : IAsyncLifetime`
- [Line 11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs#L11): `public Task DisposeAsync() => Task.CompletedTask;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Academics/RowLevelSecurityCoverageTests.cs:12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/RowLevelSecurityCoverageTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs:14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs#L14)
- [tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs:9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs#L9)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs:11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L11)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs:15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs#L15)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs:15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L15)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs:12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs:6](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs#L6)
- [tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs:13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs#L13)
- [tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs:12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs:13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs#L13)
- [tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs:8](../../../../../tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs#L8)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRowLevelSecurityCatalogueTests.cs:10](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRowLevelSecurityCatalogueTests.cs#L10)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs:12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs#L12)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Infrastructure.Tests.Fixtures;

/// <summary>Base class for real-database tests: every test starts with empty tables.</summary>
[Collection(PostgresCollection.Name)]
public abstract class PostgresTestBase(PostgresFixture fixture) : IAsyncLifetime
{
    protected PostgresFixture Fixture { get; } = fixture;

    public virtual Task InitializeAsync() => Fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
