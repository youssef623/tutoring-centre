using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TutoringCentre.Api.Auth;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 53.4: the six schedule endpoints' contract through the full HTTP pipeline — status, code, body shape and
/// formats, never messages. The application clock is fixed at <see cref="Now"/>, a Tuesday ten days before Egypt
/// moves its clocks forward (Friday 30 April 2027), so "today", the 28-day horizon, "in the past" and the
/// session instants are the same on every run and every machine. Every test builds its own subject, teacher and
/// groups over HTTP, so seeded data never decides a result.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class ScheduleEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    // Tuesday 20 April 2027, 10:00 in Cairo (UTC+2 until the change). Today in the centre is 2027-04-20, and the
    // 28-day horizon runs through 2027-05-17: Saturdays 24 April and 1, 8 and 15 May.
    private static readonly DateTimeOffset Now = new(2027, 4, 20, 8, 0, 0, TimeSpan.Zero);

    private const string Today = "2027-04-20";

    private static readonly string[] RuleFields =
        ["dayOfWeek", "durationMinutes", "effectiveFrom", "effectiveTo", "id", "room", "startTime", "version"];

    private static readonly string[] SessionFields =
        ["endsAt", "groupId", "groupName", "id", "localDate", "room", "startsAt", "status", "subjectName", "teacherId", "teacherName", "version"];

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
        factory.Time.FixAt(Now);
    }

    public Task DisposeAsync()
    {
        factory.Time.UseRealTime();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task AddRule_Returns201WithIdAndSessionsCreated_AndLocationAtTheGroupsRules()
    {
        // Only the owner may create teachers, so the owner arranges the group; the secretary schedules it.
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var secretary = await SignInHelper.SignInAsNileSecretaryAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Add Happy");

        using var response = await PostAsync(secretary.Client, $"/api/groups/{groupId}/schedule-rules", Rule());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/api/groups/{groupId}/schedule-rules", response.Headers.Location?.OriginalString);
        var body = (await ReadJsonAsync(response)).RootElement;
        Assert.Equal(2, body.EnumerateObject().Count());
        Assert.NotEqual(Guid.Empty, body.GetProperty("id").GetGuid());
        Assert.Equal(4, body.GetProperty("sessionsCreated").GetInt32());
        Assert.Equal(4, await CountSessionsAsync(groupId));
    }

    [Fact]
    public async Task ListRules_ReturnsTheContractShape_WithStartTimeAsHourMinuteAndLowerCaseWeekday()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "List Shape");
        var ruleId = await AddRuleAsync(owner.Client, groupId, Rule(startTime: "18:05", room: "Lab 2"));

        using var response = await owner.Client.GetAsync(new Uri($"/api/groups/{groupId}/schedule-rules", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = (await ReadJsonAsync(response)).RootElement;
        Assert.Single(body.EnumerateObject());
        var rule = Assert.Single(body.GetProperty("items").EnumerateArray());
        Assert.Equal(RuleFields, rule.EnumerateObject().Select(property => property.Name).Order(StringComparer.Ordinal));
        Assert.Equal(ruleId, rule.GetProperty("id").GetGuid());
        Assert.Equal("saturday", rule.GetProperty("dayOfWeek").GetString());
        Assert.Equal("18:05", rule.GetProperty("startTime").GetString());
        Assert.Equal(90, rule.GetProperty("durationMinutes").GetInt32());
        Assert.Equal("Lab 2", rule.GetProperty("room").GetString());
        Assert.Equal(Today, rule.GetProperty("effectiveFrom").GetString());
        Assert.Equal(JsonValueKind.Null, rule.GetProperty("effectiveTo").ValueKind);
        Assert.True(rule.GetProperty("version").GetUInt32() > 0);
    }

    [Fact]
    public async Task ListRules_UnknownGroup_Returns404GroupNotFound_AndAGroupWithNoneReturnsEmptyItems()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "List Empty");

        using var unknown = await owner.Client.GetAsync(new Uri($"/api/groups/{Guid.NewGuid()}/schedule-rules", UriKind.Relative));
        using var empty = await owner.Client.GetAsync(new Uri($"/api/groups/{groupId}/schedule-rules", UriKind.Relative));

        await AssertProblemAsync(unknown, HttpStatusCode.NotFound, "group.not_found");
        Assert.Equal(HttpStatusCode.OK, empty.StatusCode);
        Assert.Equal(0, (await ReadJsonAsync(empty)).RootElement.GetProperty("items").GetArrayLength());
    }

    [Theory]
    [InlineData("teacherId")]
    [InlineData("centreId")]
    public async Task AddRule_WithATeacherOrCentreInTheBody_Returns400RequestMalformedAndWritesNoRow(string extraField)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, teacherId, _) = await CreateGroupAsync(owner.Client, "Add Over Posted");
        var body = new Dictionary<string, object?>(Rule()) { [extraField] = teacherId };

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/schedule-rules", body);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "request.malformed");
        Assert.Equal(0, await CountRulesAsync(groupId));
    }

    // The contract has local values only: a time with seconds or an offset, or a UTC instant for either the time or
    // the date, is not a shape the API reads.
    [Theory]
    [InlineData("startTime", "18:00:00")]
    [InlineData("startTime", "18:00+02:00")]
    [InlineData("startTime", "2027-04-24T16:00:00Z")]
    [InlineData("effectiveFrom", "2027-04-20T00:00:00Z")]
    [InlineData("effectiveFrom", "20/04/2027")]
    [InlineData("dayOfWeek", "Sat")]
    public async Task AddRule_NonLocalOrMisshapenValue_Returns400RequestMalformedAndWritesNoRow(string field, string value)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Add Shape");
        var body = new Dictionary<string, object?>(Rule()) { [field] = value };

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/schedule-rules", body);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "request.malformed");
        Assert.Equal(0, await CountRulesAsync(groupId));
    }

    [Fact]
    public async Task AddRule_MissingRequiredFieldOrMalformedJson_Returns400RequestMalformed()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Add Missing");

        using var missing = await PostRawAsync(
            owner.Client, $"/api/groups/{groupId}/schedule-rules", """{ "startTime": "18:00", "durationMinutes": 90, "effectiveFrom": "2027-04-20" }""");
        using var malformed = await PostRawAsync(owner.Client, $"/api/groups/{groupId}/schedule-rules", "{ \"dayOfWeek\": ");

        await AssertProblemAsync(missing, HttpStatusCode.BadRequest, "request.malformed");
        await AssertProblemAsync(malformed, HttpStatusCode.BadRequest, "request.malformed");
    }

    [Fact]
    public async Task AddRule_StartTimeOffTheFiveMinuteGrid_Returns400StartTimeInvalidOnErrorsStartTime()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Add Grid");

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/schedule-rules", Rule(startTime: "18:02"));

        var body = await AssertProblemAsync(response, HttpStatusCode.BadRequest, "schedule.start_time_invalid");
        Assert.True(body.GetProperty("errors").GetProperty("startTime").GetArrayLength() > 0);
        Assert.Equal(0, await CountRulesAsync(groupId));
    }

    [Fact]
    public async Task AddRule_DurationOffTheFiveMinuteStep_Returns400DurationInvalidOnErrorsDurationMinutes()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Add Step");

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/schedule-rules", Rule(durationMinutes: 92));

        var body = await AssertProblemAsync(response, HttpStatusCode.BadRequest, "schedule.duration_invalid");
        Assert.True(body.GetProperty("errors").GetProperty("durationMinutes").GetArrayLength() > 0);
    }

    // Out of range reaches the entity (Day 54: the validator no longer repeats its rules), so the answer carries the
    // entity's own code on the field the client maps to its input.
    [Theory]
    [InlineData(20)]
    [InlineData(245)]
    public async Task AddRule_DurationOutOfRange_Returns400OnErrorsDurationMinutes(int durationMinutes)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Add Range");

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/schedule-rules", Rule(durationMinutes: durationMinutes));

        var body = await AssertProblemAsync(response, HttpStatusCode.BadRequest, "schedule.duration_invalid");
        Assert.True(body.GetProperty("errors").GetProperty("durationMinutes").GetArrayLength() > 0);
        Assert.Equal(0, await CountRulesAsync(groupId));
    }

    [Fact]
    public async Task AddRule_RoomOverFortyCharacters_Returns400OnErrorsRoom()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Add Room");

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/schedule-rules", Rule(room: new string('r', 41)));

        var body = await AssertProblemAsync(response, HttpStatusCode.BadRequest, "schedule.room_invalid");
        Assert.True(body.GetProperty("errors").GetProperty("room").GetArrayLength() > 0);
        Assert.Equal(0, await CountRulesAsync(groupId));
    }

    [Fact]
    public async Task AddRule_UnknownGroup_Returns404GroupNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await PostAsync(owner.Client, $"/api/groups/{Guid.NewGuid()}/schedule-rules", Rule());

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "group.not_found");
    }

    [Fact]
    public async Task AddRule_OverlappingTheSameTeachersOtherGroup_Returns409TeacherDoubleBookedAndCreatesNothing()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (firstGroupId, teacherId, subjectId) = await CreateGroupAsync(owner.Client, "Teacher Busy");
        var secondGroupId = await CreateGroupAsync(owner.Client, "Teacher Busy Second", subjectId, teacherId);
        await AddRuleAsync(owner.Client, firstGroupId, Rule(startTime: "18:00", durationMinutes: 90));

        // 19:00 starts inside the first group's 18:00-19:30 session.
        using var response = await PostAsync(owner.Client, $"/api/groups/{secondGroupId}/schedule-rules", Rule(startTime: "19:00", durationMinutes: 60));

        await AssertProblemAsync(response, HttpStatusCode.Conflict, "schedule.teacher_double_booked");
        Assert.Equal(0, await CountRulesAsync(secondGroupId));
        Assert.Equal(0, await CountSessionsAsync(secondGroupId));
        Assert.Equal(4, await CountSessionsAsync(firstGroupId));
    }

    [Fact]
    public async Task AddRule_OverlappingTheGroupsOwnRule_Returns409GroupDoubleBookedAndCreatesNothing()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Group Busy");
        await AddRuleAsync(owner.Client, groupId, Rule());

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/schedule-rules", Rule(startTime: "18:30"));

        await AssertProblemAsync(response, HttpStatusCode.Conflict, "schedule.group_double_booked");
        Assert.Equal(1, await CountRulesAsync(groupId));
        Assert.Equal(4, await CountSessionsAsync(groupId));
    }

    [Fact]
    public async Task AddRule_ToAnArchivedGroup_Returns422GroupArchived()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Add Archived");
        await ArchiveGroupAsync(owner.Client, groupId);

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/schedule-rules", Rule());

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "group.archived");
        Assert.Equal(0, await CountRulesAsync(groupId));
    }

    [Fact]
    public async Task AddRule_TakingEffectMoreThanAYearAhead_Returns422RangeInvalidOnErrorsEffectiveFrom()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Add Far");

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/schedule-rules", Rule(effectiveFrom: "2028-04-22"));

        var body = await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "schedule.range_invalid");
        Assert.True(body.GetProperty("errors").GetProperty("effectiveFrom").GetArrayLength() > 0);
    }

    [Fact]
    public async Task AddRule_WithoutTheAntiforgeryHeader_Returns403CsrfInvalidAndWritesNoRow()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Add No Token");

        using var response = await owner.Client.PostAsJsonAsync(new Uri($"/api/groups/{groupId}/schedule-rules", UriKind.Relative), Rule());

        await AssertProblemAsync(response, HttpStatusCode.Forbidden, "auth.csrf_invalid");
        Assert.Equal(0, await CountRulesAsync(groupId));
    }

    [Fact]
    public async Task ScheduleRoutes_NoCookie_Return401()
    {
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
        });

        using var rules = await client.GetAsync(new Uri($"/api/groups/{Guid.NewGuid()}/schedule-rules", UriKind.Relative));
        using var timetable = await client.GetAsync(new Uri($"/api/timetable?weekStart={Today}", UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, rules.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, timetable.StatusCode);
    }

    [Fact]
    public async Task ScheduleWrite_BeforeSelectingACentre_Returns403TenantNotSelected()
    {
        using var teacher = await SignInHelper.SignInAsync(factory, "teacher@both.test");
        Assert.Null(teacher.CentreId);

        using var response = await PostAsync(teacher.Client, "/api/schedule/extend", new { through = Today });

        await AssertProblemAsync(response, HttpStatusCode.Forbidden, "tenant.not_selected");
    }

    [Fact]
    public async Task EndRule_Returns204_CancelsLaterSessionsOnly_ThenAgainReturns422AlreadyEnded()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "End Happy");
        var ruleId = await AddRuleAsync(owner.Client, groupId, Rule());
        var version = (await GetRuleAsync(owner.Client, groupId, ruleId)).GetProperty("version").GetUInt32();

        using var first = await PostAsync(owner.Client, $"/api/schedule-rules/{ruleId}/end", new { lastDate = "2027-05-01", version });

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        var ended = await GetRuleAsync(owner.Client, groupId, ruleId);
        Assert.Equal("2027-05-01", ended.GetProperty("effectiveTo").GetString());
        // 24 April and 1 May stay; 8 and 15 May are cancelled, never deleted.
        Assert.Equal("2027-04-24:scheduled,2027-05-01:scheduled,2027-05-08:cancelled,2027-05-15:cancelled", await SnapshotSessionStatusesAsync(groupId));

        using var second = await PostAsync(
            owner.Client, $"/api/schedule-rules/{ruleId}/end", new { lastDate = "2027-05-08", version = ended.GetProperty("version").GetUInt32() });

        await AssertProblemAsync(second, HttpStatusCode.UnprocessableEntity, "schedule.rule_already_ended");
    }

    [Fact]
    public async Task EndRule_BeforeItTakesEffect_Returns422RuleEndInvalidAndChangesNothing()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "End Early");
        var ruleId = await AddRuleAsync(owner.Client, groupId, Rule());
        var version = (await GetRuleAsync(owner.Client, groupId, ruleId)).GetProperty("version").GetUInt32();
        var before = await SnapshotSessionStatusesAsync(groupId);

        using var response = await PostAsync(owner.Client, $"/api/schedule-rules/{ruleId}/end", new { lastDate = "2027-04-19", version });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "schedule.rule_end_invalid");
        Assert.Equal(before, await SnapshotSessionStatusesAsync(groupId));
    }

    [Fact]
    public async Task EndRule_StaleVersion_Returns409ConcurrencyStale_AndUnknownRule_Returns404RuleNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "End Stale");
        var ruleId = await AddRuleAsync(owner.Client, groupId, Rule());
        var version = (await GetRuleAsync(owner.Client, groupId, ruleId)).GetProperty("version").GetUInt32();

        using var stale = await PostAsync(owner.Client, $"/api/schedule-rules/{ruleId}/end", new { lastDate = "2027-05-01", version = version + 1 });
        using var unknown = await PostAsync(owner.Client, $"/api/schedule-rules/{Guid.NewGuid()}/end", new { lastDate = "2027-05-01", version });

        await AssertProblemAsync(stale, HttpStatusCode.Conflict, "concurrency.stale");
        await AssertProblemAsync(unknown, HttpStatusCode.NotFound, "schedule.rule_not_found");
        Assert.Equal(JsonValueKind.Null, (await GetRuleAsync(owner.Client, groupId, ruleId)).GetProperty("effectiveTo").ValueKind);
    }

    [Fact]
    public async Task Extend_ReturnsTheCountCreated_AndRepeatingItReturnsZero()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Extend Happy");
        await AddRuleAsync(owner.Client, groupId, Rule());

        // Eight weeks from today: the four horizon Saturdays plus 22 and 29 May and 5 and 12 June.
        using var first = await PostAsync(owner.Client, "/api/schedule/extend", new { through = "2027-06-15" });
        using var second = await PostAsync(owner.Client, "/api/schedule/extend", new { through = "2027-06-15" });

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        var firstBody = (await ReadJsonAsync(first)).RootElement;
        Assert.Single(firstBody.EnumerateObject());
        Assert.True(firstBody.GetProperty("sessionsCreated").GetInt32() >= 4);
        Assert.Equal(8, await CountSessionsAsync(groupId));
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        Assert.Equal(0, (await ReadJsonAsync(second)).RootElement.GetProperty("sessionsCreated").GetInt32());
        Assert.Equal(8, await CountSessionsAsync(groupId));
    }

    [Theory]
    [InlineData("2027-04-19")]
    [InlineData("2027-07-22")]
    public async Task Extend_OutsideTodayToNinetyTwoDaysAhead_Returns422RangeInvalidOnErrorsThrough(string through)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await PostAsync(owner.Client, "/api/schedule/extend", new { through });

        var body = await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "schedule.range_invalid");
        Assert.True(body.GetProperty("errors").GetProperty("through").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Extend_WithACentreInTheBody_Returns400RequestMalformed()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await PostAsync(owner.Client, "/api/schedule/extend", new { through = "2027-05-20", centreId = owner.CentreId });

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "request.malformed");
    }

    [Fact]
    public async Task Timetable_WithoutWeekStart_Returns400OnErrorsWeekStart_AndAMisshapenOneIsMalformed()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var missing = await owner.Client.GetAsync(new Uri("/api/timetable", UriKind.Relative));
        using var misshapen = await owner.Client.GetAsync(new Uri("/api/timetable?weekStart=24-04-2027", UriKind.Relative));

        var body = (await ReadJsonAsync(missing)).RootElement;
        Assert.Equal(HttpStatusCode.BadRequest, missing.StatusCode);
        Assert.True(body.GetProperty("errors").GetProperty("weekStart").GetArrayLength() > 0);
        await AssertProblemAsync(misshapen, HttpStatusCode.BadRequest, "request.malformed");
    }

    [Fact]
    public async Task Timetable_ReturnsTheZoneAndTheContractShape_WithInstantsInUtcEndingInZ()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, teacherId, _) = await CreateGroupAsync(owner.Client, "Timetable Shape");
        await AddRuleAsync(owner.Client, groupId, Rule(room: "Hall"));

        var week = await GetTimetableAsync(owner.Client, "2027-04-24", $"&groupId={groupId}");

        Assert.Equal(3, week.EnumerateObject().Count());
        Assert.Equal("Africa/Cairo", week.GetProperty("timeZoneId").GetString());
        Assert.Equal("2027-04-24", week.GetProperty("weekStart").GetString());
        var session = Assert.Single(week.GetProperty("sessions").EnumerateArray());
        Assert.Equal(SessionFields, session.EnumerateObject().Select(property => property.Name).Order(StringComparer.Ordinal));
        Assert.Equal(groupId, session.GetProperty("groupId").GetGuid());
        Assert.Equal("Timetable Shape", session.GetProperty("groupName").GetString());
        Assert.Equal(teacherId, session.GetProperty("teacherId").GetGuid());
        Assert.Equal("Timetable Shape Teacher", session.GetProperty("teacherName").GetString());
        Assert.Equal("2027-04-24", session.GetProperty("localDate").GetString());
        Assert.Equal("2027-04-24T16:00:00Z", session.GetProperty("startsAt").GetString());
        Assert.Equal("2027-04-24T17:30:00Z", session.GetProperty("endsAt").GetString());
        Assert.Equal("Hall", session.GetProperty("room").GetString());
        Assert.Equal("scheduled", session.GetProperty("status").GetString());
    }

    // Egypt moves to UTC+3 on Friday 30 April 2027: the same 18:00 Saturday is 16:00Z before and 15:00Z after.
    [Fact]
    public async Task Timetable_AcrossCairosSpringChange_KeepsSaturdayAtSixInTheEvening()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Timetable Dst");
        await AddRuleAsync(owner.Client, groupId, Rule());

        var before = Assert.Single((await GetTimetableAsync(owner.Client, "2027-04-24", $"&groupId={groupId}")).GetProperty("sessions").EnumerateArray());
        var after = Assert.Single((await GetTimetableAsync(owner.Client, "2027-05-01", $"&groupId={groupId}")).GetProperty("sessions").EnumerateArray());

        Assert.Equal("2027-04-24", before.GetProperty("localDate").GetString());
        Assert.Equal("2027-04-24T16:00:00Z", before.GetProperty("startsAt").GetString());
        Assert.Equal("2027-05-01", after.GetProperty("localDate").GetString());
        Assert.Equal("2027-05-01T15:00:00Z", after.GetProperty("startsAt").GetString());
    }

    [Fact]
    public async Task Timetable_ListsTheWeeksSessionsByStart_AndTheFiltersNarrowIt()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (firstGroupId, firstTeacherId, subjectId) = await CreateGroupAsync(owner.Client, "Timetable Filter A");
        var secondTeacherId = await CreateAsync(owner.Client, "/api/teachers", new { fullName = "Timetable Filter B Teacher", phone = (string?)null });
        var secondGroupId = await CreateGroupAsync(owner.Client, "Timetable Filter B", subjectId, secondTeacherId);
        await AddRuleAsync(owner.Client, firstGroupId, Rule(dayOfWeek: "sunday", startTime: "17:00"));
        await AddRuleAsync(owner.Client, secondGroupId, Rule(startTime: "16:00"));

        var all = await GetTimetableAsync(owner.Client, "2027-04-24", string.Empty);
        var byGroup = await GetTimetableAsync(owner.Client, "2027-04-24", $"&groupId={secondGroupId}");
        var byTeacher = await GetTimetableAsync(owner.Client, "2027-04-24", $"&teacherId={firstTeacherId}");
        var later = await GetTimetableAsync(owner.Client, "2027-06-19", string.Empty);

        // Saturday before Sunday: ordered by start instant.
        Assert.Equal(
            new[] { secondGroupId, firstGroupId },
            all.GetProperty("sessions").EnumerateArray().Select(session => session.GetProperty("groupId").GetGuid()));
        Assert.Equal(secondGroupId, Assert.Single(byGroup.GetProperty("sessions").EnumerateArray()).GetProperty("groupId").GetGuid());
        Assert.Equal(firstGroupId, Assert.Single(byTeacher.GetProperty("sessions").EnumerateArray()).GetProperty("groupId").GetGuid());
        Assert.Equal(0, later.GetProperty("sessions").GetArrayLength());
    }

    [Fact]
    public async Task CancelSession_Returns204AndTheTimetableStillListsItAsCancelled_ThenAgainReturns422AlreadyCancelled()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var secretary = await SignInHelper.SignInAsNileSecretaryAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Cancel Happy");
        await AddRuleAsync(owner.Client, groupId, Rule());
        var session = await GetOnlySessionAsync(secretary.Client, "2027-04-24", groupId);

        using var first = await PostAsync(
            secretary.Client, $"/api/sessions/{session.GetProperty("id").GetGuid()}/cancel", new { version = session.GetProperty("version").GetUInt32() });

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        var cancelled = await GetOnlySessionAsync(secretary.Client, "2027-04-24", groupId);
        Assert.Equal("cancelled", cancelled.GetProperty("status").GetString());

        using var second = await PostAsync(
            secretary.Client, $"/api/sessions/{session.GetProperty("id").GetGuid()}/cancel", new { version = cancelled.GetProperty("version").GetUInt32() });

        await AssertProblemAsync(second, HttpStatusCode.UnprocessableEntity, "schedule.session_already_cancelled");
    }

    [Fact]
    public async Task CancelSession_ThatHasStarted_Returns422SessionInPastAndStaysScheduled()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Cancel Started");
        await AddRuleAsync(owner.Client, groupId, Rule());
        var session = await GetOnlySessionAsync(owner.Client, "2027-04-24", groupId);

        // Saturday 24 April, 18:30 in Cairo: half an hour into the session.
        factory.Time.FixAt(new DateTimeOffset(2027, 4, 24, 16, 30, 0, TimeSpan.Zero));
        using var response = await PostAsync(
            owner.Client, $"/api/sessions/{session.GetProperty("id").GetGuid()}/cancel", new { version = session.GetProperty("version").GetUInt32() });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "schedule.session_in_past");
        Assert.Equal("scheduled", (await GetOnlySessionAsync(owner.Client, "2027-04-24", groupId)).GetProperty("status").GetString());
    }

    [Fact]
    public async Task CancelSession_StaleVersion_Returns409ConcurrencyStale_AndUnknownSession_Returns404SessionNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (groupId, _, _) = await CreateGroupAsync(owner.Client, "Cancel Stale");
        await AddRuleAsync(owner.Client, groupId, Rule());
        var session = await GetOnlySessionAsync(owner.Client, "2027-04-24", groupId);
        var id = session.GetProperty("id").GetGuid();
        var version = session.GetProperty("version").GetUInt32();

        using var stale = await PostAsync(owner.Client, $"/api/sessions/{id}/cancel", new { version = version + 1 });
        using var unknown = await PostAsync(owner.Client, $"/api/sessions/{Guid.NewGuid()}/cancel", new { version });
        using var overPosted = await PostAsync(owner.Client, $"/api/sessions/{id}/cancel", new { version, status = "cancelled" });

        await AssertProblemAsync(stale, HttpStatusCode.Conflict, "concurrency.stale");
        await AssertProblemAsync(unknown, HttpStatusCode.NotFound, "schedule.session_not_found");
        await AssertProblemAsync(overPosted, HttpStatusCode.BadRequest, "request.malformed");
        Assert.Equal("scheduled", (await GetOnlySessionAsync(owner.Client, "2027-04-24", groupId)).GetProperty("status").GetString());
    }

    private static Dictionary<string, object?> Rule(
        string dayOfWeek = "saturday", string startTime = "18:00", int durationMinutes = 90, string? room = null, string effectiveFrom = Today) =>
        new()
        {
            ["dayOfWeek"] = dayOfWeek,
            ["startTime"] = startTime,
            ["durationMinutes"] = durationMinutes,
            ["room"] = room,
            ["effectiveFrom"] = effectiveFrom,
        };

    private static async Task<(Guid GroupId, Guid TeacherId, Guid SubjectId)> CreateGroupAsync(HttpClient client, string name)
    {
        var subjectId = await CreateAsync(client, "/api/subjects", new { name = $"{name} Subject" });
        var teacherId = await CreateAsync(client, "/api/teachers", new { fullName = $"{name} Teacher", phone = (string?)null });
        return (await CreateGroupAsync(client, name, subjectId, teacherId), teacherId, subjectId);
    }

    private static Task<Guid> CreateGroupAsync(HttpClient client, string name, Guid subjectId, Guid teacherId) =>
        CreateAsync(client, "/api/groups", new { name, subjectId, teacherId, capacity = 20 });

    private static async Task<Guid> AddRuleAsync(HttpClient client, Guid groupId, Dictionary<string, object?> rule) =>
        await CreateAsync(client, $"/api/groups/{groupId}/schedule-rules", rule);

    private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)
    {
        using var response = await PostAsync(client, url, body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("id").GetGuid();
    }

    private static async Task ArchiveGroupAsync(HttpClient client, Guid id)
    {
        using var getResponse = await client.GetAsync(new Uri($"/api/groups/{id}", UriKind.Relative));
        var version = (await ReadJsonAsync(getResponse)).RootElement.GetProperty("version").GetUInt32();
        using var response = await PostAsync(client, $"/api/groups/{id}/archive", new { version });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static async Task<JsonElement> GetRuleAsync(HttpClient client, Guid groupId, Guid ruleId)
    {
        using var response = await client.GetAsync(new Uri($"/api/groups/{groupId}/schedule-rules", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.Clone().GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("id").GetGuid() == ruleId);
    }

    private static async Task<JsonElement> GetTimetableAsync(HttpClient client, string weekStart, string filters)
    {
        using var response = await client.GetAsync(new Uri($"/api/timetable?weekStart={weekStart}{filters}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.Clone();
    }

    private static async Task<JsonElement> GetOnlySessionAsync(HttpClient client, string weekStart, Guid groupId) =>
        Assert.Single((await GetTimetableAsync(client, weekStart, $"&groupId={groupId}")).GetProperty("sessions").EnumerateArray());

    private Task<long> CountRulesAsync(Guid groupId) =>
        factory.ScalarAsync<long>($"select count(*) from academics.schedule_rules where group_id = '{groupId}'");

    private Task<long> CountSessionsAsync(Guid groupId) =>
        factory.ScalarAsync<long>($"select count(*) from academics.class_sessions where group_id = '{groupId}'");

    private Task<string> SnapshotSessionStatusesAsync(Guid groupId) =>
        factory.ScalarAsync<string>(
            $"select coalesce(string_agg(local_date::text || ':' || status, ',' order by local_date), '') from academics.class_sessions where group_id = '{groupId}'");

    private static Task<HttpResponseMessage> PostAsync(HttpClient client, string url, object body) =>
        AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri(url, UriKind.Relative), body);

    private static async Task<HttpResponseMessage> PostRawAsync(HttpClient client, string url, string json)
    {
        var token = await AntiforgeryTestHelper.GetCsrfTokenAsync(client);
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(url, UriKind.Relative))
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };
        request.Headers.Add(AntiforgerySetup.HeaderName, token);
        return await client.SendAsync(request);
    }

    private static async Task<JsonElement> AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus, string expectedCode)
    {
        var body = (await ReadJsonAsync(response)).RootElement.Clone();
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal(expectedCode, body.GetProperty("code").GetString());
        return body;
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
