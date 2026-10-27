# `tests/TutoringCentre.Api.Tests/Staff/ChangePasswordHttpTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 30.7: POST /api/auth/change-password and the Day 30 first-login gate, through the full HTTP pipeline.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Staff/ChangePasswordHttpTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 143 lines and 7,723 bytes. Source hash: `24b395e7863ed5e9d817c9abd3853006a4179812ea1209c4ab961e6bfab21a3d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Staff/ChangePasswordHttpTests.cs#L10): `public sealed class ChangePasswordHttpTests(ApiFactory factory) : IAsyncLifetime`
- [Line 19](../../../../../tests/TutoringCentre.Api.Tests/Staff/ChangePasswordHttpTests.cs#L19): `public async Task InitializeAsync()`
- [Line 25](../../../../../tests/TutoringCentre.Api.Tests/Staff/ChangePasswordHttpTests.cs#L25): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 28](../../../../../tests/TutoringCentre.Api.Tests/Staff/ChangePasswordHttpTests.cs#L28): `public async Task WrongCurrentPassword_Returns422()`
- [Line 41](../../../../../tests/TutoringCentre.Api.Tests/Staff/ChangePasswordHttpTests.cs#L41): `public async Task WeakNewPassword_Returns400WithErrorsNewPassword()`
- [Line 55](../../../../../tests/TutoringCentre.Api.Tests/Staff/ChangePasswordHttpTests.cs#L55): `public async Task Success_ReissuesTheCookieAndTheOldCookiesNextRequestIs401()`
- [Line 75](../../../../../tests/TutoringCentre.Api.Tests/Staff/ChangePasswordHttpTests.cs#L75): `public async Task SixthAttemptInAMinute_Returns429()`
- [Line 97](../../../../../tests/TutoringCentre.Api.Tests/Staff/ChangePasswordHttpTests.cs#L97): `public async Task FirstLoginGateSequence_FromCreateToCentreSelection()`
- [Line 138](../../../../../tests/TutoringCentre.Api.Tests/Staff/ChangePasswordHttpTests.cs#L138): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Staff;

/// <summary>Task 30.7: POST /api/auth/change-password and the Day 30 first-login gate, through the full HTTP pipeline.</summary>
[Collection(ApiCollection.Name)]
public sealed class ChangePasswordHttpTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri StaffUri = new("/api/staff", UriKind.Relative);
    private static readonly Uri ChangePasswordUri = new("/api/auth/change-password", UriKind.Relative);
    private static readonly Uri MeUri = new("/api/me", UriKind.Relative);
    private static readonly Uri SelectCentreUri = new("/api/session/centre", UriKind.Relative);
    private static readonly Uri SubjectsUri = new("/api/subjects", UriKind.Relative);
    private static readonly Uri LoginUri = new("/api/auth/login", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task WrongCurrentPassword_Returns422()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, ChangePasswordUri, new { currentPassword = "totally-wrong", newPassword = "a-decent-new-password" });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("auth.current_password_invalid", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task WeakNewPassword_Returns400WithErrorsNewPassword()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, ChangePasswordUri, new { currentPassword = ApiFactory.TestSeedPassword, newPassword = "short" });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("auth.password_too_weak", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("newPassword").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Success_ReissuesTheCookieAndTheOldCookiesNextRequestIs401()
    {
        // Two independent sessions for the same user — "two devices" — both signed in before the change.
        using var deviceA = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var deviceB = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            deviceA.Client, ChangePasswordUri, new { currentPassword = ApiFactory.TestSeedPassword, newPassword = "a-brand-new-password-1" });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Contains(response.Headers.GetValues("Set-Cookie"), c => c.StartsWith("__Host-tcm.session=", StringComparison.Ordinal));

        using var meOnDeviceA = await deviceA.Client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.OK, meOnDeviceA.StatusCode);

        // SessionValidation:CacheDuration is zero in tests, so deviceB's very next request already re-checks.
        using var meOnDeviceB = await deviceB.Client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.Unauthorized, meOnDeviceB.StatusCode);
    }

    [Fact]
    public async Task SixthAttemptInAMinute_Returns429()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        const string partition = "change-password-rate-limit";

        for (var attempt = 1; attempt <= 5; attempt++)
        {
            using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
                owner.Client, ChangePasswordUri, new { currentPassword = "wrong-on-purpose", newPassword = "a-decent-new-password" }, partition);
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        using var sixth = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, ChangePasswordUri, new { currentPassword = "wrong-on-purpose", newPassword = "a-decent-new-password" }, partition);
        var body = await ReadJsonAsync(sixth);

```

The excerpt stops at line 90; use the original file for the remaining 53 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
