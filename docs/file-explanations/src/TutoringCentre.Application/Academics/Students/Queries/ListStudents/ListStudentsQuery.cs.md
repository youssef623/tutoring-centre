# `src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): The student list behind the student directory page. Every filter is optional; the caller cannot pass a centre.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 539 bytes. Source hash: `4d8ac780725814b0b54b47d9ba860e2aa89d1d308b6739f79206dc11b5d82e9b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsQuery.cs#L7): `public sealed record ListStudentsQuery(string? Search, bool IncludeArchived, string? Cursor, int PageSize)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:269](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L269)
- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:108](../../../../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L108)
- [src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsHandler.cs:8](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsHandler.cs#L8)
- [src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsValidator.cs:6](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsValidator.cs#L6)
- [tests/TutoringCentre.Application.Tests/Academics/ListStudentsHandlerTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/ListStudentsValidatorTests.cs:10](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsValidatorTests.cs#L10)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:79](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L79)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs:29](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L29)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Queries.ListStudents;

/// <summary>The student list behind the student directory page. Every filter is optional; the caller cannot pass a centre.</summary>
public sealed record ListStudentsQuery(string? Search, bool IncludeArchived, string? Cursor, int PageSize)
    : IQuery<StudentPageDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
