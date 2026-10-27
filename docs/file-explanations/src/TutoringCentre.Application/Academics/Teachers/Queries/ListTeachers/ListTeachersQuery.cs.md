# `src/TutoringCentre.Application/Academics/Teachers/Queries/ListTeachers/ListTeachersQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): The teachers list for the page and for the teacher picker on group forms. No centre: scope comes from the actor. Permission only — no role holds this partially.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Queries/ListTeachers/ListTeachersQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 552 bytes. Source hash: `9dd823dff4d22f195282e2f92da841a0a59525887bdf2b0aebc5898f3a8fb700`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Queries/ListTeachers/ListTeachersQuery.cs#L7): `public sealed record ListTeachersQuery(bool IncludeArchived) : IQuery<IReadOnlyList<TeacherDto>>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:442](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L442)
- [src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs:80](../../../../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L80)
- [src/TutoringCentre.Application/Academics/Teachers/Queries/ListTeachers/ListTeachersHandler.cs:9](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Queries/ListTeachers/ListTeachersHandler.cs#L9)
- [tests/TutoringCentre.Application.Tests/Academics/ListTeachersHandlerTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ListTeachersHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:160](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L160)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs:32](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L32)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;

/// <summary>The teachers list for the page and for the teacher picker on group forms. No centre: scope comes from the actor. Permission only — no role holds this partially.</summary>
public sealed record ListTeachersQuery(bool IncludeArchived) : IQuery<IReadOnlyList<TeacherDto>>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
