# `src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to open a group for an active subject and an active teacher in the acting actor's own centre.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 524 bytes. Source hash: `77774c422641c79fdfdae66eadb5e7be4f529cccd851590642424a0fa4e7ab2a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupCommand.cs#L7): `public sealed record CreateGroupCommand(string Name, Guid SubjectId, Guid TeacherId, int Capacity)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:523](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L523)
- [src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs:109](../../../../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L109)
- [src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs:22](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs#L22)
- [src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupValidator.cs:7](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupValidator.cs#L7)
- [tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs:20](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L20)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:36](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L36)
- [tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L18)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs:63](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L63)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs:47](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L47)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs:148](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L148)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;

/// <summary>Intent to open a group for an active subject and an active teacher in the acting actor's own centre.</summary>
public sealed record CreateGroupCommand(string Name, Guid SubjectId, Guid TeacherId, int Capacity)
    : ICommand<CreateGroupResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
