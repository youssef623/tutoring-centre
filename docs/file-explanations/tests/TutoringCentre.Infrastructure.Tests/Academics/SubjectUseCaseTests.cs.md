# `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 25.6: each Subject use case proven through the real dispatcher, unit of work, repository and read service against real PostgreSQL — the first time these six requests run against the database they were built for. Reads happen through the same actor in a fresh dispatch, never back through a scope that wrote.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 157 lines and 9,473 bytes. Source hash: `219c662bf3a2a0f550f9fa82494ac3675211c0f9fc3f7c8d0b80d4bb2b8aa7e6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Subjects;`
- Line 2: `using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;`
- Line 3: `using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;`
- Line 4: `using TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;`
- Line 8: `using RealRenameSubjectCommand = TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject.RenameSubjectCommand;`
- Line 9: `using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;`
- Line 10: `using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;`
- Line 11: `using TutoringCentre.Application.Common.Cqrs;`
- Line 12: `using TutoringCentre.Application.Common.Security;`
- Line 13: `using TutoringCentre.Domain.Academics;`
- Line 14: `using TutoringCentre.Domain.Identity;`
- Line 15: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 16: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L25): `public sealed class SubjectUseCaseTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 28](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L28): `public async Task Create_Succeeds_AndListContainsItWithAVersion()`
- [Line 44](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L44): `public async Task Create_SameNameDifferentCase_ReturnsNameTakenAndStillOneRow()`
- [Line 56](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L56): `public async Task Rename_SetsNewNameUpdatedTimestampAndChangesVersion()`
- [Line 75](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L75): `public async Task Rename_ToAnExistingName_ReturnsNameTaken()`
- [Line 89](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L89): `public async Task Archive_ExcludedFromDefaultList_IncludedWhenArchivedRequested()`
- [Line 106](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L106): `public async Task Archive_Again_ReturnsAlreadyArchived()`
- [Line 121](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L121): `public async Task Restore_BecomesActive()`
- [Line 137](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L137): `public async Task Get_UnknownId_ReturnsNotFound()`
- [Line 146](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L146): `public async Task Create_EightyOneCharacterName_ValidationFailureAndNoTransactionOpened()`
- [Line 156](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L156): `private async Task<StaffActor> NileOwnerAsync() => new(await Fixture.SeedUserAsync(), NileCentreId, StaffRole.Owner);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;
// Aliased under a different name: Task 23.6's test-only RenameSubjectCommand (a stand-in for the Day 25
// repository) is declared directly in this same test namespace, so it wins simple-name resolution over any
// using (even an aliased one) for the identical name "RenameSubjectCommand". This is the real, production one.
using RealRenameSubjectCommand = TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject.RenameSubjectCommand;
using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 25.6: each Subject use case proven through the real dispatcher, unit of work, repository and read
/// service against real PostgreSQL — the first time these six requests run against the database they were
/// built for. Reads happen through the same actor in a fresh dispatch, never back through a scope that wrote.
/// </summary>
public sealed class SubjectUseCaseTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task Create_Succeeds_AndListContainsItWithAVersion()
    {
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(
            await NileOwnerAsync(), new CreateSubjectCommand("Mathematics"));
        Assert.True(created.IsSuccess);

        var list = await Fixture.QueryAsAsync<ListSubjectsQuery, IReadOnlyList<SubjectDto>>(await NileOwnerAsync(), new ListSubjectsQuery(false));
        var fetched = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(await NileOwnerAsync(), new GetSubjectQuery(created.Value.SubjectId));

        Assert.True(list.IsSuccess);
        var dto = Assert.Single(list.Value, subject => subject.Id == created.Value.SubjectId);
        Assert.Equal("Mathematics", dto.Name);
        Assert.Equal(fetched.Value.Version, dto.Version); // the same row's version agrees across both read paths
    }

    [Fact]
    public async Task Create_SameNameDifferentCase_ReturnsNameTakenAndStillOneRow()
    {
        await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(await NileOwnerAsync(), new CreateSubjectCommand("Mathematics"));

        var second = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(await NileOwnerAsync(), new CreateSubjectCommand("MATHEMATICS"));

        Assert.True(second.IsFailure);
        Assert.Equal("subject.name_taken", second.Error!.Code);
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from academics.subjects"));
    }

    [Fact]
    public async Task Rename_SetsNewNameUpdatedTimestampAndChangesVersion()
    {
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(await NileOwnerAsync(), new CreateSubjectCommand("Mathematics"));
        var before = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(await NileOwnerAsync(), new GetSubjectQuery(created.Value.SubjectId));

        var renamed = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            await NileOwnerAsync(), new RealRenameSubjectCommand(created.Value.SubjectId, "Applied Mathematics", before.Value.Version));

        Assert.True(renamed.IsSuccess);
        var after = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(await NileOwnerAsync(), new GetSubjectQuery(created.Value.SubjectId));
        Assert.True(after.IsSuccess);
        Assert.Equal("Applied Mathematics", after.Value.Name);
        Assert.NotEqual(before.Value.Version, after.Value.Version);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>($"select count(*) from academics.subjects where id = '{created.Value.SubjectId}' and updated_at is not null"));
    }

    [Fact]
    public async Task Rename_ToAnExistingName_ReturnsNameTaken()
    {
        await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(await NileOwnerAsync(), new CreateSubjectCommand("Mathematics"));
        var second = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(await NileOwnerAsync(), new CreateSubjectCommand("Chemistry"));
        var secondDto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(await NileOwnerAsync(), new GetSubjectQuery(second.Value.SubjectId));

        var result = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            await NileOwnerAsync(), new RealRenameSubjectCommand(second.Value.SubjectId, "Mathematics", secondDto.Value.Version));

        Assert.True(result.IsFailure);
        Assert.Equal("subject.name_taken", result.Error!.Code);
    }

    [Fact]
    public async Task Archive_ExcludedFromDefaultList_IncludedWhenArchivedRequested()
    {
```

The excerpt stops at line 90; use the original file for the remaining 67 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
