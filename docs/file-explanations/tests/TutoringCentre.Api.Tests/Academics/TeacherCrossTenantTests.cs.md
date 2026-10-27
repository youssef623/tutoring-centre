# `tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 43.14: owner A cannot see or touch B's teachers, proven in both directions with CrossTenantProbe across all five ID routes, using a real victim teacher and its real current version so a 409 could never mask a leak. The account link gets its own reference probe: linking the attacker's own teacher to a user who belongs to the victim's centre must answer teacher.user_not_eligible, the same answer as any other ineligible user, never disclosing that the ID itself resolved to someone real.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 118 lines and 6,045 bytes. Source hash: `62bd40a9f20b0ff5992bf3004d2afe1d603c4714b16cc69264fa7182e897d140`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs#L16): `public sealed class TeacherCrossTenantTests(ApiFactory factory) : IAsyncLifetime`
- [Line 18](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs#L18): `public async Task InitializeAsync()`
- [Line 24](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs#L24): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 27](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs#L27): `public Task NileOwner_CannotSeeOrTouchMaadisTeachers() =>`
- [Line 31](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs#L31): `public Task MaadiOwner_CannotSeeOrTouchNilesTeachers() =>`
- [Line 35](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs#L35): `public async Task SetAccount_WithAnotherCentresUserId_ReturnsUserNotEligibleAndChangesNothing()`
- [Line 59](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs#L59): `private async Task RunDirectionAsync(string attackerEmail, string victimEmail)`
- [Line 113](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs#L113): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

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
/// Task 43.14: owner A cannot see or touch B's teachers, proven in both directions with CrossTenantProbe
/// across all five ID routes, using a real victim teacher and its real current version so a 409 could never
/// mask a leak. The account link gets its own reference probe: linking the attacker's own teacher to a user
/// who belongs to the victim's centre must answer teacher.user_not_eligible, the same answer as any other
/// ineligible user, never disclosing that the ID itself resolved to someone real.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class TeacherCrossTenantTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public Task NileOwner_CannotSeeOrTouchMaadisTeachers() =>
        RunDirectionAsync(attackerEmail: "owner@nile.test", victimEmail: "owner@maadi.test");

    [Fact]
    public Task MaadiOwner_CannotSeeOrTouchNilesTeachers() =>
        RunDirectionAsync(attackerEmail: "owner@maadi.test", victimEmail: "owner@nile.test");

    [Fact]
    public async Task SetAccount_WithAnotherCentresUserId_ReturnsUserNotEligibleAndChangesNothing()
    {
        using var nileOwner = await SignInHelper.SignInAsync(factory, "owner@nile.test");
        using var maadiOwner = await SignInHelper.SignInAsync(factory, "owner@maadi.test");

        var (teacherId, version) = await CreateTeacherAsync(nileOwner.Client, "IDOR Target");

        using var maadiMeResponse = await maadiOwner.Client.GetAsync(new Uri("/api/me", UriKind.Relative));
        var maadiMe = await ReadJsonAsync(maadiMeResponse);
        var maadiUserId = maadiMe.RootElement.GetProperty("userId").GetGuid();

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/teachers/{teacherId}/account", UriKind.Relative),
            new { userId = maadiUserId, version });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("teacher.user_not_eligible", body.RootElement.GetProperty("code").GetString());

        using var getResponse = await nileOwner.Client.GetAsync(new Uri($"/api/teachers/{teacherId}", UriKind.Relative));
        var getBody = await ReadJsonAsync(getResponse);
        Assert.True(getBody.RootElement.GetProperty("userId").ValueKind is JsonValueKind.Null);
    }

    private async Task RunDirectionAsync(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);

        var (victimTeacherId, victimVersion) = await CreateTeacherAsync(victim.Client, "Victim Teacher");

        Task<string> SnapshotAsync() =>
            factory.ScalarAsync<string>($"select full_name || '|' || status from academics.teachers where id = '{victimTeacherId}'");

        // GET by ID.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Get, $"/api/teachers/{victimTeacherId}", body: null,
            HttpStatusCode.NotFound, "teacher.not_found", SnapshotAsync);

        // PUT with a valid body and the victim's real current version — never an invented one.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"/api/teachers/{victimTeacherId}",
            new { fullName = "Hacked Name", phone = (string?)null, version = victimVersion },
            HttpStatusCode.NotFound, "teacher.not_found", SnapshotAsync);

        // Account link.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"/api/teachers/{victimTeacherId}/account",
            new { userId = (Guid?)null, version = victimVersion },
            HttpStatusCode.NotFound, "teacher.not_found", SnapshotAsync);

        // Archive.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/teachers/{victimTeacherId}/archive", new { version = victimVersion },
            HttpStatusCode.NotFound, "teacher.not_found", SnapshotAsync);

```

The excerpt stops at line 90; use the original file for the remaining 28 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
