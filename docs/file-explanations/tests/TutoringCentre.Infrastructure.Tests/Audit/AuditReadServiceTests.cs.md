# `tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 32.2: the audit read side against real PostgreSQL, through the real dispatcher (so row-level security and the EF query filter are exercised exactly as a real request would hit them), with rows seeded directly by `PostgresFixture.SeedAuditEntryAsync` so timing and ordering are fully controlled.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 188 lines and 10,434 bytes. Source hash: `df291c9a892a01e56b7cd0ae1a023244e64aa70e30f80055228960aac72f464f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Audit;`
- Line 2: `using TutoringCentre.Application.Audit.Queries.GetAuditLog;`
- Line 3: `using TutoringCentre.Domain.Identity;`
- Line 4: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 5: `using TutoringCentre.Infrastructure.Tests.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs#L14): `public sealed class AuditReadServiceTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)`
- [Line 19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs#L19): `public async Task GetPageAsync_With120SeededEntries_PagesAsFiftyFiftyTwenty_WithNoDuplicatesOrGaps()`
- [Line 54](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs#L54): `public async Task GetPageAsync_FilteredByEntityType_ReturnsOnlyThatType()`
- [Line 71](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs#L71): `public async Task GetPageAsync_FilteredByEntityId_ReturnsOnlyThatRecordsHistory()`
- [Line 87](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs#L87): `public async Task GetPageAsync_FilteredByActorUserId_ReturnsOnlyThatActorsRows()`
- [Line 106](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs#L106): `public async Task GetPageAsync_FilteredByFromAndTo_ReturnsOnlyEntriesInRange()`
- [Line 123](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs#L123): `public async Task GetPageAsync_WithCombinedFilters_AppliesAllOfThem()`
- [Line 142](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs#L142): `public async Task GetPageAsync_AsMaadiActor_NeverReceivesANileEntry()`
- [Line 167](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs#L167): `public async Task GetPageAsync_EntryInsertedBetweenPageRequests_DoesNotShiftTheNextPage()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 98 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
