# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs`

## Purpose

Source summary (intent, not independent execution proof): Shared setup for Tasks 20.5/20.6: seeds Nile and Maadi fresh for every test (tables start empty — Day 19's PostgresTestBase), and offers the one helper every filter/guard test uses to set its actor.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 35 lines and 1,604 bytes. Source hash: `1bb623101e9cd6a2af72643684ce23df56cdc583d21c07eda1d4f70c7a0a5bbd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Centres.Commands.CreateCentre;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Domain.Centres;`
- Line 4: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs#L12): `public abstract class TenantProbeTestBase(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 27](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs#L27): `private protected Task<ProbeScope> CreateProbeScope(Actor actor) => Fixture.CreateProbeScope(actor);`
- [Line 29](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs#L29): `private async Task<Guid> CreateCentreAsync(string name, string slug)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs:13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs#L13)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs:12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupIntegrityTests.cs:12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupIntegrityTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs:12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs:17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs#L17)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs:21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L21)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentRaceTests.cs:15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentRaceTests.cs#L15)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs:20](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L20)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentAuditTrailTests.cs:16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentAuditTrailTests.cs#L16)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentIntegrityTests.cs:13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentIntegrityTests.cs#L13)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs:12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentReadServicePhoneSmokeTest.cs:14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentReadServicePhoneSmokeTest.cs#L14)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs:21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L21)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs:25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L25)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs:21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L21)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRaceTests.cs:14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRaceTests.cs#L14)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs:13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L13)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs:22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs#L22)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs:25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L25)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs:23](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L23)
- 11 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Shared setup for Tasks 20.5/20.6: seeds Nile and Maadi fresh for every test (tables start empty — Day 19's
/// PostgresTestBase), and offers the one helper every filter/guard test uses to set its actor.
/// </summary>
public abstract class TenantProbeTestBase(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    protected Guid NileCentreId { get; private set; }

    protected Guid MaadiCentreId { get; private set; }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        NileCentreId = await CreateCentreAsync("Nile Tutoring Centre", "nile-centre");
        MaadiCentreId = await CreateCentreAsync("Maadi Hub", "maadi-hub");
    }

    /// <summary>Opens a probe scope as the given actor — the context is created, then the actor is set, matching
    /// how a real request can build AppDbContext before login runs (Task 20.1).</summary>
    private protected Task<ProbeScope> CreateProbeScope(Actor actor) => Fixture.CreateProbeScope(actor);

    private async Task<Guid> CreateCentreAsync(string name, string slug)
    {
        var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(
            new SystemActor(null), new CreateCentreCommand(name, slug, "Africa/Cairo", SupportedLocale.Ar));
        return result.Value.CentreId;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
