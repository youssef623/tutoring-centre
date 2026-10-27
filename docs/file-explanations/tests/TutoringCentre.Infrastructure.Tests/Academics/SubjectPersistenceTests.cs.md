# `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 23.6: Subject exercised through the real AppDbContext, interceptors and unit of work, as different actors, using the test-only requests in `SubjectTestRequests` — the repository and read service that would normally carry this arrive on Day 25.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 154 lines and 7,687 bytes. Source hash: `3d11616730174ed0dd5f8a8de098dee82c6d128ad387653ea1e6ee23d4046a2f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Npgsql;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Application.Common.Ports;`
- Line 6: `using TutoringCentre.Application.Common.Security;`
- Line 7: `using TutoringCentre.Domain.Academics;`
- Line 8: `using TutoringCentre.Domain.Common;`
- Line 9: `using TutoringCentre.Domain.Identity;`
- Line 10: `using TutoringCentre.Infrastructure.Persistence;`
- Line 11: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 12: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L21): `public sealed class SubjectPersistenceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 24](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L24): `public async Task NileActorAddsMathematics_PersistsWithNileCentreCreatedAtNoUpdatedAtActiveStatus()`
- [Line 40](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L40): `public async Task MaadiActorListsSubjects_DoesNotContainNilesSubject()`
- [Line 51](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L51): `public async Task NileActorAddsWhitespaceVariantOfMathematics_TranslatedToNameTakenConflict()`
- [Line 65](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L65): `public async Task MaadiActorAddsMathematics_Succeeds()`
- [Line 75](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L75): `public async Task StatusOutsideAllowedValuesWrittenByRawSql_ChecksViolation()`
- [Line 94](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L94): `public async Task Rename_SetsUpdatedAtAndChangesVersion()`
- [Line 111](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L111): `public async Task ConcurrentRenameFromTwoContexts_SecondSaveThrowsConcurrencyException()`
- [Line 135](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L135): `public async Task NileActorAddsSubjectConstructedWithMaadiCentre_TenantViolationAndNothingStored()`
- [Line 144](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L144): `private async Task<StaffActor> StaffActorInAsync(Guid centreId) => new(await Fixture.SeedUserAsync(), centreId, StaffRole.Teacher);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 23.6: Subject exercised through the real AppDbContext, interceptors and unit of work, as different
/// actors, using the test-only requests in <see cref="SubjectTestRequests"/> — the repository and read service
/// that would normally carry this arrive on Day 25.
/// </summary>
public sealed class SubjectPersistenceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task NileActorAddsMathematics_PersistsWithNileCentreCreatedAtNoUpdatedAtActiveStatus()
    {
        var result = await Fixture.SendAsAsync<AddSubjectCommand, Guid>(await StaffActorInAsync(NileCentreId), new AddSubjectCommand(NileCentreId, "Mathematics"));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"""
                select count(*) from academics.subjects
                where id = '{result.Value}' and centre_id = '{NileCentreId}' and status = 'active'
                  and created_at is not null and updated_at is null
                """));
    }

    [Fact]
    public async Task MaadiActorListsSubjects_DoesNotContainNilesSubject()
    {
        await Fixture.SendAsAsync<AddSubjectCommand, Guid>(await StaffActorInAsync(NileCentreId), new AddSubjectCommand(NileCentreId, "Mathematics"));

        var result = await Fixture.QueryAsAsync<ListSubjectNamesQuery, List<string>>(await StaffActorInAsync(MaadiCentreId), new ListSubjectNamesQuery());

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain("Mathematics", result.Value);
    }

    [Fact]
    public async Task NileActorAddsWhitespaceVariantOfMathematics_TranslatedToNameTakenConflict()
    {
        await Fixture.SendAsAsync<AddSubjectCommand, Guid>(await StaffActorInAsync(NileCentreId), new AddSubjectCommand(NileCentreId, "Mathematics"));

        // Day 25: the save-time unique violation is now translated to a Result, not an exception.
        var result = await Fixture.SendAsAsync<AddSubjectCommand, Guid>(await StaffActorInAsync(NileCentreId), new AddSubjectCommand(NileCentreId, " mathematics "));

        Assert.True(result.IsFailure);
        Assert.Equal("subject.name_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from academics.subjects"));
    }

    [Fact]
    public async Task MaadiActorAddsMathematics_Succeeds()
    {
        await Fixture.SendAsAsync<AddSubjectCommand, Guid>(await StaffActorInAsync(NileCentreId), new AddSubjectCommand(NileCentreId, "Mathematics"));

        var result = await Fixture.SendAsAsync<AddSubjectCommand, Guid>(await StaffActorInAsync(MaadiCentreId), new AddSubjectCommand(MaadiCentreId, "Mathematics"));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task StatusOutsideAllowedValuesWrittenByRawSql_ChecksViolation()
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.subjects (id, centre_id, name, normalized_name, status, created_at)
            values (@id, @centre_id, 'Mathematics', 'MATHEMATICS', 'retired', now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("23514", exception.SqlState); // check_violation
        Assert.Equal("ck_subjects_status", exception.ConstraintName);
```

The excerpt stops at line 90; use the original file for the remaining 64 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
