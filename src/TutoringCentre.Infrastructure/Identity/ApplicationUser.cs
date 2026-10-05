using Microsoft.AspNetCore.Identity;

namespace TutoringCentre.Infrastructure.Identity;

/// <summary>
/// The Identity persistence model for a staff user. This is Infrastructure's own type, not a Domain entity:
/// nothing outside Infrastructure (other than the composition root and CLI) may reference it.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser() => Id = Guid.CreateVersion7();

    public string DisplayName { get; set; } = string.Empty;

    /// <summary>"ar" or "en".</summary>
    public string PreferredLocale { get; set; } = string.Empty;

    /// <summary>Forces a password change on next sign-in (Month 2).</summary>
    public bool MustChangePassword { get; set; }
}
