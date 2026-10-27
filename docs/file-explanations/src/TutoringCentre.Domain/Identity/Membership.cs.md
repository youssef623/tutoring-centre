# `src/TutoringCentre.Domain/Identity/Membership.cs`

## Purpose

Source summary (intent, not independent execution proof): A staff member's role in one centre. The only way to obtain one is `Create`, which enforces its invariants. Deliberately not `ITenantOwned`: memberships are read at login, before a tenant is selected, so they cannot be filtered by the current tenant the way centre-owned data is.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Identity/Membership.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 85 lines and 2,569 bytes. Source hash: `97157928a3f481a67b8a23cb6d40dd433841ff46f4323c6589db59fc2c93ae19`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../src/TutoringCentre.Domain/Identity/Membership.cs#L10): `public sealed class Membership : Entity`
- [Line 12](../../../../../src/TutoringCentre.Domain/Identity/Membership.cs#L12): `private Membership(Guid userId, Guid centreId, StaffRole role)`
- [Line 21](../../../../../src/TutoringCentre.Domain/Identity/Membership.cs#L21): `private Membership()`
- [Line 33](../../../../../src/TutoringCentre.Domain/Identity/Membership.cs#L33): `public static Result<Membership> Create(Guid userId, Guid centreId, StaffRole role)`
- [Line 48](../../../../../src/TutoringCentre.Domain/Identity/Membership.cs#L48): `public Result ChangeRole(StaffRole role)`
- [Line 64](../../../../../src/TutoringCentre.Domain/Identity/Membership.cs#L64): `public Result Deactivate()`
- [Line 75](../../../../../src/TutoringCentre.Domain/Identity/Membership.cs#L75): `public Result Activate()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:13](../../../../onboarding-work/extend_backend_purposes.py#L13)
- [src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs:35](../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs#L35)
- [src/TutoringCentre.Application/Staff/IMembershipRepository.cs:18](../../../../../src/TutoringCentre.Application/Staff/IMembershipRepository.cs#L18)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs:7](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs#L7)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs:14](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs#L14)
- [src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs:25](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs#L25)
- [src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs:104](../../../../../src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs#L104)
- [src/TutoringCentre.Infrastructure/Identity/StaffDirectory.cs:19](../../../../../src/TutoringCentre.Infrastructure/Identity/StaffDirectory.cs#L19)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs:11](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs#L11)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.Designer.cs:160](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.Designer.cs#L160)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.Designer.cs:160](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.Designer.cs#L160)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261008190450_AddSubjects.Designer.cs:218](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261008190450_AddSubjects.Designer.cs#L218)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009092027_AddMembershipRowVersion.Designer.cs:218](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009092027_AddMembershipRowVersion.Designer.cs#L218)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009210855_AddAuditLog.Designer.cs:218](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009210855_AddAuditLog.Designer.cs#L218)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010052343_AddCentreRowVersion.Designer.cs:224](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010052343_AddCentreRowVersion.Designer.cs#L224)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010163013_AddDataProtectionKeys.Designer.cs:247](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010163013_AddDataProtectionKeys.Designer.cs#L247)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.Designer.cs:437](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.Designer.cs#L437)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.Designer.cs:510](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.Designer.cs#L510)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.Designer.cs:588](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.Designer.cs#L588)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261023174541_AddEnrollments.Designer.cs:668](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261023174541_AddEnrollments.Designer.cs#L668)
- 14 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Identity;

/// <summary>
/// A staff member's role in one centre. The only way to obtain one is <see cref="Create"/>, which enforces its invariants.
/// Deliberately not <see cref="ITenantOwned"/>: memberships are read at login, before a tenant is selected, so they
/// cannot be filtered by the current tenant the way centre-owned data is.
/// </summary>
public sealed class Membership : Entity
{
    private Membership(Guid userId, Guid centreId, StaffRole role)
    {
        UserId = userId;
        CentreId = centreId;
        Role = role;
        Status = MembershipStatus.Active;
    }

    // For EF Core materialisation only; EF then sets every property from the row.
    private Membership()
    {
    }

    public Guid UserId { get; private set; }

    public Guid CentreId { get; private set; }

    public StaffRole Role { get; private set; }

    public MembershipStatus Status { get; private set; }

    public static Result<Membership> Create(Guid userId, Guid centreId, StaffRole role)
    {
        if (userId == Guid.Empty)
        {
            return Result<Membership>.Failure(Error.Validation("membership.user_required", "A user is required."));
        }

        if (centreId == Guid.Empty)
        {
            return Result<Membership>.Failure(Error.Validation("membership.centre_required", "A centre is required."));
        }

        return Result<Membership>.Success(new Membership(userId, centreId, role));
    }

    public Result ChangeRole(StaffRole role)
    {
        if (Status == MembershipStatus.Inactive)
        {
            return Result.Failure(Error.Rule("membership.inactive", "This membership is inactive."));
        }

        if (Role == role)
        {
            return Result.Failure(Error.Rule("membership.role_unchanged", "The role is already this value."));
        }

        Role = role;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (Status == MembershipStatus.Inactive)
        {
            return Result.Failure(Error.Rule("membership.already_inactive", "This membership is already inactive."));
        }

        Status = MembershipStatus.Inactive;
        return Result.Success();
    }

    public Result Activate()
    {
        if (Status == MembershipStatus.Active)
        {
            return Result.Failure(Error.Rule("membership.already_active", "This membership is already active."));
        }

        Status = MembershipStatus.Active;
        return Result.Success();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
