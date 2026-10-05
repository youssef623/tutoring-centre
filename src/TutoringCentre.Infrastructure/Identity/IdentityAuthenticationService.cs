using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Identity;

/// <summary>
/// Verifies credentials through Identity's <see cref="UserManager{TUser}"/>, with lockout and without revealing
/// which accounts exist: an unknown email still pays the cost of a real password hash verification (against a
/// fixed dummy user), so response time carries no signal. Locked-out users are logged by ID only, never by email.
/// <see cref="UserManager{TUser}"/> saves its own counters (failed-attempt count, lockout end, security stamp)
/// through the Identity store as it goes — the one intentional write path outside the dispatcher, because signing
/// in is not a use case with a transaction boundary of its own.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed partial class IdentityAuthenticationService(UserManager<ApplicationUser> userManager, ILogger<IdentityAuthenticationService> logger)
    : IAuthenticationService
{
    private static readonly ApplicationUser DummyUser = CreateDummyUser();

    public async Task<Result<AuthenticatedUser>> VerifyCredentialsAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            // Pays the same password-hashing cost as a real check, so an unknown email cannot be told apart by timing.
            await userManager.CheckPasswordAsync(DummyUser, password);
            return InvalidCredentials();
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            LogLockedOutAttempt(logger, user.Id);
            return InvalidCredentials();
        }

        if (!await userManager.CheckPasswordAsync(user, password))
        {
            await userManager.AccessFailedAsync(user);
            return InvalidCredentials();
        }

        await userManager.ResetAccessFailedCountAsync(user);
        return Result<AuthenticatedUser>.Success(new AuthenticatedUser(user.Id, user.SecurityStamp ?? string.Empty));
    }

    private static Result<AuthenticatedUser> InvalidCredentials() =>
        Result<AuthenticatedUser>.Failure(Error.Unauthenticated("auth.invalid_credentials", "Incorrect email or password."));

    private static ApplicationUser CreateDummyUser()
    {
        var user = new ApplicationUser { UserName = "dummy@tutoring.invalid", Email = "dummy@tutoring.invalid" };
        user.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(user, "dummy-password-never-used");
        return user;
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Login attempt for locked-out user {UserId}")]
    private static partial void LogLockedOutAttempt(ILogger logger, Guid userId);
}
