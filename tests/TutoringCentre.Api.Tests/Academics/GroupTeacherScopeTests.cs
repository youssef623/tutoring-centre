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
            "capacity" => (HttpMethod.Put, $"/api/groups/{id}/capacity", new { capacity = 30, version }),
            "teacher" => (HttpMethod.Put, $"/api/groups/{id}/teacher", new { teacherId = arranged.OtherTeacherId, version }),
            "archive" => (HttpMethod.Post, $"/api/groups/{id}/archive", new { version }),
            _ => (HttpMethod.Post, $"/api/groups/{id}/restore", new { version }),
        };

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(teacher.Client, method, new Uri(url, UriKind.Relative), body);

        await AssertProblemAsync(response, HttpStatusCode.Forbidden, "auth.permission_denied");
        Assert.Equal(groupBefore, await SnapshotGroupAsync(id));
        Assert.Equal(countBefore, await CountGroupsAsync());
    }

    [Fact]
    public async Task UnlinkedTeacher_List_IsEmpty()
    {
        await ArrangeAsync();
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (unlinked, _) = await SignInHelper.CreateAndSignInNewTeacherAsync(factory, owner, "unlinked-scope-list@nile.test");
        using (unlinked)
        {
            Assert.Empty(await ListIdsAsync(unlinked.Client, "?includeArchived=true"));
        }
    }

    [Fact]
    public async Task UnlinkedTeacher_GetEitherGroupById_Returns404GroupNotFound()
    {
        var arranged = await ArrangeAsync();
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (unlinked, _) = await SignInHelper.CreateAndSignInNewTeacherAsync(factory, owner, "unlinked-scope-get@nile.test");
        using (unlinked)
        {
            foreach (var groupId in new[] { arranged.OwnGroupId, arranged.OtherGroupId })
            {
                using var response = await unlinked.Client.GetAsync(new Uri($"/api/groups/{groupId}", UriKind.Relative));
                await AssertProblemAsync(response, HttpStatusCode.NotFound, "group.not_found");
            }
        }
    }

    [Fact]
    public async Task AfterOwnerReassignsTheOtherGroup_LinkedTeachersNextListContainsBoth()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");
        Assert.Equal(new[] { arranged.OwnGroupId }, await ListIdsAsync(teacher.Client, "?includeArchived=true"));

        using (var owner = await SignInHelper.SignInAsNileOwnerAsync(factory))
        using (var reassigned = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/groups/{arranged.OtherGroupId}/teacher", UriKind.Relative),
            new { teacherId = arranged.LinkedTeacherId, version = await GetVersionAsOwnerAsync(arranged.OtherGroupId) }))
        {
            Assert.Equal(HttpStatusCode.NoContent, reassigned.StatusCode);
        }

        var ids = await ListIdsAsync(teacher.Client, "?includeArchived=true");
        Assert.Equal(2, ids.Count);
        Assert.Contains(arranged.OwnGroupId, ids);
        Assert.Contains(arranged.OtherGroupId, ids);
    }

    [Fact]
    public async Task AfterOwnerRemovesTheAccountLink_LinkedTeachersNextListIsEmptyAndOwnGroupIs404()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");
        Assert.Equal(new[] { arranged.OwnGroupId }, await ListIdsAsync(teacher.Client, "?includeArchived=true"));

        using (var owner = await SignInHelper.SignInAsNileOwnerAsync(factory))
        {
            await SetTeacherAccountAsync(owner.Client, arranged.LinkedTeacherId, null);
        }

        Assert.Empty(await ListIdsAsync(teacher.Client, "?includeArchived=true"));
        using var response = await teacher.Client.GetAsync(new Uri($"/api/groups/{arranged.OwnGroupId}", UriKind.Relative));
        await AssertProblemAsync(response, HttpStatusCode.NotFound, "group.not_found");
    }

    [Fact]
    public async Task SameAccountWithMaadiSelected_SeesNothingOfNile()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "maadi-hub");

        var ids = await ListIdsAsync(teacher.Client, "?includeArchived=true");
        Assert.DoesNotContain(arranged.OwnGroupId, ids);
        Assert.DoesNotContain(arranged.OtherGroupId, ids);
        using var response = await teacher.Client.GetAsync(new Uri($"/api/groups/{arranged.OwnGroupId}", UriKind.Relative));
        await AssertProblemAsync(response, HttpStatusCode.NotFound, "group.not_found");
    }

    [Fact]
    public async Task Owner_OnTheSameRoutes_SeesBothGroups()
    {
        var arranged = await ArrangeAsync();
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        var ids = await ListIdsAsync(owner.Client, $"?subjectId={arranged.SubjectId}");
        Assert.Equal(2, ids.Count);
        Assert.Contains(arranged.OwnGroupId, ids);
        Assert.Contains(arranged.OtherGroupId, ids);
    }

    /// <summary>
    /// As the Nile owner, over HTTP: one subject, two teachers, one group each, and the first teacher linked to
    /// teacher@both.test. Any teacher record already linked to that user in Nile (the demo seed links one) is
    /// unlinked first, so "own" means exactly the group arranged here.
    /// </summary>
    private async Task<Arrangement> ArrangeAsync()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var teacherUserId = await FindUserIdAsync(owner.Client, TwoCentreTeacherEmail);

        using (var teachersResponse = await owner.Client.GetAsync(new Uri("/api/teachers?includeArchived=true", UriKind.Relative)))
        {
            var teachers = await ReadJsonAsync(teachersResponse);
            foreach (var existing in teachers.RootElement.GetProperty("items").EnumerateArray())
            {
                if (existing.GetProperty("userId").ValueKind == JsonValueKind.String && existing.GetProperty("userId").GetGuid() == teacherUserId)
                {
                    await SetTeacherAccountAsync(owner.Client, existing.GetProperty("id").GetGuid(), null);
                }
            }
        }

        var subjectId = await CreateAsync(owner.Client, "/api/subjects", new { name = "Scope Physics" });
        var linkedTeacherId = await CreateAsync(owner.Client, "/api/teachers", new { fullName = "Scope Linked Teacher", phone = (string?)null });
        var otherTeacherId = await CreateAsync(owner.Client, "/api/teachers", new { fullName = "Scope Other Teacher", phone = (string?)null });
        await SetTeacherAccountAsync(owner.Client, linkedTeacherId, teacherUserId);
        var ownGroupId = await CreateAsync(
            owner.Client, "/api/groups", new { name = "Scope Own Group", subjectId, teacherId = linkedTeacherId, capacity = 20 });
        var otherGroupId = await CreateAsync(
            owner.Client, "/api/groups", new { name = "Scope Other Group", subjectId, teacherId = otherTeacherId, capacity = 20 });

        return new Arrangement(subjectId, linkedTeacherId, otherTeacherId, ownGroupId, otherGroupId);
    }

    private static async Task<Guid> FindUserIdAsync(HttpClient ownerClient, string email)
    {
        using var response = await ownerClient.GetAsync(new Uri("/api/staff", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("email").GetString() == email).GetProperty("userId").GetGuid();
    }

    private static async Task SetTeacherAccountAsync(HttpClient ownerClient, Guid teacherId, Guid? userId)
    {
        using var getResponse = await ownerClient.GetAsync(new Uri($"/api/teachers/{teacherId}", UriKind.Relative));
        var version = (await ReadJsonAsync(getResponse)).RootElement.GetProperty("version").GetUInt32();
        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            ownerClient, HttpMethod.Put, new Uri($"/api/teachers/{teacherId}/account", UriKind.Relative), new { userId, version });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)
    {
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri(url, UriKind.Relative), body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("id").GetGuid();
    }

    private async Task<uint> GetVersionAsOwnerAsync(Guid groupId)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await owner.Client.GetAsync(new Uri($"/api/groups/{groupId}", UriKind.Relative));
        return (await ReadJsonAsync(response)).RootElement.GetProperty("version").GetUInt32();
    }

    private Task<string> SnapshotGroupAsync(Guid id) =>
        factory.ScalarAsync<string>(
            $"select name || '|' || capacity || '|' || teacher_id || '|' || status || '|' || xmin from academics.groups where id = '{id}'");

    private Task<long> CountGroupsAsync() => factory.ScalarAsync<long>("select count(*) from academics.groups");

    private static async Task<List<Guid>> ListIdsAsync(HttpClient client, string query)
    {
        using var response = await client.GetAsync(new Uri($"/api/groups{query}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("items").EnumerateArray().Select(item => item.GetProperty("id").GetGuid()).ToList();
    }

    private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus, string expectedCode)
    {
        var body = await ReadJsonAsync(response);
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal(expectedCode, body.RootElement.GetProperty("code").GetString());
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }

    private sealed record Arrangement(Guid SubjectId, Guid LinkedTeacherId, Guid OtherTeacherId, Guid OwnGroupId, Guid OtherGroupId);
}
