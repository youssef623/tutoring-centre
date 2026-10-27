# `src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to add a staff member to the acting actor's own centre. The centre comes from the actor, never the caller.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffCommand.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 13 lines and 640 bytes. Source hash: `744284405094b45f144e54d78037ce503989e8bc9cafcc811b2e64386df2e61c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffCommand.cs#L7): `public sealed record CreateStaffCommand(string Email, string DisplayName, StaffRole Role, string PreferredLocale)`
- [Line 13](../../../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffCommand.cs#L13): `public sealed record CreateStaffResult(Guid MembershipId, Guid UserId, string? TemporaryPassword);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs:74](../../../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L74)
- [src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs:16](../../../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs#L16)
- [src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffValidator.cs:6](../../../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffValidator.cs#L6)
- [tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs:20](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs#L20)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:34](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L34)
- [tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs:14](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L14)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs:145](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L145)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs:75](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs#L75)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffProbeTestBase.cs:20](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffProbeTestBase.cs#L20)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs:84](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs#L84)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs:87](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs#L87)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs:21](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L21)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff.Commands.CreateStaff;

/// <summary>Intent to add a staff member to the acting actor's own centre. The centre comes from the actor, never the caller.</summary>
public sealed record CreateStaffCommand(string Email, string DisplayName, StaffRole Role, string PreferredLocale)
    : ICommand<CreateStaffResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StaffManage;
}

public sealed record CreateStaffResult(Guid MembershipId, Guid UserId, string? TemporaryPassword);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
