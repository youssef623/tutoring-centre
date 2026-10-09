using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Identity;

/// <summary>
/// Ensures a login account exists, over <see cref="UserManager{TUser}"/>. <see cref="UserManager{TUser}"/>
/// saves through the same scoped <c>AppDbContext</c> every other write in a request uses, so a new user row
/// participates in the dispatcher's ambient transaction exactly like a tracked entity — it is provisional
/// until the dispatcher commits, and gone if anything later in the same command fails.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class StaffAccountService(UserManager<ApplicationUser> userManager) : IStaffAccountService
{
    // 16 characters of upper/lower/digit, from a cryptographic source: comfortably clears the 10-character
    // minimum (DependencyInjection's Password.RequiredLength) with no character-class rule to satisfy.
    private const string PasswordCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
    private const int PasswordLength = 16;

    public async Task<Result<EnsureStaffAccountResult>> EnsureAccountAsync(
        string email, string displayName, string preferredLocale, CancellationToken ct)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            return Result<EnsureStaffAccountResult>.Success(new EnsureStaffAccountResult(existing.Id, Created: false, TemporaryPassword: null));
        }

        var temporaryPassword = RandomNumberGenerator.GetString(PasswordCharacters, PasswordLength);
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            DisplayName = displayName,
            PreferredLocale = preferredLocale,
            MustChangePassword = true,
        };

        var result = await userManager.CreateAsync(user, temporaryPassword);
        if (!result.Succeeded)
        {
            var messages = result.Errors.Select(error => error.Description).ToArray();
            return Result<EnsureStaffAccountResult>.Failure(new Error(
                "staff.email_invalid",
                "This email address could not be used.",
                ErrorKind.Validation,
                new Dictionary<string, string[]> { ["email"] = messages }));
        }

        return Result<EnsureStaffAccountResult>.Success(new EnsureStaffAccountResult(user.Id, Created: true, temporaryPassword));
    }
}
