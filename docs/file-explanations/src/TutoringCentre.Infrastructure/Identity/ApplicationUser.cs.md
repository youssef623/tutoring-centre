# `src/TutoringCentre.Infrastructure/Identity/ApplicationUser.cs`

## Purpose

Source summary (intent, not independent execution proof): The Identity persistence model for a staff user. This is Infrastructure's own type, not a Domain entity: nothing outside Infrastructure (other than the composition root and CLI) may reference it.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Identity/ApplicationUser.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 20 lines and 739 bytes. Source hash: `ffb4842034ba8c37dbf4fb3d64d0dbaa26e35182e6d3b0a83a73bae912b2f17a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../src/TutoringCentre.Infrastructure/Identity/ApplicationUser.cs#L9): `public sealed class ApplicationUser : IdentityUser<Guid>`
- [Line 11](../../../../../src/TutoringCentre.Infrastructure/Identity/ApplicationUser.cs#L11): `public ApplicationUser() => Id = Guid.CreateVersion7();`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/ResetPasswordCommand.cs:31](../../../../../src/TutoringCentre.Api/Cli/ResetPasswordCommand.cs#L31)
- [src/TutoringCentre.Api/Cli/SeedCommand.cs:420](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L420)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:161](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L161)
- [src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs:16](../../../../../src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs#L16)
- [src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs:18](../../../../../src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs#L18)
- [src/TutoringCentre.Infrastructure/Identity/StaffAccountService.cs:16](../../../../../src/TutoringCentre.Infrastructure/Identity/StaffAccountService.cs#L16)
- [src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs:21](../../../../../src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs#L21)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs:49](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs#L49)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs:8](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs#L8)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs:34](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs#L34)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.Designer.cs:212](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.Designer.cs#L212)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.Designer.cs:212](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.Designer.cs#L212)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261008190450_AddSubjects.Designer.cs:270](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261008190450_AddSubjects.Designer.cs#L270)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009092027_AddMembershipRowVersion.Designer.cs:276](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009092027_AddMembershipRowVersion.Designer.cs#L276)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009210855_AddAuditLog.Designer.cs:353](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009210855_AddAuditLog.Designer.cs#L353)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010052343_AddCentreRowVersion.Designer.cs:359](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010052343_AddCentreRowVersion.Designer.cs#L359)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010163013_AddDataProtectionKeys.Designer.cs:382](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010163013_AddDataProtectionKeys.Designer.cs#L382)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.Designer.cs:572](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.Designer.cs#L572)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.Designer.cs:645](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.Designer.cs#L645)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.Designer.cs:723](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.Designer.cs#L723)
- 12 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
