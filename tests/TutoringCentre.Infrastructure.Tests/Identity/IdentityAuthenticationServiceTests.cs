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
        Assert.Equal(0, await FailedAttemptCountAsync(userId));
        AssertNoEmailLogged(log);
    }

    private IServiceScope CreateScope(CapturingLoggerProvider log) =>
        Fixture.CreateServiceProvider(services => services.AddLogging(builder => builder.AddProvider(log))).CreateScope();

    private static IAuthenticationService AuthServiceFor(IServiceScope scope) =>
        scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

    private static async Task<Guid> CreateUserAsync(IServiceScope scope, string email, string password)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            DisplayName = "Staff Member",
            PreferredLocale = "en",
        };
        var result = await userManager.CreateAsync(user, password);
        Assert.True(result.Succeeded, string.Join(", ", result.Errors.Select(e => e.Description)));
        return user.Id;
    }

    private Task<int> FailedAttemptCountAsync(Guid userId) =>
        Fixture.ScalarAsync<int>($"select access_failed_count from identity.users where id = '{userId}'");

    private static void AssertNoEmailLogged(CapturingLoggerProvider log) =>
        Assert.DoesNotContain(log.Lines, line => line.Contains("@nile.test", StringComparison.OrdinalIgnoreCase));

    /// <summary>Captures formatted log lines in memory so a test can assert what was (not) logged.</summary>
    private sealed class CapturingLoggerProvider : ILoggerProvider
    {
        public List<string> Lines { get; } = [];

        public ILogger CreateLogger(string categoryName) => new CapturingLogger(Lines);

        public void Dispose()
        {
        }

        private sealed class CapturingLogger(List<string> lines) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
                lines.Add(formatter(state, exception));
        }
    }
}
