using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 53.4: the ownership guard on schedules, as black-box behaviour through teacher@both.test's real session. A
/// group's rules inherit the group's rule: 200 for the own group, 404 (never 403) for another teacher's. The
/// timetable lists only the teacher's own sessions, and its filters only narrow that scope. Writes are 403 even on
/// the own group, because permission is evaluated before ownership, and they change nothing. The clock is fixed so
/// the arranged sessions fall in a known week.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class ScheduleTeacherScopeTests(ApiFactory factory) : IAsyncLifetime
{
    private const string TwoCentreTeacherEmail = "teacher@both.test";

    private const string WeekStart = "2027-04-24";

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
        factory.Time.FixAt(new DateTimeOffset(2027, 4, 20, 8, 0, 0, TimeSpan.Zero));
    }

    public Task DisposeAsync()
    {
        factory.Time.UseRealTime();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task LinkedTeacher_RulesOfTheOwnGroup_Returns200WithTheRule()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await teacher.Client.GetAsync(new Uri($"/api/groups/{arranged.OwnGroupId}/schedule-rules", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rule = Assert.Single((await ReadJsonAsync(response)).RootElement.GetProperty("items").EnumerateArray());
        Assert.Equal(arranged.OwnRuleId, rule.GetProperty("id").GetGuid());
    }

    [Fact]
    public async Task LinkedTeacher_RulesOfAnotherTeachersGroup_Returns404GroupNotFound()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await teacher.Client.GetAsync(new Uri($"/api/groups/{arranged.OtherGroupId}/schedule-rules", UriKind.Relative));

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "group.not_found");
    }

    [Fact]
    public async Task LinkedTeacher_Timetable_ListsOnlyTheOwnSessions()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        var teacherSessions = await GetSessionsAsync(teacher.Client, string.Empty);
        var ownerSessions = await GetSessionsAsync(owner.Client, string.Empty);

        var only = Assert.Single(teacherSessions);
        Assert.Equal(arranged.OwnGroupId, only.GetProperty("groupId").GetGuid());
        Assert.Equal(arranged.LinkedTeacherId, only.GetProperty("teacherId").GetGuid());

        // The same route as the owner shows both, so the teacher's single session is the scope, not missing data.
        Assert.Equal(
            new[] { arranged.OwnGroupId, arranged.OtherGroupId }.Order(),
            ownerSessions.Select(session => session.GetProperty("groupId").GetGuid()).Order());
    }

    [Fact]
    public async Task LinkedTeacher_TimetableFilteredToAnotherTeacherOrTheirGroup_ReturnsNoSessions()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        var byTeacher = await GetSessionsAsync(teacher.Client, $"&teacherId={arranged.OtherTeacherId}");
        var byGroup = await GetSessionsAsync(teacher.Client, $"&groupId={arranged.OtherGroupId}");
        var byOwnGroup = await GetSessionsAsync(teacher.Client, $"&groupId={arranged.OwnGroupId}");

        Assert.Empty(byTeacher);
        Assert.Empty(byGroup);
        Assert.Single(byOwnGroup);
    }

    [Theory]
    [InlineData("add")]
    [InlineData("end")]
    [InlineData("extend")]
    [InlineData("cancel")]
    public async Task LinkedTeacher_EveryWriteRoute_Returns403PermissionDeniedAndChangesNothing(string route)
    {
        var arranged = await ArrangeAsync();
        var (ruleVersion, sessionId, sessionVersion) = await GetOwnVersionsAsOwnerAsync(arranged);
        var before = await SnapshotScheduleAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");
        var (url, body) = route switch
        {
            "add" => ($"/api/groups/{arranged.OwnGroupId}/schedule-rules",
                (object)new { dayOfWeek = "sunday", startTime = "10:00", durationMinutes = 60, room = (string?)null, effectiveFrom = "2027-04-20" }),
            "end" => ($"/api/schedule-rules/{arranged.OwnRuleId}/end", new { lastDate = "2027-05-01", version = ruleVersion }),
            "extend" => ("/api/schedule/extend", new { through = "2027-06-15" }),
            _ => ($"/api/sessions/{sessionId}/cancel", new { version = sessionVersion }),
        };

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(teacher.Client, new Uri(url, UriKind.Relative), body);

        await AssertProblemAsync(response, HttpStatusCode.Forbidden, "auth.permission_denied");
        Assert.Equal(before, await SnapshotScheduleAsync());
    }

    /// <summary>
    /// As the Nile owner, over HTTP: one subject, two teachers, the first linked to teacher@both.test; one group
    /// each, each with a Saturday rule, so each has one session in <see cref="WeekStart"/>'s week. Any teacher
    /// record already linked to that user in Nile (the demo seed links one) is unlinked first, so "own" means
    /// exactly the group arranged here.
    /// </summary>
    private async Task<Arrangement> ArrangeAsync()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var teacherUserId = await FindUserIdAsync(owner.Client, TwoCentreTeacherEmail);

        using (var teachersResponse = await owner.Client.GetAsync(new Uri("/api/teachers?includeArchived=true", UriKind.Relative)))
        {
            var teachers = await ReadJsonAsync(teachersResponse);
            foreach (var existing in teachers.RootElement.GetProperty("items").EnumerateArray())
            {
                if (existing.GetProperty("userId").ValueKind == JsonValueKind.String && existing.GetProperty("userId").GetGuid() == teacherUserId)
                {
                    await SetTeacherAccountAsync(owner.Client, existing.GetProperty("id").GetGuid(), null);
                }
            }
        }

        var subjectId = await CreateAsync(owner.Client, "/api/subjects", new { name = "Scope Schedule Physics" });
        var linkedTeacherId = await CreateAsync(owner.Client, "/api/teachers", new { fullName = "Scope Linked Teacher", phone = (string?)null });
        var otherTeacherId = await CreateAsync(owner.Client, "/api/teachers", new { fullName = "Scope Other Teacher", phone = (string?)null });
        await SetTeacherAccountAsync(owner.Client, linkedTeacherId, teacherUserId);
        var ownGroupId = await CreateAsync(
            owner.Client, "/api/groups", new { name = "Scope Own Group", subjectId, teacherId = linkedTeacherId, capacity = 20 });
        var otherGroupId = await CreateAsync(
            owner.Client, "/api/groups", new { name = "Scope Other Group", subjectId, teacherId = otherTeacherId, capacity = 20 });
        var ownRuleId = await CreateAsync(
            owner.Client, $"/api/groups/{ownGroupId}/schedule-rules",
            new { dayOfWeek = "saturday", startTime = "18:00", durationMinutes = 90, room = (string?)null, effectiveFrom = "2027-04-20" });
        await CreateAsync(
            owner.Client, $"/api/groups/{otherGroupId}/schedule-rules",
            new { dayOfWeek = "saturday", startTime = "18:00", durationMinutes = 90, room = (string?)null, effectiveFrom = "2027-04-20" });

        return new Arrangement(ownGroupId, otherGroupId, linkedTeacherId, otherTeacherId, ownRuleId);
    }

    private async Task<(uint RuleVersion, Guid SessionId, uint SessionVersion)> GetOwnVersionsAsOwnerAsync(Arrangement arranged)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var rulesResponse = await owner.Client.GetAsync(new Uri($"/api/groups/{arranged.OwnGroupId}/schedule-rules", UriKind.Relative));
        var ruleVersion = Assert.Single((await ReadJsonAsync(rulesResponse)).RootElement.GetProperty("items").EnumerateArray()).GetProperty("version").GetUInt32();
        var session = Assert.Single(await GetSessionsAsync(owner.Client, $"&groupId={arranged.OwnGroupId}"));
        return (ruleVersion, session.GetProperty("id").GetGuid(), session.GetProperty("version").GetUInt32());
    }

    private static async Task<List<JsonElement>> GetSessionsAsync(HttpClient client, string filters)
    {
        using var response = await client.GetAsync(new Uri($"/api/timetable?weekStart={WeekStart}{filters}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.Clone().GetProperty("sessions").EnumerateArray().ToList();
    }

    private static async Task<Guid> FindUserIdAsync(HttpClient ownerClient, string email)
    {
        using var response = await ownerClient.GetAsync(new Uri("/api/staff", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("email").GetString() == email).GetProperty("userId").GetGuid();
    }

    private static async Task SetTeacherAccountAsync(HttpClient ownerClient, Guid teacherId, Guid? userId)
    {
        using var getResponse = await ownerClient.GetAsync(new Uri($"/api/teachers/{teacherId}", UriKind.Relative));
        var version = (await ReadJsonAsync(getResponse)).RootElement.GetProperty("version").GetUInt32();
        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            ownerClient, HttpMethod.Put, new Uri($"/api/teachers/{teacherId}/account", UriKind.Relative), new { userId, version });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)
    {
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri(url, UriKind.Relative), body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("id").GetGuid();
    }

    // Every rule and session row with its end date, status and row version: any write changes this string.
    private Task<string> SnapshotScheduleAsync() =>
        factory.ScalarAsync<string>(
            """
            select (select count(*)::text || ':' || coalesce(string_agg(id::text || ':' || coalesce(effective_to::text, '-') || ':' || xmin::text, ',' order by id), '')
                    from academics.schedule_rules)
                || '|' ||
                   (select count(*)::text || ':' || coalesce(string_agg(id::text || ':' || status || ':' || xmin::text, ',' order by id), '')
                    from academics.class_sessions)
            """);

    private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus, string expectedCode)
    {
        var body = await ReadJsonAsync(response);
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal(expectedCode, body.RootElement.GetProperty("code").GetString());
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }

    private sealed record Arrangement(Guid OwnGroupId, Guid OtherGroupId, Guid LinkedTeacherId, Guid OtherTeacherId, Guid OwnRuleId);
}
