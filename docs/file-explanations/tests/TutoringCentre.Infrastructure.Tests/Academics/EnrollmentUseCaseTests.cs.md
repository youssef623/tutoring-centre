# `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 48.10: the enrollment use cases and the rules they added to earlier ones, through the real dispatcher, real adapters and real PostgreSQL, plus the audit rows each command writes through the policy alone.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 222 lines and 12,764 bytes. Source hash: `66f8490ad1690c6894fafc5f086f37f432a7eae9ea06ecda069d92255018e1d4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Enrollments;`
- Line 2: `using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;`
- Line 3: `using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;`
- Line 4: `using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;`
- Line 5: `using TutoringCentre.Application.Academics.Groups;`
- Line 6: `using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;`
- Line 7: `using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;`
- Line 8: `using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;`
- Line 9: `using TutoringCentre.Application.Academics.Students;`
- Line 10: `using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;`
- Line 11: `using TutoringCentre.Application.Academics.Students.Queries.GetStudent;`
- Line 12: `using TutoringCentre.Application.Common.Cqrs;`
- Line 13: `using TutoringCentre.Domain.Academics;`
- Line 14: `using TutoringCentre.Domain.Common;`
- Line 15: `using TutoringCentre.Domain.Identity;`
- Line 16: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 24](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L24): `public sealed class EnrollmentUseCaseTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)`
- [Line 30](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L30): `public async Task Withdraw_FreesTheSeatThatTheNextEnrollmentTakes()`
- [Line 48](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L48): `public async Task ReEnrollAfterWithdrawal_CreatesASecondRowAndKeepsTheFirstAsHistory()`
- [Line 70](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L70): `public async Task ArchiveGroup_WithAnActiveEnrollment_IsRefusedUntilTheStudentIsWithdrawn()`
- [Line 85](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L85): `public async Task ArchiveStudent_WithAnActiveEnrollment_IsRefusedUntilTheStudentIsWithdrawn()`
- [Line 100](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L100): `public async Task ChangeCapacity_BelowTheEnrolledCount_IsRefusedAndEqualToItSucceeds()`
- [Line 115](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L115): `public async Task GroupListDetailAndRoster_ShowTheRealEnrolledCountAndNamesOnly()`
- [Line 136](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L136): `public async Task Enroll_WritesOneCreatedAuditRow()`
- [Line 151](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L151): `public async Task Withdraw_WritesOneUpdatedAuditRowFromActiveToEndedWithTheReason()`
- [Line 166](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L166): `public async Task Transfer_WritesAnUpdatedRowForTheOldEnrollmentAndACreatedRowForTheNewOne()`
- [Line 187](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L187): `private async Task<Result<Unit>> WithdrawAsync(World world, Guid enrollmentId)`
- [Line 194](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L194): `private async Task<Result<Unit>> ArchiveGroupAsync(World world) =>`
- [Line 198](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L198): `private async Task<Result<Unit>> ChangeCapacityAsync(World world, int capacity) =>`
- [Line 203](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L203): `private async Task<Result<Unit>> ArchiveStudentAsync(World world, Guid studentId)`
- [Line 210](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L210): `private async Task<World> ArrangeAsync(string tag, int capacity = 20)`
- [Line 221](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L221): `private sealed record World(CreatedStaff Secretary, Guid SubjectId, Guid TeacherId, Guid GroupId, IReadOnlyList<Guid> Students);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;
using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;
using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;
using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;
using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;
using TutoringCentre.Application.Academics.Students.Queries.GetStudent;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 48.10: the enrollment use cases and the rules they added to earlier ones, through the real dispatcher,
/// real adapters and real PostgreSQL, plus the audit rows each command writes through the policy alone.
/// </summary>
public sealed class EnrollmentUseCaseTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)
{
    // Names only: no phone or any other contact field of a student or parent may appear on a roster entry.
    private static readonly string[] RosterEntryFields = ["EnrolledAt", "EnrollmentId", "StudentId", "StudentName", "Version"];

    [Fact]
    public async Task Withdraw_FreesTheSeatThatTheNextEnrollmentTakes()
    {
        var world = await ArrangeAsync("uc1", capacity: 1);
        var first = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);
        Assert.Equal("group.full", (await EnrollAsync(world.Secretary.Actor, world.GroupId, world.Students[1])).Error!.Code);

        var withdrawn = await WithdrawAsync(world, first);
        var second = await EnrollAsync(world.Secretary.Actor, world.GroupId, world.Students[1]);

        Assert.True(withdrawn.IsSuccess, withdrawn.Error?.Message);
        Assert.True(second.IsSuccess, second.Error?.Message);
        Assert.Equal(1, await ActiveRowsAsync(world.GroupId));
        Assert.Equal(
            "ended|withdrawn",
            await Fixture.ScalarAsync<string>($"select concat_ws('|', status, end_reason) from academics.enrollments where id = '{first}'"));
    }

    [Fact]
    public async Task ReEnrollAfterWithdrawal_CreatesASecondRowAndKeepsTheFirstAsHistory()
    {
        var world = await ArrangeAsync("uc2");
        var first = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);
        Assert.True((await WithdrawAsync(world, first)).IsSuccess);

        var second = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);

        Assert.NotEqual(first, second);
        Assert.Equal(
            2,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.enrollments where student_id = '{world.Students[0]}' and group_id = '{world.GroupId}'"));
        Assert.Equal(1, await ActiveRowsAsync(world.GroupId));
        var history = await Fixture.QueryAsAsync<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>(
            world.Secretary.Actor, new ListStudentEnrollmentsQuery(world.Students[0]));
        Assert.Equal([second, first], history.Value.Select(enrollment => enrollment.EnrollmentId).ToArray());
        Assert.Equal(EnrollmentStatus.Active, history.Value[0].Status);
        Assert.Equal(EnrollmentEndReason.Withdrawn, history.Value[1].EndReason);
    }

    [Fact]
    public async Task ArchiveGroup_WithAnActiveEnrollment_IsRefusedUntilTheStudentIsWithdrawn()
    {
        var world = await ArrangeAsync("uc3");
        var enrollmentId = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);

        var refused = await ArchiveGroupAsync(world);
        Assert.Equal("group.has_active_enrollments", refused.Error!.Code);
        Assert.Equal("active", await Fixture.ScalarAsync<string>($"select status from academics.groups where id = '{world.GroupId}'"));

        Assert.True((await WithdrawAsync(world, enrollmentId)).IsSuccess);
        var archived = await ArchiveGroupAsync(world);
        Assert.True(archived.IsSuccess, archived.Error?.Message);
    }

    [Fact]
    public async Task ArchiveStudent_WithAnActiveEnrollment_IsRefusedUntilTheStudentIsWithdrawn()
    {
        var world = await ArrangeAsync("uc4");
        var enrollmentId = await EnrollOkAsync(world.Secretary.Actor, world.GroupId, world.Students[0]);

        var refused = await ArchiveStudentAsync(world, world.Students[0]);
```

The excerpt stops at line 90; use the original file for the remaining 132 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
