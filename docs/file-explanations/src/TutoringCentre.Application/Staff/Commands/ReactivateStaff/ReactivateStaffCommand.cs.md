# `src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to restore a deactivated staff member's access to the acting actor's own centre. No centre: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffCommand.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 507 bytes. Source hash: `ef2011e518a873a1a5567d8ff0f9d5a5d0e03f06765474ba90aa4b15e95f2d71`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffCommand.cs#L7): `public sealed record ReactivateStaffCommand(Guid MembershipId, uint Version)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs:95](../../../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L95)
- [src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffHandler.cs:11](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffHandler.cs#L11)
- [src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffValidator.cs:5](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Staff/ReactivateStaffHandlerTests.cs:21](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/ReactivateStaffHandlerTests.cs#L21)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:73](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L73)
- [tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs:104](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L104)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs:66](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs#L66)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs:78](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L78)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff.Commands.ReactivateStaff;

/// <summary>Intent to restore a deactivated staff member's access to the acting actor's own centre. No centre: scope comes from the actor.</summary>
public sealed record ReactivateStaffCommand(Guid MembershipId, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StaffManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
