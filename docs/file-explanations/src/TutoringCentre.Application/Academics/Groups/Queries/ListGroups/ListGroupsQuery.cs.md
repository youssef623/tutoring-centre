# `src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): The groups list. No centre: tenancy comes from the actor. The filters are intersected with the actor's teacher scope, so a scoped teacher filtering by another teacher gets an empty list, not an error.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 632 bytes. Source hash: `afc1911fc381b3ef401768e1afccc845ce38ba8f7c54a5f29af891e2e4db85b4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsQuery.cs#L10): `public sealed record ListGroupsQuery(Guid? SubjectId, Guid? TeacherId, bool IncludeArchived)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:347](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L347)
- [src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs:96](../../../../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L96)
- [src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsHandler.cs:11](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsHandler.cs#L11)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:90](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L90)
- [tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs:22](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L22)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs:124](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L124)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs:118](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L118)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Queries.ListGroups;

/// <summary>
/// The groups list. No centre: tenancy comes from the actor. The filters are intersected with the actor's
/// teacher scope, so a scoped teacher filtering by another teacher gets an empty list, not an error.
/// </summary>
public sealed record ListGroupsQuery(Guid? SubjectId, Guid? TeacherId, bool IncludeArchived)
    : IQuery<IReadOnlyList<GroupDto>>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
