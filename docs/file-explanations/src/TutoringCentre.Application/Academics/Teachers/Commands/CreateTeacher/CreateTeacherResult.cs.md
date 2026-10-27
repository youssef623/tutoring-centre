# `src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherResult.cs`

## Purpose

Declares `CreateTeacherResult`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherResult.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 3 lines and 139 bytes. Source hash: `2f83434cc694c5551bb6272b1ba46ef444134a1550daa9229063a60bbf2f0968`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherResult.cs#L3): `public sealed record CreateTeacherResult(Guid TeacherId);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:467](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L467)
- [src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs:92](../../../../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L92)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherCommand.cs:11](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherCommand.cs#L11)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs:12](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs#L12)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:37](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L37)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs:33](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L33)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs:181](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L181)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs:28](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L28)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs:104](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L104)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;

public sealed record CreateTeacherResult(Guid TeacherId);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
