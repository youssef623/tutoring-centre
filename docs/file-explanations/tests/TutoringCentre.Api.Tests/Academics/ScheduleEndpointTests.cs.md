# `tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 53.4: the six schedule endpoints' contract through the full HTTP pipeline — status, code, body shape and formats, never messages. The application clock is fixed at `Now`, a Tuesday ten days before Egypt moves its clocks forward (Friday 30 April 2027), so "today", the 28-day horizon, "in the past" and the session instants are the same on every run and every machine. Every test builds its own subject, teacher and groups over HTTP, so seeded data never decides a result.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 642 lines and 35,400 bytes. Source hash: `49b1c1b608c79d47112e5ee5cbfcaf0dc7615889c8ecc4d45f9adb99f113ed8b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Net.Http.Json;`
- Line 3: `using System.Text;`
- Line 4: `using System.Text.Json;`
- Line 5: `using TutoringCentre.Api.Auth;`
- Line 6: `using TutoringCentre.Api.Cli;`
- Line 7: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L19): `public sealed class ScheduleEndpointTests(ApiFactory factory) : IAsyncLifetime`
- [Line 33](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L33): `public async Task InitializeAsync()`
- [Line 40](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L40): `public Task DisposeAsync()`
- [Line 47](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L47): `public async Task AddRule_Returns201WithIdAndSessionsCreated_AndLocationAtTheGroupsRules()`
- [Line 66](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L66): `public async Task ListRules_ReturnsTheContractShape_WithStartTimeAsHourMinuteAndLowerCaseWeekday()`
- [Line 90](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L90): `public async Task ListRules_UnknownGroup_Returns404GroupNotFound_AndAGroupWithNoneReturnsEmptyItems()`
- [Line 106](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L106): `public async Task AddRule_WithATeacherOrCentreInTheBody_Returns400RequestMalformedAndWritesNoRow(string extraField)`
- [Line 127](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L127): `public async Task AddRule_NonLocalOrMisshapenValue_Returns400RequestMalformedAndWritesNoRow(string field, string value)`
- [Line 140](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L140): `public async Task AddRule_MissingRequiredFieldOrMalformedJson_Returns400RequestMalformed()`
- [Line 154](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L154): `public async Task AddRule_StartTimeOffTheFiveMinuteGrid_Returns400StartTimeInvalidOnErrorsStartTime()`
- [Line 167](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L167): `public async Task AddRule_DurationOffTheFiveMinuteStep_Returns400DurationInvalidOnErrorsDurationMinutes()`
- [Line 183](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L183): `public async Task AddRule_DurationOutOfRange_Returns400OnErrorsDurationMinutes(int durationMinutes)`
- [Line 196](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L196): `public async Task AddRule_RoomOverFortyCharacters_Returns400OnErrorsRoom()`
- [Line 209](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L209): `public async Task AddRule_UnknownGroup_Returns404GroupNotFound()`
- [Line 219](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L219): `public async Task AddRule_OverlappingTheSameTeachersOtherGroup_Returns409TeacherDoubleBookedAndCreatesNothing()`
- [Line 236](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L236): `public async Task AddRule_OverlappingTheGroupsOwnRule_Returns409GroupDoubleBookedAndCreatesNothing()`
- [Line 250](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L250): `public async Task AddRule_ToAnArchivedGroup_Returns422GroupArchived()`
- [Line 263](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L263): `public async Task AddRule_TakingEffectMoreThanAYearAhead_Returns422RangeInvalidOnErrorsEffectiveFrom()`
- [Line 275](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L275): `public async Task AddRule_WithoutTheAntiforgeryHeader_Returns403CsrfInvalidAndWritesNoRow()`
- [Line 287](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L287): `public async Task ScheduleRoutes_NoCookie_Return401()`
- [Line 302](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L302): `public async Task ScheduleWrite_BeforeSelectingACentre_Returns403TenantNotSelected()`
- [Line 313](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L313): `public async Task EndRule_Returns204_CancelsLaterSessionsOnly_ThenAgainReturns422AlreadyEnded()`
- [Line 335](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L335): `public async Task EndRule_BeforeItTakesEffect_Returns422RuleEndInvalidAndChangesNothing()`
- [Line 350](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L350): `public async Task EndRule_StaleVersion_Returns409ConcurrencyStale_AndUnknownRule_Returns404RuleNotFound()`
- [Line 366](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L366): `public async Task Extend_ReturnsTheCountCreated_AndRepeatingItReturnsZero()`
- [Line 389](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L389): `public async Task Extend_OutsideTodayToNinetyTwoDaysAhead_Returns422RangeInvalidOnErrorsThrough(string through)`
- [Line 400](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L400): `public async Task Extend_WithACentreInTheBody_Returns400RequestMalformed()`
- [Line 410](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L410): `public async Task Timetable_WithoutWeekStart_Returns400OnErrorsWeekStart_AndAMisshapenOneIsMalformed()`
- [Line 424](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L424): `public async Task Timetable_ReturnsTheZoneAndTheContractShape_WithInstantsInUtcEndingInZ()`
- [Line 450](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L450): `public async Task Timetable_AcrossCairosSpringChange_KeepsSaturdayAtSixInTheEvening()`
- [Line 466](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L466): `public async Task Timetable_ListsTheWeeksSessionsByStart_AndTheFiltersNarrowIt()`
- [Line 490](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L490): `public async Task CancelSession_Returns204AndTheTimetableStillListsItAsCancelled_ThenAgainReturns422AlreadyCancelled()`
- [Line 512](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L512): `public async Task CancelSession_ThatHasStarted_Returns422SessionInPastAndStaysScheduled()`
- [Line 529](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L529): `public async Task CancelSession_StaleVersion_Returns409ConcurrencyStale_AndUnknownSession_Returns404SessionNotFound()`
- [Line 566](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L566): `private static Task<Guid> CreateGroupAsync(HttpClient client, string name, Guid subjectId, Guid teacherId) =>`
- [Line 569](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L569): `private static async Task<Guid> AddRuleAsync(HttpClient client, Guid groupId, Dictionary<string, object?> rule) =>`
- [Line 572](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L572): `private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)`
- [Line 579](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L579): `private static async Task ArchiveGroupAsync(HttpClient client, Guid id)`
- [Line 587](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L587): `private static async Task<JsonElement> GetRuleAsync(HttpClient client, Guid groupId, Guid ruleId)`
- [Line 595](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L595): `private static async Task<JsonElement> GetTimetableAsync(HttpClient client, string weekStart, string filters)`
- [Line 602](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L602): `private static async Task<JsonElement> GetOnlySessionAsync(HttpClient client, string weekStart, Guid groupId) =>`
- [Line 605](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L605): `private Task<long> CountRulesAsync(Guid groupId) =>`
- [Line 608](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L608): `private Task<long> CountSessionsAsync(Guid groupId) =>`
- [Line 611](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L611): `private Task<string> SnapshotSessionStatusesAsync(Guid groupId) =>`
- [Line 615](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L615): `private static Task<HttpResponseMessage> PostAsync(HttpClient client, string url, object body) =>`
- [Line 618](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L618): `private static async Task<HttpResponseMessage> PostRawAsync(HttpClient client, string url, string json)`
- [Line 629](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L629): `private static async Task<JsonElement> AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus, string expectedCode)`
- [Line 637](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L637): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 552 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
