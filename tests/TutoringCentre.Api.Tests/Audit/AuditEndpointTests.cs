using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Audit;

/// <summary>Task 32.4: proves the GET /api/audit contract (Day 32) through the full HTTP pipeline.</summary>
[Collection(ApiCollection.Name)]
public sealed class AuditEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri AuditUri = new("/api/audit", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Get_AfterRenameAndRoleChange_ReturnsBothEntriesNewestFirstWithBeforeAndAfter()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, membershipId) = await RenameSubjectAndChangeRoleAsync(owner);

        using var response = await owner.Client.GetAsync(AuditUri);
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = body.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.True(items.Count >= 2);

        // Newest first: the role change happened after the rename.
        var roleChange = items[0];
        Assert.Equal("membership", roleChange.GetProperty("entityType").GetString());
        Assert.Equal(membershipId, roleChange.GetProperty("entityId").GetGuid());
        var roleChangeField = roleChange.GetProperty("changes").EnumerateArray().Single(c => c.GetProperty("field").GetString() == "role");
        Assert.Equal("secretary", roleChangeField.GetProperty("before").GetString());
        Assert.Equal("teacher", roleChangeField.GetProperty("after").GetString());

        var rename = items[1];
        Assert.Equal("subject", rename.GetProperty("entityType").GetString());
        Assert.Equal(subjectId, rename.GetProperty("entityId").GetGuid());
        var nameField = rename.GetProperty("changes").EnumerateArray().Single(c => c.GetProperty("field").GetString() == "name");
        Assert.Equal("History", nameField.GetProperty("before").GetString());
        Assert.Equal("World History", nameField.GetProperty("after").GetString());
    }

    [Fact]
    public async Task Get_FilteredByEntityIdOfSubject_ReturnsOnlyThatSubjectsHistory()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, _) = await RenameSubjectAndChangeRoleAsync(owner);

        using var response = await owner.Client.GetAsync(
            new Uri($"/api/audit?entityType=subject&entityId={subjectId}", UriKind.Relative));
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = body.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.All(items, item => Assert.Equal(subjectId, item.GetProperty("entityId").GetGuid()));
        Assert.All(items, item => Assert.Equal("subject", item.GetProperty("entityType").GetString()));
        Assert.Contains(items, item => item.GetProperty("action").GetString() == "updated");
    }

    [Fact]
    public async Task Get_PageSizeOne_YieldsCursorAndSecondPageReturnsTheNextEntry()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        await RenameSubjectAndChangeRoleAsync(owner);

        using var firstResponse = await owner.Client.GetAsync(new Uri("/api/audit?pageSize=1", UriKind.Relative));
        var firstBody = await ReadJsonAsync(firstResponse);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        var firstItems = firstBody.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.Single(firstItems);
        var cursor = firstBody.RootElement.GetProperty("nextCursor").GetString();
        Assert.False(string.IsNullOrEmpty(cursor));

        using var secondResponse = await owner.Client.GetAsync(new Uri($"/api/audit?pageSize=1&cursor={Uri.EscapeDataString(cursor!)}", UriKind.Relative));
        var secondBody = await ReadJsonAsync(secondResponse);
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        var secondItems = secondBody.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.Single(secondItems);
        Assert.NotEqual(firstItems[0].GetProperty("id").GetGuid(), secondItems[0].GetProperty("id").GetGuid());
    }

    [Theory]
    [InlineData("pageSize=0")]
    [InlineData("pageSize=101")]
    [InlineData("cursor=not-a-valid-cursor!!")]
    public async Task Get_WithInvalidQueryParameter_Returns400(string query)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await owner.Client.GetAsync(new Uri($"/api/audit?{query}", UriKind.Relative));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_AsMaadiOwner_SeesNoneOfNilesEntries_WithOrWithoutNilesEntityIdFilter()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (nileSubjectId, _) = await RenameSubjectAndChangeRoleAsync(nileOwner);

        using var maadiOwner = await SignInHelper.SignInAsMaadiOwnerAsync(factory);

        using var unfiltered = await maadiOwner.Client.GetAsync(AuditUri);
        var unfilteredBody = await ReadJsonAsync(unfiltered);
        Assert.Equal(HttpStatusCode.OK, unfiltered.StatusCode);
        Assert.All(
            unfilteredBody.RootElement.GetProperty("items").EnumerateArray(),
            item => Assert.NotEqual(nileSubjectId, item.GetProperty("entityId").GetGuid()));

        using var filtered = await maadiOwner.Client.GetAsync(
            new Uri($"/api/audit?entityType=subject&entityId={nileSubjectId}", UriKind.Relative));
        var filteredBody = await ReadJsonAsync(filtered);
        Assert.Equal(HttpStatusCode.OK, filtered.StatusCode);
        Assert.Empty(filteredBody.RootElement.GetProperty("items").EnumerateArray());
    }

    [Theory]
    [InlineData("secretary@nile.test")]
    [InlineData("teacher")]
    public async Task Get_AsSecretaryOrTeacher_Returns403(string role)
    {
        using var signedIn = role == "teacher"
            ? await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre")
            : await SignInHelper.SignInAsync(factory, role);

        using var response = await signedIn.Client.GetAsync(AuditUri);
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.permission_denied", body.RootElement.GetProperty("code").GetString());
    }

    [Theory]
    [MemberData(nameof(UnsupportedMethods))]
    public async Task UnsupportedMethod_Returns404Or405(HttpMethod method)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var request = new HttpRequestMessage(method, AuditUri);
        using var response = await owner.Client.SendAsync(request);

        Assert.True(
            response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.MethodNotAllowed,
            $"Expected 404 or 405, got {response.StatusCode}.");
    }

    public static IEnumerable<object[]> UnsupportedMethods()
    {
        yield return [HttpMethod.Post];
        yield return [HttpMethod.Put];
        yield return [HttpMethod.Delete];
    }

    [Fact]
    public async Task Get_ResponseContainsNoEmailOrPasswordField()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        await RenameSubjectAndChangeRoleAsync(owner);

        using var response = await owner.Client.GetAsync(AuditUri);
        var text = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("email", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("password", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("owner@nile.test", text, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Creates a subject named "History", renames it to "World History", then changes secretary@nile.test's role from secretary to teacher. Returns both changed records' ids.</summary>
    private static async Task<(Guid SubjectId, Guid MembershipId)> RenameSubjectAndChangeRoleAsync(SignedInClient owner)
    {
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/subjects", UriKind.Relative), new { name = "History" });
        var subjectId = (await ReadJsonAsync(createResponse)).RootElement.GetProperty("id").GetGuid();

        using var getResponse = await owner.Client.GetAsync(new Uri($"/api/subjects/{subjectId}", UriKind.Relative));
        var subjectVersion = (await ReadJsonAsync(getResponse)).RootElement.GetProperty("version").GetUInt32();

        using var renameResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/subjects/{subjectId}", UriKind.Relative),
            new { name = "World History", version = subjectVersion });
        Assert.Equal(HttpStatusCode.NoContent, renameResponse.StatusCode);

        using var staffListResponse = await owner.Client.GetAsync(new Uri("/api/staff", UriKind.Relative));
        var staffList = await ReadJsonAsync(staffListResponse);
        var secretary = staffList.RootElement.GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("email").GetString() == "secretary@nile.test");
        var membershipId = secretary.GetProperty("membershipId").GetGuid();
        var membershipVersion = secretary.GetProperty("version").GetUInt32();

        using var roleResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/staff/{membershipId}/role", UriKind.Relative),
            new { role = "teacher", version = membershipVersion });
        Assert.Equal(HttpStatusCode.NoContent, roleResponse.StatusCode);

        return (subjectId, membershipId);
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
