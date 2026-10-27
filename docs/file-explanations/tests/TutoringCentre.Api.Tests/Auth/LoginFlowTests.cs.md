# `tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs`

## Purpose

Declares `LoginFlowTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 273 lines and 13,049 bytes. Source hash: `3ebca3331b9768bb5df1bd08f928660b71a6e753d5578f80031a338163fbb12e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Net.Http.Json;`
- Line 3: `using System.Text.Json;`
- Line 4: `using Microsoft.AspNetCore.Mvc.Testing;`
- Line 5: `using TutoringCentre.Api.Cli;`
- Line 6: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L11): `public sealed class LoginFlowTests(ApiFactory factory) : IAsyncLifetime`
- [Line 26](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L26): `public async Task InitializeAsync()`
- [Line 32](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L32): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 35](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L35): `public async Task Login_AsOwner_SetsHostPrefixedSecureCookieAndAutoSelectsTheirOnlyCentre()`
- [Line 67](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L67): `public async Task Login_AsSecretaryWithOneCentre_AutoSelectsAndReturnsItsPermissions()`
- [Line 85](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L85): `public async Task Login_AsTeacherWithTwoCentres_LeavesNoActiveCentreSelected()`
- [Line 102](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L102): `public async Task Login_WhileAlreadySignedIn_ReplacesTheSessionWithTheNewIdentity()`
- [Line 127](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L127): `public async Task SelectCentre_ForAMembershipTheUserHolds_Returns204AndActivatesIt()`
- [Line 142](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L142): `public async Task SelectCentre_ForACentreTheUserDoesNotBelongTo_Returns403AndLeavesSessionUnchanged()`
- [Line 162](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L162): `public async Task GetMe_AsOwner_ReturnsAllSixteenPermissionsSorted()`
- [Line 179](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L179): `public async Task GetMe_AsSecretary_ReturnsStudentsAndSubjectsPermissionsSorted()`
- [Line 196](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L196): `public async Task GetMe_AsTwoCentreTeacherBeforeSelectingACentre_ReturnsNoPermissions()`
- [Line 212](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L212): `public async Task Logout_Returns204AndClearsTheCookie()`
- [Line 228](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L228): `public async Task GetMe_AfterLogout_Returns401()`
- [Line 246](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L246): `private HttpClient CreateSessionClient() =>`
- [Line 249](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L249): `private static async Task<Guid> LoginAsTeacherAndGetMaadiHubCentreIdAsync(HttpClient client)`
- [Line 268](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L268): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Auth;

[Collection(ApiCollection.Name)]
public sealed class LoginFlowTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri LoginUri = new("/api/auth/login", UriKind.Relative);
    private static readonly Uri LogoutUri = new("/api/auth/logout", UriKind.Relative);
    private static readonly Uri MeUri = new("/api/me", UriKind.Relative);
    private static readonly Uri SelectCentreUri = new("/api/session/centre", UriKind.Relative);

    private static readonly string[] AllSixteenPermissionsSorted =
    [
        "audit.view", "centre.settings.manage", "enrollments.manage", "enrollments.view", "groups.manage", "groups.view",
        "schedule.manage", "schedule.view", "staff.manage", "staff.view",
        "students.manage", "students.view", "subjects.manage", "subjects.view",
        "teachers.manage", "teachers.view",
    ];

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Login_AsOwner_SetsHostPrefixedSecureCookieAndAutoSelectsTheirOnlyCentre()
    {
        using var client = CreateSessionClient();

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            client,
            LoginUri,
            new { email = "owner@nile.test", password = ApiFactory.TestSeedPassword });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var setCookie = Assert.Single(response.Headers.GetValues("Set-Cookie"));
        Assert.StartsWith("__Host-tcm.session=", setCookie, StringComparison.Ordinal);
        Assert.Contains("secure", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("httponly", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=lax", setCookie, StringComparison.OrdinalIgnoreCase);

        var body = await ReadJsonAsync(response);
        Assert.Equal("owner", body.RootElement.GetProperty("activeRole").GetString());
        Assert.False(string.IsNullOrEmpty(body.RootElement.GetProperty("activeCentreId").GetString()));
        Assert.Equal("nile-centre", body.RootElement.GetProperty("memberships")[0].GetProperty("centreSlug").GetString());

        // Day 53: every membership names its centre's zone, so any role can work out "today" in the centre's calendar.
        Assert.Equal("Africa/Cairo", body.RootElement.GetProperty("memberships")[0].GetProperty("timeZoneId").GetString());

        // The login response itself must carry the auto-selected centre's permissions, not the pre-selection
        // (empty) snapshot — a client that renders straight off the login response, without a follow-up
        // GET /api/me, must still see the correct permission-gated UI.
        var permissions = body.RootElement.GetProperty("permissions").EnumerateArray().Select(p => p.GetString() ?? string.Empty).ToArray();
        Assert.Equal(AllSixteenPermissionsSorted, permissions);
    }

    [Fact]
    public async Task Login_AsSecretaryWithOneCentre_AutoSelectsAndReturnsItsPermissions()
    {
        using var client = CreateSessionClient();

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            client,
            LoginUri,
            new { email = "secretary@nile.test", password = ApiFactory.TestSeedPassword });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal("secretary", body.RootElement.GetProperty("activeRole").GetString());

        var permissions = body.RootElement.GetProperty("permissions").EnumerateArray().Select(p => p.GetString() ?? string.Empty).ToArray();
        Assert.Equal(["enrollments.manage", "enrollments.view", "groups.manage", "groups.view", "schedule.manage", "schedule.view", "students.manage", "students.view", "subjects.manage", "subjects.view", "teachers.view"], permissions);
    }

    [Fact]
    public async Task Login_AsTeacherWithTwoCentres_LeavesNoActiveCentreSelected()
    {
        using var client = CreateSessionClient();

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            client,
```

The excerpt stops at line 90; use the original file for the remaining 183 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
