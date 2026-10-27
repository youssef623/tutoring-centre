# `tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Proves the authentication adapter's lockout and no-enumeration behaviour against real PostgreSQL.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 215 lines and 9,500 bytes. Source hash: `4d07abb329ab1a48b640f9b424e3010cf9b37a4abc685781cc78095df39c904d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.Identity;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Infrastructure.Identity;`
- Line 6: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L11): `public sealed class IdentityAuthenticationServiceTests(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L17): `public async Task VerifyCredentialsAsync_CorrectPassword_SucceedsWithUserIdAndStamp()`
- [Line 32](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L32): `public async Task VerifyCredentialsAsync_WrongPassword_FailsAndRecordsOneFailedAttempt()`
- [Line 47](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L47): `public async Task VerifyCredentialsAsync_UnknownEmail_FailsWithTheIdenticalError()`
- [Line 60](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L60): `public async Task VerifyCredentialsAsync_FiveWrongAttemptsThenCorrectPassword_StillFails()`
- [Line 80](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L80): `public async Task VerifyCredentialsAsync_SuccessAfterAFailure_ResetsTheCounter()`
- [Line 96](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L96): `public async Task ChangePasswordAsync_CorrectCurrentAndValidNew_RotatesStampClearsFlagAndAppliesPolicy()`
- [Line 116](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L116): `public async Task ChangePasswordAsync_WrongCurrentPassword_FailsWithoutRotatingTheStampOrClearingTheFlag()`
- [Line 131](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L131): `public async Task ChangePasswordAsync_NewPasswordFailsThePolicy_FailsWithFieldError()`
- [Line 145](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L145): `public async Task ChangePasswordAsync_NewPasswordSameAsCurrent_FailsAsTooWeak()`
- [Line 156](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L156): `private IServiceScope CreateScope(CapturingLoggerProvider log) =>`
- [Line 159](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L159): `private static IAuthenticationService AuthServiceFor(IServiceScope scope) =>`
- [Line 162](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L162): `private static Task<Guid> CreateUserAsync(IServiceScope scope, string email, string password) =>`
- [Line 165](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L165): `private static async Task<Guid> CreateUserAsync(IServiceScope scope, string email, string password, bool mustChangePassword)`
- [Line 182](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L182): `private Task<int> FailedAttemptCountAsync(Guid userId) =>`
- [Line 185](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L185): `private Task<string> StampAsync(Guid userId) =>`
- [Line 188](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L188): `private Task<bool> MustChangePasswordAsync(Guid userId) =>`
- [Line 191](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L191): `private static void AssertNoEmailLogged(CapturingLoggerProvider log) =>`
- [Line 195](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L195): `private sealed class CapturingLoggerProvider : ILoggerProvider`
- [Line 199](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L199): `public ILogger CreateLogger(string categoryName) => new CapturingLogger(Lines);`
- [Line 201](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L201): `public void Dispose()`
- [Line 205](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L205): `private sealed class CapturingLogger(List<string> lines) : ILogger`
- [Line 207](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L207): `public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;`
- [Line 209](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L209): `public bool IsEnabled(LogLevel logLevel) => true;`
- [Line 211](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L211): `public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>Proves the authentication adapter's lockout and no-enumeration behaviour against real PostgreSQL.</summary>
public sealed class IdentityAuthenticationServiceTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    private const string Email = "staff@nile.test";
    private const string Password = "a-strong-enough-password";

    [Fact]
    public async Task VerifyCredentialsAsync_CorrectPassword_SucceedsWithUserIdAndStamp()
    {
        var log = new CapturingLoggerProvider();
        using var scope = CreateScope(log);
        var userId = await CreateUserAsync(scope, Email, Password);

        var result = await AuthServiceFor(scope).VerifyCredentialsAsync(Email, Password, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value.UserId);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.SecurityStamp));
        AssertNoEmailLogged(log);
    }

    [Fact]
    public async Task VerifyCredentialsAsync_WrongPassword_FailsAndRecordsOneFailedAttempt()
    {
        var log = new CapturingLoggerProvider();
        using var scope = CreateScope(log);
        var userId = await CreateUserAsync(scope, Email, Password);

        var result = await AuthServiceFor(scope).VerifyCredentialsAsync(Email, "totally-wrong", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("auth.invalid_credentials", result.Error!.Code);
        Assert.Equal(1, await FailedAttemptCountAsync(userId));
        AssertNoEmailLogged(log);
    }

    [Fact]
    public async Task VerifyCredentialsAsync_UnknownEmail_FailsWithTheIdenticalError()
    {
        var log = new CapturingLoggerProvider();
        using var scope = CreateScope(log);

        var result = await AuthServiceFor(scope).VerifyCredentialsAsync("nobody@nile.test", Password, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("auth.invalid_credentials", result.Error!.Code);
        AssertNoEmailLogged(log);
    }

    [Fact]
    public async Task VerifyCredentialsAsync_FiveWrongAttemptsThenCorrectPassword_StillFails()
    {
        var log = new CapturingLoggerProvider();
        using var scope = CreateScope(log);
        await CreateUserAsync(scope, Email, Password);
        var authService = AuthServiceFor(scope);

        for (var attempt = 0; attempt < 5; attempt++)
        {
            await authService.VerifyCredentialsAsync(Email, "totally-wrong", CancellationToken.None);
        }

        var result = await authService.VerifyCredentialsAsync(Email, Password, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("auth.invalid_credentials", result.Error!.Code);
        AssertNoEmailLogged(log);
    }

    [Fact]
    public async Task VerifyCredentialsAsync_SuccessAfterAFailure_ResetsTheCounter()
    {
        var log = new CapturingLoggerProvider();
        using var scope = CreateScope(log);
        var userId = await CreateUserAsync(scope, Email, Password);
        var authService = AuthServiceFor(scope);

        await authService.VerifyCredentialsAsync(Email, "totally-wrong", CancellationToken.None);
        var result = await authService.VerifyCredentialsAsync(Email, Password, CancellationToken.None);

        Assert.True(result.IsSuccess);
```

The excerpt stops at line 90; use the original file for the remaining 125 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
