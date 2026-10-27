# `src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): One group. No centre: tenancy comes from the actor; ownership from the actor's teacher scope.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 446 bytes. Source hash: `739ae823804dc40ac03447b824c0f3ddb19c3a46b6651dfad845993810a53203`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupQuery.cs#L7): `public sealed record GetGroupQuery(Guid GroupId) : IQuery<GroupDto>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs:103](../../../../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L103)
- [src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs:14](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs#L14)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:92](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L92)
- [tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs:55](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L55)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs:56](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L56)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs:30](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L30)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs:55](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L55)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Queries.GetGroup;

/// <summary>One group. No centre: tenancy comes from the actor; ownership from the actor's teacher scope.</summary>
public sealed record GetGroupQuery(Guid GroupId) : IQuery<GroupDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
