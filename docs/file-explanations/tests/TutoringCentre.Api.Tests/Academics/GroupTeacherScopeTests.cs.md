# `tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 45.6: the ownership guard proven as black-box behaviour through real sessions. The owner arranges, over HTTP, two teachers and two groups in Nile and links one teacher record to teacher@both.test; the tests then act as that user. The unlinked teacher is covered too, because that is exactly where a default of "All" instead of "None" would hide. Reads outside the scope are 404, never 403; writes are 403 for a teacher even on their own group, because permission is evaluated before ownership.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 288 lines and 14,387 bytes. Source hash: `de2f11415fbd9ac48432d680c36b035b3fb99032d7c33aa77fb10b60aa083400`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L10): `/// HTTP, two teachers and two groups in Nile and links one teacher record to teacher@both.test; the tests then`
- [Line 16](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L16): `public sealed class GroupTeacherScopeTests(ApiFactory factory) : IAsyncLifetime`
- [Line 20](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L20): `public async Task InitializeAsync()`
- [Line 26](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L26): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 29](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L29): `public async Task LinkedTeacher_List_ContainsExactlyTheOwnGroup()`
- [Line 38](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L38): `public async Task LinkedTeacher_GetOwnGroup_Returns200()`
- [Line 52](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L52): `public async Task LinkedTeacher_GetOtherTeachersGroup_Returns404GroupNotFound()`
- [Line 63](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L63): `public async Task LinkedTeacher_FilterByOtherTeacher_ReturnsEmptyItems()`
- [Line 78](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L78): `public async Task LinkedTeacher_EveryWriteRoute_Returns403PermissionDeniedAndChangesNothing(string route)`
- [Line 105](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L105): `public async Task UnlinkedTeacher_List_IsEmpty()`
- [Line 117](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L117): `public async Task UnlinkedTeacher_GetEitherGroupById_Returns404GroupNotFound()`
- [Line 133](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L133): `public async Task AfterOwnerReassignsTheOtherGroup_LinkedTeachersNextListContainsBoth()`
- [Line 154](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L154): `public async Task AfterOwnerRemovesTheAccountLink_LinkedTeachersNextListIsEmptyAndOwnGroupIs404()`
- [Line 171](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L171): `public async Task SameAccountWithMaadiSelected_SeesNothingOfNile()`
- [Line 184](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L184): `public async Task Owner_OnTheSameRoutes_SeesBothGroups()`
- [Line 197](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L197): `/// teacher@both.test. Any teacher record already linked to that user in Nile (the demo seed links one) is`
- [Line 200](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L200): `private async Task<Arrangement> ArrangeAsync()`
- [Line 229](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L229): `private static async Task<Guid> FindUserIdAsync(HttpClient ownerClient, string email)`
- [Line 237](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L237): `private static async Task SetTeacherAccountAsync(HttpClient ownerClient, Guid teacherId, Guid? userId)`
- [Line 246](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L246): `private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)`
- [Line 253](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L253): `private async Task<uint> GetVersionAsOwnerAsync(Guid groupId)`
- [Line 260](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L260): `private Task<string> SnapshotGroupAsync(Guid id) =>`
- [Line 264](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L264): `private Task<long> CountGroupsAsync() => factory.ScalarAsync<long>("select count(*) from academics.groups");`
- [Line 266](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L266): `private static async Task<List<Guid>> ListIdsAsync(HttpClient client, string query)`
- [Line 274](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L274): `private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus, string expectedCode)`
- [Line 281](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L281): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`
- [Line 287](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L287): `private sealed record Arrangement(Guid SubjectId, Guid LinkedTeacherId, Guid OtherTeacherId, Guid OwnGroupId, Guid OtherGroupId);`

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
/// Task 45.6: the ownership guard proven as black-box behaviour through real sessions. The owner arranges, over
/// HTTP, two teachers and two groups in Nile and links one teacher record to teacher@both.test; the tests then
/// act as that user. The unlinked teacher is covered too, because that is exactly where a default of "All"
/// instead of "None" would hide. Reads outside the scope are 404, never 403; writes are 403 for a teacher even
/// on their own group, because permission is evaluated before ownership.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class GroupTeacherScopeTests(ApiFactory factory) : IAsyncLifetime
{
    private const string TwoCentreTeacherEmail = "teacher@both.test";

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task LinkedTeacher_List_ContainsExactlyTheOwnGroup()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        Assert.Equal(new[] { arranged.OwnGroupId }, await ListIdsAsync(teacher.Client, "?includeArchived=true"));
    }

    [Fact]
    public async Task LinkedTeacher_GetOwnGroup_Returns200()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await teacher.Client.GetAsync(new Uri($"/api/groups/{arranged.OwnGroupId}", UriKind.Relative));

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(arranged.OwnGroupId, body.RootElement.GetProperty("id").GetGuid());
        Assert.Equal(arranged.LinkedTeacherId, body.RootElement.GetProperty("teacherId").GetGuid());
    }

    [Fact]
    public async Task LinkedTeacher_GetOtherTeachersGroup_Returns404GroupNotFound()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await teacher.Client.GetAsync(new Uri($"/api/groups/{arranged.OtherGroupId}", UriKind.Relative));

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "group.not_found");
    }

    [Fact]
    public async Task LinkedTeacher_FilterByOtherTeacher_ReturnsEmptyItems()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        Assert.Empty(await ListIdsAsync(teacher.Client, $"?teacherId={arranged.OtherTeacherId}&includeArchived=true"));
    }

    [Theory]
    [InlineData("create")]
    [InlineData("rename")]
    [InlineData("capacity")]
    [InlineData("teacher")]
    [InlineData("archive")]
    [InlineData("restore")]
    public async Task LinkedTeacher_EveryWriteRoute_Returns403PermissionDeniedAndChangesNothing(string route)
    {
        var arranged = await ArrangeAsync();
        var id = arranged.OwnGroupId;
        var version = await GetVersionAsOwnerAsync(id);
        var groupBefore = await SnapshotGroupAsync(id);
        var countBefore = await CountGroupsAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");
        var (method, url, body) = route switch
        {
            "create" => (HttpMethod.Post, "/api/groups",
                (object)new { name = "Teacher Attempt", subjectId = arranged.SubjectId, teacherId = arranged.LinkedTeacherId, capacity = 20 }),
            "rename" => (HttpMethod.Put, $"/api/groups/{id}", new { name = "Teacher Attempt", version }),
```

The excerpt stops at line 90; use the original file for the remaining 198 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
