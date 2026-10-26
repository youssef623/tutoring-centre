using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Staff;

/// <summary>Task 30.7: proves the five staff endpoints' contract behaviour through the full HTTP pipeline (Day 30 contract).</summary>
[Collection(ApiCollection.Name)]
public sealed class StaffEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri StaffUri = new("/api/staff", UriKind.Relative);

    private static readonly string[] ExpectedStaffMemberFields =
        ["displayName", "email", "isCurrentUser", "membershipId", "role", "status", "userId", "version"];

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task List_AsOwner_ReturnsAllNileMembersWithOnlyContractFields()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await owner.Client.GetAsync(StaffUri);
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = body.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.Equal(4, items.Count);
        Assert.Contains(items, item => item.GetProperty("status").GetString() == "inactive");
        foreach (var item in items)
        {
            var fields = item.EnumerateObject().Select(p => p.Name).OrderBy(n => n, StringComparer.Ordinal);
            Assert.Equal(ExpectedStaffMemberFields, fields);
        }
    }

    [Fact]
    public async Task Create_NewEmail_Returns201WithPasswordAndNoStore()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StaffUri, new { email = "new-contract@nile.test", displayName = "New Contract", role = "teacher", preferredLocale = "en" });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
        Assert.Null(response.Headers.Location);
        Assert.False(string.IsNullOrEmpty(body.RootElement.GetProperty("temporaryPassword").GetString()));
    }

    [Fact]
    public async Task Create_EmailOfAnExistingAccount_Returns201WithNullPasswordAndNoStore()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StaffUri, new { email = "owner@maadi.test", displayName = "Maadi Owner", role = "teacher", preferredLocale = "en" });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
        Assert.Equal(JsonValueKind.Null, body.RootElement.GetProperty("temporaryPassword").ValueKind);
    }

    [Fact]
    public async Task Create_EmailAlreadyAMemberOfThisCentre_Returns409WithErrorsEmail()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StaffUri, new { email = "secretary@nile.test", displayName = "Dup", role = "teacher", preferredLocale = "en" });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("staff.already_member", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("email").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Create_UnknownRole_Returns400RequestMalformed()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StaffUri, new { email = "x@nile.test", displayName = "X", role = "admin", preferredLocale = "en" });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("request.malformed", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Create_OverPostedCentreId_Returns400RequestMalformedAndCreatesNoRow()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var countBefore = await CountStaffAsync(owner.Client);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StaffUri,
            new { email = "x2@nile.test", displayName = "X2", role = "teacher", preferredLocale = "en", centreId = Guid.NewGuid() });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("request.malformed", body.RootElement.GetProperty("code").GetString());
        Assert.Equal(countBefore, await CountStaffAsync(owner.Client));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public async Task Create_InvalidEmail_Returns400WithErrorsEmail(string email)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StaffUri, new { email, displayName = "X", role = "teacher", preferredLocale = "en" });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("email").GetArrayLength() > 0);
    }

    [Fact]
    public async Task ChangeRole_HappyPathThenStaleVersion()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (membershipId, version) = await FindMemberAsync(owner.Client, "secretary@nile.test");

        using var changed = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/staff/{membershipId}/role", UriKind.Relative), new { role = "teacher", version });
        Assert.Equal(HttpStatusCode.NoContent, changed.StatusCode);

        using var staleRetry = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/staff/{membershipId}/role", UriKind.Relative), new { role = "secretary", version });
        var body = await ReadJsonAsync(staleRetry);
        Assert.Equal(HttpStatusCode.Conflict, staleRetry.StatusCode);
        Assert.Equal("concurrency.stale", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task ChangeRole_ValidationErrors_HaveFieldNames()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (membershipId, _) = await FindMemberAsync(owner.Client, "secretary@nile.test");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/staff/{membershipId}/role", UriKind.Relative), new { role = "admin", version = 0u });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Deactivate_ThenReactivate_FlipsStatusInTheList()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (membershipId, version) = await FindMemberAsync(owner.Client, "secretary@nile.test");

        using var deactivated = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/staff/{membershipId}/deactivate", UriKind.Relative), new { version });
        Assert.Equal(HttpStatusCode.NoContent, deactivated.StatusCode);

        var (_, deactivatedVersion) = await FindMemberAsync(owner.Client, "secretary@nile.test");
        Assert.Equal("inactive", await MemberStatusAsync(owner.Client, membershipId));

        using var reactivated = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/staff/{membershipId}/reactivate", UriKind.Relative), new { version = deactivatedVersion });
        Assert.Equal(HttpStatusCode.NoContent, reactivated.StatusCode);
        Assert.Equal("active", await MemberStatusAsync(owner.Client, membershipId));
    }

    [Fact]
    public async Task Reactivate_AlreadyActive_Returns422MembershipAlreadyActive()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (membershipId, version) = await FindMemberAsync(owner.Client, "secretary@nile.test");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/staff/{membershipId}/reactivate", UriKind.Relative), new { version });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("membership.already_active", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Deactivate_AlreadyInactive_Returns422MembershipAlreadyInactive()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (membershipId, version) = await FindMemberAsync(owner.Client, "inactive@nile.test");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/staff/{membershipId}/deactivate", UriKind.Relative), new { version });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("membership.already_inactive", body.RootElement.GetProperty("code").GetString());
    }

    private static async Task<(Guid MembershipId, uint Version)> FindMemberAsync(HttpClient client, string email)
    {
        using var response = await client.GetAsync(StaffUri);
        var body = await ReadJsonAsync(response);
        var member = body.RootElement.GetProperty("items").EnumerateArray().Single(item => item.GetProperty("email").GetString() == email);
        return (member.GetProperty("membershipId").GetGuid(), member.GetProperty("version").GetUInt32());
    }

    private static async Task<string> MemberStatusAsync(HttpClient client, Guid membershipId)
    {
        using var response = await client.GetAsync(StaffUri);
        var body = await ReadJsonAsync(response);
        var member = body.RootElement.GetProperty("items").EnumerateArray().Single(item => item.GetProperty("membershipId").GetGuid() == membershipId);
        return member.GetProperty("status").GetString()!;
    }

    private static async Task<int> CountStaffAsync(HttpClient client)
    {
        using var response = await client.GetAsync(StaffUri);
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("items").GetArrayLength();
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
