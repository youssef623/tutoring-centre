# `src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): The list behind the subjects page and every subject picker. No centre: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 488 bytes. Source hash: `ba3fa700cbe068c081e26a2931e541fbe289dd2d4a208ca98341e5cb0ada5a85`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsQuery.cs#L7): `public sealed record ListSubjectsQuery(bool IncludeArchived) : IQuery<IReadOnlyList<SubjectDto>>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:443](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L443)
- [src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs:70](../../../../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L70)
- [src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsHandler.cs:9](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsHandler.cs#L9)
- [tests/TutoringCentre.Application.Tests/Academics/ListSubjectsHandlerTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ListSubjectsHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:134](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L134)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:78](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L78)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs:86](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L86)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs:34](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L34)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;

/// <summary>The list behind the subjects page and every subject picker. No centre: scope comes from the actor.</summary>
public sealed record ListSubjectsQuery(bool IncludeArchived) : IQuery<IReadOnlyList<SubjectDto>>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.SubjectsView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
