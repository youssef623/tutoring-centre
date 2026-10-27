# `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 38.5: the Student aggregate exercised through the real AppDbContext, interceptors and unit of work, using the test-only requests in `StudentParentTestRequests` — the repositories and use cases that would normally carry this arrive on Days 39–40.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 193 lines and 11,438 bytes. Source hash: `3c4fc1fc4e8c2bd783dc75b9d25a99844f96155b0a54d76c2221f20ebc5ee529`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`
- Line 4: `using TutoringCentre.Application.Common.Ports;`
- Line 5: `using TutoringCentre.Application.Common.Security;`
- Line 6: `using TutoringCentre.Domain.Academics;`
- Line 7: `using TutoringCentre.Domain.Common;`
- Line 8: `using TutoringCentre.Domain.Identity;`
- Line 9: `using TutoringCentre.Infrastructure.Persistence;`
- Line 10: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 11: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 20](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L20): `public sealed class StudentAggregatePersistenceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 23](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L23): `public async Task NileActorSavesParentAndStudent_PersistsThreeRowsWithNileCentreNormalizedSearchNameAndE164Phone()`
- [Line 46](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L46): `public async Task Reload_StudentHasOnePrimaryLink()`
- [Line 62](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L62): `public async Task AddSecondParentAndSave_LinksRevisionIsOneAndStudentVersionChanged()`
- [Line 83](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L83): `public async Task TwoContextsLoadSameStudentAndEachAddsADifferentParent_SecondSaveThrowsConcurrencyException()`
- [Line 112](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L112): `public async Task SetPrimaryParentAndSave_SucceedsWithExactlyOnePrimaryRow()`
- [Line 136](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L136): `public async Task RemoveParentAndSave_LinkRowIsGone()`
- [Line 156](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L156): `public async Task MaadiActorListsStudents_DoesNotContainNilesStudent()`
- [Line 170](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L170): `public async Task NileActorSavesStudentWhoseLinkPointsAtAMaadiParent_SaveFailsOnForeignKeyAndNothingStored()`
- [Line 183](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L183): `private async Task<StaffActor> StaffActorInAsync(Guid centreId) => new(await Fixture.SeedUserAsync(), centreId, StaffRole.Teacher);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
/// Task 38.5: the Student aggregate exercised through the real AppDbContext, interceptors and unit of work,
/// using the test-only requests in <see cref="StudentParentTestRequests"/> — the repositories and use cases
/// that would normally carry this arrive on Days 39–40.
/// </summary>
public sealed class StudentAggregatePersistenceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task NileActorSavesParentAndStudent_PersistsThreeRowsWithNileCentreNormalizedSearchNameAndE164Phone()
    {
        var actor = await StaffActorInAsync(NileCentreId);

        var parentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(actor, new AddParentCommand(NileCentreId, "Omar Hassan", "01012345678"))).Value;
        var studentId = (await Fixture.SendAsAsync<RegisterStudentCommand, Guid>(
            actor, new RegisterStudentCommand(NileCentreId, "Youssef Omar", "01112345678", "Grade 10", parentId, ParentRelationship.Father))).Value;

        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.parents where id = '{parentId}' and centre_id = '{NileCentreId}' and search_name = 'omar hassan' and phone = '+201012345678'"));
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.students where id = '{studentId}' and centre_id = '{NileCentreId}' and search_name = 'youssef omar' and phone = '+201112345678'"));
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.student_parents where student_id = '{studentId}' and parent_id = '{parentId}'"));
    }

    [Fact]
    public async Task Reload_StudentHasOnePrimaryLink()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var parentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(actor, new AddParentCommand(NileCentreId, "Omar Hassan", "01012345678"))).Value;
        var studentId = (await Fixture.SendAsAsync<RegisterStudentCommand, Guid>(
            actor, new RegisterStudentCommand(NileCentreId, "Youssef Omar", null, null, parentId, ParentRelationship.Father))).Value;

        var snapshot = await Fixture.QueryAsAsync<GetStudentLinksSnapshotQuery, StudentLinksSnapshot>(actor, new GetStudentLinksSnapshotQuery(studentId));

        Assert.True(snapshot.IsSuccess);
        var link = Assert.Single(snapshot.Value.Links);
        Assert.Equal(parentId, link.ParentId);
        Assert.True(link.IsPrimary);
    }

    [Fact]
    public async Task AddSecondParentAndSave_LinksRevisionIsOneAndStudentVersionChanged()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var firstParentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(actor, new AddParentCommand(NileCentreId, "Omar Hassan", "01012345678"))).Value;
        var secondParentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(actor, new AddParentCommand(NileCentreId, "Amal Fathy", "01212345678"))).Value;
        var studentId = (await Fixture.SendAsAsync<RegisterStudentCommand, Guid>(
            actor, new RegisterStudentCommand(NileCentreId, "Youssef Omar", null, null, firstParentId, ParentRelationship.Father))).Value;
        var versionBefore = await Fixture.ScalarAsync<string>($"select xmin::text from academics.students where id = '{studentId}'");

        var result = await Fixture.SendAsAsync<AddStudentParentCommand, Unit>(
            actor, new AddStudentParentCommand(studentId, secondParentId, ParentRelationship.Mother));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<int>($"select links_revision from academics.students where id = '{studentId}'"));
        var versionAfter = await Fixture.ScalarAsync<string>($"select xmin::text from academics.students where id = '{studentId}'");
        Assert.NotEqual(versionBefore, versionAfter);
    }

    [Fact]
    public async Task TwoContextsLoadSameStudentAndEachAddsADifferentParent_SecondSaveThrowsConcurrencyException()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var firstParentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(actor, new AddParentCommand(NileCentreId, "Omar Hassan", "01012345678"))).Value;
        var secondParentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(actor, new AddParentCommand(NileCentreId, "Amal Fathy", "01212345678"))).Value;
        var thirdParentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(actor, new AddParentCommand(NileCentreId, "Hoda Ali", "01512345678"))).Value;
        var studentId = (await Fixture.SendAsAsync<RegisterStudentCommand, Guid>(
            actor, new RegisterStudentCommand(NileCentreId, "Youssef Omar", null, null, firstParentId, ParentRelationship.Father))).Value;
```

The excerpt stops at line 90; use the original file for the remaining 103 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
