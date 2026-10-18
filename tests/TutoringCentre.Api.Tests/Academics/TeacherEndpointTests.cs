using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>Task 43.14: proves the seven Teacher endpoints' behaviour through the full HTTP pipeline, against the Day 43 contract.</summary>
[Collection(ApiCollection.Name)]
public sealed class TeacherEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri TeachersUri = new("/api/teachers", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_ThenGet_Returns201WithLocationThen200()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, TeachersUri, new { fullName = "Amina Hassan", phone = "+201012345678" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var location = createResponse.Headers.Location;
        Assert.NotNull(location);

        using var getResponse = await nileOwner.Client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal("Amina Hassan", body.RootElement.GetProperty("fullName").GetString());
        Assert.Equal("+201012345678", body.RootElement.GetProperty("phone").GetString());
        Assert.Equal("active", body.RootElement.GetProperty("status").GetString());
        Assert.True(body.RootElement.GetProperty("userId").ValueKind is JsonValueKind.Null);
    }

    [Fact]
    public async Task Create_TwoTeachersSharingAName_BothSucceed()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var first = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, TeachersUri, new { fullName = "Amina Hassan", phone = (string?)null });
        using var second = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, TeachersUri, new { fullName = "Amina Hassan", phone = (string?)null });

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        Assert.NotEqual(first.Headers.Location, second.Headers.Location);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_EmptyName_Returns400WithErrorsFullName(string fullName)
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, TeachersUri, new { fullName, phone = (string?)null });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("fullName").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Create_WithExtraUserIdField_Returns400RequestMalformedAndCreatesNoRow()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var countBefore = await CountTeachersAsync(nileOwner.Client);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, TeachersUri, new { fullName = "Amina Hassan", phone = (string?)null, userId = Guid.NewGuid() });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("request.malformed", body.RootElement.GetProperty("code").GetString());
        Assert.Equal(countBefore, await CountTeachersAsync(nileOwner.Client));
    }

    [Fact]
    public async Task Update_WithExtraUserIdField_Returns400RequestMalformedAndLeavesLinkUnchanged()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateTeacherAsync(nileOwner.Client, "Amina Hassan");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/teachers/{id}", UriKind.Relative),
            new { fullName = "Amina Hassan", phone = (string?)null, version, userId = Guid.NewGuid() });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("request.malformed", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Update_HappyPathThenStaleVersion()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateTeacherAsync(nileOwner.Client, "Amina Hassan");

        using var updated = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/teachers/{id}", UriKind.Relative),
            new { fullName = "Amina Youssef", phone = "+201012345678", version });
        Assert.Equal(HttpStatusCode.NoContent, updated.StatusCode);

        using var staleUpdate = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/teachers/{id}", UriKind.Relative),
            new { fullName = "Someone Else", phone = (string?)null, version });
        var body = await ReadJsonAsync(staleUpdate);
        Assert.Equal(HttpStatusCode.Conflict, staleUpdate.StatusCode);
        Assert.Equal("concurrency.stale", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task ArchiveThenRestore_AndTheirRuleViolations()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateTeacherAsync(nileOwner.Client, "Amina Hassan");

        using var archived = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Post, new Uri($"/api/teachers/{id}/archive", UriKind.Relative), new { version });
        Assert.Equal(HttpStatusCode.NoContent, archived.StatusCode);

        var archivedVersion = await GetVersionAsync(nileOwner.Client, id);
        using var archiveAgain = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Post, new Uri($"/api/teachers/{id}/archive", UriKind.Relative), new { version = archivedVersion });
        var archiveAgainBody = await ReadJsonAsync(archiveAgain);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, archiveAgain.StatusCode);
        Assert.Equal("teacher.already_archived", archiveAgainBody.RootElement.GetProperty("code").GetString());

        using var restored = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Post, new Uri($"/api/teachers/{id}/restore", UriKind.Relative), new { version = archivedVersion });
        Assert.Equal(HttpStatusCode.NoContent, restored.StatusCode);

        var restoredVersion = await GetVersionAsync(nileOwner.Client, id);
        using var restoreAgain = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Post, new Uri($"/api/teachers/{id}/restore", UriKind.Relative), new { version = restoredVersion });
        var restoreAgainBody = await ReadJsonAsync(restoreAgain);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, restoreAgain.StatusCode);
        Assert.Equal("teacher.not_archived", restoreAgainBody.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Update_ArchivedTeacher_Returns422ArchivedReadOnly()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateTeacherAsync(nileOwner.Client, "Amina Hassan");
        using (var archived = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Post, new Uri($"/api/teachers/{id}/archive", UriKind.Relative), new { version }))
        {
            Assert.Equal(HttpStatusCode.NoContent, archived.StatusCode);
        }
        var archivedVersion = await GetVersionAsync(nileOwner.Client, id);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/teachers/{id}", UriKind.Relative),
            new { fullName = "Changed", phone = (string?)null, version = archivedVersion });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("teacher.archived_read_only", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task SetAccount_IneligibleUserId_Returns422UserNotEligibleAndLeavesLinkUnchanged()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateTeacherAsync(nileOwner.Client, "Amina Hassan");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/teachers/{id}/account", UriKind.Relative),
            new { userId = Guid.NewGuid(), version });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("teacher.user_not_eligible", body.RootElement.GetProperty("code").GetString());

        using var getResponse = await nileOwner.Client.GetAsync(new Uri($"/api/teachers/{id}", UriKind.Relative));
        var getBody = await ReadJsonAsync(getResponse);
        Assert.True(getBody.RootElement.GetProperty("userId").ValueKind is JsonValueKind.Null);
    }

    [Fact]
    public async Task SetAccount_LinkThenUnlink_Returns204EachAndListShowsDisplayName()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateTeacherAsync(nileOwner.Client, "Amina Hassan");
        var (userId, displayName) = await CreateEligibleTeacherUserAsync(nileOwner.Client, "teacher-account-link@nile.test");

        using var linked = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/teachers/{id}/account", UriKind.Relative), new { userId, version });
        Assert.Equal(HttpStatusCode.NoContent, linked.StatusCode);

        using var listResponse = await nileOwner.Client.GetAsync(TeachersUri);
        var listBody = await ReadJsonAsync(listResponse);
        var item = listBody.RootElement.GetProperty("items").EnumerateArray().Single(i => i.GetProperty("id").GetGuid() == id);
        Assert.Equal(displayName, item.GetProperty("userDisplayName").GetString());

        var linkedVersion = await GetVersionAsync(nileOwner.Client, id);
        using var unlinked = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/teachers/{id}/account", UriKind.Relative), new { userId = (Guid?)null, version = linkedVersion });
        Assert.Equal(HttpStatusCode.NoContent, unlinked.StatusCode);

        using var finalGet = await nileOwner.Client.GetAsync(new Uri($"/api/teachers/{id}", UriKind.Relative));
        var finalBody = await ReadJsonAsync(finalGet);
        Assert.True(finalBody.RootElement.GetProperty("userId").ValueKind is JsonValueKind.Null);
    }

    [Fact]
    public async Task SetAccount_UserAlreadyLinkedToAnotherTeacher_Returns409AccountTaken()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (firstId, firstVersion) = await CreateTeacherAsync(nileOwner.Client, "Amina Hassan");
        var (secondId, secondVersion) = await CreateTeacherAsync(nileOwner.Client, "Mona Said");
        var (userId, _) = await CreateEligibleTeacherUserAsync(nileOwner.Client, "teacher-account-taken@nile.test");

        using (var linked = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/teachers/{firstId}/account", UriKind.Relative), new { userId, version = firstVersion }))
        {
            Assert.Equal(HttpStatusCode.NoContent, linked.StatusCode);
        }

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/teachers/{secondId}/account", UriKind.Relative), new { userId, version = secondVersion });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("teacher.account_taken", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task List_NoCookie_Returns401()
    {
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
        });

        using var response = await client.GetAsync(TeachersUri);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await nileOwner.Client.PostAsJsonAsync(TeachersUri, new { fullName = "Amina Hassan", phone = (string?)null });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.csrf_invalid", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Get_NonGuidId_Returns404RouteNotFound()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await nileOwner.Client.GetAsync(new Uri("/api/teachers/abc", UriKind.Relative));

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("route.not_found", body.RootElement.GetProperty("code").GetString());
    }

    private static async Task<(Guid Id, uint Version)> CreateTeacherAsync(HttpClient client, string fullName)
    {
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(client, TeachersUri, new { fullName, phone = (string?)null });
        var location = createResponse.Headers.Location!;
        using var getResponse = await client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        return (body.RootElement.GetProperty("id").GetGuid(), body.RootElement.GetProperty("version").GetUInt32());
    }

    /// <summary>Creates a real Nile Teacher-role staff member through /api/staff, an eligible user for the account link.</summary>
    private static async Task<(Guid UserId, string DisplayName)> CreateEligibleTeacherUserAsync(HttpClient client, string email)
    {
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, new Uri("/api/staff", UriKind.Relative), new { email, displayName = "Linked Teacher", role = "teacher", preferredLocale = "en" });
        var body = await ReadJsonAsync(createResponse);
        return (body.RootElement.GetProperty("userId").GetGuid(), "Linked Teacher");
    }

    private static async Task<uint> GetVersionAsync(HttpClient client, Guid id)
    {
        using var response = await client.GetAsync(new Uri($"/api/teachers/{id}", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("version").GetUInt32();
    }

    private static async Task<int> CountTeachersAsync(HttpClient client)
    {
        using var response = await client.GetAsync(new Uri("/api/teachers?includeArchived=true", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("items").GetArrayLength();
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
