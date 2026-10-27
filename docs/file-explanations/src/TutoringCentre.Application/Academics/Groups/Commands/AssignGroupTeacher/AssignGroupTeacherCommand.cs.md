# `src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to move a group to another teacher, guarded by the version the client last read. This changes which teacher account can see the group.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 13 lines and 561 bytes. Source hash: `c1ee4f6aa43c1aaa584e7ae39d138c0ad3d30306cf98100427bfadb57a042697`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherCommand.cs#L10): `public sealed record AssignGroupTeacherCommand(Guid GroupId, Guid TeacherId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs:131](../../../../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L131)
- [src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs:24](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs#L24)
- [src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:39](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L39)
- [tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs:68](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L68)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs:82](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L82)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs:29](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L29)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs:98](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L98)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs:171](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L171)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;

/// <summary>
/// Intent to move a group to another teacher, guarded by the version the client last read. This changes which
/// teacher account can see the group.
/// </summary>
public sealed record AssignGroupTeacherCommand(Guid GroupId, Guid TeacherId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
