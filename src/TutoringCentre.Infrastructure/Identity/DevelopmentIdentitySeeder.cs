using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Identity;

/// <summary>
/// Development-only bootstrapping, not a use case: creates a realistic two-centre staff set directly against
/// Infrastructure, using the Domain's own factories so seeded data obeys the same rules as data created through
/// the Application layer. Never runs at normal application startup or during build-time OpenAPI generation.
/// </summary>
public sealed class DevelopmentIdentitySeeder(UserManager<ApplicationUser> userManager, AppDbContext db, IConfiguration configuration)
{
    private const string PasswordConfigurationKey = "Seed:Password";

    private static readonly SeedUser[] Users =
    [
        new("owner@nile.test", "Nile Owner", "ar", [new("nile-centre", StaffRole.Owner, Active: true)]),
        new("owner@maadi.test", "Maadi Owner", "en", [new("maadi-hub", StaffRole.Owner, Active: true)]),
        new(
            "teacher@both.test",
            "Two-Centre Teacher",
            "en",
            [new("nile-centre", StaffRole.Teacher, Active: true), new("maadi-hub", StaffRole.Teacher, Active: true)]),
        new("secretary@nile.test", "Nile Secretary", "ar", [new("nile-centre", StaffRole.Secretary, Active: true)]),
        new("inactive@nile.test", "Former Secretary", "ar", [new("nile-centre", StaffRole.Secretary, Active: false)]),
    ];

    public async Task<Result> SeedAsync(CancellationToken ct)
    {
        var password = configuration[PasswordConfigurationKey];
        if (string.IsNullOrWhiteSpace(password))
        {
            return Result.Failure(Error.Rule(
                "seed.password_missing",
                $"Configuration key '{PasswordConfigurationKey}' is required to seed development staff."));
        }

        var centreIdsBySlug = await db.Set<Centre>().ToDictionaryAsync(centre => centre.Slug, centre => centre.Id, ct);

        foreach (var seedUser in Users)
        {
            var userId = await GetOrCreateUserAsync(seedUser, password, ct);
            if (userId is null)
            {
                return Result.Failure(Error.Rule("seed.user_creation_failed", $"Could not create user '{seedUser.Email}'."));
            }

            foreach (var membership in seedUser.Memberships)
            {
                if (!centreIdsBySlug.TryGetValue(membership.CentreSlug, out var centreId))
                {
                    return Result.Failure(Error.Rule(
                        "seed.centre_missing",
                        $"Centre '{membership.CentreSlug}' does not exist; seed centres before seeding staff."));
                }

                await EnsureMembershipAsync(userId.Value, centreId, membership, ct);
            }
        }

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }

    private async Task<Guid?> GetOrCreateUserAsync(SeedUser seedUser, string password, CancellationToken ct)
    {
        var existing = await userManager.FindByEmailAsync(seedUser.Email);
        if (existing is not null)
        {
            return existing.Id;
        }

        var user = new ApplicationUser
        {
            UserName = seedUser.Email,
            Email = seedUser.Email,
            EmailConfirmed = true,
            DisplayName = seedUser.DisplayName,
            PreferredLocale = seedUser.Locale,
        };

        var result = await userManager.CreateAsync(user, password);
        return result.Succeeded ? user.Id : null;
    }

    private async Task EnsureMembershipAsync(Guid userId, Guid centreId, SeedMembership spec, CancellationToken ct)
    {
        var alreadyExists = await db.Set<Membership>()
            .AnyAsync(membership => membership.UserId == userId && membership.CentreId == centreId, ct);
        if (alreadyExists)
        {
            return;
        }

        var membership = Membership.Create(userId, centreId, spec.Role).Value;
        if (!spec.Active)
        {
            membership.Deactivate();
        }

        db.Add(membership);
    }

    private sealed record SeedMembership(string CentreSlug, StaffRole Role, bool Active);

    private sealed record SeedUser(string Email, string DisplayName, string Locale, IReadOnlyList<SeedMembership> Memberships);
}
