# `src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to give a student of the acting centre a seat in a group of the same centre.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 494 bytes. Source hash: `8bfcd8fc228b81b91ff2e5b0da21e4dbc9e9fe179fa20802a50f032594209568`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentCommand.cs#L7): `public sealed record EnrollStudentCommand(Guid GroupId, Guid StudentId)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:372](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L372)
- [src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs:79](../../../../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L79)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:25](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L25)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs:24](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L24)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:34](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L34)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs:76](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L76)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;

/// <summary>Intent to give a student of the acting centre a seat in a group of the same centre.</summary>
public sealed record EnrollStudentCommand(Guid GroupId, Guid StudentId)
    : ICommand<EnrollStudentResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.EnrollmentsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
