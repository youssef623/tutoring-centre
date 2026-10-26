using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;
using TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;
using TutoringCentre.Application.Academics.Parents.Queries.GetParent;
using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 39.9: the production parent use cases exercised through the real AppDbContext, interceptors and unit
/// of work — the repository, read service and dispatcher wiring all real, nothing test-only.
/// </summary>
public sealed class ParentPersistenceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task CreateParent_PersistsWithActorsCentreAndCanonicalPhone()
    {
        var actor = await StaffActorInAsync(NileCentreId);

        var result = await Fixture.SendAsAsync<CreateParentCommand, CreateParentResult>(
            actor, new CreateParentCommand("Omar Hassan", "01012345678"));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.parents where id = '{result.Value.ParentId}' and centre_id = '{NileCentreId}' and phone = '+201012345678'"));
    }

    [Fact]
    public async Task GetParent_WithChildrenSeededBySql_ReturnsDetailOrderedByStudentName()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var parentId = await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");
        var zId = await Fixture.SeedStudentAsync(NileCentreId, "Zeinab Omar");
        var aId = await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Omar");
        await Fixture.SeedStudentParentLinkAsync(NileCentreId, zId, parentId, isPrimary: false, relationship: "father");
        await Fixture.SeedStudentParentLinkAsync(NileCentreId, aId, parentId, isPrimary: true, relationship: "father");

        var result = await Fixture.QueryAsAsync<GetParentQuery, ParentDetailDto>(actor, new GetParentQuery(parentId));

        Assert.True(result.IsSuccess);
        Assert.Equal("Omar Hassan", result.Value.FullName);
        Assert.Equal("+201012345678", result.Value.Phone);
        Assert.Equal(2, result.Value.Children.Count);
        Assert.Equal(["Ahmed Omar", "Zeinab Omar"], result.Value.Children.Select(child => child.FullName));
        Assert.True(result.Value.Children.Single(child => child.FullName == "Ahmed Omar").IsPrimary);
    }

    [Fact]
    public async Task UpdateParent_ChangesNameAndPhoneAndBumpsVersion()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var created = await Fixture.SendAsAsync<CreateParentCommand, CreateParentResult>(
            actor, new CreateParentCommand("Omar Hassan", "01012345678"));
        var versionBefore = await ReadVersionAsync(created.Value.ParentId);

        var result = await Fixture.SendAsAsync<UpdateParentCommand, Unit>(
            actor, new UpdateParentCommand(created.Value.ParentId, "Omar Hassan Ali", "01112345678", versionBefore));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.parents where id = '{created.Value.ParentId}' and full_name = 'Omar Hassan Ali' and phone = '+201112345678'"));
        var versionAfter = await ReadVersionAsync(created.Value.ParentId);
        Assert.NotEqual(versionBefore, versionAfter);
    }

    [Fact]
    public async Task LookupParentByPhone_DifferentlySpelled_FindsTheParent()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        await Fixture.SendAsAsync<CreateParentCommand, CreateParentResult>(actor, new CreateParentCommand("Omar Hassan", "01012345678"));

        var result = await Fixture.QueryAsAsync<LookupParentByPhoneQuery, ParentLookupDto>(actor, new LookupParentByPhoneQuery("0101 234 5678"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.Parent);
        Assert.Equal("Omar Hassan", result.Value.Parent!.FullName);
    }

    [Fact]
    public async Task LookupParentByPhone_NoSuchParent_SucceedsWithNull()
    {
        var actor = await StaffActorInAsync(NileCentreId);

        var result = await Fixture.QueryAsAsync<LookupParentByPhoneQuery, ParentLookupDto>(actor, new LookupParentByPhoneQuery("01012345678"));

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Parent);
    }

    [Fact]
    public async Task UpdateParent_StaleVersion_ReturnsConcurrencyStale()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var created = await Fixture.SendAsAsync<CreateParentCommand, CreateParentResult>(
            actor, new CreateParentCommand("Omar Hassan", "01012345678"));
        var staleVersion = await ReadVersionAsync(created.Value.ParentId);

        var firstUpdate = await Fixture.SendAsAsync<UpdateParentCommand, Unit>(
            actor, new UpdateParentCommand(created.Value.ParentId, "Omar Hassan Ali", "01012345678", staleVersion));
        Assert.True(firstUpdate.IsSuccess);

        // staleVersion was read before the first update committed; it is now stale.
        var secondUpdate = await Fixture.SendAsAsync<UpdateParentCommand, Unit>(
            actor, new UpdateParentCommand(created.Value.ParentId, "Another Name", "01012345678", staleVersion));

        Assert.True(secondUpdate.IsFailure);
        Assert.Equal("concurrency.stale", secondUpdate.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, secondUpdate.Error.Kind);
    }

    private async Task<StaffActor> StaffActorInAsync(Guid centreId) => new(await Fixture.SeedUserAsync(), centreId, StaffRole.Owner);

    private async Task<uint> ReadVersionAsync(Guid parentId) =>
        uint.Parse(
            await Fixture.ScalarAsync<string>($"select xmin::text from academics.parents where id = '{parentId}'"),
            System.Globalization.CultureInfo.InvariantCulture);
}
