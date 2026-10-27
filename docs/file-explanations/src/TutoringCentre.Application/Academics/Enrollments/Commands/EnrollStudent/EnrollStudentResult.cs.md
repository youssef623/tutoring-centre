# `src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentResult.cs`

## Purpose

Declares `EnrollStudentResult`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentResult.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 3 lines and 145 bytes. Source hash: `6a8753f26b449525e6f9294a4762084a9eaa74d7bdbcd9b7490ece055a6b2872`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentResult.cs#L3): `public sealed record EnrollStudentResult(Guid EnrollmentId);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:372](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L372)
- [src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs:79](../../../../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L79)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentCommand.cs:8](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentCommand.cs#L8)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:25](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L25)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs:111](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L111)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:115](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L115)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs:75](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L75)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs:47](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs#L47)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;

public sealed record EnrollStudentResult(Guid EnrollmentId);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
