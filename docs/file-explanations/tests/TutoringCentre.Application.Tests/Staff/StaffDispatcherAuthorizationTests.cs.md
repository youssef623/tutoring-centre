# `tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 29.11: through the real dispatcher, Secretary and Teacher are refused all five staff requests (auth.permission_denied), and a centre-less actor is refused all five before permission is even considered (tenant.not_selected) — the same two pipeline guarantees Day 28/22 already proved for Subjects.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 196 lines and 8,183 bytes. Source hash: `0e64c04aea3b7da65fc8416f90cb520952e30c64a96d965890d3558051a0effd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Application.Common.Security;`
- Line 6: `using TutoringCentre.Application.Staff;`
- Line 7: `using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;`
- Line 8: `using TutoringCentre.Application.Staff.Commands.CreateStaff;`
- Line 9: `using TutoringCentre.Application.Staff.Commands.DeactivateStaff;`
- Line 10: `using TutoringCentre.Application.Staff.Commands.ReactivateStaff;`
- Line 11: `using TutoringCentre.Application.Staff.Queries.ListStaff;`
- Line 12: `using TutoringCentre.Application.Tests.Fakes;`
- Line 13: `using TutoringCentre.Domain.Common;`
- Line 14: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 23](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L23): `public sealed class StaffDispatcherAuthorizationTests`
- [Line 30](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L30): `public async Task SendAsync_CreateStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- [Line 43](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L43): `public async Task SendAsync_ChangeStaffRole_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- [Line 56](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L56): `public async Task SendAsync_DeactivateStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- [Line 69](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L69): `public async Task SendAsync_ReactivateStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- [Line 82](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L82): `public async Task QueryAsync_ListStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- [Line 93](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L93): `public async Task SendAsync_CreateStaff_CentrelessActor_ReturnsTenantNotSelected()`
- [Line 104](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L104): `public async Task SendAsync_ChangeStaffRole_CentrelessActor_ReturnsTenantNotSelected()`
- [Line 115](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L115): `public async Task SendAsync_DeactivateStaff_CentrelessActor_ReturnsTenantNotSelected()`
- [Line 126](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L126): `public async Task SendAsync_ReactivateStaff_CentrelessActor_ReturnsTenantNotSelected()`
- [Line 137](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L137): `public async Task QueryAsync_ListStaff_CentrelessActor_ReturnsTenantNotSelected()`
- [Line 147](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L147): `private static void AssertPermissionDenied<T>(Result<T> result)`
- [Line 154](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L154): `private static void AssertTenantNotSelected<T>(Result<T> result)`
- [Line 162](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L162): `private static Fixture CreateSut(StaffRole? role)`
- [Line 190](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L190): `private sealed class Fixture(Dispatcher dispatcher, ServiceProvider provider) : IDisposable`
- [Line 194](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L194): `public void Dispose() => provider.Dispose();`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Application.Staff.Commands.DeactivateStaff;
using TutoringCentre.Application.Staff.Commands.ReactivateStaff;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Staff;

/// <summary>
/// Task 29.11: through the real dispatcher, Secretary and Teacher are refused all five staff requests
/// (auth.permission_denied), and a centre-less actor is refused all five before permission is even considered
/// (tenant.not_selected) — the same two pipeline guarantees Day 28/22 already proved for Subjects.
/// </summary>
public sealed class StaffDispatcherAuthorizationTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_CreateStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<CreateStaffCommand, CreateStaffResult>(
            new CreateStaffCommand("new@nile.test", "New Person", StaffRole.Teacher, "en"), CancellationToken.None);

        AssertPermissionDenied(result);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_ChangeStaffRole_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<ChangeStaffRoleCommand, Unit>(
            new ChangeStaffRoleCommand(Guid.CreateVersion7(), StaffRole.Teacher, 0), CancellationToken.None);

        AssertPermissionDenied(result);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_DeactivateStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<DeactivateStaffCommand, Unit>(
            new DeactivateStaffCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        AssertPermissionDenied(result);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_ReactivateStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<ReactivateStaffCommand, Unit>(
            new ReactivateStaffCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        AssertPermissionDenied(result);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task QueryAsync_ListStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.QueryAsync<ListStaffQuery, IReadOnlyList<StaffMemberDto>>(
            new ListStaffQuery(), CancellationToken.None);

        AssertPermissionDenied(result);
    }
```

The excerpt stops at line 90; use the original file for the remaining 106 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
