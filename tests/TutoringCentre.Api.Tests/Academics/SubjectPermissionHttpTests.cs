using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 28.10: the Day 28 permission matrix proved through HTTP for the six Subject endpoints, over Nile's
/// three roles (the Day 26 harness). 18 literal cases (6 endpoints x 3 roles) plus one extra: permission is
/// evaluated before existence, so a forbidden request against an unknown ID is still 403, never 404.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class SubjectPermissionHttpTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri SubjectsUri = new("/api/subjects", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Teacher_List_Returns200()
    {
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await teacher.Client.GetAsync(SubjectsUri);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Teacher_GetById_Returns200()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, _) = await CreateSubjectAsync(owner.Client, "Teacher Get Target");
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await teacher.Client.GetAsync(new Uri($"/api/subjects/{id}", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Teacher_Create_IsForbiddenAndCreatesNoRow()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var countBefore = await CountSubjectsAsync(owner.Client);
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        await AssertPermissionDeniedAsync(AntiforgeryTestHelper.PostAsJsonAsync(
            teacher.Client, SubjectsUri, new { name = "Teacher Create Attempt" }));

        Assert.Equal(countBefore, await CountSubjectsAsync(owner.Client));
    }

    [Fact]
    public async Task Teacher_Rename_IsForbiddenAndLeavesTheRowUnchanged()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateSubjectAsync(owner.Client, "Teacher Rename Target");
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        await AssertPermissionDeniedAsync(AntiforgeryTestHelper.SendAsJsonAsync(
            teacher.Client, HttpMethod.Put, new Uri($"/api/subjects/{id}", UriKind.Relative), new { name = "Teacher Rename Attempt", version }));

        Assert.Equal("Teacher Rename Target", await GetNameAsync(owner.Client, id));
        Assert.Equal(version, await GetVersionAsync(owner.Client, id));
    }

    [Fact]
    public async Task Teacher_Archive_IsForbiddenAndLeavesTheRowUnchanged()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateSubjectAsync(owner.Client, "Teacher Archive Target");
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        await AssertPermissionDeniedAsync(AntiforgeryTestHelper.SendAsJsonAsync(
            teacher.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/archive", UriKind.Relative), new { version }));

        Assert.Equal(version, await GetVersionAsync(owner.Client, id));
    }

    [Fact]
    public async Task Teacher_Restore_IsForbiddenAndLeavesTheRowUnchanged()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateSubjectAsync(owner.Client, "Teacher Restore Target");
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        await AssertPermissionDeniedAsync(AntiforgeryTestHelper.SendAsJsonAsync(
            teacher.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/restore", UriKind.Relative), new { version }));

        Assert.Equal(version, await GetVersionAsync(owner.Client, id));
    }

    [Theory]
    [InlineData("owner@nile.test")]
    [InlineData("secretary@nile.test")]
    public async Task OwnerAndSecretary_CanListGetCreateRenameArchiveAndRestore(string email)
    {
        using var actor = await SignInHelper.SignInAsync(factory, email);

        using (var list = await actor.Client.GetAsync(SubjectsUri))
        {
            Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        }

        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(actor.Client, SubjectsUri, new { name = $"{email} subject" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var location = createResponse.Headers.Location!;

        using (var get = await actor.Client.GetAsync(location))
        {
            Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        }

        var (id, version) = await ReadIdAndVersionAsync(actor.Client, location);

        using (var rename = await AntiforgeryTestHelper.SendAsJsonAsync(
            actor.Client, HttpMethod.Put, new Uri($"/api/subjects/{id}", UriKind.Relative), new { name = $"{email} renamed", version }))
        {
            Assert.Equal(HttpStatusCode.NoContent, rename.StatusCode);
        }

        var renamedVersion = await GetVersionAsync(actor.Client, id);
        using (var archive = await AntiforgeryTestHelper.SendAsJsonAsync(
            actor.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/archive", UriKind.Relative), new { version = renamedVersion }))
        {
            Assert.Equal(HttpStatusCode.NoContent, archive.StatusCode);
        }

        var archivedVersion = await GetVersionAsync(actor.Client, id);
        using (var restore = await AntiforgeryTestHelper.SendAsJsonAsync(
            actor.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/restore", UriKind.Relative), new { version = archivedVersion }))
        {
            Assert.Equal(HttpStatusCode.NoContent, restore.StatusCode);
        }
    }

    [Fact]
    public async Task Teacher_ForbiddenRenameAgainstAnUnknownId_Returns403NotFound_PermissionBeforeExistence()
    {
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        await AssertPermissionDeniedAsync(AntiforgeryTestHelper.SendAsJsonAsync(
            teacher.Client,
            HttpMethod.Put,
            new Uri($"/api/subjects/{Guid.NewGuid()}", UriKind.Relative),
            new { name = "Does not matter", version = 0u }));
    }

    private static async Task AssertPermissionDeniedAsync(Task<HttpResponseMessage> responseTask)
    {
        using var response = await responseTask;
        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.permission_denied", body.RootElement.GetProperty("code").GetString());
    }

    private static async Task<(Guid Id, uint Version)> CreateSubjectAsync(HttpClient client, string name)
    {
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(client, SubjectsUri, new { name });
        return await ReadIdAndVersionAsync(client, createResponse.Headers.Location!);
    }

    private static async Task<(Guid Id, uint Version)> ReadIdAndVersionAsync(HttpClient client, Uri location)
    {
        using var getResponse = await client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        return (body.RootElement.GetProperty("id").GetGuid(), body.RootElement.GetProperty("version").GetUInt32());
    }

    private static async Task<string> GetNameAsync(HttpClient client, Guid id)
    {
        using var response = await client.GetAsync(new Uri($"/api/subjects/{id}", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("name").GetString()!;
    }

    private static async Task<uint> GetVersionAsync(HttpClient client, Guid id)
    {
        using var response = await client.GetAsync(new Uri($"/api/subjects/{id}", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("version").GetUInt32();
    }

    private static async Task<int> CountSubjectsAsync(HttpClient client)
    {
        using var response = await client.GetAsync(new Uri("/api/subjects?includeArchived=true", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("items").GetArrayLength();
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
