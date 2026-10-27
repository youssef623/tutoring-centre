# `src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to change a group's number of seats only, guarded by the version the client last read.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 499 bytes. Source hash: `4d740d18dc94bb4ea404fdbb5c0f171a7f3775e7e7bf6825d5bd0bb2f1a81ba5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityCommand.cs#L7): `public sealed record ChangeGroupCapacityCommand(Guid GroupId, int Capacity, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs:123](../../../../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L123)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityHandler.cs:13](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityHandler.cs#L13)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityValidator.cs:6](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityValidator.cs#L6)
- [tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs:17](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs#L17)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:38](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L38)
- [tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs:60](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L60)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs:199](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L199)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs:29](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L29)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs:82](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L82)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs:122](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs#L122)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;

/// <summary>Intent to change a group's number of seats only, guarded by the version the client last read.</summary>
public sealed record ChangeGroupCapacityCommand(Guid GroupId, int Capacity, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
