# `tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 26.10: the week's first demo as an automated test — owner A cannot see or touch B's subjects, proven in both directions with CrossTenantProbe, using a real victim subject and its real current version so a 409 could never mask a leak.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 113 lines and 5,564 bytes. Source hash: `048f53b83385abded67e4c04d1a0d94e8c1dbfa2b0e102edcc48ce57c16094ca`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L14): `public sealed class SubjectCrossTenantProbeTests(ApiFactory factory) : IAsyncLifetime`
- [Line 16](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L16): `public async Task InitializeAsync()`
- [Line 22](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L22): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 25](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L25): `public Task NileOwner_CannotSeeOrTouchMaadisSubjects() =>`
- [Line 29](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L29): `public Task MaadiOwner_CannotSeeOrTouchNilesSubjects() =>`
- [Line 33](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L33): `public async Task TwoCentreTeacher_SeesNilesListThenMaadisAfterSwitching_NeverAMix()`
- [Line 51](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L51): `private async Task RunDirectionAsync(string attackerEmail, string victimEmail)`
- [Line 101](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L101): `private static async Task<List<string>> ListNamesAsync(HttpClient client)`
- [Line 108](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L108): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

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
/// Task 26.10: the week's first demo as an automated test — owner A cannot see or touch B's subjects, proven in
/// both directions with CrossTenantProbe, using a real victim subject and its real current version so a 409
/// could never mask a leak.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class SubjectCrossTenantProbeTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public Task NileOwner_CannotSeeOrTouchMaadisSubjects() =>
        RunDirectionAsync(attackerEmail: "owner@nile.test", victimEmail: "owner@maadi.test");

    [Fact]
    public Task MaadiOwner_CannotSeeOrTouchNilesSubjects() =>
        RunDirectionAsync(attackerEmail: "owner@maadi.test", victimEmail: "owner@nile.test");

    [Fact]
    public async Task TwoCentreTeacher_SeesNilesListThenMaadisAfterSwitching_NeverAMix()
    {
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        var nileNames = await ListNamesAsync(teacher.Client);
        Assert.Equal(3, nileNames.Count);
        Assert.DoesNotContain("Mathematics", nileNames);
        Assert.DoesNotContain("Physics", nileNames);

        await teacher.SwitchCentreAsync("maadi-hub");

        var maadiNames = await ListNamesAsync(teacher.Client);
        Assert.Equal(2, maadiNames.Count);
        Assert.DoesNotContain("رياضيات", maadiNames);
        Assert.DoesNotContain("فيزياء", maadiNames);
        Assert.DoesNotContain("كيمياء", maadiNames);
    }

    private async Task RunDirectionAsync(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);

        using var victimListResponse = await victim.Client.GetAsync(new Uri("/api/subjects", UriKind.Relative));
        var victimList = await ReadJsonAsync(victimListResponse);
        var firstVictimSubject = victimList.RootElement.GetProperty("items")[0];
        var victimSubjectId = firstVictimSubject.GetProperty("id").GetGuid();
        var victimSubjectName = firstVictimSubject.GetProperty("name").GetString()!;
        var victimVersion = firstVictimSubject.GetProperty("version").GetUInt32();

        Task<string> SnapshotAsync() =>
            factory.ScalarAsync<string>($"select name || '|' || status from academics.subjects where id = '{victimSubjectId}'");

        // GET by ID.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Get, $"/api/subjects/{victimSubjectId}", body: null,
            HttpStatusCode.NotFound, "subject.not_found", SnapshotAsync);

        // PUT with a valid body and the victim's real current version — never an invented one.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"/api/subjects/{victimSubjectId}",
            new { name = "Hacked Name", version = victimVersion },
            HttpStatusCode.NotFound, "subject.not_found", SnapshotAsync);

        // Archive.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/subjects/{victimSubjectId}/archive", new { version = victimVersion },
            HttpStatusCode.NotFound, "subject.not_found", SnapshotAsync);

        // Restore.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/subjects/{victimSubjectId}/restore", new { version = victimVersion },
            HttpStatusCode.NotFound, "subject.not_found", SnapshotAsync);

        // The attacker's own list contains none of the victim's IDs or names.
        using var attackerListResponse = await attacker.Client.GetAsync(new Uri("/api/subjects?includeArchived=true", UriKind.Relative));
        var attackerList = await ReadJsonAsync(attackerListResponse);
        var attackerItems = attackerList.RootElement.GetProperty("items").EnumerateArray().ToList();
```

The excerpt stops at line 90; use the original file for the remaining 23 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
