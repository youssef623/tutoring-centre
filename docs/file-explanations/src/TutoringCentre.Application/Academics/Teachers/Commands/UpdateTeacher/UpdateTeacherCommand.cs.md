# `src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to replace a teacher's name and phone, guarded by the version the client last read. Full replacement: a missing phone clears it. The account link is never touched here. No centre: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 15 lines and 654 bytes. Source hash: `8a0583c2649f7b6af2ce7eb2e0011566f5842a62433fecac97fe951f76556a8d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherCommand.cs#L11): `public sealed record UpdateTeacherCommand(Guid TeacherId, string FullName, string? Phone, uint Version)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs:99](../../../../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L99)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherHandler.cs:13](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherHandler.cs#L13)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherValidator.cs:7](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherValidator.cs#L7)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:62](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L62)
- [tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs:19](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs#L19)
- [tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherValidatorTests.cs:10](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherValidatorTests.cs#L10)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs:48](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L48)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;

/// <summary>
/// Intent to replace a teacher's name and phone, guarded by the version the client last read. Full
/// replacement: a missing phone clears it. The account link is never touched here. No centre: scope comes
/// from the actor.
/// </summary>
public sealed record UpdateTeacherCommand(Guid TeacherId, string FullName, string? Phone, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
