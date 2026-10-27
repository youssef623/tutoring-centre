# `tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs`

## Purpose

Source summary (intent, not independent execution proof): An HTTP client signed in as a specific seeded user through the real login endpoint — its own cookies, its own session. Feature-agnostic: knows nothing about subjects or any other module. Never shared between tests or users; each sign-in gets its own client.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 157 lines and 8,151 bytes. Source hash: `36c625b2969e8d92d241309b62c236e6c4541d8d561634e7068e3a13f8cf2283`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using Microsoft.AspNetCore.Mvc.Testing;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs#L12): `internal sealed class SignedInClient(HttpClient client, Guid? centreId) : IDisposable`
- [Line 23](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs#L23): `public async Task SwitchCentreAsync(string centreSlug)`
- [Line 40](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs#L40): `public void Dispose() => Client.Dispose();`
- [Line 44](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs#L44): `internal static class SignInHelper`
- [Line 50](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs#L50): `public static Task<SignedInClient> SignInAsNileOwnerAsync(ApiFactory factory) =>`
- [Line 53](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs#L53): `public static Task<SignedInClient> SignInAsMaadiOwnerAsync(ApiFactory factory) =>`
- [Line 56](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs#L56): `public static Task<SignedInClient> SignInAsNileSecretaryAsync(ApiFactory factory) =>`
- [Line 60](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs#L60): `public static Task<SignedInClient> SignInAsTwoCentreTeacherAsync(ApiFactory factory, string centreSlug) =>`
- [Line 64](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs#L64): `public static async Task<SignedInClient> SignInAsync(ApiFactory factory, string email, string? selectCentreSlug = null)`
- [Line 139](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs#L139): `internal static Guid? FindMembershipCentreId(JsonDocument me, string centreSlug)`
- [Line 152](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs#L152): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs:35](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L35)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs:40](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L40)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs:34](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L34)
- [tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs:31](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L31)
- [tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs:35](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L35)
- [tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs:32](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L32)
- [tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs:40](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L40)
- [tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs:44](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L44)
- [tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs:34](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs#L34)
- [tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs:26](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L26)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs:42](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L42)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleDoubleBookingHttpTests.cs:40](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleDoubleBookingHttpTests.cs#L40)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs:50](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L50)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs:39](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L39)
- [tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs:44](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L44)
- [tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs:25](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L25)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs:35](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L35)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs:25](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L25)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs:29](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L29)
- [tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs:37](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs#L37)
- 11 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>
/// An HTTP client signed in as a specific seeded user through the real login endpoint — its own cookies, its
/// own session. Feature-agnostic: knows nothing about subjects or any other module. Never shared between tests
/// or users; each sign-in gets its own client.
/// </summary>
internal sealed class SignedInClient(HttpClient client, Guid? centreId) : IDisposable
{
    private static readonly Uri MeUri = new("/api/me", UriKind.Relative);
    private static readonly Uri SelectCentreUri = new("/api/session/centre", UriKind.Relative);

    public HttpClient Client { get; } = client;

    /// <summary>The actor's active centre after sign-in (and, if requested, centre selection) — null if none is active.</summary>
    public Guid? CentreId { get; private set; } = centreId;

    /// <summary>Switches the active centre on this same session (same cookies, same client) — proves a later request on this client reflects the switch, not a fresh login.</summary>
    public async Task SwitchCentreAsync(string centreSlug)
    {
        using var meResponse = await Client.GetAsync(MeUri);
        var text = await meResponse.Content.ReadAsStringAsync();
        using var me = JsonDocument.Parse(text);
        var targetCentreId = SignInHelper.FindMembershipCentreId(me, centreSlug)
            ?? throw new InvalidOperationException($"No membership in centre '{centreSlug}'.");

        using var selectResponse = await AntiforgeryTestHelper.PostAsJsonAsync(Client, SelectCentreUri, new { centreId = targetCentreId });
        if (selectResponse.StatusCode != HttpStatusCode.NoContent)
        {
            throw new InvalidOperationException($"Switching to centre '{centreSlug}' failed with {selectResponse.StatusCode}.");
        }

        CentreId = targetCentreId;
    }

    public void Dispose() => Client.Dispose();
}

/// <summary>Signs a fresh client in as a seeded user via the real login (and, when asked, centre-selection) endpoints.</summary>
internal static class SignInHelper
{
    private static readonly Uri LoginUri = new("/api/auth/login", UriKind.Relative);
    private static readonly Uri SelectCentreUri = new("/api/session/centre", UriKind.Relative);
    private static readonly Uri ChangePasswordUri = new("/api/auth/change-password", UriKind.Relative);

    public static Task<SignedInClient> SignInAsNileOwnerAsync(ApiFactory factory) =>
        SignInAsync(factory, "owner@nile.test");

    public static Task<SignedInClient> SignInAsMaadiOwnerAsync(ApiFactory factory) =>
        SignInAsync(factory, "owner@maadi.test");

    public static Task<SignedInClient> SignInAsNileSecretaryAsync(ApiFactory factory) =>
        SignInAsync(factory, "secretary@nile.test");

    /// <summary>The two-centre teacher has no auto-selected centre after login; <paramref name="centreSlug"/> picks one explicitly.</summary>
    public static Task<SignedInClient> SignInAsTwoCentreTeacherAsync(ApiFactory factory, string centreSlug) =>
        SignInAsync(factory, "teacher@both.test", centreSlug);

    /// <summary>Signs in as any seeded user; optionally selects a centre by slug when the account has more than one membership.</summary>
    public static async Task<SignedInClient> SignInAsync(ApiFactory factory, string email, string? selectCentreSlug = null)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        using var loginResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email, password = ApiFactory.TestSeedPassword });
        if (loginResponse.StatusCode != HttpStatusCode.OK)
        {
            client.Dispose();
            throw new InvalidOperationException($"Sign-in as '{email}' failed with {loginResponse.StatusCode}.");
        }

        using var me = await ReadJsonAsync(loginResponse);
        Guid? centreId = me.RootElement.GetProperty("activeCentreId").ValueKind == JsonValueKind.String
            ? me.RootElement.GetProperty("activeCentreId").GetGuid()
            : null;

        if (selectCentreSlug is not null)
        {
            centreId = FindMembershipCentreId(me, selectCentreSlug)
                ?? throw new InvalidOperationException($"'{email}' has no membership in centre '{selectCentreSlug}'.");

            using var selectResponse = await AntiforgeryTestHelper.PostAsJsonAsync(client, SelectCentreUri, new { centreId });
            if (selectResponse.StatusCode != HttpStatusCode.NoContent)
            {
                client.Dispose();
```

The excerpt stops at line 90; use the original file for the remaining 67 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
