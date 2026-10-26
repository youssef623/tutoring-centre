using System.Globalization;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;
using TutoringCentre.Application.Academics.Students.Commands.LinkParent;
using TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;
using TutoringCentre.Application.Academics.Students.Queries.GetStudent;
using TutoringCentre.Application.Academics.Students.Queries.ListStudents;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;
using RealRegisterStudentCommand = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentCommand;
using RealRegisterStudentParent = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentParent;
using RealRegisterStudentResult = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentResult;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>Task 40.13: student search, cross-tenant isolation and optimistic-concurrency proofs through the real dispatcher.</summary>
public sealed class StudentUseCaseIntegrationTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task ListStudents_HamzaInsensitiveSearch_FindsStudentRegisteredWithHamza()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        await RegisterAsync(actor, "أحمد حسن");

        var result = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(actor, new ListStudentsQuery("احمد", false, null, 25));

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value.Items, item => item.FullName == "أحمد حسن");
    }

    [Fact]
    public async Task ListStudents_PhoneFragment_FindsStudentByOwnPhoneSuffix()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        await RegisterAsync(actor, "Youssef Omar", studentPhone: "01112345678");

        var result = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(actor, new ListStudentsQuery("5678", false, null, 25));

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value.Items, item => item.FullName == "Youssef Omar");
    }

    [Fact]
    public async Task ListStudents_PhoneFragment_FindsStudentByParentPhoneSuffix()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        await RegisterAsync(actor, "Sara Ali", parentPhone: "01098765432");

        var result = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(actor, new ListStudentsQuery("5432", false, null, 25));

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value.Items, item => item.FullName == "Sara Ali");
    }

    [Fact]
    public async Task ListStudents_ArchivedExcludedByDefault_IncludedWhenRequested()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var studentId = await RegisterAsync(actor, "Mona Adel");
        var version = await ReadVersionAsync(studentId);
        await Fixture.SendAsAsync<ArchiveStudentCommand, Unit>(actor, new ArchiveStudentCommand(studentId, version));

        var activeOnly = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(actor, new ListStudentsQuery("Mona", false, null, 25));
        var includingArchived = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(actor, new ListStudentsQuery("Mona", true, null, 25));

        Assert.DoesNotContain(activeOnly.Value.Items, item => item.Id == studentId);
        Assert.Contains(includingArchived.Value.Items, item => item.Id == studentId);
    }

    [Fact]
    public async Task ListStudents_KeysetPaging_SecondPageContinuesAfterTheFirst()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        await RegisterAsync(actor, "Aya Tarek");
        await RegisterAsync(actor, "Bassem Nabil");
        await RegisterAsync(actor, "Cherif Adel");

        var firstPage = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(actor, new ListStudentsQuery(null, false, null, 2));
        Assert.Equal(2, firstPage.Value.Items.Count);
        Assert.NotNull(firstPage.Value.NextCursor);

        var secondPage = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(
            actor, new ListStudentsQuery(null, false, firstPage.Value.NextCursor, 2));

        Assert.DoesNotContain(secondPage.Value.Items, item => firstPage.Value.Items.Select(first => first.Id).Contains(item.Id));
    }

    [Fact]
    public async Task MaadiActor_CannotSeeOrRegisterAgainstNilesParent()
    {
        var nileActor = await StaffActorInAsync(NileCentreId);
        var maadiActor = await StaffActorInAsync(MaadiCentreId);
        var nileParentId = await RegisterParentOnlyAsync(nileActor);

        var registerAcrossTenant = await Fixture.SendAsAsync<RealRegisterStudentCommand, RealRegisterStudentResult>(
            maadiActor,
            new RealRegisterStudentCommand("Smuggled Student", null, null, new RealRegisterStudentParent(nileParentId, null, null), ParentRelationship.Father));

        Assert.True(registerAcrossTenant.IsFailure);
        Assert.Equal("parent.not_found", registerAcrossTenant.Error!.Code);
    }

    [Fact]
    public async Task MaadiActor_ListStudents_DoesNotSeeNilesStudent()
    {
        var nileActor = await StaffActorInAsync(NileCentreId);
        var maadiActor = await StaffActorInAsync(MaadiCentreId);
        await RegisterAsync(nileActor, "Nile Only Student");

        var result = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(maadiActor, new ListStudentsQuery("Nile Only", false, null, 25));

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Items);
    }

    [Fact]
    public async Task MaadiActor_GetStudent_SeesNotFoundForNilesStudent()
    {
        var nileActor = await StaffActorInAsync(NileCentreId);
        var maadiActor = await StaffActorInAsync(MaadiCentreId);
        var studentId = await RegisterAsync(nileActor, "Cross Tenant Target");

        var result = await Fixture.QueryAsAsync<GetStudentQuery, StudentDetailDto>(maadiActor, new GetStudentQuery(studentId));

        Assert.True(result.IsFailure);
        Assert.Equal("student.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task UpdateStudent_StaleVersion_ReturnsConcurrencyStale()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var studentId = await RegisterAsync(actor, "Hana Fathy");
        var staleVersion = await ReadVersionAsync(studentId);

        var firstUpdate = await Fixture.SendAsAsync<UpdateStudentCommand, Unit>(
            actor, new UpdateStudentCommand(studentId, "Hana Fathy Ali", null, null, staleVersion));
        Assert.True(firstUpdate.IsSuccess);

        var secondUpdate = await Fixture.SendAsAsync<UpdateStudentCommand, Unit>(
            actor, new UpdateStudentCommand(studentId, "Another Name", null, null, staleVersion));

        Assert.True(secondUpdate.IsFailure);
        Assert.Equal("concurrency.stale", secondUpdate.Error!.Code);
    }

    [Fact]
    public async Task LinkParent_StaleVersion_ReturnsConcurrencyStale()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var studentId = await RegisterAsync(actor, "Karim Said");
        var secondParentId = await Fixture.SeedParentAsync(NileCentreId, "Second Parent", "+201212345678");
        var staleVersion = await ReadVersionAsync(studentId);

        var concurrentUpdate = await Fixture.SendAsAsync<UpdateStudentCommand, Unit>(
            actor, new UpdateStudentCommand(studentId, "Karim Said Updated", null, null, staleVersion));
        Assert.True(concurrentUpdate.IsSuccess);

        var linkWithStaleVersion = await Fixture.SendAsAsync<LinkParentCommand, Unit>(
            actor, new LinkParentCommand(studentId, secondParentId, ParentRelationship.Mother, staleVersion));

        Assert.True(linkWithStaleVersion.IsFailure);
        Assert.Equal("concurrency.stale", linkWithStaleVersion.Error!.Code);
    }

    [Fact]
    public async Task RegisterStudent_StudentValidationFailsAfterNewParentWasStaged_NothingPersists()
    {
        var actor = await StaffActorInAsync(NileCentreId);

        var result = await Fixture.SendAsAsync<RealRegisterStudentCommand, RealRegisterStudentResult>(
            actor,
            new RealRegisterStudentCommand(
                "Y", null, null, new RealRegisterStudentParent(null, "Brand New Parent", "01099998888"), ParentRelationship.Father));

        Assert.True(result.IsFailure);
        Assert.Equal("student.name_invalid", result.Error!.Code);
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from academics.parents where full_name = 'Brand New Parent'"));
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from academics.students"));
    }

    private int _nextDefaultParentPhoneDigits = 10000000;

    private async Task<Guid> RegisterAsync(
        Actor actor, string fullName, string? studentPhone = null, string? parentPhone = null)
    {
        var defaultParentPhone = "010" + (_nextDefaultParentPhoneDigits++).ToString(CultureInfo.InvariantCulture);
        var result = await Fixture.SendAsAsync<RealRegisterStudentCommand, RealRegisterStudentResult>(
            actor,
            new RealRegisterStudentCommand(
                fullName, studentPhone, null, new RealRegisterStudentParent(null, $"{fullName} Parent", parentPhone ?? defaultParentPhone), ParentRelationship.Father));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value.StudentId;
    }

    private async Task<Guid> RegisterParentOnlyAsync(Actor actor)
    {
        var studentId = await RegisterAsync(actor, "Temporary Holder");
        var detail = await Fixture.QueryAsAsync<GetStudentQuery, StudentDetailDto>(actor, new GetStudentQuery(studentId));
        return detail.Value.Parents[0].ParentId;
    }

    private async Task<StaffActor> StaffActorInAsync(Guid centreId) => new(await Fixture.SeedUserAsync(), centreId, StaffRole.Owner);

    private async Task<uint> ReadVersionAsync(Guid studentId) =>
        uint.Parse(await Fixture.ScalarAsync<string>($"select xmin::text from academics.students where id = '{studentId}'"), CultureInfo.InvariantCulture);
}
