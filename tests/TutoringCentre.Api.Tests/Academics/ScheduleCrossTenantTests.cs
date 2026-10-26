using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 53.4: tenant isolation for schedules at the HTTP boundary, every case in both directions. Each of the
/// three schedule resources (group, rule, session) is the foreign one in at least one case, attacked with its real
/// ID and real version, and each answers the 404 of that resource with the victim's schedule rows compared before
/// and after through the superuser connection. The timetable has no ID to probe, so it is checked by content: the
/// other centre's sessions never appear, filtered or not — not even for the one account that is staff in both
/// centres. The clock is fixed so the arranged sessions fall in a known week.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class ScheduleCrossTenantTests(ApiFactory factory) : IAsyncLifetime
{
    private const string WeekStart = "2027-04-24";

    private static readonly object SaturdayRule =
        new { dayOfWeek = "saturday", startTime = "18:00", durationMinutes = 90, room = (string?)null, effectiveFrom = "2027-04-20" };

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

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task ListRules_OfTheOtherCentresGroup_Returns404GroupNotFound(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var victimGroupId = await CreateScheduledGroupAsync(victim.Client, "Victim");

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Get, $"/api/groups/{victimGroupId}/schedule-rules", null,
            HttpStatusCode.NotFound, "group.not_found", () => SnapshotGroupScheduleAsync(victimGroupId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task AddRule_ToTheOtherCentresGroup_Returns404GroupNotFoundAndWritesNothing(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim");

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/groups/{victimGroupId}/schedule-rules", SaturdayRule,
            HttpStatusCode.NotFound, "group.not_found", () => SnapshotGroupScheduleAsync(victimGroupId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task EndRule_TheOtherCentresRuleWithItsRealVersion_Returns404RuleNotFoundAndLeavesItOpen(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var victimGroupId = await CreateScheduledGroupAsync(victim.Client, "Victim");
        var rule = await GetOnlyRuleAsync(victim.Client, victimGroupId);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/schedule-rules/{rule.GetProperty("id").GetGuid()}/end",
            new { lastDate = "2027-05-01", version = rule.GetProperty("version").GetUInt32() },
            HttpStatusCode.NotFound, "schedule.rule_not_found", () => SnapshotGroupScheduleAsync(victimGroupId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task CancelSession_TheOtherCentresSessionWithItsRealVersion_Returns404SessionNotFoundAndLeavesItScheduled(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var victimGroupId = await CreateScheduledGroupAsync(victim.Client, "Victim");
        var session = Assert.Single(await GetSessionsAsync(victim.Client, $"&groupId={victimGroupId}"));

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/sessions/{session.GetProperty("id").GetGuid()}/cancel",
            new { version = session.GetProperty("version").GetUInt32() },
            HttpStatusCode.NotFound, "schedule.session_not_found", () => SnapshotGroupScheduleAsync(victimGroupId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Timetable_NeverListsTheOtherCentresSessions_FilteredByItsGroupOrTeacherOrNot(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var attackerGroupId = await CreateScheduledGroupAsync(attacker.Client, "Attacker");
        var victimGroupId = await CreateScheduledGroupAsync(victim.Client, "Victim");
        var victimSession = Assert.Single(await GetSessionsAsync(victim.Client, $"&groupId={victimGroupId}"));
        var victimTeacherId = victimSession.GetProperty("teacherId").GetGuid();

        var unfiltered = await GetSessionsAsync(attacker.Client, string.Empty);
        var byVictimGroup = await GetSessionsAsync(attacker.Client, $"&groupId={victimGroupId}");
        var byVictimTeacher = await GetSessionsAsync(attacker.Client, $"&teacherId={victimTeacherId}");

        Assert.Equal(attackerGroupId, Assert.Single(unfiltered).GetProperty("groupId").GetGuid());
        Assert.Empty(byVictimGroup);
        Assert.Empty(byVictimTeacher);
    }

    // teacher@both.test is staff in both centres and, here, the linked teacher of one group in each. Whichever centre
    // the session is in, it lists only that centre's own session: the account is the same, the tenant is not.
    [Theory]
    [InlineData("nile-centre", "maadi-hub")]
    [InlineData("maadi-hub", "nile-centre")]
    public async Task TwoCentreTeacher_Timetable_ListsOnlyTheSelectedCentresOwnSession(string selectedSlug, string otherSlug)
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var maadiOwner = await SignInHelper.SignInAsMaadiOwnerAsync(factory);
        var groups = new Dictionary<string, Guid>
        {
            ["nile-centre"] = await CreateLinkedTeachersScheduledGroupAsync(nileOwner.Client, "Nile"),
            ["maadi-hub"] = await CreateLinkedTeachersScheduledGroupAsync(maadiOwner.Client, "Maadi"),
        };
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, selectedSlug);

        var sessions = await GetSessionsAsync(teacher.Client, string.Empty);
        var byOtherGroup = await GetSessionsAsync(teacher.Client, $"&groupId={groups[otherSlug]}");

        Assert.Equal(groups[selectedSlug], Assert.Single(sessions).GetProperty("groupId").GetGuid());
        Assert.Empty(byOtherGroup);
    }

    private static async Task<Guid> CreateScheduledGroupAsync(HttpClient client, string prefix)
    {
        var groupId = await CreateGroupAsync(client, prefix);
        await CreateAsync(client, $"/api/groups/{groupId}/schedule-rules", SaturdayRule);
        return groupId;
    }

    private static async Task<Guid> CreateGroupAsync(HttpClient client, string prefix, Guid? teacherId = null)
    {
        var subjectId = await CreateAsync(client, "/api/subjects", new { name = $"{prefix} Subject {Guid.NewGuid():N}" });
        teacherId ??= await CreateAsync(client, "/api/teachers", new { fullName = $"{prefix} Teacher", phone = (string?)null });
        return await CreateAsync(client, "/api/groups", new { name = $"{prefix} Group {Guid.NewGuid():N}", subjectId, teacherId, capacity = 20 });
    }

    /// <summary>
    /// A teacher record in the owner's centre linked to teacher@both.test (any record already linked to that user is
    /// unlinked first, so the group created here is the teacher's only one), teaching one group with a Saturday rule.
    /// </summary>
    private static async Task<Guid> CreateLinkedTeachersScheduledGroupAsync(HttpClient ownerClient, string prefix)
    {
        using var staffResponse = await ownerClient.GetAsync(new Uri("/api/staff", UriKind.Relative));
        var userId = (await ReadJsonAsync(staffResponse)).RootElement.GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("email").GetString() == "teacher@both.test").GetProperty("userId").GetGuid();

        using (var teachersResponse = await ownerClient.GetAsync(new Uri("/api/teachers?includeArchived=true", UriKind.Relative)))
        {
            foreach (var existing in (await ReadJsonAsync(teachersResponse)).RootElement.GetProperty("items").EnumerateArray())
            {
                if (existing.GetProperty("userId").ValueKind == JsonValueKind.String && existing.GetProperty("userId").GetGuid() == userId)
                {
                    await SetTeacherAccountAsync(ownerClient, existing.GetProperty("id").GetGuid(), null);
                }
            }
        }

        var teacherId = await CreateAsync(ownerClient, "/api/teachers", new { fullName = $"{prefix} Linked Teacher", phone = (string?)null });
        await SetTeacherAccountAsync(ownerClient, teacherId, userId);
        var groupId = await CreateGroupAsync(ownerClient, prefix, teacherId);
        await CreateAsync(ownerClient, $"/api/groups/{groupId}/schedule-rules", SaturdayRule);
        return groupId;
    }

    private static async Task SetTeacherAccountAsync(HttpClient ownerClient, Guid teacherId, Guid? userId)
    {
        using var getResponse = await ownerClient.GetAsync(new Uri($"/api/teachers/{teacherId}", UriKind.Relative));
        var version = (await ReadJsonAsync(getResponse)).RootElement.GetProperty("version").GetUInt32();
        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            ownerClient, HttpMethod.Put, new Uri($"/api/teachers/{teacherId}/account", UriKind.Relative), new { userId, version });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static async Task<JsonElement> GetOnlyRuleAsync(HttpClient client, Guid groupId)
    {
        using var response = await client.GetAsync(new Uri($"/api/groups/{groupId}/schedule-rules", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return Assert.Single((await ReadJsonAsync(response)).RootElement.Clone().GetProperty("items").EnumerateArray());
    }

    private static async Task<List<JsonElement>> GetSessionsAsync(HttpClient client, string filters)
    {
        using var response = await client.GetAsync(new Uri($"/api/timetable?weekStart={WeekStart}{filters}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.Clone().GetProperty("sessions").EnumerateArray().ToList();
    }

    private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)
    {
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri(url, UriKind.Relative), body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("id").GetGuid();
    }

    // The group row plus every rule and session of it, with end date, status and row version: any rule added or
    // ended, or session generated or cancelled, in the group changes this string.
    private Task<string> SnapshotGroupScheduleAsync(Guid groupId) =>
        factory.ScalarAsync<string>(
            $"""
            select g.status || '|' || g.xmin::text
                || '|' || (select count(*)::text || ':' || coalesce(string_agg(r.id::text || ':' || coalesce(r.effective_to::text, '-') || ':' || r.xmin::text, ',' order by r.id), '')
                           from academics.schedule_rules r where r.group_id = g.id)
                || '|' || (select count(*)::text || ':' || coalesce(string_agg(s.id::text || ':' || s.status || ':' || s.xmin::text, ',' order by s.id), '')
                           from academics.class_sessions s where s.group_id = g.id)
            from academics.groups g where g.id = '{groupId}'
            """);

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
