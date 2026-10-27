# `tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 53.4: tenant isolation for schedules at the HTTP boundary, every case in both directions. Each of the three schedule resources (group, rule, session) is the foreign one in at least one case, attacked with its real ID and real version, and each answers the 404 of that resource with the victim's schedule rows compared before and after through the superuser connection. The timetable has no ID to probe, so it is checked by content: the other centre's sessions never appear, filtered or not — not even for the one account that is staff in both centres. The clock is fixed so the arranged sessions fall in a known week.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 231 lines and 13,202 bytes. Source hash: `28964bce61fb322ecd0502a2d2f41e731cce261730ea9f1573f154db9786d1a6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L17): `public sealed class ScheduleCrossTenantTests(ApiFactory factory) : IAsyncLifetime`
- [Line 24](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L24): `public async Task InitializeAsync()`
- [Line 31](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L31): `public Task DisposeAsync()`
- [Line 40](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L40): `public async Task ListRules_OfTheOtherCentresGroup_Returns404GroupNotFound(string attackerEmail, string victimEmail)`
- [Line 54](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L54): `public async Task AddRule_ToTheOtherCentresGroup_Returns404GroupNotFoundAndWritesNothing(string attackerEmail, string victimEmail)`
- [Line 68](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L68): `public async Task EndRule_TheOtherCentresRuleWithItsRealVersion_Returns404RuleNotFoundAndLeavesItOpen(string attackerEmail, string victimEmail)`
- [Line 84](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L84): `public async Task CancelSession_TheOtherCentresSessionWithItsRealVersion_Returns404SessionNotFoundAndLeavesItScheduled(string attackerEmail, string victimEmail)`
- [Line 100](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L100): `public async Task Timetable_NeverListsTheOtherCentresSessions_FilteredByItsGroupOrTeacherOrNot(string attackerEmail, string victimEmail)`
- [Line 123](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L123): `public async Task TwoCentreTeacher_Timetable_ListsOnlyTheSelectedCentresOwnSession(string selectedSlug, string otherSlug)`
- [Line 141](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L141): `private static async Task<Guid> CreateScheduledGroupAsync(HttpClient client, string prefix)`
- [Line 148](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L148): `private static async Task<Guid> CreateGroupAsync(HttpClient client, string prefix, Guid? teacherId = null)`
- [Line 156](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L156): `/// A teacher record in the owner's centre linked to teacher@both.test (any record already linked to that user is`
- [Line 159](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L159): `private static async Task<Guid> CreateLinkedTeachersScheduledGroupAsync(HttpClient ownerClient, string prefix)`
- [Line 183](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L183): `private static async Task SetTeacherAccountAsync(HttpClient ownerClient, Guid teacherId, Guid? userId)`
- [Line 192](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L192): `private static async Task<JsonElement> GetOnlyRuleAsync(HttpClient client, Guid groupId)`
- [Line 199](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L199): `private static async Task<List<JsonElement>> GetSessionsAsync(HttpClient client, string filters)`
- [Line 206](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L206): `private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)`
- [Line 215](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L215): `private Task<string> SnapshotGroupScheduleAsync(Guid groupId) =>`
- [Line 226](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L226): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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

```

The excerpt stops at line 90; use the original file for the remaining 141 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
