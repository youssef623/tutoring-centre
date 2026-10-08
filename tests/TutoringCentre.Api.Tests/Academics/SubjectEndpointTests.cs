using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>Task 26.9: proves the six Subject endpoints' behaviour through the full HTTP pipeline, against the Day 26 contract.</summary>
[Collection(ApiCollection.Name)]
public sealed class SubjectEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri SubjectsUri = new("/api/subjects", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task List_ReturnsOnlyTheCallersCentresSubjectsInNameOrder()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await nileOwner.Client.GetAsync(SubjectsUri);
        var json = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var names = json.RootElement.GetProperty("items").EnumerateArray().Select(item => item.GetProperty("name").GetString()!).ToList();
        Assert.Equal(3, names.Count);
        Assert.DoesNotContain("Mathematics", names);
        Assert.DoesNotContain("Physics", names);
        Assert.Equal(names.OrderBy(name => name, StringComparer.Ordinal), names);
    }

    [Fact]
    public async Task Create_ThenGet_Returns201WithLocationThen200()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, SubjectsUri, new { name = "Geography" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var location = createResponse.Headers.Location;
        Assert.NotNull(location);

        using var getResponse = await nileOwner.Client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal("Geography", body.RootElement.GetProperty("name").GetString());
        Assert.Equal("active", body.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Create_Duplicate_Returns409WithErrorsName()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using (var first = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, SubjectsUri, new { name = "Geography" }))
        {
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        }

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, SubjectsUri, new { name = "geography" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("subject.name_taken", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("name").GetArrayLength() > 0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_EmptyName_Returns400WithErrorsName(string name)
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, SubjectsUri, new { name });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("name").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Create_EightyOneCharacterName_Returns400WithErrorsName()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, SubjectsUri, new { name = new string('a', 81) });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("name").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Create_WithExtraCentreIdField_Returns400RequestMalformedAndCreatesNoRow()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, SubjectsUri, new { name = "X", centreId = Guid.NewGuid() });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("request.malformed", body.RootElement.GetProperty("code").GetString());
        Assert.Equal(3, await CountSubjectsAsync(nileOwner.Client));
    }

    [Fact]
    public async Task Rename_HappyPathThenStaleVersion()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateSubjectAsync(nileOwner.Client, "Geography");

        using var renamed = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/subjects/{id}", UriKind.Relative), new { name = "World Geography", version });
        Assert.Equal(HttpStatusCode.NoContent, renamed.StatusCode);

        using var staleRename = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/subjects/{id}", UriKind.Relative), new { name = "Physical Geography", version });
        var body = await ReadJsonAsync(staleRename);
        Assert.Equal(HttpStatusCode.Conflict, staleRename.StatusCode);
        Assert.Equal("concurrency.stale", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task ArchiveThenRestore_AndTheirRuleViolations()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateSubjectAsync(nileOwner.Client, "Geography");

        using var archived = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/archive", UriKind.Relative), new { version });
        Assert.Equal(HttpStatusCode.NoContent, archived.StatusCode);

        var archivedVersion = await GetVersionAsync(nileOwner.Client, id);
        using var archiveAgain = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/archive", UriKind.Relative), new { version = archivedVersion });
        var archiveAgainBody = await ReadJsonAsync(archiveAgain);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, archiveAgain.StatusCode);
        Assert.Equal("subject.already_archived", archiveAgainBody.RootElement.GetProperty("code").GetString());

        using var restored = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/restore", UriKind.Relative), new { version = archivedVersion });
        Assert.Equal(HttpStatusCode.NoContent, restored.StatusCode);

        var restoredVersion = await GetVersionAsync(nileOwner.Client, id);
        using var restoreAgain = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/restore", UriKind.Relative), new { version = restoredVersion });
        var restoreAgainBody = await ReadJsonAsync(restoreAgain);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, restoreAgain.StatusCode);
        Assert.Equal("subject.not_archived", restoreAgainBody.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task List_NoCookie_Returns401()
    {
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
        });

        using var response = await client.GetAsync(SubjectsUri);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await nileOwner.Client.PostAsJsonAsync(SubjectsUri, new { name = "Geography" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.csrf_invalid", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task List_TwoCentreTeacherBeforeSelectingACentre_Returns403TenantNotSelected()
    {
        using var teacher = await SignInHelper.SignInAsync(factory, "teacher@both.test");
        Assert.Null(teacher.CentreId);

        using var response = await teacher.Client.GetAsync(SubjectsUri);

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("tenant.not_selected", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Get_NonGuidId_Returns404RouteNotFound()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await nileOwner.Client.GetAsync(new Uri("/api/subjects/abc", UriKind.Relative));

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("route.not_found", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task ErrorBody_AlwaysHasCodeTraceIdCorrelationIdAndNoStackTrace()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await nileOwner.Client.GetAsync(new Uri($"/api/subjects/{Guid.NewGuid()}", UriKind.Relative));
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(string.IsNullOrEmpty(body.RootElement.GetProperty("code").GetString()));
        Assert.False(string.IsNullOrEmpty(body.RootElement.GetProperty("traceId").GetString()));
        Assert.False(string.IsNullOrEmpty(body.RootElement.GetProperty("correlationId").GetString()));
        Assert.False(body.RootElement.TryGetProperty("stackTrace", out _));
        Assert.False(body.RootElement.TryGetProperty("exception", out _));
        var raw = body.RootElement.GetRawText();
        Assert.DoesNotContain("TutoringCentre.", raw, StringComparison.Ordinal);
        Assert.DoesNotContain(" at ", raw, StringComparison.Ordinal);
    }

    private static async Task<(Guid Id, uint Version)> CreateSubjectAsync(HttpClient client, string name)
    {
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(client, SubjectsUri, new { name });
        var location = createResponse.Headers.Location!;
        using var getResponse = await client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        return (body.RootElement.GetProperty("id").GetGuid(), body.RootElement.GetProperty("version").GetUInt32());
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
