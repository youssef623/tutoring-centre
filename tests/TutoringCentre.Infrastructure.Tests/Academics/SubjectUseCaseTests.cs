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
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(await NileOwnerAsync(), new CreateSubjectCommand("Mathematics"));
        var dto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(await NileOwnerAsync(), new GetSubjectQuery(created.Value.SubjectId));

        var archived = await Fixture.SendAsAsync<ArchiveSubjectCommand, Unit>(
            await NileOwnerAsync(), new ArchiveSubjectCommand(created.Value.SubjectId, dto.Value.Version));
        Assert.True(archived.IsSuccess);

        var defaultList = await Fixture.QueryAsAsync<ListSubjectsQuery, IReadOnlyList<SubjectDto>>(await NileOwnerAsync(), new ListSubjectsQuery(false));
        var fullList = await Fixture.QueryAsAsync<ListSubjectsQuery, IReadOnlyList<SubjectDto>>(await NileOwnerAsync(), new ListSubjectsQuery(true));

        Assert.DoesNotContain(defaultList.Value, subject => subject.Id == created.Value.SubjectId);
        Assert.Contains(fullList.Value, subject => subject.Id == created.Value.SubjectId && subject.Status == SubjectStatus.Archived);
    }

    [Fact]
    public async Task Archive_Again_ReturnsAlreadyArchived()
    {
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(await NileOwnerAsync(), new CreateSubjectCommand("Mathematics"));
        var dto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(await NileOwnerAsync(), new GetSubjectQuery(created.Value.SubjectId));
        await Fixture.SendAsAsync<ArchiveSubjectCommand, Unit>(await NileOwnerAsync(), new ArchiveSubjectCommand(created.Value.SubjectId, dto.Value.Version));
        var archivedDto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(await NileOwnerAsync(), new GetSubjectQuery(created.Value.SubjectId));

        var result = await Fixture.SendAsAsync<ArchiveSubjectCommand, Unit>(
            await NileOwnerAsync(), new ArchiveSubjectCommand(created.Value.SubjectId, archivedDto.Value.Version));

        Assert.True(result.IsFailure);
        Assert.Equal("subject.already_archived", result.Error!.Code);
    }

    [Fact]
    public async Task Restore_BecomesActive()
    {
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(await NileOwnerAsync(), new CreateSubjectCommand("Mathematics"));
        var dto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(await NileOwnerAsync(), new GetSubjectQuery(created.Value.SubjectId));
        await Fixture.SendAsAsync<ArchiveSubjectCommand, Unit>(await NileOwnerAsync(), new ArchiveSubjectCommand(created.Value.SubjectId, dto.Value.Version));
        var archivedDto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(await NileOwnerAsync(), new GetSubjectQuery(created.Value.SubjectId));

        var restored = await Fixture.SendAsAsync<RestoreSubjectCommand, Unit>(
            await NileOwnerAsync(), new RestoreSubjectCommand(created.Value.SubjectId, archivedDto.Value.Version));

        Assert.True(restored.IsSuccess);
        var finalDto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(await NileOwnerAsync(), new GetSubjectQuery(created.Value.SubjectId));
        Assert.Equal(SubjectStatus.Active, finalDto.Value.Status);
    }

    [Fact]
    public async Task Get_UnknownId_ReturnsNotFound()
    {
        var result = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(await NileOwnerAsync(), new GetSubjectQuery(Guid.CreateVersion7()));

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task Create_EightyOneCharacterName_ValidationFailureAndNoTransactionOpened()
    {
        var result = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(
            await NileOwnerAsync(), new CreateSubjectCommand(new string('a', 81)));

        Assert.True(result.IsFailure);
        Assert.Equal("validation.failed", result.Error!.Code);
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from academics.subjects"));
    }

    private async Task<StaffActor> NileOwnerAsync() => new(await Fixture.SeedUserAsync(), NileCentreId, StaffRole.Owner);
}
