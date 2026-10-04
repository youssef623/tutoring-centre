using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Auth;

[Collection(ApiCollection.Name)]
public sealed class LoginFlowTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri LoginUri = new("/api/auth/login", UriKind.Relative);
    private static readonly Uri LogoutUri = new("/api/auth/logout", UriKind.Relative);
    private static readonly Uri MeUri = new("/api/me", UriKind.Relative);
    private static readonly Uri SelectCentreUri = new("/api/session/centre", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Login_AsOwner_SetsHostPrefixedSecureCookieAndAutoSelectsTheirOnlyCentre()
    {
        using var client = CreateSessionClient();

        using var response = await client.PostAsJsonAsync(
            LoginUri,
            new { email = "owner@nile.test", password = ApiFactory.TestSeedPassword });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var setCookie = Assert.Single(response.Headers.GetValues("Set-Cookie"));
        Assert.StartsWith("__Host-tcm.session=", setCookie, StringComparison.Ordinal);
        Assert.Contains("secure", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("httponly", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=lax", setCookie, StringComparison.OrdinalIgnoreCase);

        var body = await ReadJsonAsync(response);
        Assert.Equal("owner", body.RootElement.GetProperty("activeRole").GetString());
        Assert.False(string.IsNullOrEmpty(body.RootElement.GetProperty("activeCentreId").GetString()));
        Assert.Equal("nile-centre", body.RootElement.GetProperty("memberships")[0].GetProperty("centreSlug").GetString());
    }

    [Fact]
    public async Task Login_AsTeacherWithTwoCentres_LeavesNoActiveCentreSelected()
    {
        using var client = CreateSessionClient();

        using var response = await client.PostAsJsonAsync(
            LoginUri,
            new { email = "teacher@both.test", password = ApiFactory.TestSeedPassword });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(JsonValueKind.Null, body.RootElement.GetProperty("activeCentreId").ValueKind);
        Assert.Equal(2, body.RootElement.GetProperty("memberships").GetArrayLength());
    }

    [Fact]
    public async Task SelectCentre_ForAMembershipTheUserHolds_Returns204AndActivatesIt()
    {
        using var client = CreateSessionClient();
        var maadiHubCentreId = await LoginAsTeacherAndGetMaadiHubCentreIdAsync(client);

        using var selectResponse = await client.PostAsJsonAsync(SelectCentreUri, new { centreId = maadiHubCentreId });
        Assert.Equal(HttpStatusCode.NoContent, selectResponse.StatusCode);

        using var meResponse = await client.GetAsync(MeUri);
        var me = await ReadJsonAsync(meResponse);
        Assert.Equal(maadiHubCentreId.ToString(), me.RootElement.GetProperty("activeCentreId").GetString());
        Assert.Equal("teacher", me.RootElement.GetProperty("activeRole").GetString());
    }

    [Fact]
    public async Task SelectCentre_ForACentreTheUserDoesNotBelongTo_Returns403AndLeavesSessionUnchanged()
    {
        using var client = CreateSessionClient();
        var maadiHubCentreId = await LoginAsTeacherAndGetMaadiHubCentreIdAsync(client);
        using (var firstSelect = await client.PostAsJsonAsync(SelectCentreUri, new { centreId = maadiHubCentreId }))
        {
            Assert.Equal(HttpStatusCode.NoContent, firstSelect.StatusCode);
        }

        using var forbidden = await client.PostAsJsonAsync(SelectCentreUri, new { centreId = Guid.Empty });
        var problem = await ReadJsonAsync(forbidden);
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
        Assert.Equal("tenant.no_membership", problem.RootElement.GetProperty("code").GetString());

        using var meResponse = await client.GetAsync(MeUri);
        var me = await ReadJsonAsync(meResponse);
        Assert.Equal(maadiHubCentreId.ToString(), me.RootElement.GetProperty("activeCentreId").GetString());
    }

    [Fact]
    public async Task Logout_Returns204AndClearsTheCookie()
    {
        using var client = CreateSessionClient();
        using (var login = await client.PostAsJsonAsync(LoginUri, new { email = "owner@nile.test", password = ApiFactory.TestSeedPassword }))
        {
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        }

        using var logout = await client.PostAsync(LogoutUri, content: null);

        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);
        var setCookie = Assert.Single(logout.Headers.GetValues("Set-Cookie"));
        Assert.StartsWith("__Host-tcm.session=;", setCookie, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetMe_AfterLogout_Returns401()
    {
        using var client = CreateSessionClient();
        using (var login = await client.PostAsJsonAsync(LoginUri, new { email = "owner@nile.test", password = ApiFactory.TestSeedPassword }))
        {
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        }
        using (var logout = await client.PostAsync(LogoutUri, content: null))
        {
            Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);
        }

        using var meResponse = await client.GetAsync(MeUri);

        Assert.Equal(HttpStatusCode.Unauthorized, meResponse.StatusCode);
    }

    // CookieSecurePolicy.Always means the CookieContainer only resends the session cookie over https.
    private HttpClient CreateSessionClient() =>
        factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

    private static async Task<Guid> LoginAsTeacherAndGetMaadiHubCentreIdAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            LoginUri,
            new { email = "teacher@both.test", password = ApiFactory.TestSeedPassword });
        var body = await ReadJsonAsync(response);

        foreach (var membership in body.RootElement.GetProperty("memberships").EnumerateArray())
        {
            if (membership.GetProperty("centreSlug").GetString() == "maadi-hub")
            {
                return membership.GetProperty("centreId").GetGuid();
            }
        }

        throw new InvalidOperationException("Seeded teacher is missing the maadi-hub membership.");
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
