# `tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Proves the attacks Day 16 closes are actually closed: user enumeration, CSRF (16.6, written by hand), and cookie theft, tenant escalation, stale sessions, claim forgery, spraying, brute force, broken identity propagation and forgotten authorization (16.7). Uses `ConventionsFactory` rather than the plain `ApiFactory` because two cases need its test-only endpoints (current actor, default protection).

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 358 lines and 16,611 bytes. Source hash: `bbbfe1f8515b4486c14650fecd2d945bccef7733226464804a4839b2dbda9f4d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Net.Http.Json;`
- Line 3: `using System.Text.Json;`
- Line 4: `using Microsoft.AspNetCore.Mvc.Testing;`
- Line 5: `using Microsoft.Extensions.DependencyInjection;`
- Line 6: `using TutoringCentre.Api.Cli;`
- Line 7: `using TutoringCentre.Api.Tests.Fixtures;`
- Line 8: `using TutoringCentre.Application.Common.Cqrs;`
- Line 9: `using TutoringCentre.Application.Common.Security;`
- Line 10: `using TutoringCentre.Application.Staff;`
- Line 11: `using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;`
- Line 12: `using TutoringCentre.Application.Staff.Queries.ListStaff;`
- Line 13: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 24](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L24): `public sealed class SecurityTests(ConventionsFactory factory) : IAsyncLifetime`
- [Line 32](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L32): `public async Task InitializeAsync()`
- [Line 38](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L38): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 43](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L43): `public async Task Login_WrongPasswordForKnownUserAndUnknownUser_ReturnIdenticalResponses()`
- [Line 73](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L73): `public async Task Logout_WithoutCsrfHeader_Returns403AndSessionStillWorksAfterwards()`
- [Line 94](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L94): `public async Task Login_WithoutCsrfHeader_Returns403()`
- [Line 109](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L109): `public async Task Login_SetsAHostPrefixedSecureHttpOnlyLaxCookie()`
- [Line 124](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L124): `public async Task GetMe_WithNoCookie_Returns401()`
- [Line 134](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L134): `public async Task SelectCentre_ForACentreOwnerDoesNotBelongTo_Returns403AndMeStillShowsTheOriginalCentre()`
- [Line 155](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L155): `public async Task RevokedMembership_NextRequestAfterRevocation_Returns401()`
- [Line 178](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L178): `public async Task ChangedSecurityStamp_NextRequest_Returns401()`
- [Line 195](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L195): `public async Task RoleChangedByTheOwner_NextRequestForThatUser_Returns401()`
- [Line 231](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L231): `public async Task TamperedCookie_Returns401()`
- [Line 257](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L257): `public async Task Login_11TimesWithinAMinuteFromOneClient_11thReturns429WithRetryAfter()`
- [Line 279](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L279): `public async Task Login_FiveWrongPasswordsThenCorrect_StillReturns401WithTheOrdinaryFailureBody()`
- [Line 299](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L299): `public async Task CurrentActor_AfterSelectingACentre_IsAStaffActorWithThatCentreInApplication()`
- [Line 330](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L330): `public async Task EndpointWithNoAuthorizationAttribute_Returns401ByDefault()`
- [Line 339](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L339): `private HttpClient CreateSessionClient() =>`
- [Line 342](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L342): `private static string FlipLastCharacter(string value)`
- [Line 350](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L350): `private static IEnumerable<string> FieldNames(JsonDocument document) =>`
- [Line 353](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L353): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Api.Tests.Auth;

/// <summary>
/// Proves the attacks Day 16 closes are actually closed: user enumeration, CSRF (16.6, written by hand), and
/// cookie theft, tenant escalation, stale sessions, claim forgery, spraying, brute force, broken identity
/// propagation and forgotten authorization (16.7). Uses <see cref="ConventionsFactory"/> rather than the plain
/// <see cref="ApiFactory"/> because two cases need its test-only endpoints (current actor, default protection).
/// </summary>
[Collection(ConventionsCollection.Name)]
public sealed class SecurityTests(ConventionsFactory factory) : IAsyncLifetime
{
    private static readonly Uri LoginUri = new("/api/auth/login", UriKind.Relative);
    private static readonly Uri MeUri = new("/api/me", UriKind.Relative);
    private static readonly Uri SelectCentreUri = new("/api/session/centre", UriKind.Relative);
    private static readonly Uri ActorUri = new("/api/test/actor", UriKind.Relative);
    private static readonly Uri DefaultProtectionUri = new("/api/test-default-protection", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // --- 16.6: written by hand ---

    [Fact]
    public async Task Login_WrongPasswordForKnownUserAndUnknownUser_ReturnIdenticalResponses()
    {
        using var client = CreateSessionClient();

        using var wrongPassword = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "owner@nile.test", password = "clearly-the-wrong-password" });
        using var unknownEmail = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "nobody@nile.test", password = "clearly-the-wrong-password" });

        Assert.Equal(HttpStatusCode.Unauthorized, wrongPassword.StatusCode);
        Assert.Equal(wrongPassword.StatusCode, unknownEmail.StatusCode);

        var wrongPasswordBody = await ReadJsonAsync(wrongPassword);
        var unknownEmailBody = await ReadJsonAsync(unknownEmail);

        // Same field set...
        Assert.Equal(
            FieldNames(wrongPasswordBody),
            FieldNames(unknownEmailBody));

        // ...and every value identical except the two per-request identifiers.
        foreach (var field in FieldNames(wrongPasswordBody).Where(name => name is not "traceId" and not "correlationId"))
        {
            Assert.Equal(
                wrongPasswordBody.RootElement.GetProperty(field).ToString(),
                unknownEmailBody.RootElement.GetProperty(field).ToString());
        }
    }

    [Fact]
    public async Task Logout_WithoutCsrfHeader_Returns403AndSessionStillWorksAfterwards()
    {
        using var client = CreateSessionClient();
        using (var login = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email = "owner@nile.test", password = ApiFactory.TestSeedPassword }))
        {
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        }

        using (var logoutWithoutToken = await client.PostAsync(new Uri("/api/auth/logout", UriKind.Relative), content: null))
        {
            var problem = await ReadJsonAsync(logoutWithoutToken);
            Assert.Equal(HttpStatusCode.Forbidden, logoutWithoutToken.StatusCode);
            Assert.Equal("auth.csrf_invalid", problem.RootElement.GetProperty("code").GetString());
        }

        using var me = await client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
```

The excerpt stops at line 90; use the original file for the remaining 268 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
