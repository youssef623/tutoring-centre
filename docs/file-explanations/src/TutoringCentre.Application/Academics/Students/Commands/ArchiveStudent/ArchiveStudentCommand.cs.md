# `src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to take a student out of the active list without deleting their history, guarded by the version the client last read.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 512 bytes. Source hash: `b0ba04498207f9df80fcea4dac77a3a276a02a730310d412439ef3762bca41d5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentCommand.cs#L7): `public sealed record ArchiveStudentCommand(Guid StudentId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:136](../../../../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L136)
- [src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentHandler.cs:11](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentHandler.cs#L11)
- [src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs:21](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L21)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:47](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L47)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs:207](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L207)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs:65](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L65)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;

/// <summary>Intent to take a student out of the active list without deleting their history, guarded by the version the client last read.</summary>
public sealed record ArchiveStudentCommand(Guid StudentId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
