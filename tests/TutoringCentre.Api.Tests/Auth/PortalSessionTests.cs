using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TutoringCentre.Api.Auth;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Application.Identity.Portal.Commands.CreatePortalAccount;
using TutoringCentre.Application.Identity.Queries.ValidateStaffSession;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Api.Tests.Auth;

[Collection(ConventionsCollection.Name)]
public sealed class PortalSessionTests(ConventionsFactory factory) : IAsyncLifetime
{
    private static readonly Uri LoginUri = new("/api/auth/login", UriKind.Relative);
    private static readonly Uri MeUri = new("/api/me", UriKind.Relative);
    private static readonly Uri ActorUri = new("/api/test/actor", UriKind.Relative);
    private static readonly Uri PasswordUri = new("/api/auth/change-password", UriKind.Relative);
    private static readonly Uri CentreUri = new("/api/session/centre", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }
    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData(StaffRole.Parent)]
    [InlineData(StaffRole.Student)]
    public async Task PortalLoginBuildsCorrectActorAndServerResolvedStudentScope(StaffRole role)
    {
        var portal = await SeedPortalAsync(role);
        using var client = Client(factory);
        using (var login = await LoginAsync(client, "secretary@nile.test", ApiFactory.TestSeedPassword))
        {
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
            using var body = await JsonAsync(login);
            Assert.Equal(["attendance.view", "portal.chat"], body.RootElement.GetProperty("permissions").EnumerateArray().Select(value => value.GetString()!).ToArray());
        }
        using var response = await client.GetAsync(ActorUri);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var actor = await JsonAsync(response);
        Assert.Equal(role.ToString(), actor.RootElement.GetProperty("kind").GetString());
        Assert.Equal(portal.SubjectId, actor.RootElement.GetProperty("subjectId").GetGuid());
        Assert.Equal(portal.UserId, actor.RootElement.GetProperty("userId").GetGuid());
        Assert.Equal(portal.CentreId, actor.RootElement.GetProperty("centreId").GetGuid());
        var students = actor.RootElement.GetProperty("studentIds").EnumerateArray().Select(value => value.GetGuid()).ToHashSet();
        if (role == StaffRole.Student)
            Assert.Equal(portal.SubjectId, Assert.Single(students));
        else
        {
            var expected = await factory.ScalarAsync<Guid[]>($"""
                select array_agg(s.id) from academics.student_parents sp
                join academics.students s on s.centre_id=sp.centre_id and s.id=sp.student_id
                where sp.centre_id='{portal.CentreId}' and sp.parent_id='{portal.SubjectId}' and s.status='active'
                """);
            Assert.True(students.SetEquals(expected));
        }
        using var me = await client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
        using var staff = await client.GetAsync(new Uri("/api/staff", UriKind.Relative));
        Assert.Equal(HttpStatusCode.Forbidden, staff.StatusCode);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CachedPortalSubjectSurvivesCacheHitButRevocationEndsSessionAtNextRevalidation(bool removeLink)
    {
        var portal = await SeedPortalAsync(StaffRole.Parent);
        using var host = factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            services.PostConfigure<SessionValidationOptions>(options => options.CacheDuration = TimeSpan.FromMinutes(1))));
        using var client = Client(host);
        using var login = await LoginAsync(client, "secretary@nile.test", ApiFactory.TestSeedPassword);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        using (var before = await client.GetAsync(ActorUri)) Assert.Equal(HttpStatusCode.OK, before.StatusCode);
        var stamp = await factory.ScalarAsync<string>($"select security_stamp from identity.users where id='{portal.UserId}'");
        var key = ("session-valid", portal.UserId, stamp, (Guid?)portal.CentreId, (StaffRole?)StaffRole.Parent);
        var cache = host.Services.GetRequiredService<IMemoryCache>();
        Assert.True(cache.TryGetValue(key, out SessionValidationResult validation));
        Assert.True(validation.IsValid);
        Assert.Equal(portal.SubjectId, validation.PortalSubject!.Id);
        await factory.ExecuteAsync(removeLink
            ? $"delete from identity.portal_links where id='{portal.LinkId}'"
            : $"update identity.memberships set status='inactive' where user_id='{portal.UserId}' and centre_id='{portal.CentreId}'");
        using (var cached = await client.GetAsync(ActorUri))
        {
            using var actor = await JsonAsync(cached);
            Assert.Equal("Parent", actor.RootElement.GetProperty("kind").GetString());
        }
        // Shorten only this real cached validation's expiry so the test exercises expiration without a 60s wait.
        cache.Set(key, validation, TimeSpan.FromMilliseconds(1));
        await Task.Delay(30);
        using var after = await client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.Unauthorized, after.StatusCode);
        Assert.Equal(1L, await factory.ScalarAsync<long>($"select count(*) from identity.users where id='{portal.UserId}'"));
    }

    [Fact]
    public async Task MissingLinkRefusesPortalSessionImmediately()
    {
        var portal = await SeedPortalAsync(StaffRole.Parent);
        await factory.ExecuteAsync($"delete from identity.portal_links where id='{portal.LinkId}'");
        using var client = Client(factory);
        using var login = await LoginAsync(client, "secretary@nile.test", ApiFactory.TestSeedPassword);
        using var response = await client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ForgedRoleIsRejectedAgainstMembership()
    {
        var portal = await SeedPortalAsync(StaffRole.Parent);
        var stamp = await factory.ScalarAsync<string>($"select security_stamp from identity.users where id='{portal.UserId}'");
        var principal = SessionPrincipalFactory.Create(portal.UserId, stamp, portal.CentreId, StaffRole.Owner);
        var options = factory.Services.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>().Get(CookieAuthenticationDefaults.AuthenticationScheme);
        var ticket = options.TicketDataFormat.Protect(new AuthenticationTicket(principal, CookieAuthenticationDefaults.AuthenticationScheme));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), HandleCookies = false });
        client.DefaultRequestHeaders.Add("Cookie", $"{options.Cookie.Name}={ticket}");
        using var response = await client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("invalid-centre", null)]
    [InlineData(null, "invalid-role")]
    [InlineData("invalid-centre", "invalid-role")]
    public async Task MalformedClaimPairsRejectAndClearCookie(string? centreClaim, string? roleClaim)
    {
        var user = await factory.ScalarAsync<Guid>("select id from identity.users where email='secretary@nile.test'");
        var stamp = await factory.ScalarAsync<string>($"select security_stamp from identity.users where id='{user}'");
        var principal = SessionPrincipalFactory.Create(user, stamp, null, null);
        var identity = (System.Security.Claims.ClaimsIdentity)principal.Identity!;
        if (centreClaim is not null) identity.AddClaim(new(SessionClaimNames.CentreId, centreClaim));
        if (roleClaim is not null) identity.AddClaim(new(SessionClaimNames.Role, roleClaim));
        var options = factory.Services.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>().Get(CookieAuthenticationDefaults.AuthenticationScheme);
        var ticket = options.TicketDataFormat.Protect(new AuthenticationTicket(principal, CookieAuthenticationDefaults.AuthenticationScheme));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), HandleCookies = false });
        client.DefaultRequestHeaders.Add("Cookie", $"{options.Cookie.Name}={ticket}");
        using var response = await client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));
    }

    [Fact]
    public async Task ActivePortalActorCanChangePasswordAndSelectCentreAgain()
    {
        var portal = await SeedPortalAsync(StaffRole.Parent);
        using var client = Client(factory);
        using var login = await LoginAsync(client, "secretary@nile.test", ApiFactory.TestSeedPassword);
        using var changed = await AntiforgeryTestHelper.PostAsJsonAsync(client, PasswordUri,
            new { currentPassword = ApiFactory.TestSeedPassword, newPassword = "PortalChangedPassword-123!" });
        Assert.Equal(HttpStatusCode.NoContent, changed.StatusCode);
        using var selected = await AntiforgeryTestHelper.PostAsJsonAsync(client, CentreUri, new { centreId = portal.CentreId });
        Assert.Equal(HttpStatusCode.NoContent, selected.StatusCode);
        using var response = await client.GetAsync(ActorUri);
        using var actor = await JsonAsync(response);
        Assert.Equal("Parent", actor.RootElement.GetProperty("kind").GetString());
    }

    [Fact]
    public async Task CreatedPortalAccountMustChangeTemporaryPasswordBeforeSelectingCentre()
    {
        var centre = await factory.ScalarAsync<Guid>("select id from platform.centres where slug='nile-centre'");
        var parent = await ParentAsync(centre);
        var owner = await factory.ScalarAsync<Guid>("select id from identity.users where email='owner@nile.test'");
        await using var scope = factory.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new StaffActor(owner, centre, StaffRole.Owner));
        var created = await scope.ServiceProvider.GetRequiredService<Dispatcher>().SendAsync<CreatePortalAccountCommand, CreatePortalAccountResult>(
            new(PortalSubjectKind.Parent, parent, "new-portal@nile.test", SupportedLocale.En), CancellationToken.None);
        Assert.True(created.IsSuccess, created.Error?.Message);
        using var client = Client(factory);
        using var login = await LoginAsync(client, "new-portal@nile.test", created.Value.TemporaryPassword!);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        using (var body = await JsonAsync(login)) Assert.True(body.RootElement.GetProperty("mustChangePassword").GetBoolean());
        using (var refused = await AntiforgeryTestHelper.PostAsJsonAsync(client, CentreUri, new { centreId = centre }))
            Assert.Equal(HttpStatusCode.Forbidden, refused.StatusCode);
        using var changed = await AntiforgeryTestHelper.PostAsJsonAsync(client, PasswordUri,
            new { currentPassword = created.Value.TemporaryPassword, newPassword = "PortalChangedPassword-123!" });
        Assert.Equal(HttpStatusCode.NoContent, changed.StatusCode);
        using var selected = await AntiforgeryTestHelper.PostAsJsonAsync(client, CentreUri, new { centreId = centre });
        Assert.Equal(HttpStatusCode.NoContent, selected.StatusCode);
        using var response = await client.GetAsync(ActorUri);
        using var actor = await JsonAsync(response);
        Assert.Equal("Parent", actor.RootElement.GetProperty("kind").GetString());
        Assert.Equal(parent, actor.RootElement.GetProperty("subjectId").GetGuid());
    }

    [Theory]
    [InlineData(StaffRole.Parent)]
    [InlineData(StaffRole.Student)]
    public void CookieContainsOnlyIdentityCentreAndRoleClaims(StaffRole role)
    {
        var principal = SessionPrincipalFactory.Create(Guid.NewGuid(), "stamp", Guid.NewGuid(), role);
        Assert.Equal(["sub", "stamp", "centre", "role"], principal.Claims.Select(claim => claim.Type));
    }

    private async Task<PortalSetup> SeedPortalAsync(StaffRole role)
    {
        var centre = await factory.ScalarAsync<Guid>("select id from platform.centres where slug='nile-centre'");
        var user = await factory.ScalarAsync<Guid>("select id from identity.users where email='secretary@nile.test'");
        var owner = await factory.ScalarAsync<Guid>("select id from identity.users where email='owner@nile.test'");
        var subject = role == StaffRole.Parent ? await ParentAsync(centre)
            : await factory.ScalarAsync<Guid>($"select id from academics.students where centre_id='{centre}' and status='active' order by id limit 1");
        var link = Guid.NewGuid();
        await factory.ExecuteAsync($"""
            update identity.memberships set role='{role.ToString().ToLowerInvariant()}' where centre_id='{centre}' and user_id='{user}';
            insert into identity.portal_links (id,centre_id,user_id,role_key,parent_id,student_id,created_at,created_by)
            values ('{link}','{centre}','{user}','{role.ToString().ToLowerInvariant()}',
                {(role == StaffRole.Parent ? $"'{subject}'" : "null")},{(role == StaffRole.Student ? $"'{subject}'" : "null")},now(),'{owner}');
            """);
        return new(user, centre, subject, link);
    }

    private Task<Guid> ParentAsync(Guid centre) => factory.ScalarAsync<Guid>($"""
        select sp.parent_id from academics.student_parents sp join academics.students s on s.centre_id=sp.centre_id and s.id=sp.student_id
        where sp.centre_id='{centre}' and s.status='active' order by sp.parent_id limit 1
        """);
    private static HttpClient Client(WebApplicationFactory<Program> host) => host.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
    private static Task<HttpResponseMessage> LoginAsync(HttpClient client, string email, string password) =>
        AntiforgeryTestHelper.PostAsJsonAsync(client, LoginUri, new { email, password });
    private static async Task<JsonDocument> JsonAsync(HttpResponseMessage response) => JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    private sealed record PortalSetup(Guid UserId, Guid CentreId, Guid SubjectId, Guid LinkId);
}
