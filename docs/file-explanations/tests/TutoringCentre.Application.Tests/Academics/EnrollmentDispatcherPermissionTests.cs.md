# `tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Through the real dispatcher: tenancy and permission. Owner and Secretary hold enrollments.manage; Teacher holds only enrollments.view, so the three commands are denied for Teacher before any handler or transaction runs, while both queries are allowed (ownership narrows them later, in the read). An actor with no selected centre is refused every enrollment request as tenant.not_selected.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 193 lines and 9,273 bytes. Source hash: `a8b5cf7f13457b469ba4ba5af698c1fbc492b0d21887515fe53e3cd6184ccedb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 4: `using TutoringCentre.Application.Academics.Enrollments;`
- Line 5: `using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;`
- Line 6: `using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;`
- Line 7: `using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;`
- Line 8: `using TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;`
- Line 9: `using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;`
- Line 10: `using TutoringCentre.Application.Academics.Groups;`
- Line 11: `using TutoringCentre.Application.Academics.Students;`
- Line 12: `using TutoringCentre.Application.Common.Cqrs;`
- Line 13: `using TutoringCentre.Application.Common.Ports;`
- Line 14: `using TutoringCentre.Application.Common.Security;`
- Line 15: `using TutoringCentre.Application.Tests.Fakes;`
- Line 16: `using TutoringCentre.Domain.Academics;`
- Line 17: `using TutoringCentre.Domain.Common;`
- Line 18: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 28](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L28): `public sealed class EnrollmentDispatcherPermissionTests`
- [Line 41](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L41): `public async Task SendAsync_EveryCommand_ReturnsPermissionDeniedForTeacher(string commandName)`
- [Line 56](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L56): `public async Task SendAsync_EveryCommand_ReturnsTenantNotSelectedForACentrelessActor(string commandName)`
- [Line 70](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L70): `public async Task SendAsync_Enroll_SucceedsForOwnerAndSecretary(StaffRole role)`
- [Line 81](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L81): `public async Task QueryAsync_RosterAndStudentEnrollments_AreAllowedForTeacher()`
- [Line 96](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L96): `public async Task QueryAsync_RosterAndStudentEnrollments_ReturnTenantNotSelectedForACentrelessActor()`
- [Line 110](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L110): `private static async Task<Error?> SendAsync(Fixture fixture, string commandName)`
- [Line 127](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L127): `private static Fixture CreateSut(Actor actor)`
- [Line 170](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L170): `private sealed class Fixture(`
- [Line 191](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L191): `public void Dispose() => provider.Dispose();`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;
using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;
using TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;
using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Through the real dispatcher: tenancy and permission. Owner and Secretary hold enrollments.manage; Teacher holds
/// only enrollments.view, so the three commands are denied for Teacher before any handler or transaction runs,
/// while both queries are allowed (ownership narrows them later, in the read). An actor with no selected centre
/// is refused every enrollment request as tenant.not_selected.
/// </summary>
public sealed class EnrollmentDispatcherPermissionTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    public static TheoryData<string> CommandNames => new()
    {
        nameof(EnrollStudentCommand),
        nameof(WithdrawEnrollmentCommand),
        nameof(TransferEnrollmentCommand),
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
        Assert.Empty(fixture.Enrollments.Added);
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
    public async Task SendAsync_Enroll_SucceedsForOwnerAndSecretary(StaffRole role)
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, role));

        var error = await SendAsync(fixture, nameof(EnrollStudentCommand));

        Assert.Null(error);
        Assert.Single(fixture.Enrollments.Added);
    }

    [Fact]
    public async Task QueryAsync_RosterAndStudentEnrollments_AreAllowedForTeacher()
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, StaffRole.Teacher));

        var roster = await fixture.Dispatcher.QueryAsync<GetGroupRosterQuery, GroupRosterDto>(
            new GetGroupRosterQuery(Guid.CreateVersion7()), CancellationToken.None);
        var list = await fixture.Dispatcher.QueryAsync<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>(
            new ListStudentEnrollmentsQuery(fixture.Student.Id), CancellationToken.None);

        // Past the permission step: the read itself answers (not found for an unknown group, a list for a known student).
```

The excerpt stops at line 90; use the original file for the remaining 103 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
