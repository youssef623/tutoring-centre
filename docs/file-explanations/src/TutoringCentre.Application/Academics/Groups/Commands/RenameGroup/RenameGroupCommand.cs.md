# `src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to change a group's name only, guarded by the version the client last read.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 471 bytes. Source hash: `f347877401d5528bfbcdea06be9d1eea19e3a4d299876611fdf50993de0fe788`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupCommand.cs#L7): `public sealed record RenameGroupCommand(Guid GroupId, string Name, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs:116](../../../../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L116)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupHandler.cs:12](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupHandler.cs#L12)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupValidator.cs:7](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupValidator.cs#L7)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:37](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L37)
- [tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs:44](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L44)
- [tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs:17](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs#L17)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs:29](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L29)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs:55](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L55)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;

/// <summary>Intent to change a group's name only, guarded by the version the client last read.</summary>
public sealed record RenameGroupCommand(Guid GroupId, string Name, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
