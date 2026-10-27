# `tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Through the real dispatcher: tenancy and permission, the first two of the three checks. Owner and Secretary hold groups.manage; Teacher holds only groups.view, so all six commands are denied for Teacher before any handler or transaction runs, while both queries are allowed (ownership narrows them later, in the read). An actor with no selected centre is refused every group request as tenant.not_selected.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 203 lines and 9,717 bytes. Source hash: `9cddd31eac4266b5c4cfa7478e0056c150e37eeb961030b5641767951d0ebd71`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 4: `using TutoringCentre.Application.Academics.Groups;`
- Line 5: `using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;`
- Line 6: `using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;`
- Line 7: `using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;`
- Line 8: `using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;`
- Line 9: `using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;`
- Line 10: `using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;`
- Line 11: `using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;`
- Line 12: `using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;`
- Line 13: `using TutoringCentre.Application.Academics.Subjects;`
- Line 14: `using TutoringCentre.Application.Academics.Teachers;`
- Line 15: `using TutoringCentre.Application.Common.Cqrs;`
- Line 16: `using TutoringCentre.Application.Common.Security;`
- Line 17: `using TutoringCentre.Application.Tests.Fakes;`
- Line 18: `using TutoringCentre.Domain.Academics;`
- Line 19: `using TutoringCentre.Domain.Common;`
- Line 20: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 30](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L30): `public sealed class GroupDispatcherPermissionTests`
- [Line 46](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L46): `public async Task SendAsync_EveryCommand_ReturnsPermissionDeniedForTeacher(string commandName)`
- [Line 61](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L61): `public async Task SendAsync_EveryCommand_ReturnsTenantNotSelectedForACentrelessActor(string commandName)`
- [Line 75](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L75): `public async Task SendAsync_CreateGroup_SucceedsForOwnerAndSecretary(StaffRole role)`
- [Line 86](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L86): `public async Task QueryAsync_ListAndGet_AreAllowedForTeacher()`
- [Line 100](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L100): `public async Task QueryAsync_ListAndGet_ReturnTenantNotSelectedForACentrelessActor()`
- [Line 114](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L114): `private static async Task<Error?> SendAsync(Fixture fixture, string commandName)`
- [Line 138](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L138): `private static Fixture CreateSut(Actor actor)`
- [Line 180](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L180): `private sealed class Fixture(`
- [Line 201](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L201): `public void Dispose() => provider.Dispose();`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;
using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;
using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;
using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;
using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;
using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Through the real dispatcher: tenancy and permission, the first two of the three checks. Owner and Secretary
/// hold groups.manage; Teacher holds only groups.view, so all six commands are denied for Teacher before any
/// handler or transaction runs, while both queries are allowed (ownership narrows them later, in the read).
/// An actor with no selected centre is refused every group request as tenant.not_selected.
/// </summary>
public sealed class GroupDispatcherPermissionTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    public static TheoryData<string> CommandNames => new()
    {
        nameof(CreateGroupCommand),
        nameof(RenameGroupCommand),
        nameof(ChangeGroupCapacityCommand),
        nameof(AssignGroupTeacherCommand),
        nameof(ArchiveGroupCommand),
        nameof(RestoreGroupCommand),
    };

    [Theory]
    [MemberData(nameof(CommandNames))]
    public async Task SendAsync_EveryCommand_ReturnsPermissionDeniedForTeacher(string commandName)
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, StaffRole.Teacher));

        var error = await SendAsync(fixture, commandName);

        Assert.NotNull(error);
        Assert.Equal("auth.permission_denied", error.Code);
        Assert.Equal(ErrorKind.Forbidden, error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
        Assert.Empty(fixture.Groups.Added);
    }

    [Theory]
    [MemberData(nameof(CommandNames))]
    public async Task SendAsync_EveryCommand_ReturnsTenantNotSelectedForACentrelessActor(string commandName)
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), null, null));

        var error = await SendAsync(fixture, commandName);

        Assert.NotNull(error);
        Assert.Equal("tenant.not_selected", error.Code);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Theory]
    [InlineData(StaffRole.Owner)]
    [InlineData(StaffRole.Secretary)]
    public async Task SendAsync_CreateGroup_SucceedsForOwnerAndSecretary(StaffRole role)
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, role));

        var error = await SendAsync(fixture, nameof(CreateGroupCommand));

        Assert.Null(error);
        Assert.Single(fixture.Groups.Added);
    }

    [Fact]
    public async Task QueryAsync_ListAndGet_AreAllowedForTeacher()
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, StaffRole.Teacher));

        var list = await fixture.Dispatcher.QueryAsync<ListGroupsQuery, IReadOnlyList<GroupDto>>(
```

The excerpt stops at line 90; use the original file for the remaining 113 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
