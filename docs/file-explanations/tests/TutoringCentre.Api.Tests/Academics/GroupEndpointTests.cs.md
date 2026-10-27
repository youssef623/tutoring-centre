# `tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 45.5: the eight group endpoints' contract through the full HTTP pipeline — status, code, body shape and headers, never messages — plus the one outcome Day 44 added to Month 2's subject archive endpoint, and the two outcomes scheduling (Day 52) added to reassigning and archiving a group (Task 53.4). Every test creates its own subjects, teachers and groups, so the seeded demo data never decides a result.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 654 lines and 35,949 bytes. Source hash: `3af17885fc885e10b9ea2a099f66111921e98924f88cedf3777d3a0a640251b2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L15): `public sealed class GroupEndpointTests(ApiFactory factory) : IAsyncLifetime`
- [Line 19](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L19): `public async Task InitializeAsync()`
- [Line 26](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L26): `public Task DisposeAsync()`
- [Line 33](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L33): `public async Task Create_ThenGet_Returns201WithLocationThen200WithFullShape()`
- [Line 67](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L67): `public async Task Create_NameTakenIgnoringCase_Returns409NameTakenWithErrorsName()`
- [Line 86](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L86): `public async Task Create_CapacityOutOfRange_Returns400WithErrorsCapacityAndCreatesNoRow(int capacity)`
- [Line 104](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L104): `public async Task Create_EmptyName_Returns400WithErrorsName(string name)`
- [Line 119](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L119): `public async Task Create_WithOverPostedField_Returns400RequestMalformedAndCreatesNoRow(string extraField)`
- [Line 142](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L142): `public async Task Create_UnknownSubject_Returns404SubjectNotFound()`
- [Line 154](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L154): `public async Task Create_UnknownTeacher_Returns404TeacherNotFound()`
- [Line 166](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L166): `public async Task Create_ArchivedSubject_Returns422SubjectArchived()`
- [Line 179](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L179): `public async Task Create_ArchivedTeacher_Returns422TeacherArchived()`
- [Line 192](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L192): `public async Task Rename_HappyPathThenStaleVersion()`
- [Line 208](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L208): `public async Task Rename_ToAnotherGroupsName_Returns409NameTaken()`
- [Line 228](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L228): `public async Task Put_WithSubjectIdInBody_Returns400RequestMalformedAndLeavesSubjectUnchanged(string suffix)`
- [Line 251](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L251): `public async Task ChangeCapacity_HappyPathThenZeroReturns400WithErrorsCapacity()`
- [Line 270](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L270): `public async Task AssignTeacher_ActiveTeacher_Returns204AndGetShowsTheNewTeacher()`
- [Line 287](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L287): `public async Task AssignTeacher_ArchivedTeacher_Returns422TeacherArchived()`
- [Line 303](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L303): `public async Task AssignTeacher_UnknownTeacher_Returns404TeacherNotFound()`
- [Line 316](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L316): `public async Task ArchiveThenRestore_AndTheirRuleViolations()`
- [Line 341](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L341): `public async Task Restore_AfterItsTeacherWasArchived_Returns422TeacherArchived()`
- [Line 355](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L355): `public async Task Restore_AfterItsSubjectWasArchived_Returns422SubjectArchived()`
- [Line 369](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L369): `public async Task Get_UnknownId_Returns404GroupNotFound()`
- [Line 379](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L379): `public async Task List_FiltersBySubjectAndByTeacher_AndHidesArchivedUnlessAsked()`
- [Line 398](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L398): `public async Task SubjectArchive_WithAnActiveGroup_Returns422HasActiveGroupsAndSubjectStaysActive()`
- [Line 422](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L422): `public async Task ChangeCapacity_BelowTheEnrolledCount_Returns422CapacityBelowEnrolledAndCapacityStays()`
- [Line 444](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L444): `public async Task Archive_WithAnActiveEnrollment_Returns422HasActiveEnrollmentsAndGroupStaysActive()`
- [Line 470](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L470): `public async Task List_NoCookie_Returns401()`
- [Line 483](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L483): `public async Task Create_WithoutTheAntiforgeryHeader_Returns403CsrfInvalidAndCreatesNoRow()`
- [Line 497](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L497): `public async Task AssignTeacher_ToATeacherBusyAtOneOfTheGroupsSessions_Returns409TeacherDoubleBookedAndKeepsTeacherAndSessions()`
- [Line 518](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L518): `public async Task Archive_WithAScheduleRuleInEffect_Returns422HasActiveSchedule()`
- [Line 535](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L535): `private static async Task AddSaturdayRuleAsync(HttpClient client, Guid groupId, string startTime)`
- [Line 543](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L543): `private Task<string> SnapshotGroupSessionsAsync(Guid groupId) =>`
- [Line 550](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L550): `private static async Task<Guid> CreateSubjectAsync(HttpClient client, string name)`
- [Line 558](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L558): `private static async Task<Guid> CreateTeacherAsync(HttpClient client, string fullName)`
- [Line 567](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L567): `private static async Task<Guid> CreateGroupAsync(HttpClient client, string name, Guid subjectId, Guid teacherId)`
- [Line 575](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L575): `private static async Task<Guid> EnrollNewStudentAsync(HttpClient client, Guid groupId, string fullName, string parentPhone)`
- [Line 588](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L588): `private static async Task ArchiveGroupAsync(HttpClient client, Guid id)`
- [Line 594](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L594): `private static async Task ArchiveSubjectAsync(HttpClient client, Guid id)`
- [Line 600](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L600): `private static async Task ArchiveTeacherAsync(HttpClient client, Guid id)`
- [Line 608](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L608): `private static async Task<JsonElement> GetGroupAsync(HttpClient client, Guid id)`
- [Line 615](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L615): `private static async Task<uint> GetVersionAsync(HttpClient client, Guid id) =>`
- [Line 618](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L618): `private static async Task<uint> GetSubjectVersionAsync(HttpClient client, Guid id)`
- [Line 624](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L624): `private static async Task<List<Guid>> ListIdsAsync(HttpClient client, string query)`
- [Line 632](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L632): `private static async Task<int> CountGroupsAsync(HttpClient client)`
- [Line 639](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L639): `private static Task<HttpResponseMessage> SendAsync(HttpClient client, HttpMethod method, string url, object body) =>`
- [Line 642](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L642): `private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus, string expectedCode)`
- [Line 649](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L649): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 45.5: the eight group endpoints' contract through the full HTTP pipeline — status, code, body shape and
/// headers, never messages — plus the one outcome Day 44 added to Month 2's subject archive endpoint, and the two
/// outcomes scheduling (Day 52) added to reassigning and archiving a group (Task 53.4). Every test creates its own
/// subjects, teachers and groups, so the seeded demo data never decides a result.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class GroupEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri GroupsUri = new("/api/groups", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    // The two schedule cases fix the clock; every other case runs on real time, as before.
    public Task DisposeAsync()
    {
        factory.Time.UseRealTime();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Create_ThenGet_Returns201WithLocationThen200WithFullShape()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var subjectId = await CreateSubjectAsync(owner.Client, "Physics Endpoint");
        var teacherId = await CreateTeacherAsync(owner.Client, "Amina Hassan");

        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, GroupsUri, new { name = "Physics – Grade 10 أ", subjectId, teacherId, capacity = 20 });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createBody = await ReadJsonAsync(createResponse);
        var id = createBody.RootElement.GetProperty("id").GetGuid();
        Assert.Equal($"/api/groups/{id}", createResponse.Headers.Location?.OriginalString);

        using var getResponse = await owner.Client.GetAsync(createResponse.Headers.Location);
        var body = await ReadJsonAsync(getResponse);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var group = body.RootElement;
        Assert.Equal(id, group.GetProperty("id").GetGuid());
        Assert.Equal("Physics – Grade 10 أ", group.GetProperty("name").GetString());
        Assert.Equal(subjectId, group.GetProperty("subjectId").GetGuid());
        Assert.Equal("Physics Endpoint", group.GetProperty("subjectName").GetString());
        Assert.Equal(teacherId, group.GetProperty("teacherId").GetGuid());
        Assert.Equal("Amina Hassan", group.GetProperty("teacherName").GetString());
        Assert.Equal(20, group.GetProperty("capacity").GetInt32());
        Assert.Equal(0, group.GetProperty("enrolledCount").GetInt32());
        Assert.Equal("active", group.GetProperty("status").GetString());
        Assert.True(group.GetProperty("version").GetUInt32() > 0);

        // Groups carry no contact data: exactly the ten GroupDto fields, nothing else.
        Assert.Equal(10, group.EnumerateObject().Count());
    }

    [Fact]
    public async Task Create_NameTakenIgnoringCase_Returns409NameTakenWithErrorsName()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, GroupsUri, new { name = "grade 10 physics", subjectId, teacherId, capacity = 20 });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("group.name_taken", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("name").GetArrayLength() > 0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    [InlineData(500)]
    public async Task Create_CapacityOutOfRange_Returns400WithErrorsCapacityAndCreatesNoRow(int capacity)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var countBefore = await CountGroupsAsync(owner.Client);
```

The excerpt stops at line 90; use the original file for the remaining 564 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
