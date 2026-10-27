# `src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to change a staff member's role within the acting actor's own centre. No centre: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleCommand.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 512 bytes. Source hash: `b2dd527529596872fdbcd23dcf13c23f9eca964f0cb2495cfd00946233c3a176`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleCommand.cs#L7): `public sealed record ChangeStaffRoleCommand(Guid MembershipId, StaffRole Role, uint Version)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs:82](../../../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L82)
- [src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleHandler.cs:16](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleHandler.cs#L16)
- [src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleValidator.cs:5](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleValidator.cs#L5)
- [tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs:221](../../../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L221)
- [tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs:20](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs#L20)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:47](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L47)
- [tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs:80](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L80)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs:126](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L126)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs:28](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs#L28)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs:40](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs#L40)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs:43](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L43)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff.Commands.ChangeStaffRole;

/// <summary>Intent to change a staff member's role within the acting actor's own centre. No centre: scope comes from the actor.</summary>
public sealed record ChangeStaffRoleCommand(Guid MembershipId, StaffRole Role, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StaffManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
