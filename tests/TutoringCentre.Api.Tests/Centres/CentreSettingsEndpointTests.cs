using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Centres;

/// <summary>Task 32.10: proves the GET/PUT /api/centre/settings contract (Day 32) through the full HTTP pipeline.</summary>
[Collection(ApiCollection.Name)]
public sealed class CentreSettingsEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri SettingsUri = new("/api/centre/settings", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Get_AsOwner_ReturnsTheContractShape()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await owner.Client.GetAsync(SettingsUri);
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Nile Tutoring Centre", body.RootElement.GetProperty("name").GetString());
        Assert.Equal("nile-centre", body.RootElement.GetProperty("slug").GetString());
        Assert.Equal("Africa/Cairo", body.RootElement.GetProperty("timeZoneId").GetString());
        Assert.Equal("ar", body.RootElement.GetProperty("defaultLocale").GetString());
        Assert.True(body.RootElement.GetProperty("version").GetUInt32() >= 0);
    }

    [Fact]
    public async Task Put_HappyPath_ThenGetShowsTheNewNameAndVersion_AndMeShowsTheNewName()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var version = await GetVersionAsync(owner.Client);

        using var putResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, SettingsUri, new { name = "Nile Learning Centre", defaultLocale = "en", version });
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        using var getResponse = await owner.Client.GetAsync(SettingsUri);
        var getBody = await ReadJsonAsync(getResponse);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal("Nile Learning Centre", getBody.RootElement.GetProperty("name").GetString());
        Assert.Equal("en", getBody.RootElement.GetProperty("defaultLocale").GetString());
        Assert.NotEqual(version, getBody.RootElement.GetProperty("version").GetUInt32());

        using var meResponse = await owner.Client.GetAsync(new Uri("/api/me", UriKind.Relative));
        var meBody = await ReadJsonAsync(meResponse);
        var activeCentreId = meBody.RootElement.GetProperty("activeCentreId").GetGuid();
        var membership = meBody.RootElement.GetProperty("memberships").EnumerateArray()
            .Single(item => item.GetProperty("centreId").GetGuid() == activeCentreId);
        Assert.Equal("Nile Learning Centre", membership.GetProperty("centreName").GetString());
    }

    [Fact]
    public async Task Put_BodyWithSlug_Returns400()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var version = await GetVersionAsync(owner.Client);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, SettingsUri, new { name = "Nile Learning Centre", defaultLocale = "en", version, slug = "sneaky-slug" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_EmptyName_Returns400WithErrorsName()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var version = await GetVersionAsync(owner.Client);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, SettingsUri, new { name = "", defaultLocale = "en", version });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("name").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Get_AsSecretary_Returns403()
    {
        using var secretary = await SignInHelper.SignInAsNileSecretaryAsync(factory);

        using var response = await secretary.Client.GetAsync(SettingsUri);
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.permission_denied", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Put_AsSecretary_Returns403()
    {
        using var secretary = await SignInHelper.SignInAsNileSecretaryAsync(factory);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            secretary.Client, HttpMethod.Put, SettingsUri, new { name = "Hacked Name", defaultLocale = "en", version = 0u });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.permission_denied", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Get_AsTwoCentreTeacher_Returns403()
    {
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await teacher.Client.GetAsync(SettingsUri);
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.permission_denied", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Put_AsTwoCentreTeacher_Returns403()
    {
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            teacher.Client, HttpMethod.Put, SettingsUri, new { name = "Hacked Name", defaultLocale = "en", version = 0u });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.permission_denied", body.RootElement.GetProperty("code").GetString());
    }

    private static async Task<uint> GetVersionAsync(HttpClient client)
    {
        using var response = await client.GetAsync(SettingsUri);
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("version").GetUInt32();
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
