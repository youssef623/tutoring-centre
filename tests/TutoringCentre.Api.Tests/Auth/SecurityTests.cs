using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Api.Tests.Auth;

/// <summary>
/// Proves the attacks Day 16 closes are actually closed: user enumeration, CSRF (16.6, written by hand), and
/// cookie theft, tenant escalation, stale sessions, claim forgery, spraying, brute force, broken identity
/// propagation and forgotten authorization (16.7). Uses <see cref="ConventionsFactory"/> rather than the plain
/// <see cref="ApiFactory"/> because two cases need its test-only endpoints (current actor, default protection).
/// </summary>
[Collection(ConventionsCollection.Name)]
public sealed class SecurityTests(ConventionsFactory factory) : IAsyncLifetime
{
    private static readonly Uri LoginUri = new("/api/auth/login", UriKind.Relative);
    private static readonly Uri MeUri = new("/api/me", UriKind.Relative);
    private static readonly Uri SelectCentreUri = new("/api/session/centre", UriKind.Relative);
    private static readonly Uri ActorUri = new("/api/test/actor", UriKind.Relative);
    private static readonly Uri DefaultProtectionUri = new("/api/test-default-protection", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // --- 16.6: written by hand ---

    [Fact]
    public async Task Login_WrongPasswordForKnownUserAndUnknownUser_ReturnIdenticalResponses()
    {
        using var client = CreateSessionClient();

        using var wrongPassword = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "owner@nile.test", password = "clearly-the-wrong-password" });
        using var unknownEmail = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "nobody@nile.test", password = "clearly-the-wrong-password" });

        Assert.Equal(HttpStatusCode.Unauthorized, wrongPassword.StatusCode);
        Assert.Equal(wrongPassword.StatusCode, unknownEmail.StatusCode);

        var wrongPasswordBody = await ReadJsonAsync(wrongPassword);
        var unknownEmailBody = await ReadJsonAsync(unknownEmail);

        // Same field set...
        Assert.Equal(
            FieldNames(wrongPasswordBody),
            FieldNames(unknownEmailBody));

        // ...and every value identical except the two per-request identifiers.
        foreach (var field in FieldNames(wrongPasswordBody).Where(name => name is not "traceId" and not "correlationId"))
        {
            Assert.Equal(
                wrongPasswordBody.RootElement.GetProperty(field).ToString(),
                unknownEmailBody.RootElement.GetProperty(field).ToString());
        }
    }

    [Fact]
    public async Task Logout_WithoutCsrfHeader_Returns403AndSessionStillWorksAfterwards()
    {
        using var client = CreateSessionClient();
        using (var login = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "owner@nile.test", password = ApiFactory.TestSeedPassword }))
        {
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        }

        using (var logoutWithoutToken = await client.PostAsync(new Uri("/api/auth/logout", UriKind.Relative), content: null))
        {
            var problem = await ReadJsonAsync(logoutWithoutToken);
            Assert.Equal(HttpStatusCode.Forbidden, logoutWithoutToken.StatusCode);
            Assert.Equal("auth.csrf_invalid", problem.RootElement.GetProperty("code").GetString());
        }

        using var me = await client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
    }

    [Fact]
    public async Task Login_WithoutCsrfHeader_Returns403()
    {
        using var client = CreateSessionClient();

        using var response = await client.PostAsJsonAsync(
            LoginUri, new { email = "owner@nile.test", password = ApiFactory.TestSeedPassword });

        var problem = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.csrf_invalid", problem.RootElement.GetProperty("code").GetString());
    }

    // --- 16.7: AI-written ---

    [Fact]
    public async Task Login_SetsAHostPrefixedSecureHttpOnlyLaxCookie()
    {
        using var client = CreateSessionClient();

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "owner@nile.test", password = ApiFactory.TestSeedPassword });

        var setCookie = Assert.Single(response.Headers.GetValues("Set-Cookie"));
        Assert.StartsWith("__Host-tcm.session=", setCookie, StringComparison.Ordinal);
        Assert.Contains("httponly", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("secure", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=lax", setCookie, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetMe_WithNoCookie_Returns401()
    {
        using var client = CreateSessionClient();

        using var response = await client.GetAsync(MeUri);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SelectCentre_ForACentreOwnerDoesNotBelongTo_Returns403AndMeStillShowsTheOriginalCentre()
    {
        using var client = CreateSessionClient();
        using var login = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "owner@nile.test", password = ApiFactory.TestSeedPassword });
        var loginBody = await ReadJsonAsync(login);
        var nileCentreId = loginBody.RootElement.GetProperty("activeCentreId").GetString();

        var maadiHubCentreId = await factory.ScalarAsync<Guid>("select id from platform.centres where slug = 'maadi-hub'");

        using var select = await AntiforgeryTestHelper.PostAsJsonAsync(client, SelectCentreUri, new { centreId = maadiHubCentreId });
        var problem = await ReadJsonAsync(select);
        Assert.Equal(HttpStatusCode.Forbidden, select.StatusCode);
        Assert.Equal("tenant.no_membership", problem.RootElement.GetProperty("code").GetString());

        using var me = await client.GetAsync(MeUri);
        var meBody = await ReadJsonAsync(me);
        Assert.Equal(nileCentreId, meBody.RootElement.GetProperty("activeCentreId").GetString());
    }

    [Fact]
    public async Task RevokedMembership_NextRequestAfterRevocation_Returns401()
    {
        using var client = CreateSessionClient();
        using (var login = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "secretary@nile.test", password = ApiFactory.TestSeedPassword }))
        {
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        }
        using (var before = await client.GetAsync(MeUri))
        {
            Assert.Equal(HttpStatusCode.OK, before.StatusCode);
        }

        await factory.ExecuteAsync(
            "update identity.memberships set status = 'inactive' " +
            "where user_id = (select id from identity.users where email = 'secretary@nile.test')");

        // SessionValidation:CacheDuration is zero in tests (ApiFactory), so the very next request re-checks.
        using var after = await client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.Unauthorized, after.StatusCode);
    }

    [Fact]
    public async Task ChangedSecurityStamp_NextRequest_Returns401()
    {
        using var client = CreateSessionClient();
        using (var login = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "owner@nile.test", password = ApiFactory.TestSeedPassword }))
        {
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        }

        await factory.ExecuteAsync(
            "update identity.users set security_stamp = 'tampered-security-stamp-value' where email = 'owner@nile.test'");

        using var response = await client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RoleChangedByTheOwner_NextRequestForThatUser_Returns401()
    {
        // Task 29.10: no endpoint exists yet for ChangeStaffRoleCommand, so it is dispatched directly —
        // the same way an owner's real request will reach it once Day 30 adds the endpoint.
        using var client = CreateSessionClient();
        using (var login = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "secretary@nile.test", password = ApiFactory.TestSeedPassword }))
        {
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        }
        using (var before = await client.GetAsync(MeUri))
        {
            Assert.Equal(HttpStatusCode.OK, before.StatusCode);
        }

        var ownerId = await factory.ScalarAsync<Guid>("select id from identity.users where email = 'owner@nile.test'");
        var nileCentreId = await factory.ScalarAsync<Guid>("select id from platform.centres where slug = 'nile-centre'");

        await using var scope = factory.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new StaffActor(ownerId, nileCentreId, StaffRole.Owner));
        var dispatcher = scope.ServiceProvider.GetRequiredService<Dispatcher>();

        var listResult = await dispatcher.QueryAsync<ListStaffQuery, IReadOnlyList<StaffMemberDto>>(new ListStaffQuery(), CancellationToken.None);
        Assert.True(listResult.IsSuccess);
        var secretary = listResult.Value.Single(member => member.Email == "secretary@nile.test");

        var changeResult = await dispatcher.SendAsync<ChangeStaffRoleCommand, Unit>(
            new ChangeStaffRoleCommand(secretary.MembershipId, StaffRole.Teacher, secretary.Version), CancellationToken.None);
        Assert.True(changeResult.IsSuccess);

        // SessionValidation:CacheDuration is zero in tests (ApiFactory), so the very next request re-checks.
        using var after = await client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.Unauthorized, after.StatusCode);
    }

    [Fact]
    public async Task TamperedCookie_Returns401()
    {
        // Login happens on an ordinary client (CSRF needs its double-submit cookie auto-attached). The tampered
        // request goes through a second, HandleCookies: false client, so the only Cookie header it sends is the
        // one set manually here — nothing from a CookieContainer merges the genuine value back in.
        using var loginClient = CreateSessionClient();
        using var login = await AntiforgeryTestHelper.PostAsJsonAsync(
            loginClient, LoginUri, new { email = "owner@nile.test", password = ApiFactory.TestSeedPassword });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var setCookie = Assert.Single(login.Headers.GetValues("Set-Cookie"));
        var cookiePair = setCookie.Split(';')[0];
        var tamperedCookiePair = FlipLastCharacter(cookiePair);

        using var tamperedClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            HandleCookies = false,
        });
        using var request = new HttpRequestMessage(HttpMethod.Get, MeUri);
        request.Headers.Add("Cookie", tamperedCookiePair);
        using var response = await tamperedClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_11TimesWithinAMinuteFromOneClient_11thReturns429WithRetryAfter()
    {
        using var client = CreateSessionClient();
        const string partition = "security-suite-rate-limit";

        for (var attempt = 1; attempt <= 10; attempt++)
        {
            using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
                client, LoginUri, new { email = "owner@nile.test", password = "wrong-password" }, partition);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        using var eleventh = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "owner@nile.test", password = "wrong-password" }, partition);

        var problem = await ReadJsonAsync(eleventh);
        Assert.Equal(HttpStatusCode.TooManyRequests, eleventh.StatusCode);
        Assert.Equal("auth.rate_limited", problem.RootElement.GetProperty("code").GetString());
        Assert.True(eleventh.Headers.TryGetValues("Retry-After", out _));
    }

    [Fact]
    public async Task Login_FiveWrongPasswordsThenCorrect_StillReturns401WithTheOrdinaryFailureBody()
    {
        using var client = CreateSessionClient();

        for (var attempt = 1; attempt <= 5; attempt++)
        {
            using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
                client, LoginUri, new { email = "teacher@both.test", password = "wrong-password" });
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        using var afterLockout = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "teacher@both.test", password = ApiFactory.TestSeedPassword });

        var problem = await ReadJsonAsync(afterLockout);
        Assert.Equal(HttpStatusCode.Unauthorized, afterLockout.StatusCode);
        Assert.Equal("auth.invalid_credentials", problem.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task CurrentActor_AfterSelectingACentre_IsAStaffActorWithThatCentreInApplication()
    {
        using var client = CreateSessionClient();
        using var login = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "teacher@both.test", password = ApiFactory.TestSeedPassword });
        var loginBody = await ReadJsonAsync(login);

        var maadiHubCentreId = Guid.Empty;
        foreach (var membership in loginBody.RootElement.GetProperty("memberships").EnumerateArray())
        {
            if (membership.GetProperty("centreSlug").GetString() == "maadi-hub")
            {
                maadiHubCentreId = membership.GetProperty("centreId").GetGuid();
            }
        }

        using (var select = await AntiforgeryTestHelper.PostAsJsonAsync(client, SelectCentreUri, new { centreId = maadiHubCentreId }))
        {
            Assert.Equal(HttpStatusCode.NoContent, select.StatusCode);
        }

        using var actorResponse = await client.GetAsync(ActorUri);
        var actor = await ReadJsonAsync(actorResponse);

        Assert.Equal(HttpStatusCode.OK, actorResponse.StatusCode);
        Assert.Equal("Staff", actor.RootElement.GetProperty("kind").GetString());
        Assert.Equal(maadiHubCentreId.ToString(), actor.RootElement.GetProperty("centreId").GetString());
        Assert.Equal("teacher", actor.RootElement.GetProperty("role").GetString());
    }

    [Fact]
    public async Task EndpointWithNoAuthorizationAttribute_Returns401ByDefault()
    {
        using var client = CreateSessionClient();

        using var response = await client.GetAsync(DefaultProtectionUri);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private HttpClient CreateSessionClient() =>
        factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

    private static string FlipLastCharacter(string value)
    {
        var chars = value.ToCharArray();
        var lastIndex = chars.Length - 1;
        chars[lastIndex] = chars[lastIndex] == 'a' ? 'b' : 'a';
        return new string(chars);
    }

    private static IEnumerable<string> FieldNames(JsonDocument document) =>
        document.RootElement.EnumerateObject().Select(property => property.Name).OrderBy(name => name, StringComparer.Ordinal);

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
