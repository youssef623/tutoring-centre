# `tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 32.4: proves the GET /api/audit contract (Day 32) through the full HTTP pipeline.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 283 lines and 14,700 bytes. Source hash: `fa32498b5eb1032c3c9e4500fa631caf4fb98a113b4aaf15ed82e5801a27f336`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L10): `public sealed class AuditEndpointTests(ApiFactory factory) : IAsyncLifetime`
- [Line 14](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L14): `public async Task InitializeAsync()`
- [Line 20](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L20): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 23](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L23): `public async Task Get_AfterRenameAndRoleChange_ReturnsBothEntriesNewestFirstWithBeforeAndAfter()`
- [Line 52](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L52): `public async Task Get_AfterGroupCapacityChange_ReturnsTheNumericBeforeAndAfterAsText()`
- [Line 88](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L88): `public async Task Get_FilteredByEntityIdOfSubject_ReturnsOnlyThatSubjectsHistory()`
- [Line 112](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L112): `public async Task Get_FilteredByAMonthThreeEntityType_ReturnsOnlyThatTypesEntries(string entityType)`
- [Line 127](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L127): `public async Task Get_FilteredByClassSession_IsAcceptedEvenWithNoEntries()`
- [Line 140](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L140): `public async Task Get_PageSizeOne_YieldsCursorAndSecondPageReturnsTheNextEntry()`
- [Line 165](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L165): `public async Task Get_WithInvalidQueryParameter_Returns400(string query)`
- [Line 175](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L175): `public async Task Get_AsMaadiOwner_SeesNoneOfNilesEntries_WithOrWithoutNilesEntityIdFilter()`
- [Line 199](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L199): `public async Task Get_AsSecretaryOrTeacher_Returns403(string role)`
- [Line 214](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L214): `public async Task UnsupportedMethod_Returns404Or405(HttpMethod method)`
- [Line 226](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L226): `public static IEnumerable<object[]> UnsupportedMethods()`
- [Line 234](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L234): `public async Task Get_ResponseContainsNoEmailOrPasswordField()`
- [Line 278](../../../../../tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs#L278): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Audit;

/// <summary>Task 32.4: proves the GET /api/audit contract (Day 32) through the full HTTP pipeline.</summary>
[Collection(ApiCollection.Name)]
public sealed class AuditEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri AuditUri = new("/api/audit", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Get_AfterRenameAndRoleChange_ReturnsBothEntriesNewestFirstWithBeforeAndAfter()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, membershipId) = await RenameSubjectAndChangeRoleAsync(owner);

        using var response = await owner.Client.GetAsync(AuditUri);
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = body.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.True(items.Count >= 2);

        // Newest first: the role change happened after the rename.
        var roleChange = items[0];
        Assert.Equal("membership", roleChange.GetProperty("entityType").GetString());
        Assert.Equal(membershipId, roleChange.GetProperty("entityId").GetGuid());
        var roleChangeField = roleChange.GetProperty("changes").EnumerateArray().Single(c => c.GetProperty("field").GetString() == "role");
        Assert.Equal("secretary", roleChangeField.GetProperty("before").GetString());
        Assert.Equal("teacher", roleChangeField.GetProperty("after").GetString());

        var rename = items[1];
        Assert.Equal("subject", rename.GetProperty("entityType").GetString());
        Assert.Equal(subjectId, rename.GetProperty("entityId").GetGuid());
        var nameField = rename.GetProperty("changes").EnumerateArray().Single(c => c.GetProperty("field").GetString() == "name");
        Assert.Equal("History", nameField.GetProperty("before").GetString());
        Assert.Equal("World History", nameField.GetProperty("after").GetString());
    }

    [Fact]
    public async Task Get_AfterGroupCapacityChange_ReturnsTheNumericBeforeAndAfterAsText()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var subjectResponse = await AntiforgeryTestHelper.PostAsJsonAsync(owner.Client, new Uri("/api/subjects", UriKind.Relative), new { name = "Audit Physics" });
        var subjectId = (await ReadJsonAsync(subjectResponse)).RootElement.GetProperty("id").GetGuid();
        using var teacherResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/teachers", UriKind.Relative), new { fullName = "Audit Teacher", phone = (string?)null });
        var teacherId = (await ReadJsonAsync(teacherResponse)).RootElement.GetProperty("id").GetGuid();
        const int capacity = 20;
        using var groupResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/groups", UriKind.Relative), new { name = "Audit Group", subjectId, teacherId, capacity });
        var groupId = (await ReadJsonAsync(groupResponse)).RootElement.GetProperty("id").GetGuid();
        using var groupGet = await owner.Client.GetAsync(new Uri($"/api/groups/{groupId}", UriKind.Relative));
        var group = (await ReadJsonAsync(groupGet)).RootElement;
        using (var changed = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/groups/{groupId}/capacity", UriKind.Relative),
            new { capacity = capacity + 1, version = group.GetProperty("version").GetUInt32() }))
        {
            Assert.Equal(HttpStatusCode.NoContent, changed.StatusCode);
        }

        // Unfiltered and newest first: the capacity change is the latest entry in the centre.
        using var response = await owner.Client.GetAsync(AuditUri);
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var update = body.RootElement.GetProperty("items")[0];
        Assert.Equal("group", update.GetProperty("entityType").GetString());
        Assert.Equal(groupId, update.GetProperty("entityId").GetGuid());
        Assert.Equal("updated", update.GetProperty("action").GetString());
        var capacityField = update.GetProperty("changes").EnumerateArray().Single(c => c.GetProperty("field").GetString() == "capacity");
        Assert.Equal(capacity.ToString(System.Globalization.CultureInfo.InvariantCulture), capacityField.GetProperty("before").GetString());
        Assert.Equal((capacity + 1).ToString(System.Globalization.CultureInfo.InvariantCulture), capacityField.GetProperty("after").GetString());
    }

    [Fact]
    public async Task Get_FilteredByEntityIdOfSubject_ReturnsOnlyThatSubjectsHistory()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
```

The excerpt stops at line 90; use the original file for the remaining 193 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
