# `src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to replace a student's name, phone and school year, guarded by the version the client last read. Full replacement: a missing phone or school year clears it, it does not mean "keep the old one". Parent links are untouched. No centre: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 15 lines and 727 bytes. Source hash: `4b93033efbbb75fcaa621648bad9f3120011fae80235ed8638073385257572cd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentCommand.cs#L11): `public sealed record UpdateStudentCommand(Guid StudentId, string FullName, string? Phone, string? SchoolYear, uint Version)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:129](../../../../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L129)
- [src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentHandler.cs:13](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentHandler.cs#L13)
- [src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentValidator.cs:7](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentValidator.cs#L7)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:44](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L44)
- [tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs:21](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs#L21)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs:140](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L140)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;

/// <summary>
/// Intent to replace a student's name, phone and school year, guarded by the version the client last read.
/// Full replacement: a missing phone or school year clears it, it does not mean "keep the old one". Parent
/// links are untouched. No centre: scope comes from the actor.
/// </summary>
public sealed record UpdateStudentCommand(Guid StudentId, string FullName, string? Phone, string? SchoolYear, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
