# `src/TutoringCentre.Infrastructure/Persistence/Schemas.cs`

## Purpose

Source summary (intent, not independent execution proof): PostgreSQL schema per feature module. Every entity configuration uses these in ToTable(name, schema).

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Persistence/Schemas.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 397 bytes. Source hash: `641a4d850ec192770bcef4a0ee0ed26206666a58ac371a127c301362f12a9697`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../src/TutoringCentre.Infrastructure/Persistence/Schemas.cs#L4): `internal static class Schemas`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [db/bootstrap-roles.sh:80](../../../../../db/bootstrap-roles.sh#L80)
- [src/TutoringCentre.Infrastructure/Persistence/AppDbContextOptionsConfigurator.cs:26](../../../../../src/TutoringCentre.Infrastructure/Persistence/AppDbContextOptionsConfigurator.cs#L26)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs:28](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs#L28)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs:29](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs#L29)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/GroupConfiguration.cs:25](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/GroupConfiguration.cs#L25)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ParentConfiguration.cs:19](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ParentConfiguration.cs#L19)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ScheduleRuleConfiguration.cs:24](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ScheduleRuleConfiguration.cs#L24)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs:25](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs#L25)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentParentLinkConfiguration.cs:25](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentParentLinkConfiguration.cs#L25)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/SubjectConfiguration.cs:20](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/SubjectConfiguration.cs#L20)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/TeacherConfiguration.cs:28](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/TeacherConfiguration.cs#L28)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs:25](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs#L25)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs:21](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs#L21)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs:19](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs#L19)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/IdentityUserClaimConfiguration.cs:13](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/IdentityUserClaimConfiguration.cs#L13)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/IdentityUserLoginConfiguration.cs:13](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/IdentityUserLoginConfiguration.cs#L13)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/IdentityUserTokenConfiguration.cs:13](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/IdentityUserTokenConfiguration.cs#L13)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs:22](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs#L22)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Platform/DataProtectionKeyConfiguration.cs:19](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Platform/DataProtectionKeyConfiguration.cs#L19)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.cs:30](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.cs#L30)
- 7 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>PostgreSQL schema per feature module. Every entity configuration uses these in ToTable(name, schema).</summary>
internal static class Schemas
{
    public const string Platform = "platform";
    public const string Identity = "identity";
    public const string Academics = "academics";
    public const string Audit = "audit";
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
