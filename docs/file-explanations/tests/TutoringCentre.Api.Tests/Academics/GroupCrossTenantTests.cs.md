# `tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 45.7: tenant isolation for groups at the HTTP boundary, every case in both directions. Ordinary probes attack the victim's group directly on all six ID routes with its real current version, so a 409 could never mask a missing guard. Reference probes put the victim centre's subject or teacher ID inside the attacker's own request: the handler must resolve it inside the attacker's centre and answer not-found, with no row written on either side.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 200 lines and 11,668 bytes. Source hash: `cf00640b738769fefcfa2b4f311fdc469c13a255082b41a8fb0c2ab66368ec5c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L16): `public sealed class GroupCrossTenantTests(ApiFactory factory) : IAsyncLifetime`
- [Line 18](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L18): `public async Task InitializeAsync()`
- [Line 24](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L24): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 29](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L29): `public async Task EveryIdRoute_OnTheOtherCentresGroup_Returns404AndLeavesTheRowUnchanged(string attackerEmail, string victimEmail)`
- [Line 66](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L66): `public async Task List_ContainsNoneOfTheOtherCentresGroups(string attackerEmail, string victimEmail)`
- [Line 88](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L88): `public async Task Create_WithTheOtherCentresSubject_Returns404SubjectNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)`
- [Line 105](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L105): `public async Task Create_WithTheOtherCentresTeacher_Returns404TeacherNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)`
- [Line 122](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L122): `public async Task AssignTeacher_WithTheOtherCentresTeacher_Returns404TeacherNotFoundAndChangesNeitherSide(string attackerEmail, string victimEmail)`
- [Line 141](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L141): `public async Task Create_WithANameUsedInTheOtherCentre_Returns201(string attackerEmail, string victimEmail)`
- [Line 156](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L156): `private static async Task<References> CreateReferencesAsync(HttpClient client, string prefix)`
- [Line 163](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L163): `private static Task<Guid> CreateGroupAsync(HttpClient client, string name, References references) =>`
- [Line 166](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L166): `private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)`
- [Line 173](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L173): `private static async Task<uint> GetGroupVersionAsync(HttpClient client, Guid id)`
- [Line 179](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L179): `private Task<string> SnapshotGroupAsync(Guid id) =>`
- [Line 184](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L184): `private Task<string> SnapshotAllGroupsAsync() =>`
- [Line 187](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L187): `private Task<string> SnapshotSubjectAsync(Guid id) =>`
- [Line 190](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L190): `private Task<string> SnapshotTeacherAsync(Guid id) =>`
- [Line 193](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L193): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`
- [Line 199](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L199): `private sealed record References(Guid SubjectId, Guid TeacherId);`

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
/// Task 45.7: tenant isolation for groups at the HTTP boundary, every case in both directions. Ordinary probes
/// attack the victim's group directly on all six ID routes with its real current version, so a 409 could never
/// mask a missing guard. Reference probes put the victim centre's subject or teacher ID inside the attacker's
/// own request: the handler must resolve it inside the attacker's centre and answer not-found, with no row
/// written on either side.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class GroupCrossTenantTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task EveryIdRoute_OnTheOtherCentresGroup_Returns404AndLeavesTheRowUnchanged(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var attackerTeacherId = (await CreateReferencesAsync(attacker.Client, "Attacker")).TeacherId;
        var victimRefs = await CreateReferencesAsync(victim.Client, "Victim");
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim Group", victimRefs);
        var version = await GetGroupVersionAsync(victim.Client, victimGroupId);
        var url = $"/api/groups/{victimGroupId}";
        Task<string> SnapshotAsync() => SnapshotGroupAsync(victimGroupId);

        await CrossTenantProbe.RunAsync(attacker.Client, HttpMethod.Get, url, null, HttpStatusCode.NotFound, "group.not_found", SnapshotAsync);
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, url, new { name = "Hacked Name", version }, HttpStatusCode.NotFound, "group.not_found", SnapshotAsync);
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"{url}/capacity", new { capacity = 2, version }, HttpStatusCode.NotFound, "group.not_found", SnapshotAsync);
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"{url}/teacher", new { teacherId = attackerTeacherId, version },
            HttpStatusCode.NotFound, "group.not_found", SnapshotAsync);
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"{url}/archive", new { version }, HttpStatusCode.NotFound, "group.not_found", SnapshotAsync);

        // Restore needs an archived victim: archived by the victim itself, then probed with its new real version.
        using (var archived = await AntiforgeryTestHelper.SendAsJsonAsync(
            victim.Client, HttpMethod.Post, new Uri($"{url}/archive", UriKind.Relative), new { version }))
        {
            Assert.Equal(HttpStatusCode.NoContent, archived.StatusCode);
        }

        var archivedVersion = await GetGroupVersionAsync(victim.Client, victimGroupId);
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"{url}/restore", new { version = archivedVersion }, HttpStatusCode.NotFound, "group.not_found", SnapshotAsync);
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task List_ContainsNoneOfTheOtherCentresGroups(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var victimRefs = await CreateReferencesAsync(victim.Client, "Victim");
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim Only Group", victimRefs);

        using var response = await attacker.Client.GetAsync(new Uri("/api/groups?includeArchived=true", UriKind.Relative));

        var items = (await ReadJsonAsync(response)).RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(items, item => item.GetProperty("id").GetGuid() == victimGroupId);
        Assert.DoesNotContain(items, item => item.GetProperty("name").GetString() == "Victim Only Group");
        Assert.DoesNotContain(items, item => item.GetProperty("subjectId").GetGuid() == victimRefs.SubjectId);

        using var filtered = await attacker.Client.GetAsync(new Uri($"/api/groups?subjectId={victimRefs.SubjectId}&includeArchived=true", UriKind.Relative));
        Assert.Equal(0, (await ReadJsonAsync(filtered)).RootElement.GetProperty("items").GetArrayLength());
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Create_WithTheOtherCentresSubject_Returns404SubjectNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
```

The excerpt stops at line 90; use the original file for the remaining 110 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
