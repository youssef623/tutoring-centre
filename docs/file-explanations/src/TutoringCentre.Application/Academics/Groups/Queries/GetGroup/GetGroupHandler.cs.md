# `src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Uniform not-found: an unknown ID, another centre's group and a group outside the actor's teacher scope all answer group.not_found, never 403, so the response never confirms that the group exists.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 1,162 bytes. Source hash: `41396117542586a444906d9c530632e7d3e733f9404117c47e81031e49eeb0be`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs#L13): `internal sealed class GetGroupHandler(ITeacherScopeProvider scopeProvider, IGroupReadService readService)`
- [Line 16](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs#L16): `public async Task<Result<GroupDto>> HandleAsync(GetGroupQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:167](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L167)
- [tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs:53](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L53)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Queries.GetGroup;

/// <summary>
/// Uniform not-found: an unknown ID, another centre's group and a group outside the actor's teacher scope all
/// answer group.not_found, never 403, so the response never confirms that the group exists.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetGroupHandler(ITeacherScopeProvider scopeProvider, IGroupReadService readService)
    : IQueryHandler<GetGroupQuery, GroupDto>
{
    public async Task<Result<GroupDto>> HandleAsync(GetGroupQuery query, CancellationToken cancellationToken)
    {
        var scope = await scopeProvider.GetAsync(cancellationToken);
        var group = await readService.GetAsync(query.GroupId, scope, cancellationToken);
        return group is null ? Result<GroupDto>.Failure(GroupErrors.NotFound()) : Result<GroupDto>.Success(group);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
