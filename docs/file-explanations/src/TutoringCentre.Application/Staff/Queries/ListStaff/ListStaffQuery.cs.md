# `src/TutoringCentre.Application/Staff/Queries/ListStaff/ListStaffQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): The list behind the staff page. No centre, no parameters: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Staff/Queries/ListStaff/ListStaffQuery.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 435 bytes. Source hash: `b25f8862d8b7ed61316454f34872da966175412bb43f1c30f3a13406f00537f8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../src/TutoringCentre.Application/Staff/Queries/ListStaff/ListStaffQuery.cs#L7): `public sealed record ListStaffQuery : IQuery<IReadOnlyList<StaffMemberDto>>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs:64](../../../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L64)
- [src/TutoringCentre.Application/Staff/Queries/ListStaff/ListStaffHandler.cs:10](../../../../../../../src/TutoringCentre.Application/Staff/Queries/ListStaff/ListStaffHandler.cs#L10)
- [tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs:217](../../../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L217)
- [tests/TutoringCentre.Application.Tests/Staff/ListStaffHandlerTests.cs:20](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/ListStaffHandlerTests.cs#L20)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:86](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L86)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs:81](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs#L81)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffProbeTestBase.cs:28](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffProbeTestBase.cs#L28)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs:93](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L93)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff.Queries.ListStaff;

/// <summary>The list behind the staff page. No centre, no parameters: scope comes from the actor.</summary>
public sealed record ListStaffQuery : IQuery<IReadOnlyList<StaffMemberDto>>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StaffView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
