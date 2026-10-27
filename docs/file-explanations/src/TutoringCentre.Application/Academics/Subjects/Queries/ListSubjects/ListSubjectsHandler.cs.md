# `src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): A read needs no invariants, so it skips the entity and delegates straight to the read service.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 16 lines and 899 bytes. Source hash: `1ac186922dbaef62c9f8b81a1088faf6c528f750fdcc2fa84c43093c22338559`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsHandler.cs#L9): `internal sealed class ListSubjectsHandler(ISubjectReadService readService) : IQueryHandler<ListSubjectsQuery, IReadOnlyList<SubjectDto>>`
- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsHandler.cs#L11): `public async Task<Result<IReadOnlyList<SubjectDto>>> HandleAsync(ListSubjectsQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ListSubjectsHandlerTests.cs:16](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ListSubjectsHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:187](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L187)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:119](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L119)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;

/// <summary>A read needs no invariants, so it skips the entity and delegates straight to the read service.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ListSubjectsHandler(ISubjectReadService readService) : IQueryHandler<ListSubjectsQuery, IReadOnlyList<SubjectDto>>
{
    public async Task<Result<IReadOnlyList<SubjectDto>>> HandleAsync(ListSubjectsQuery query, CancellationToken cancellationToken)
    {
        var subjects = await readService.ListAsync(query.IncludeArchived, cancellationToken);
        return Result<IReadOnlyList<SubjectDto>>.Success(subjects);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
