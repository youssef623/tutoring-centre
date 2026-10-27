# `tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 43.14: each Teacher use case (excluding the account link, proven separately in `TeacherAccountLinkTests`) through the real dispatcher, unit of work, repository and read service against real PostgreSQL. Reads happen through the same actor in a fresh dispatch, never back through a scope that wrote.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 120 lines and 7,031 bytes. Source hash: `356d32a5eb860cdc1538cf7cfc292d7115e7da019dfd15bfe497355a0ebbbaa2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers;`
- Line 2: `using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;`
- Line 3: `using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;`
- Line 4: `using TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;`
- Line 5: `using TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;`
- Line 6: `using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;`
- Line 7: `using TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;`
- Line 8: `using TutoringCentre.Application.Common.Cqrs;`
- Line 9: `using TutoringCentre.Application.Common.Security;`
- Line 10: `using TutoringCentre.Domain.Academics;`
- Line 11: `using TutoringCentre.Domain.Identity;`
- Line 12: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 13: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 23](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L23): `public sealed class TeacherUseCaseTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 26](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L26): `public async Task Create_Succeeds_AndListContainsItWithAVersion()`
- [Line 42](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L42): `public async Task Update_SetsNewDetailsAndChangesVersion()`
- [Line 60](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L60): `public async Task Archive_ExcludedFromDefaultList_IncludedWhenArchivedRequested()`
- [Line 78](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L78): `public async Task Archive_Again_ReturnsAlreadyArchived()`
- [Line 94](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L94): `public async Task Restore_BecomesActive()`
- [Line 111](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L111): `public async Task Get_UnknownId_ReturnsNotFound()`
- [Line 119](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L119): `private async Task<StaffActor> NileOwnerAsync() => new(await Fixture.SeedUserAsync(), NileCentreId, StaffRole.Owner);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 43.14: each Teacher use case (excluding the account link, proven separately in
/// <see cref="TeacherAccountLinkTests"/>) through the real dispatcher, unit of work, repository and read
/// service against real PostgreSQL. Reads happen through the same actor in a fresh dispatch, never back
/// through a scope that wrote.
/// </summary>
public sealed class TeacherUseCaseTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task Create_Succeeds_AndListContainsItWithAVersion()
    {
        var created = await Fixture.SendAsAsync<CreateTeacherCommand, CreateTeacherResult>(
            await NileOwnerAsync(), new CreateTeacherCommand("Amina Hassan", null));
        Assert.True(created.IsSuccess);

        var list = await Fixture.QueryAsAsync<ListTeachersQuery, IReadOnlyList<TeacherDto>>(await NileOwnerAsync(), new ListTeachersQuery(false));
        var fetched = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(await NileOwnerAsync(), new GetTeacherQuery(created.Value.TeacherId));

        Assert.True(list.IsSuccess);
        var dto = Assert.Single(list.Value, teacher => teacher.Id == created.Value.TeacherId);
        Assert.Equal("Amina Hassan", dto.FullName);
        Assert.Equal(fetched.Value.Version, dto.Version);
    }

    [Fact]
    public async Task Update_SetsNewDetailsAndChangesVersion()
    {
        var created = await Fixture.SendAsAsync<CreateTeacherCommand, CreateTeacherResult>(
            await NileOwnerAsync(), new CreateTeacherCommand("Amina Hassan", null));
        var before = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(await NileOwnerAsync(), new GetTeacherQuery(created.Value.TeacherId));

        var updated = await Fixture.SendAsAsync<UpdateTeacherCommand, Unit>(
            await NileOwnerAsync(), new UpdateTeacherCommand(created.Value.TeacherId, "Amina Youssef", "+201012345678", before.Value.Version));

        Assert.True(updated.IsSuccess);
        var after = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(await NileOwnerAsync(), new GetTeacherQuery(created.Value.TeacherId));
        Assert.True(after.IsSuccess);
        Assert.Equal("Amina Youssef", after.Value.FullName);
        Assert.Equal("+201012345678", after.Value.Phone);
        Assert.NotEqual(before.Value.Version, after.Value.Version);
    }

    [Fact]
    public async Task Archive_ExcludedFromDefaultList_IncludedWhenArchivedRequested()
    {
        var created = await Fixture.SendAsAsync<CreateTeacherCommand, CreateTeacherResult>(
            await NileOwnerAsync(), new CreateTeacherCommand("Amina Hassan", null));
        var dto = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(await NileOwnerAsync(), new GetTeacherQuery(created.Value.TeacherId));

        var archived = await Fixture.SendAsAsync<ArchiveTeacherCommand, Unit>(
            await NileOwnerAsync(), new ArchiveTeacherCommand(created.Value.TeacherId, dto.Value.Version));
        Assert.True(archived.IsSuccess);

        var defaultList = await Fixture.QueryAsAsync<ListTeachersQuery, IReadOnlyList<TeacherDto>>(await NileOwnerAsync(), new ListTeachersQuery(false));
        var fullList = await Fixture.QueryAsAsync<ListTeachersQuery, IReadOnlyList<TeacherDto>>(await NileOwnerAsync(), new ListTeachersQuery(true));

        Assert.DoesNotContain(defaultList.Value, teacher => teacher.Id == created.Value.TeacherId);
        Assert.Contains(fullList.Value, teacher => teacher.Id == created.Value.TeacherId && teacher.Status == TeacherStatus.Archived);
    }

    [Fact]
    public async Task Archive_Again_ReturnsAlreadyArchived()
    {
        var created = await Fixture.SendAsAsync<CreateTeacherCommand, CreateTeacherResult>(
            await NileOwnerAsync(), new CreateTeacherCommand("Amina Hassan", null));
        var dto = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(await NileOwnerAsync(), new GetTeacherQuery(created.Value.TeacherId));
        await Fixture.SendAsAsync<ArchiveTeacherCommand, Unit>(await NileOwnerAsync(), new ArchiveTeacherCommand(created.Value.TeacherId, dto.Value.Version));
        var archivedDto = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(await NileOwnerAsync(), new GetTeacherQuery(created.Value.TeacherId));

        var result = await Fixture.SendAsAsync<ArchiveTeacherCommand, Unit>(
            await NileOwnerAsync(), new ArchiveTeacherCommand(created.Value.TeacherId, archivedDto.Value.Version));

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.already_archived", result.Error!.Code);
```

The excerpt stops at line 90; use the original file for the remaining 30 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
