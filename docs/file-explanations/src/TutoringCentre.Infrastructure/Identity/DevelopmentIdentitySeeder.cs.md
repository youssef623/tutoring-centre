# `src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs`

## Purpose

Source summary (intent, not independent execution proof): Development-only bootstrapping, not a use case: creates a realistic two-centre staff set directly against Infrastructure, using the Domain's own factories so seeded data obeys the same rules as data created through the Application layer. Never runs at normal application startup or during build-time OpenAPI generation.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 123 lines and 5,409 bytes. Source hash: `c4b6f72e77976d6e4321dac943b7bd39ed6513222933b5d401f278f8a2fdf279`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.Identity;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using Microsoft.Extensions.Configuration;`
- Line 4: `using TutoringCentre.Domain.Centres;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`
- Line 7: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs#L16): `public sealed class DevelopmentIdentitySeeder(UserManager<ApplicationUser> userManager, AppDbContext db, IConfiguration configuration)`
- [Line 18](../../../../../src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs#L18): `/// <summary>The seeded account with a Teacher membership in both demo centres; the demo seed links it to a teacher record in each.</summary>`
- [Line 44](../../../../../src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs#L44): `public async Task<Result> SeedAsync(bool includeSetupCentreOwner, CancellationToken ct)`
- [Line 81](../../../../../src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs#L81): `private async Task<Guid?> GetOrCreateUserAsync(SeedUser seedUser, string password, CancellationToken ct)`
- [Line 102](../../../../../src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs#L102): `private async Task EnsureMembershipAsync(Guid userId, Guid centreId, SeedMembership spec, CancellationToken ct)`
- [Line 120](../../../../../src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs#L120): `private sealed record SeedMembership(string CentreSlug, StaffRole Role, bool Active);`
- [Line 122](../../../../../src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs#L122): `private sealed record SeedUser(string Email, string DisplayName, string Locale, IReadOnlyList<SeedMembership> Memberships);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:53](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L53)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:177](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L177)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs:236](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L236)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
    /// <summary>The seeded account with a Teacher membership in both demo centres; the demo seed links it to a teacher record in each.</summary>
    public const string TwoCentreTeacherEmail = "teacher@both.test";

    /// <summary>The empty centre the full-setup journey builds from nothing; seeded in Development and Testing only.</summary>
    public const string SetupCentreSlug = "giza-new";

    private const string PasswordConfigurationKey = "Seed:Password";

    private static readonly SeedUser[] Users =
    [
        new("owner@nile.test", "Nile Owner", "ar", [new("nile-centre", StaffRole.Owner, Active: true)]),
        new("owner@maadi.test", "Maadi Owner", "en", [new("maadi-hub", StaffRole.Owner, Active: true)]),
        new(
            TwoCentreTeacherEmail,
            "Two-Centre Teacher",
            "en",
            [new("nile-centre", StaffRole.Teacher, Active: true), new("maadi-hub", StaffRole.Teacher, Active: true)]),
        new("secretary@nile.test", "Nile Secretary", "ar", [new("nile-centre", StaffRole.Secretary, Active: true)]),
        new("inactive@nile.test", "Former Secretary", "ar", [new("nile-centre", StaffRole.Secretary, Active: false)]),
    ];

    /// <summary>The one owner of the empty setup centre the full-setup journey starts from (Development and Testing only).</summary>
    private static readonly SeedUser SetupCentreOwner =
        new("owner@giza.test", "Giza Owner", "ar", [new(SetupCentreSlug, StaffRole.Owner, Active: true)]);

    /// <param name="includeSetupCentreOwner">Also seed the owner of the empty setup centre, which must exist already.</param>
    public async Task<Result> SeedAsync(bool includeSetupCentreOwner, CancellationToken ct)
    {
        var password = configuration[PasswordConfigurationKey];
        if (string.IsNullOrWhiteSpace(password))
        {
            return Result.Failure(Error.Rule(
                "seed.password_missing",
                $"Configuration key '{PasswordConfigurationKey}' is required to seed development staff."));
        }

        var centreIdsBySlug = await db.Set<Centre>().ToDictionaryAsync(centre => centre.Slug, centre => centre.Id, ct);

        foreach (var seedUser in includeSetupCentreOwner ? [.. Users, SetupCentreOwner] : Users)
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
```

The excerpt stops at line 90; use the original file for the remaining 33 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
