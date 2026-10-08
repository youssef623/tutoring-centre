using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

// Aliased for the same reason as SubjectUseCaseTests: Task 23.6's test-only RenameSubjectCommand, declared
// directly in this namespace, would otherwise shadow the real (production) one.
using RealRenameSubjectCommand = TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject.RenameSubjectCommand;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 25.9: the first Tenant A → Tenant B tests on real business data. Every assertion has two parts — the
/// attacker's (Maadi's) result, and the victim's (Nile's) row, read back through the superuser connection, never
/// through the application. Maadi always uses S's real ID and real version, never an invented one.
/// </summary>
public sealed class SubjectCrossTenantTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task Get_MaadiOwnerForNileSubject_ReturnsNotFound()
    {
        var subjectId = await CreateNileSubjectAsync("Mathematics");

        var result = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(MaadiOwner(), new GetSubjectQuery(subjectId));

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task Rename_MaadiOwnerForNileSubjectWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()
    {
        var subjectId = await CreateNileSubjectAsync("Mathematics");
        var version = await ReadVersionAsNileAsync(subjectId);

        var result = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            MaadiOwner(), new RealRenameSubjectCommand(subjectId, "Hacked", version));

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(
            1, await Fixture.ScalarAsync<long>($"select count(*) from academics.subjects where id = '{subjectId}' and name = 'Mathematics'"));
    }

    [Fact]
    public async Task Archive_MaadiOwnerForNileSubjectWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()
    {
        var subjectId = await CreateNileSubjectAsync("Mathematics");
        var version = await ReadVersionAsNileAsync(subjectId);

        var result = await Fixture.SendAsAsync<ArchiveSubjectCommand, Unit>(MaadiOwner(), new ArchiveSubjectCommand(subjectId, version));

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(
            1, await Fixture.ScalarAsync<long>($"select count(*) from academics.subjects where id = '{subjectId}' and status = 'active'"));
    }

    [Fact]
    public async Task Restore_MaadiOwnerForNileSubjectWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()
    {
        var subjectId = await CreateNileSubjectAsync("Mathematics");
        var version = await ReadVersionAsNileAsync(subjectId);

        var result = await Fixture.SendAsAsync<RestoreSubjectCommand, Unit>(MaadiOwner(), new RestoreSubjectCommand(subjectId, version));

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(
            1, await Fixture.ScalarAsync<long>($"select count(*) from academics.subjects where id = '{subjectId}' and status = 'active'"));
    }

    [Fact]
    public async Task List_MaadiOwner_ContainsNoNileSubject()
    {
        await CreateNileSubjectAsync("Mathematics");

        var result = await Fixture.QueryAsAsync<ListSubjectsQuery, IReadOnlyList<SubjectDto>>(MaadiOwner(), new ListSubjectsQuery(true));

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(result.Value, subject => subject.Name == "Mathematics");
    }

    [Fact]
    public async Task Create_MaadiOwnerWithNilesExactName_Succeeds()
    {
        await CreateNileSubjectAsync("Mathematics");

        var result = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(MaadiOwner(), new CreateSubjectCommand("Mathematics"));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CentrelessActor_AllSixRequests_ReturnTenantNotSelected()
    {
        var centreless = new StaffActor(Guid.CreateVersion7(), CentreId: null, Role: null);

        var create = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(centreless, new CreateSubjectCommand("Mathematics"));
        var rename = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            centreless, new RealRenameSubjectCommand(Guid.CreateVersion7(), "Mathematics", 0));
        var archive = await Fixture.SendAsAsync<ArchiveSubjectCommand, Unit>(centreless, new ArchiveSubjectCommand(Guid.CreateVersion7(), 0));
        var restore = await Fixture.SendAsAsync<RestoreSubjectCommand, Unit>(centreless, new RestoreSubjectCommand(Guid.CreateVersion7(), 0));
        var list = await Fixture.QueryAsAsync<ListSubjectsQuery, IReadOnlyList<SubjectDto>>(centreless, new ListSubjectsQuery(false));
        var get = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(centreless, new GetSubjectQuery(Guid.CreateVersion7()));

        Result[] results = [create, rename, archive, restore, list, get];
        Assert.All(results, result =>
        {
            Assert.True(result.IsFailure);
            Assert.Equal("tenant.not_selected", result.Error!.Code);
        });
    }

    private StaffActor NileOwner() => new(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner);

    private StaffActor MaadiOwner() => new(Guid.CreateVersion7(), MaadiCentreId, StaffRole.Owner);

    private async Task<Guid> CreateNileSubjectAsync(string name)
    {
        var result = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(NileOwner(), new CreateSubjectCommand(name));
        return result.Value.SubjectId;
    }

    private async Task<uint> ReadVersionAsNileAsync(Guid subjectId)
    {
        var dto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(NileOwner(), new GetSubjectQuery(subjectId));
        return dto.Value.Version;
    }
}
