using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Api.Auth;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Http;
using TutoringCentre.Api.Tests.Fixtures;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Api.Tests.Staff;

/// <summary>
/// Task 30.7: threat-driven tests for the five staff endpoints — self-escalation, over-posting, a secretary
/// attempting to manage staff, and a role change ending the affected user's session.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class StaffSecurityTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri StaffUri = new("/api/staff", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Owner_ChangesOwnRole_Returns422CannotChangeSelf()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (membershipId, version) = await FindMemberAsync(owner.Client, "owner@nile.test");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/staff/{membershipId}/role", UriKind.Relative), new { role = "teacher", version });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("staff.cannot_change_self", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Owner_DeactivatesSelf_Returns422CannotChangeSelf()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (membershipId, version) = await FindMemberAsync(owner.Client, "owner@nile.test");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/staff/{membershipId}/deactivate", UriKind.Relative), new { version });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("staff.cannot_change_self", body.RootElement.GetProperty("code").GetString());
    }

    /// <summary>
    /// staff.last_owner cannot be reached over sequential HTTP requests — the self rule fires first whenever
    /// the sole owner with staff.manage tries to touch the only other owner-shaped target, themselves. The
    /// invariant itself stays covered by the real concurrent race (Task 29.12); this asserts only that the
    /// HTTP layer maps the code correctly, the same way ResultHttpExtensionsTests proves every other mapping.
    /// </summary>
    [Fact]
    public async Task LastOwnerError_MapsTo422ThroughTheRealHttpMapping()
    {
        var error = Error.Rule("staff.last_owner", "This centre must keep at least one active owner.");

        using var services = new ServiceCollection().AddLogging().AddOptions().BuildServiceProvider();
        var context = new DefaultHttpContext { RequestServices = services };
        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        await error.ToProblemResult().ExecuteAsync(context);

        buffer.Position = 0;
        using var body = await JsonDocument.ParseAsync(buffer);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, context.Response.StatusCode);
        Assert.Equal("staff.last_owner", body.RootElement.GetProperty("code").GetString());
    }

    [Theory]
    [InlineData("""{"role":"owner","version":1,"centreId":"11111111-1111-1111-1111-111111111111"}""")]
    [InlineData("""{"role":"admin","version":1}""")]
    public async Task ChangeRole_OverPostedOrUnknownRole_Returns400(string rawBody)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (membershipId, _) = await FindMemberAsync(owner.Client, "secretary@nile.test");
        var token = await AntiforgeryTestHelper.GetCsrfTokenAsync(owner.Client);

        using var request = new HttpRequestMessage(HttpMethod.Put, new Uri($"/api/staff/{membershipId}/role", UriKind.Relative))
        {
            Content = new StringContent(rawBody, Encoding.UTF8, "application/json"),
        };
        request.Headers.Add(AntiforgerySetup.HeaderName, token);
        using var response = await owner.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Secretary_PostsValidCreateBody_Returns403AndCreatesNoUserOrMembership()
    {
        using var secretary = await SignInHelper.SignInAsNileSecretaryAsync(factory);
        var usersBefore = await factory.ScalarAsync<long>("select count(*) from identity.users");
        var membershipsBefore = await factory.ScalarAsync<long>("select count(*) from identity.memberships");

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            secretary.Client, StaffUri, new { email = "secretary-attempt@nile.test", displayName = "Attempt", role = "teacher", preferredLocale = "en" });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.permission_denied", body.RootElement.GetProperty("code").GetString());
        Assert.Equal(usersBefore, await factory.ScalarAsync<long>("select count(*) from identity.users"));
        Assert.Equal(membershipsBefore, await factory.ScalarAsync<long>("select count(*) from identity.memberships"));
    }

    [Fact]
    public async Task Secretary_GetsStaffList_Returns403()
    {
        using var secretary = await SignInHelper.SignInAsNileSecretaryAsync(factory);

        using var response = await secretary.Client.GetAsync(StaffUri);
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.permission_denied", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task ChangeRole_AffectedUsersNextRequest_Returns401()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var secretarySession = await SignInHelper.SignInAsNileSecretaryAsync(factory);
        using (var before = await secretarySession.Client.GetAsync(new Uri("/api/me", UriKind.Relative)))
        {
            Assert.Equal(HttpStatusCode.OK, before.StatusCode);
        }

        var (membershipId, version) = await FindMemberAsync(owner.Client, "secretary@nile.test");
        using var changed = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/staff/{membershipId}/role", UriKind.Relative), new { role = "teacher", version });
        Assert.Equal(HttpStatusCode.NoContent, changed.StatusCode);

        using var after = await secretarySession.Client.GetAsync(new Uri("/api/me", UriKind.Relative));
        Assert.Equal(HttpStatusCode.Unauthorized, after.StatusCode);
    }

    private static async Task<(Guid MembershipId, uint Version)> FindMemberAsync(HttpClient client, string email)
    {
        using var response = await client.GetAsync(StaffUri);
        var body = await ReadJsonAsync(response);
        var member = body.RootElement.GetProperty("items").EnumerateArray().Single(item => item.GetProperty("email").GetString() == email);
        return (member.GetProperty("membershipId").GetGuid(), member.GetProperty("version").GetUInt32());
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
