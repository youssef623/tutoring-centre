# `src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to put an archived student back on the active list, guarded by the version the client last read.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 491 bytes. Source hash: `37ab654e0536e662263b5bdca808a623144c99cc4cbfb58a8535b611a5be420c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentCommand.cs#L7): `public sealed record RestoreStudentCommand(Guid StudentId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:142](../../../../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L142)
- [src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentHandler.cs:9](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs:98](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L98)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:50](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L50)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.RestoreStudent;

/// <summary>Intent to put an archived student back on the active list, guarded by the version the client last read.</summary>
public sealed record RestoreStudentCommand(Guid StudentId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
