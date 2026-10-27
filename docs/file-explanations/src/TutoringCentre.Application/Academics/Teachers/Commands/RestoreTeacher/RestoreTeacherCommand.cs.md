# `src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to bring an archived teacher back, guarded by the version the client last read. The kept account link takes effect again.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 516 bytes. Source hash: `5e592b6061c3d700d303e4e02580895b94028e643f6fd5277ac7ebcf7959ad85`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherCommand.cs#L7): `public sealed record RestoreTeacherCommand(Guid TeacherId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs:119](../../../../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L119)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherHandler.cs:9](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherHandlerTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherValidatorTests.cs:10](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherValidatorTests.cs#L10)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:134](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L134)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs:102](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L102)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;

/// <summary>Intent to bring an archived teacher back, guarded by the version the client last read. The kept account link takes effect again.</summary>
public sealed record RestoreTeacherCommand(Guid TeacherId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
