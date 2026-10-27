# `src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): The weekly rules of one group. No centre: tenancy comes from the actor; ownership from the actor's teacher scope.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 526 bytes. Source hash: `c523dcc01ffe459b195e47d5d7b55991eda77a99148a96576d12c2d60e0386b9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesQuery.cs#L7): `public sealed record ListGroupScheduleRulesQuery(Guid GroupId)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:605](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L605)
- [src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs:83](../../../../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L83)
- [src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs:15](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:89](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L89)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs:57](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs#L57)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTeacherScopeTests.cs:32](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTeacherScopeTests.cs#L32)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs:76](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs#L76)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs:74](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L74)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;

/// <summary>The weekly rules of one group. No centre: tenancy comes from the actor; ownership from the actor's teacher scope.</summary>
public sealed record ListGroupScheduleRulesQuery(Guid GroupId)
    : IQuery<IReadOnlyList<ScheduleRuleDto>>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ScheduleView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
