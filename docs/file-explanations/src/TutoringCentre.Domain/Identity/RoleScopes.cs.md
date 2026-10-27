# `src/TutoringCentre.Domain/Identity/RoleScopes.cs`

## Purpose

Source summary (intent, not independent execution proof): Which roles are limited to their own records on an ownership-guarded request, rather than the whole centre's. Kept beside `RolePermissions`, in the Domain, so handlers never compare against `StaffRole` directly to make this decision themselves.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Identity/RoleScopes.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 12 lines and 586 bytes. Source hash: `dc31e47d91c6d35389d2b047aaa29290b4e9b05835d756b14fca73ff70b4cdd0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../src/TutoringCentre.Domain/Identity/RoleScopes.cs#L8): `public static class RoleScopes`
- [Line 11](../../../../../src/TutoringCentre.Domain/Identity/RoleScopes.cs#L11): `public static bool IsLimitedToOwnGroups(StaffRole role) => role == StaffRole.Teacher;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Identity/TeacherScopeProvider.cs:41](../../../../../src/TutoringCentre.Infrastructure/Identity/TeacherScopeProvider.cs#L41)
- [tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs:47](../../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L47)
- [tests/TutoringCentre.Domain.Tests/Identity/RoleScopesTests.cs:13](../../../../../tests/TutoringCentre.Domain.Tests/Identity/RoleScopesTests.cs#L13)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Domain.Identity;

/// <summary>
/// Which roles are limited to their own records on an ownership-guarded request, rather than the whole
/// centre's. Kept beside <see cref="RolePermissions"/>, in the Domain, so handlers never compare against
/// <see cref="StaffRole"/> directly to make this decision themselves.
/// </summary>
public static class RoleScopes
{
    /// <summary>True only for Teacher: Owner and Secretary always see every group in the centre.</summary>
    public static bool IsLimitedToOwnGroups(StaffRole role) => role == StaffRole.Teacher;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
