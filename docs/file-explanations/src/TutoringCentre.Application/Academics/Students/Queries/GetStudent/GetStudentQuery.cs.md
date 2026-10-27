# `src/TutoringCentre.Application/Academics/Students/Queries/GetStudent/GetStudentQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): Used by the student detail page. No centre: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/GetStudent/GetStudentQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 442 bytes. Source hash: `de0de58b2aeba9b696226b755d9323d89dc24cf4ffb5733ad048f3277e9a10d4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/GetStudent/GetStudentQuery.cs#L7): `public sealed record GetStudentQuery(Guid StudentId) : IQuery<StudentDetailDto>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:115](../../../../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L115)
- [src/TutoringCentre.Application/Academics/Students/Queries/GetStudent/GetStudentHandler.cs:9](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/GetStudent/GetStudentHandler.cs#L9)
- [tests/TutoringCentre.Application.Tests/Academics/GetStudentHandlerTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GetStudentHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:68](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L68)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs:205](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L205)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs:127](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L127)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Queries.GetStudent;

/// <summary>Used by the student detail page. No centre: scope comes from the actor.</summary>
public sealed record GetStudentQuery(Guid StudentId) : IQuery<StudentDetailDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
