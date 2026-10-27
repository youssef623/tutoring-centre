# `src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): Who holds a seat in one group. No centre: tenancy comes from the actor; ownership from the actor's teacher scope.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 494 bytes. Source hash: `b204c26945bb393e7d8ab0cc561890b75fc393b927323b5cd9c860e82649bae6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterQuery.cs#L7): `public sealed record GetGroupRosterQuery(Guid GroupId) : IQuery<GroupRosterDto>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs:85](../../../../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L85)
- [src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs:15](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:85](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L85)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs:24](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs#L24)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs:21](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs#L21)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs:87](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L87)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;

/// <summary>Who holds a seat in one group. No centre: tenancy comes from the actor; ownership from the actor's teacher scope.</summary>
public sealed record GetGroupRosterQuery(Guid GroupId) : IQuery<GroupRosterDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.EnrollmentsView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
