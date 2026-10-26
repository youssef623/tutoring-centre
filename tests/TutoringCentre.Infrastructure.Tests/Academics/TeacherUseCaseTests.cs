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
    }

    [Fact]
    public async Task Restore_BecomesActive()
    {
        var created = await Fixture.SendAsAsync<CreateTeacherCommand, CreateTeacherResult>(
            await NileOwnerAsync(), new CreateTeacherCommand("Amina Hassan", null));
        var dto = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(await NileOwnerAsync(), new GetTeacherQuery(created.Value.TeacherId));
        await Fixture.SendAsAsync<ArchiveTeacherCommand, Unit>(await NileOwnerAsync(), new ArchiveTeacherCommand(created.Value.TeacherId, dto.Value.Version));
        var archivedDto = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(await NileOwnerAsync(), new GetTeacherQuery(created.Value.TeacherId));

        var restored = await Fixture.SendAsAsync<RestoreTeacherCommand, Unit>(
            await NileOwnerAsync(), new RestoreTeacherCommand(created.Value.TeacherId, archivedDto.Value.Version));

        Assert.True(restored.IsSuccess);
        var finalDto = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(await NileOwnerAsync(), new GetTeacherQuery(created.Value.TeacherId));
        Assert.Equal(TeacherStatus.Active, finalDto.Value.Status);
    }

    [Fact]
    public async Task Get_UnknownId_ReturnsNotFound()
    {
        var result = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(await NileOwnerAsync(), new GetTeacherQuery(Guid.CreateVersion7()));

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_found", result.Error!.Code);
    }

    private async Task<StaffActor> NileOwnerAsync() => new(await Fixture.SeedUserAsync(), NileCentreId, StaffRole.Owner);
}
