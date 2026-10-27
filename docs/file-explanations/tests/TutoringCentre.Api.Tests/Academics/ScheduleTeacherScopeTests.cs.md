# `tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 53.4: the ownership guard on schedules, as black-box behaviour through teacher@both.test's real session. A group's rules inherit the group's rule: 200 for the own group, 404 (never 403) for another teacher's. The timetable lists only the teacher's own sessions, and its filters only narrow that scope. Writes are 403 even on the own group, because permission is evaluated before ownership, and they change nothing. The clock is fixed so the arranged sessions fall in a known week.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 226 lines and 12,207 bytes. Source hash: `84c3b694640e11df3875e2a50d9deb160c71db098d513c9ee1508809ec6b5376`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L16): `public sealed class ScheduleTeacherScopeTests(ApiFactory factory) : IAsyncLifetime`
- [Line 22](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L22): `public async Task InitializeAsync()`
- [Line 29](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L29): `public Task DisposeAsync()`
- [Line 36](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L36): `public async Task LinkedTeacher_RulesOfTheOwnGroup_Returns200WithTheRule()`
- [Line 49](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L49): `public async Task LinkedTeacher_RulesOfAnotherTeachersGroup_Returns404GroupNotFound()`
- [Line 60](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L60): `public async Task LinkedTeacher_Timetable_ListsOnlyTheOwnSessions()`
- [Line 80](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L80): `public async Task LinkedTeacher_TimetableFilteredToAnotherTeacherOrTheirGroup_ReturnsNoSessions()`
- [Line 99](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L99): `public async Task LinkedTeacher_EveryWriteRoute_Returns403PermissionDeniedAndChangesNothing(string route)`
- [Line 123](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L123): `/// record already linked to that user in Nile (the demo seed links one) is unlinked first, so "own" means`
- [Line 126](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L126): `private async Task<Arrangement> ArrangeAsync()`
- [Line 170](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L170): `private static async Task<List<JsonElement>> GetSessionsAsync(HttpClient client, string filters)`
- [Line 177](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L177): `private static async Task<Guid> FindUserIdAsync(HttpClient ownerClient, string email)`
- [Line 185](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L185): `private static async Task SetTeacherAccountAsync(HttpClient ownerClient, Guid teacherId, Guid? userId)`
- [Line 194](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L194): `private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)`
- [Line 202](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L202): `private Task<string> SnapshotScheduleAsync() =>`
- [Line 212](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L212): `private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus, string expectedCode)`
- [Line 219](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L219): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`
- [Line 225](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L225): `private sealed record Arrangement(Guid OwnGroupId, Guid OtherGroupId, Guid LinkedTeacherId, Guid OtherTeacherId, Guid OwnRuleId);`

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
```

The excerpt stops at line 90; use the original file for the remaining 136 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
