# `src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsHandler.cs`

## Purpose

Declares `ListStudentsHandler`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 16 lines and 844 bytes. Source hash: `172b334c9c6289739f7d037bed1554025ce014fc38854f0af4ff3fe4b623adcb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsHandler.cs#L8): `internal sealed class ListStudentsHandler(IStudentReadService readService) : IQueryHandler<ListStudentsQuery, StudentPageDto>`
- [Line 10](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsHandler.cs#L10): `public async Task<Result<StudentPageDto>> HandleAsync(ListStudentsQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ListStudentsHandlerTests.cs:16](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:175](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L175)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Queries.ListStudents;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ListStudentsHandler(IStudentReadService readService) : IQueryHandler<ListStudentsQuery, StudentPageDto>
{
    public async Task<Result<StudentPageDto>> HandleAsync(ListStudentsQuery query, CancellationToken cancellationToken)
    {
        var criteria = new StudentSearchCriteria(query.Search, query.IncludeArchived, query.Cursor, query.PageSize);
        var page = await readService.ListAsync(criteria, cancellationToken);
        return Result<StudentPageDto>.Success(page);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
