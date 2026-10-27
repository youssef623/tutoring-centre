# `src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to reopen an archived group, guarded by the version the client last read.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 458 bytes. Source hash: `b2e4c479a8e5ffe53d2ebc53d1d9a8e5d4720d597851acb6e28b63a74433400c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupCommand.cs#L7): `public sealed record RestoreGroupCommand(Guid GroupId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs:144](../../../../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L144)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs:16](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs#L16)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:41](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L41)
- [tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs:84](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L84)
- [tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs:17](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs#L17)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs:30](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L30)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs:119](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L119)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;

/// <summary>Intent to reopen an archived group, guarded by the version the client last read.</summary>
public sealed record RestoreGroupCommand(Guid GroupId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
