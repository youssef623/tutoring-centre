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

        await using var scope1 = Fixture.Services.CreateAsyncScope();
        await using var scope2 = Fixture.Services.CreateAsyncScope();

        var (unitOfWork1, db1) = await BeginScopeAsync(scope1, actor);
        var (unitOfWork2, db2) = await BeginScopeAsync(scope2, actor);

        var student1 = await db1.Set<Student>().Include(s => s.Parents).SingleAsync(s => s.Id == studentId);
        var student2 = await db2.Set<Student>().Include(s => s.Parents).SingleAsync(s => s.Id == studentId);

        Assert.True(student1.AddParent(secondParentId, ParentRelationship.Mother).IsSuccess);
        await db1.SaveChangesAsync();
        await unitOfWork1.CommitAsync(CancellationToken.None);

        Assert.True(student2.AddParent(thirdParentId, ParentRelationship.Guardian).IsSuccess);
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => db2.SaveChangesAsync());

        await unitOfWork2.RollbackAsync(CancellationToken.None);
    }

    [Fact]
    public async Task SetPrimaryParentAndSave_SucceedsWithExactlyOnePrimaryRow()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var firstParentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(actor, new AddParentCommand(NileCentreId, "Omar Hassan", "01012345678"))).Value;
        var secondParentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(actor, new AddParentCommand(NileCentreId, "Amal Fathy", "01212345678"))).Value;
        var studentId = (await Fixture.SendAsAsync<RegisterStudentCommand, Guid>(
            actor, new RegisterStudentCommand(NileCentreId, "Youssef Omar", null, null, firstParentId, ParentRelationship.Father))).Value;
        await Fixture.SendAsAsync<AddStudentParentCommand, Unit>(actor, new AddStudentParentCommand(studentId, secondParentId, ParentRelationship.Mother));

        var result = await Fixture.SendAsAsync<SetPrimaryStudentParentCommand, Unit>(
            actor, new SetPrimaryStudentParentCommand(studentId, secondParentId));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.student_parents where student_id = '{studentId}' and is_primary = true"));
        Assert.Equal(
            secondParentId,
            await Fixture.ScalarAsync<Guid>(
                $"select parent_id from academics.student_parents where student_id = '{studentId}' and is_primary = true"));
    }

    [Fact]
    public async Task RemoveParentAndSave_LinkRowIsGone()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var firstParentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(actor, new AddParentCommand(NileCentreId, "Omar Hassan", "01012345678"))).Value;
        var secondParentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(actor, new AddParentCommand(NileCentreId, "Amal Fathy", "01212345678"))).Value;
        var studentId = (await Fixture.SendAsAsync<RegisterStudentCommand, Guid>(
            actor, new RegisterStudentCommand(NileCentreId, "Youssef Omar", null, null, firstParentId, ParentRelationship.Father))).Value;
        await Fixture.SendAsAsync<AddStudentParentCommand, Unit>(actor, new AddStudentParentCommand(studentId, secondParentId, ParentRelationship.Mother));

        var result = await Fixture.SendAsAsync<RemoveStudentParentCommand, Unit>(
            actor, new RemoveStudentParentCommand(studentId, secondParentId));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            0,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.student_parents where student_id = '{studentId}' and parent_id = '{secondParentId}'"));
    }

    [Fact]
    public async Task MaadiActorListsStudents_DoesNotContainNilesStudent()
    {
        var nileActor = await StaffActorInAsync(NileCentreId);
        var parentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(nileActor, new AddParentCommand(NileCentreId, "Omar Hassan", "01012345678"))).Value;
        await Fixture.SendAsAsync<RegisterStudentCommand, Guid>(
            nileActor, new RegisterStudentCommand(NileCentreId, "Youssef Omar", null, null, parentId, ParentRelationship.Father));

        var result = await Fixture.QueryAsAsync<ListStudentNamesQuery, List<string>>(await StaffActorInAsync(MaadiCentreId), new ListStudentNamesQuery());

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain("Youssef Omar", result.Value);
    }

    [Fact]
    public async Task NileActorSavesStudentWhoseLinkPointsAtAMaadiParent_SaveFailsOnForeignKeyAndNothingStored()
    {
        var nileActor = await StaffActorInAsync(NileCentreId);
        var maadiParentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(
            await StaffActorInAsync(MaadiCentreId), new AddParentCommand(MaadiCentreId, "Laila Said", "01512345678"))).Value;

        await Assert.ThrowsAsync<DbUpdateException>(() => Fixture.SendAsAsync<RegisterStudentCommand, Guid>(
            nileActor, new RegisterStudentCommand(NileCentreId, "Smuggled Student", null, null, maadiParentId, ParentRelationship.Father)));

        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from academics.students"));
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from academics.student_parents"));
    }

    private async Task<StaffActor> StaffActorInAsync(Guid centreId) => new(await Fixture.SeedUserAsync(), centreId, StaffRole.Teacher);

    private static async Task<(IUnitOfWork UnitOfWork, AppDbContext Db)> BeginScopeAsync(AsyncServiceScope scope, Actor actor)
    {
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor);
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await unitOfWork.BeginAsync(readOnly: false, CancellationToken.None);
        return (unitOfWork, db);
    }
}
