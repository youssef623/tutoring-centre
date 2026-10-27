# `src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): A missing parent is an error, an empty collection is an answer: an unknown or foreign student is student.not_found, while a known student with no visible enrollment is an empty list, never a 404.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 25 lines and 1,446 bytes. Source hash: `c51ee8e31f0d3f250cf38f5cd32fdfe8d7b4af65634a58f5f72afbc1913812fd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Students;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs#L14): `internal sealed class ListStudentEnrollmentsHandler(ITeacherScopeProvider scopeProvider, IEnrollmentReadService readService)`
- [Line 17](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs#L17): `public async Task<Result<IReadOnlyList<StudentEnrollmentDto>>> HandleAsync(ListStudentEnrollmentsQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:160](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L160)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs:49](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs#L49)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;

/// <summary>
/// A missing parent is an error, an empty collection is an answer: an unknown or foreign student is
/// student.not_found, while a known student with no visible enrollment is an empty list, never a 404.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ListStudentEnrollmentsHandler(ITeacherScopeProvider scopeProvider, IEnrollmentReadService readService)
    : IQueryHandler<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>
{
    public async Task<Result<IReadOnlyList<StudentEnrollmentDto>>> HandleAsync(ListStudentEnrollmentsQuery query, CancellationToken cancellationToken)
    {
        var scope = await scopeProvider.GetAsync(cancellationToken);
        var enrollments = await readService.ListForStudentAsync(query.StudentId, scope, cancellationToken);
        return enrollments is null
            ? Result<IReadOnlyList<StudentEnrollmentDto>>.Failure(StudentErrors.NotFound())
            : Result<IReadOnlyList<StudentEnrollmentDto>>.Success(enrollments);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
