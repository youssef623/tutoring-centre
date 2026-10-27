# `src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Uniform not-found: an unknown ID, another centre's group and a group outside the actor's teacher scope all answer group.not_found, the same as reading the group itself.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 25 lines and 1,367 bytes. Source hash: `91ffb233b3d90def204ba1b52da9c2382b3f86bf4d94618e480c301fe68622d3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Groups;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs#L14): `internal sealed class ListGroupScheduleRulesHandler(ITeacherScopeProvider scopeProvider, IScheduleReadService readService)`
- [Line 17](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs#L17): `public async Task<Result<IReadOnlyList<ScheduleRuleDto>>> HandleAsync(ListGroupScheduleRulesQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:155](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L155)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs:55](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs#L55)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;

/// <summary>
/// Uniform not-found: an unknown ID, another centre's group and a group outside the actor's teacher scope all
/// answer group.not_found, the same as reading the group itself.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ListGroupScheduleRulesHandler(ITeacherScopeProvider scopeProvider, IScheduleReadService readService)
    : IQueryHandler<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>
{
    public async Task<Result<IReadOnlyList<ScheduleRuleDto>>> HandleAsync(ListGroupScheduleRulesQuery query, CancellationToken cancellationToken)
    {
        var scope = await scopeProvider.GetAsync(cancellationToken);
        var rules = await readService.ListRulesAsync(query.GroupId, scope, cancellationToken);
        return rules is null
            ? Result<IReadOnlyList<ScheduleRuleDto>>.Failure(GroupErrors.NotFound())
            : Result<IReadOnlyList<ScheduleRuleDto>>.Success(rules);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
