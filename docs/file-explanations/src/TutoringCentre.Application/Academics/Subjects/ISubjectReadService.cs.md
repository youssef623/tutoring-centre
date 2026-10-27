# `src/TutoringCentre.Application/Academics/Subjects/ISubjectReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-side port: returns DTO-shaped data directly. Never exposes the Subject entity or EF types.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Subjects/ISubjectReadService.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 16 lines and 889 bytes. Source hash: `49087f5194f35e2ad8d317fdcca24f9f4d7b968ec60f70761376656fabbbd0d0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../src/TutoringCentre.Application/Academics/Subjects/ISubjectReadService.cs#L6): `public interface ISubjectReadService`
- [Line 16](../../../../../../src/TutoringCentre.Application/Academics/Subjects/ISubjectReadService.cs#L16): `public sealed record SubjectDto(Guid Id, string Name, SubjectStatus Status, uint Version);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Subjects/Queries/GetSubject/GetSubjectHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/GetSubject/GetSubjectHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsHandler.cs#L9)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:129](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L129)
- [src/TutoringCentre.Infrastructure/ReadServices/SubjectReadService.cs:17](../../../../../../src/TutoringCentre.Infrastructure/ReadServices/SubjectReadService.cs#L17)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:182](../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L182)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:114](../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L114)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectReadService.cs:6](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectReadService.cs#L6)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Subjects;

/// <summary>Read-side port: returns DTO-shaped data directly. Never exposes the Subject entity or EF types.</summary>
public interface ISubjectReadService
{
    /// <summary>Subjects ordered by name, scoped to the acting centre. Capped at 500 rows; there is no paging.</summary>
    Task<IReadOnlyList<SubjectDto>> ListAsync(bool includeArchived, CancellationToken ct);

    /// <summary>One subject by ID, or null when it does not exist or is not visible to the acting centre.</summary>
    Task<SubjectDto?> GetAsync(Guid id, CancellationToken ct);
}

/// <summary>The one shape every subject read returns. No centre, no normalized name, no timestamps — those are persistence details.</summary>
public sealed record SubjectDto(Guid Id, string Name, SubjectStatus Status, uint Version);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
