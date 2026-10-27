# `src/TutoringCentre.Infrastructure/Persistence/DatabaseRoles.cs`

## Purpose

Source summary (intent, not independent execution proof): The two least-privilege PostgreSQL roles bootstrapped outside migrations (db/bootstrap-roles.sh). Named once so grant migrations and the readiness check can't drift from what was actually created.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Persistence/DatabaseRoles.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 636 bytes. Source hash: `b56480f1599729bc92ddc4bcc27d24c44733dcaa5d7c8f0b09a3216007a90d9a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Infrastructure/Persistence/DatabaseRoles.cs#L7): `public static class DatabaseRoles`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.cs:20](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.cs#L20)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261008190450_AddSubjects.cs:60](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261008190450_AddSubjects.cs#L60)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010163013_AddDataProtectionKeys.cs:35](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261010163013_AddDataProtectionKeys.cs#L35)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.cs:159](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.cs#L159)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.cs:76](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.cs#L76)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.cs:82](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.cs#L82)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261023174541_AddEnrollments.cs:84](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261023174541_AddEnrollments.cs#L84)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.cs:165](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.cs#L165)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/AppendOnlyAccessGrants.cs:18](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/AppendOnlyAccessGrants.cs#L18)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/RuntimeAccessGrants.cs:19](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/RuntimeAccessGrants.cs#L19)
- [src/TutoringCentre.Infrastructure/Persistence/RuntimeRolePrivilegeHealthCheck.cs:27](../../../../../src/TutoringCentre.Infrastructure/Persistence/RuntimeRolePrivilegeHealthCheck.cs#L27)
- [tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs:38](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs#L38)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/RowLevelSecurityCoverageTests.cs:61](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/RowLevelSecurityCoverageTests.cs#L61)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:56](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L56)
- [tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs:27](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L27)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// The two least-privilege PostgreSQL roles bootstrapped outside migrations (db/bootstrap-roles.sh). Named once so
/// grant migrations and the readiness check can't drift from what was actually created.
/// </summary>
public static class DatabaseRoles
{
    /// <summary>Owns the schemas and tables; migrates. Never used by the running app.</summary>
    public const string Owner = "tutoring_owner";

    /// <summary>The app's runtime login: no DDL rights beyond what later migrations grant it explicitly.</summary>
    public const string App = "tutoring_app";
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
