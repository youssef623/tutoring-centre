using System.Net;
using TutoringCentre.Api.Tests.Fixtures;
using static TutoringCentre.Api.Tests.Fixtures.AttendanceHttp;

namespace TutoringCentre.Api.Tests.Attendance;

[Collection(ApiCollection.Name)]
public sealed class CardEndpointTests(ApiFactory factory) : CardScanTestBase(factory)
{
    [Fact]
    public async Task ConcurrentIssue_One201One409_OneActiveCard()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        var scenario = await Arrange(owner.Client);
        Factory.AttendanceRace.Enable(cards: true);
        try
        {
            var uri = new Uri($"/api/students/{scenario.Students[0]}/card", UriKind.Relative);
            var responses = await Task.WhenAll(
                AntiforgeryTestHelper.PostAsync(owner.Client, uri, null), AntiforgeryTestHelper.PostAsync(owner.Client, uri, null));
            try
            {
                Assert.Single(responses, response => response.StatusCode == HttpStatusCode.Created);
                await Problem(Assert.Single(responses, response => response.StatusCode == HttpStatusCode.Conflict), HttpStatusCode.Conflict, "card.concurrent_issue");
                Assert.Equal(1L, await Factory.ScalarAsync<long>("select count(*) from attendance.student_cards where status = 'active'"));
            }
            finally { foreach (var response in responses) response.Dispose(); }
        }
        finally { Factory.AttendanceRace.Disable(); }
    }

    [Theory]
    [InlineData("bulk")]
    [InlineData("revoke")]
    public async Task UnknownBodyField_IsMalformed(string operation)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        using var response = operation == "bulk"
            ? await Post(owner.Client, $"/api/groups/{Guid.NewGuid()}/cards", new { studentIds = new[] { Guid.NewGuid() }, token = "foreign" })
            : await Post(owner.Client, $"/api/student-cards/{Guid.NewGuid()}/revoke", new { reason = "lost", version = 1, studentId = Guid.NewGuid() });
        await Problem(response, HttpStatusCode.BadRequest, "request.malformed");
    }

    [Fact]
    public async Task IssueStatusBulkReissueRevoke_ContractHeadersAndNoSecretsInLogs()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        var scenario = await Arrange(owner.Client);
        Assert.Equal(System.Text.Json.JsonValueKind.Null, (await Get(owner.Client, $"/api/students/{scenario.Students[0]}/card")).GetProperty("card").ValueKind);
        var issued = await Issue(owner.Client, scenario.Students[0]);
        Assert.StartsWith("TCM1.", issued.GetProperty("token").GetString());
        var status = (await Get(owner.Client, $"/api/students/{scenario.Students[0]}/card")).GetProperty("card");
        Assert.Equal("active", status.GetProperty("status").GetString());
        Assert.False(status.TryGetProperty("token", out _));
        using var bulk = await Post(owner.Client, $"/api/groups/{scenario.GroupId}/cards", new { studentIds = scenario.Students });
        Assert.Equal(HttpStatusCode.OK, bulk.StatusCode);
        Assert.True(bulk.Headers.CacheControl!.NoStore);
        Assert.Equal(3, (await Json(bulk)).GetProperty("cards").GetArrayLength());
        status = (await Get(owner.Client, $"/api/students/{scenario.Students[0]}/card")).GetProperty("card");
        using var revoke = await Post(owner.Client, $"/api/student-cards/{status.GetProperty("cardId").GetGuid()}/revoke", new { reason = "lost", version = status.GetProperty("version").GetUInt32() });
        Assert.Equal(HttpStatusCode.NoContent, revoke.StatusCode);
        Factory.Time.FixAt(InSession);
        using var scan = await Scan(owner.Client, scenario.SessionId, issued.GetProperty("token").GetString()!);
        await Problem(scan, HttpStatusCode.UnprocessableEntity, "card.revoked");
        var events = Factory.AttendanceLogs.Snapshot();
        Assert.NotEmpty(events);
        Assert.DoesNotContain(events, entry => entry.RenderMessage(System.Globalization.CultureInfo.InvariantCulture).Contains("TCM1.", StringComparison.Ordinal)
            || entry.Properties.Values.Any(value => value.ToString().Contains("TCM1.", StringComparison.Ordinal)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(61)]
    public async Task BulkLimit_FieldValidation(int count)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        using var response = await Post(owner.Client, $"/api/groups/{Guid.NewGuid()}/cards", new { studentIds = Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToArray() });
        await Problem(response, HttpStatusCode.BadRequest, "validation.failed");
        Assert.True((await Json(response)).GetProperty("errors").TryGetProperty("studentIds", out _));
    }

    [Theory]
    [InlineData("stale", HttpStatusCode.Conflict, "concurrency.stale")]
    [InlineData("reissued", HttpStatusCode.BadRequest, "validation.failed")]
    [InlineData("already", HttpStatusCode.UnprocessableEntity, "card.already_revoked")]
    public async Task RevokeFailures_Contract(string failure, HttpStatusCode expected, string code)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        var scenario = await Arrange(owner.Client);
        var issued = await Issue(owner.Client, scenario.Students[0]);
        var url = $"/api/student-cards/{issued.GetProperty("cardId").GetGuid()}/revoke";
        var status = (await Get(owner.Client, $"/api/students/{scenario.Students[0]}/card")).GetProperty("card");
        if (failure == "already")
        {
            using var first = await Post(owner.Client, url, new { reason = "lost", version = status.GetProperty("version").GetUInt32() });
            Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
            status = (await Get(owner.Client, $"/api/students/{scenario.Students[0]}/card")).GetProperty("card");
        }
        using var response = await Post(owner.Client, url, new { reason = failure == "reissued" ? "reissued" : "lost", version = status.GetProperty("version").GetUInt32() + (failure == "stale" ? 1 : 0) });
        await Problem(response, expected, code);
    }

    [Theory]
    [InlineData("student", "student.not_found")]
    [InlineData("group", "group.not_found")]
    [InlineData("card", "card.not_found")]
    public async Task UnknownReferences_NotFound(string reference, string code)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        using var response = reference switch
        {
            "student" => await AntiforgeryTestHelper.PostAsync(owner.Client, new($"/api/students/{Guid.NewGuid()}/card", UriKind.Relative), null),
            "group" => await Post(owner.Client, $"/api/groups/{Guid.NewGuid()}/cards", new { studentIds = new[] { Guid.NewGuid() } }),
            _ => await Post(owner.Client, $"/api/student-cards/{Guid.NewGuid()}/revoke", new { reason = "lost", version = 1 }),
        };
        await Problem(response, HttpStatusCode.NotFound, code);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ArchivedOrOutsider_RejectedWithoutCards(bool archived)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        var scenario = await Arrange(owner.Client);
        if (archived)
        {
            var parentId = (await Get(owner.Client, $"/api/students/{scenario.Students[0]}")).GetProperty("parents")[0].GetProperty("parentId").GetGuid();
            scenario.Students[0] = await Create(owner.Client, "/api/students", new { fullName = "Archive scan student", parent = new { id = parentId }, relationship = "mother" });
            var student = await Get(owner.Client, $"/api/students/{scenario.Students[0]}");
            using var archive = await Post(owner.Client, $"/api/students/{scenario.Students[0]}/archive", new { version = student.GetProperty("version").GetUInt32() });
            Assert.Equal(HttpStatusCode.NoContent, archive.StatusCode);
        }
        using var response = archived
            ? await AntiforgeryTestHelper.PostAsync(owner.Client, new($"/api/students/{scenario.Students[0]}/card", UriKind.Relative), null)
            : await Post(owner.Client, $"/api/groups/{scenario.GroupId}/cards", new { studentIds = new[] { Guid.NewGuid() } });
        await Problem(response, HttpStatusCode.UnprocessableEntity, archived ? "card.student_archived" : "card.student_not_in_group");
        Assert.Equal(0L, await Factory.ScalarAsync<long>("select count(*) from attendance.student_cards"));
    }
}

