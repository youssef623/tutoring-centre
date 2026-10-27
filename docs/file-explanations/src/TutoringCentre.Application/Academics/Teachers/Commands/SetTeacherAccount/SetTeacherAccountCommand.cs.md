# `src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to link a teacher record to a staff user's login, or to clear the link when `UserId` is null. This is the action that gives the ownership guard (Day 44) its meaning. Guarded by the version the client last read. No centre: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 15 lines and 698 bytes. Source hash: `b822d5a753b38471497af752f36a808a8e34aaa446e2e25e76d819b6b46367c9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountCommand.cs#L7): `/// Intent to link a teacher record to a staff user's login, or to clear the link when <see cref="UserId"/> is`
- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountCommand.cs#L11): `public sealed record SetTeacherAccountCommand(Guid TeacherId, Guid? UserId, uint Version)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:491](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L491)
- [src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs:106](../../../../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L106)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs:20](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs#L20)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs:26](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L26)
- [tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountValidatorTests.cs:10](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountValidatorTests.cs#L10)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:86](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L86)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs:37](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L37)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs:30](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L30)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs:107](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L107)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;

/// <summary>
/// Intent to link a teacher record to a staff user's login, or to clear the link when <see cref="UserId"/> is
/// null. This is the action that gives the ownership guard (Day 44) its meaning. Guarded by the version the
/// client last read. No centre: scope comes from the actor.
/// </summary>
public sealed record SetTeacherAccountCommand(Guid TeacherId, Guid? UserId, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
