using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Audit.Queries.GetAuditLog;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Identity;

namespace TutoringCentre.Infrastructure.Tests.Audit;

/// <summary>
/// Task 32.2: the audit read side against real PostgreSQL, through the real dispatcher (so row-level security
/// and the EF query filter are exercised exactly as a real request would hit them), with rows seeded directly
/// by <see cref="PostgresFixture.SeedAuditEntryAsync"/> so timing and ordering are fully controlled.
/// </summary>
public sealed class AuditReadServiceTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    private static readonly DateTimeOffset BaseTime = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task GetPageAsync_With120SeededEntries_PagesAsFiftyFiftyTwenty_WithNoDuplicatesOrGaps()
    {
        // CreateStaffAsync's own membership-creation writes one incidental "membership" audit row; filtering
        // to "subject" (what every seeded row below uses) keeps this test's count exactly the 120 it seeded.
        var owner = await CreateStaffAsync(NileCentreId, "owner-page@nile.test", StaffRole.Owner);
        var seededIds = new List<Guid>();
        for (var i = 0; i < 120; i++)
        {
            seededIds.Add(await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime.AddSeconds(-i)));
        }

        var collected = new List<Guid>();
        string? cursor = null;
        var pageSizes = new List<int>();
        for (var page = 0; page < 10 && (page == 0 || cursor is not null); page++)
        {
            var result = await Fixture.QueryAsAsync<GetAuditLogQuery, AuditPageDto>(
                owner.Actor, new GetAuditLogQuery("subject", null, null, null, null, cursor, 50));
            Assert.True(result.IsSuccess, result.Error?.Message);
            pageSizes.Add(result.Value.Items.Count);
            collected.AddRange(result.Value.Items.Select(item => item.Id));
            cursor = result.Value.NextCursor;
            if (cursor is null)
            {
                break;
            }
        }

        Assert.Equal([50, 50, 20], pageSizes);
        Assert.Equal(120, collected.Count);
        Assert.Equal(seededIds.OrderBy(id => id).ToList(), collected.OrderBy(id => id).ToList());
        Assert.Equal(seededIds, collected); // newest (smallest offset, i.e. largest timestamp) first
    }

    [Fact]
    public async Task GetPageAsync_FilteredByEntityType_ReturnsOnlyThatType()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-entitytype@nile.test", StaffRole.Owner);
        var subjectId = await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime, entityType: "subject");
        await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime.AddSeconds(-1), entityType: "membership");
        await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime.AddSeconds(-2), entityType: "centre");

        var result = await Fixture.QueryAsAsync<GetAuditLogQuery, AuditPageDto>(
            owner.Actor, new GetAuditLogQuery("subject", null, null, null, null, null, 50));

        Assert.True(result.IsSuccess, result.Error?.Message);
        var item = Assert.Single(result.Value.Items);
        Assert.Equal(subjectId, item.Id);
        Assert.Equal("subject", item.EntityType);
    }

    [Fact]
    public async Task GetPageAsync_FilteredByEntityId_ReturnsOnlyThatRecordsHistory()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-entityid@nile.test", StaffRole.Owner);
        var targetEntityId = Guid.CreateVersion7();
        var created = await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime, entityType: "subject", entityId: targetEntityId, action: "created");
        var updated = await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime.AddSeconds(-1), entityType: "subject", entityId: targetEntityId, action: "updated");
        await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime.AddSeconds(-2), entityType: "subject", entityId: Guid.CreateVersion7());

        var result = await Fixture.QueryAsAsync<GetAuditLogQuery, AuditPageDto>(
            owner.Actor, new GetAuditLogQuery("subject", targetEntityId, null, null, null, null, 50));

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal([created, updated], result.Value.Items.Select(item => item.Id).ToList()); // newest (created, at BaseTime) first
    }

    [Fact]
    public async Task GetPageAsync_FilteredByActorUserId_ReturnsOnlyThatActorsRows()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-actor@nile.test", StaffRole.Owner);
        var actorOneId = await Fixture.SeedUserAsync("Actor One");
        var actorTwoId = await Fixture.SeedUserAsync("Actor Two");
        var byActorOne = await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime, actorUserId: actorOneId);
        await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime.AddSeconds(-1), actorUserId: actorTwoId);

        var result = await Fixture.QueryAsAsync<GetAuditLogQuery, AuditPageDto>(
            owner.Actor, new GetAuditLogQuery(null, null, actorOneId, null, null, null, 50));

        Assert.True(result.IsSuccess, result.Error?.Message);
        var item = Assert.Single(result.Value.Items);
        Assert.Equal(byActorOne, item.Id);
        Assert.Equal(actorOneId, item.ActorUserId);
        Assert.Equal("Actor One", item.ActorDisplayName);
    }

    [Fact]
    public async Task GetPageAsync_FilteredByFromAndTo_ReturnsOnlyEntriesInRange()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-range@nile.test", StaffRole.Owner);
        await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime); // too new
        var inRange = await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime.AddHours(-1));
        await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime.AddHours(-5)); // too old

        var result = await Fixture.QueryAsAsync<GetAuditLogQuery, AuditPageDto>(
            owner.Actor,
            new GetAuditLogQuery(null, null, null, BaseTime.AddHours(-2), BaseTime.AddMinutes(-30), null, 50));

        Assert.True(result.IsSuccess, result.Error?.Message);
        var item = Assert.Single(result.Value.Items);
        Assert.Equal(inRange, item.Id);
    }

    [Fact]
    public async Task GetPageAsync_WithCombinedFilters_AppliesAllOfThem()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-combined@nile.test", StaffRole.Owner);
        var actorId = await Fixture.SeedUserAsync("Combined Actor");
        var matching = await Fixture.SeedAuditEntryAsync(
            NileCentreId, BaseTime.AddMinutes(-10), entityType: "membership", actorUserId: actorId);
        await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime.AddMinutes(-10), entityType: "subject", actorUserId: actorId); // wrong type
        await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime.AddMinutes(-10), entityType: "membership"); // wrong actor

        var result = await Fixture.QueryAsAsync<GetAuditLogQuery, AuditPageDto>(
            owner.Actor,
            new GetAuditLogQuery("membership", null, actorId, BaseTime.AddHours(-1), BaseTime, null, 50));

        Assert.True(result.IsSuccess, result.Error?.Message);
        var item = Assert.Single(result.Value.Items);
        Assert.Equal(matching, item.Id);
    }

    [Fact]
    public async Task GetPageAsync_AsMaadiActor_NeverReceivesANileEntry()
    {
        var nileOwner = await CreateStaffAsync(NileCentreId, "owner-cross@nile.test", StaffRole.Owner);
        var maadiOwner = await CreateStaffAsync(MaadiCentreId, "owner-cross@maadi.test", StaffRole.Owner);
        var nileEntityId = Guid.CreateVersion7();
        await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime, entityType: "subject", entityId: nileEntityId);
        var maadiEntryId = await Fixture.SeedAuditEntryAsync(MaadiCentreId, BaseTime.AddSeconds(-1), entityType: "subject");

        // "subject" excludes CreateStaffAsync's own incidental "membership" rows for both centres, isolating
        // the comparison to the two rows this test seeded itself.
        var unfiltered = await Fixture.QueryAsAsync<GetAuditLogQuery, AuditPageDto>(
            maadiOwner.Actor, new GetAuditLogQuery("subject", null, null, null, null, null, 50));
        Assert.True(unfiltered.IsSuccess, unfiltered.Error?.Message);
        Assert.Equal([maadiEntryId], unfiltered.Value.Items.Select(item => item.Id).ToList());

        // Even naming Nile's own entityId explicitly must not leak it across the tenant boundary.
        var filteredByNileId = await Fixture.QueryAsAsync<GetAuditLogQuery, AuditPageDto>(
            maadiOwner.Actor, new GetAuditLogQuery("subject", nileEntityId, null, null, null, null, 50));
        Assert.True(filteredByNileId.IsSuccess, filteredByNileId.Error?.Message);
        Assert.Empty(filteredByNileId.Value.Items);

        _ = nileOwner;
    }

    [Fact]
    public async Task GetPageAsync_EntryInsertedBetweenPageRequests_DoesNotShiftTheNextPage()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-insert@nile.test", StaffRole.Owner);
        var newest = await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime);
        var middle = await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime.AddSeconds(-1));
        var oldest = await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime.AddSeconds(-2));

        var firstPage = await Fixture.QueryAsAsync<GetAuditLogQuery, AuditPageDto>(
            owner.Actor, new GetAuditLogQuery("subject", null, null, null, null, null, 2));
        Assert.True(firstPage.IsSuccess, firstPage.Error?.Message);
        Assert.Equal([newest, middle], firstPage.Value.Items.Select(item => item.Id).ToList());
        Assert.NotNull(firstPage.Value.NextCursor);

        // A write that happens "between" page requests, newer than everything already paged.
        await Fixture.SeedAuditEntryAsync(NileCentreId, BaseTime.AddSeconds(1));

        var secondPage = await Fixture.QueryAsAsync<GetAuditLogQuery, AuditPageDto>(
            owner.Actor, new GetAuditLogQuery("subject", null, null, null, null, firstPage.Value.NextCursor, 2));
        Assert.True(secondPage.IsSuccess, secondPage.Error?.Message);
        Assert.Equal([oldest], secondPage.Value.Items.Select(item => item.Id).ToList());
    }
}
