# `src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentResult.cs`

## Purpose

Declares `RegisterStudentResult`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentResult.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 3 lines and 158 bytes. Source hash: `6ee26b0706fe22195709fb4582870638cba40f555ba026a519a8376a61816d74`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentResult.cs#L3): `public sealed record RegisterStudentResult(Guid StudentId, Guid ParentId);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:281](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L281)
- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:122](../../../../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L122)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentCommand.cs:14](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentCommand.cs#L14)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs:18](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:40](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L40)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs:16](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L16)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs:10](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs#L10)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;

public sealed record RegisterStudentResult(Guid StudentId, Guid ParentId);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
