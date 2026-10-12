using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>Task 39.9: proves the four Parent endpoints' behaviour through the full HTTP pipeline, against the Day 39 contract's outcome table.</summary>
[Collection(ApiCollection.Name)]
public sealed class ParentEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri ParentsUri = new("/api/parents", UriKind.Relative);
    private static readonly Uri LookupUri = new("/api/parents/lookup", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_ThenLookupWithDifferentSpelling_ThenGet_ThenUpdate_HappyPath()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, ParentsUri, new { fullName = "Omar Hassan", phone = "01012345678" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var location = createResponse.Headers.Location;
        Assert.NotNull(location);
        var createBody = await ReadJsonAsync(createResponse);
        var id = createBody.RootElement.GetProperty("id").GetGuid();

        using var lookupResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, LookupUri, new { phone = "0101 234 5678" });
        var lookupBody = await ReadJsonAsync(lookupResponse);
        Assert.Equal(HttpStatusCode.OK, lookupResponse.StatusCode);
        Assert.Equal("no-store", lookupResponse.Headers.CacheControl?.ToString());
        Assert.Equal(id, lookupBody.RootElement.GetProperty("parent").GetProperty("id").GetGuid());

        using var getResponse = await nileOwner.Client.GetAsync(location);
        var getBody = await ReadJsonAsync(getResponse);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal("Omar Hassan", getBody.RootElement.GetProperty("fullName").GetString());
        Assert.Equal("+201012345678", getBody.RootElement.GetProperty("phone").GetString());
        Assert.Empty(getBody.RootElement.GetProperty("children").EnumerateArray());
        var version = getBody.RootElement.GetProperty("version").GetUInt32();

        using var updateResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, location, new { fullName = "Omar Hassan Ali", phone = "+201012345678", version });
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        using var getAfterUpdate = await nileOwner.Client.GetAsync(location);
        var afterBody = await ReadJsonAsync(getAfterUpdate);
        Assert.Equal("Omar Hassan Ali", afterBody.RootElement.GetProperty("fullName").GetString());
    }

    [Fact]
    public async Task Create_DuplicatePhone_Returns409WithErrorsPhone()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using (var first = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, ParentsUri, new { fullName = "Omar Hassan", phone = "01012345678" }))
        {
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        }

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, ParentsUri, new { fullName = "Another Parent", phone = "0101 234 5678" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("parent.phone_taken", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("phone").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Create_InvalidPhone_Returns400PhoneInvalidWithErrorsPhone()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, ParentsUri, new { fullName = "Omar Hassan", phone = "12345" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("phone.invalid", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("phone").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Create_OneCharacterName_Returns400WithErrorsFullName()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, ParentsUri, new { fullName = "X", phone = "01012345678" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("fullName").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Create_EmptyPhone_Returns400WithErrorsPhone()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, ParentsUri, new { fullName = "Omar Hassan", phone = "" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("phone").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Create_WithExtraCentreIdField_Returns400RequestMalformedAndCreatesNoRow()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, ParentsUri, new { fullName = "Omar Hassan", phone = "01012345678", centreId = Guid.NewGuid() });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("request.malformed", body.RootElement.GetProperty("code").GetString());
        Assert.Equal(0, await factory.ScalarAsync<long>("select count(*) from academics.parents"));
    }

    [Fact]
    public async Task Update_StaleVersion_Returns409ConcurrencyStale()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateParentAsync(nileOwner.Client, "Omar Hassan", "01012345678");

        using var firstUpdate = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/parents/{id}", UriKind.Relative),
            new { fullName = "Omar Hassan Ali", phone = "+201012345678", version });
        Assert.Equal(HttpStatusCode.NoContent, firstUpdate.StatusCode);

        using var staleUpdate = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/parents/{id}", UriKind.Relative),
            new { fullName = "Another Name", phone = "+201012345678", version });

        var body = await ReadJsonAsync(staleUpdate);
        Assert.Equal(HttpStatusCode.Conflict, staleUpdate.StatusCode);
        Assert.Equal("concurrency.stale", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Update_UnknownId_Returns404ParentNotFound()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/parents/{Guid.NewGuid()}", UriKind.Relative),
            new { fullName = "Omar Hassan", phone = "01012345678", version = 0 });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("parent.not_found", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Get_UnknownId_Returns404ParentNotFound()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await nileOwner.Client.GetAsync(new Uri($"/api/parents/{Guid.NewGuid()}", UriKind.Relative));

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("parent.not_found", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Lookup_InvalidPhone_Returns400PhoneInvalidWithErrorsPhone()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, LookupUri, new { phone = "12345" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("phone.invalid", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("phone").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Lookup_NoSuchParent_Returns200WithNullParent()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, LookupUri, new { phone = "01012345678" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(JsonValueKind.Null, body.RootElement.GetProperty("parent").ValueKind);
    }

    [Fact]
    public async Task Get_NoCookie_Returns401()
    {
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
        });

        using var response = await client.GetAsync(new Uri($"/api/parents/{Guid.NewGuid()}", UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await nileOwner.Client.PostAsJsonAsync(ParentsUri, new { fullName = "Omar Hassan", phone = "01012345678" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.csrf_invalid", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Lookup_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await nileOwner.Client.PostAsJsonAsync(LookupUri, new { phone = "01012345678" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.csrf_invalid", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Create_TwoCentreTeacherBeforeSelectingACentre_Returns403TenantNotSelected()
    {
        using var teacher = await SignInHelper.SignInAsync(factory, "teacher@both.test");
        Assert.Null(teacher.CentreId);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(teacher.Client, ParentsUri, new { fullName = "Omar Hassan", phone = "01012345678" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("tenant.not_selected", body.RootElement.GetProperty("code").GetString());
    }

    private static async Task<(Guid Id, uint Version)> CreateParentAsync(HttpClient client, string fullName, string phone)
    {
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(client, ParentsUri, new { fullName, phone });
        var location = createResponse.Headers.Location!;
        using var getResponse = await client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        return (body.RootElement.GetProperty("id").GetGuid(), body.RootElement.GetProperty("version").GetUInt32());
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
