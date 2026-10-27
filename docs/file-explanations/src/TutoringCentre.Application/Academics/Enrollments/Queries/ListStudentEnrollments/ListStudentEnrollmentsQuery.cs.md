# `src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): A student's current and past enrollments, limited to groups within the actor's teacher scope.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 517 bytes. Source hash: `ce431af1dce3939aaf555721d611bb8de9eca77ef11bc4cb3cf5a270b1ba3c3f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsQuery.cs#L7): `public sealed record ListStudentEnrollmentsQuery(Guid StudentId)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs:91](../../../../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L91)
- [src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs:15](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:87](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L87)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs:51](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs#L51)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs:35](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs#L35)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs:94](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L94)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs:62](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L62)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;

/// <summary>A student's current and past enrollments, limited to groups within the actor's teacher scope.</summary>
public sealed record ListStudentEnrollmentsQuery(Guid StudentId)
    : IQuery<IReadOnlyList<StudentEnrollmentDto>>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.EnrollmentsView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
