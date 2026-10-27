# `src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to close a group without deleting it, guarded by the version the client last read.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 467 bytes. Source hash: `a419cd840fb239a0655452ff19f67289a6c76f091257cddb9de171093f400c89`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupCommand.cs#L7): `public sealed record ArchiveGroupCommand(Guid GroupId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs:138](../../../../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L138)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs:22](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs#L22)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs:17](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L17)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:40](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L40)
- [tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs:76](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L76)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs:195](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L195)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs:30](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L30)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs:113](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L113)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs:204](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L204)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;

/// <summary>Intent to close a group without deleting it, guarded by the version the client last read.</summary>
public sealed record ArchiveGroupCommand(Guid GroupId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
