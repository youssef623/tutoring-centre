using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Api.Cli;

/// <summary>
/// Sets a user's password directly, without knowing the old one (Task 36.2's password-rotation drill needs
/// this and no tool for it existed: the self-service change-password flow, Task 29/30, requires the current
/// password, which an operator rotating a demo or compromised account does not have). A deploy-pipeline-grade
/// operation — same trust level as <see cref="MigrateCommand"/>/<see cref="SeedCommand"/>: it needs direct
/// process execution against the database, never exposed over HTTP.
/// </summary>
public static class ResetPasswordCommand
{
    [SuppressMessage(
        "Performance",
        "CA1848:Use the LoggerMessage delegates",
        Justification = "A handful of one-off log lines in a short-lived CLI command; a source-generated delegate is unwarranted here.")]
    public static async Task<int> RunAsync(IServiceProvider services, string email, string newPassword)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrEmpty(email);
        ArgumentException.ThrowIfNullOrEmpty(newPassword);

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("TutoringCentre.Api.Cli.ResetPasswordCommand");

        await using var scope = services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            // Never echoes the email back as "found" or "not found" beyond this one operator-only log line —
            // the same account-existence caution as everywhere else, even though this command never faces the internet.
            logger.LogError("Reset-password: no user with that email.");
            return 1;
        }

        var removeResult = await userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded && !removeResult.Errors.Any(error => error.Code == "PasswordMismatch" || error.Code == "UserHasNoPassword"))
        {
            // RemovePasswordAsync on a user with no password already set fails harmlessly; anything else is real.
            logger.LogError("Reset-password: could not clear the existing password: {Errors}",
                string.Join("; ", removeResult.Errors.Select(error => error.Description)));
            return 1;
        }

        var addResult = await userManager.AddPasswordAsync(user, newPassword);
        if (!addResult.Succeeded)
        {
            logger.LogError("Reset-password: the new password was rejected: {Errors}",
                string.Join("; ", addResult.Errors.Select(error => error.Description)));
            return 1;
        }

        logger.LogInformation("Reset-password: password set for the requested account.");
        return 0;
    }
}
