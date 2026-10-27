# `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 48.10: a transfer is one transaction that ends the old enrollment and starts a new one. It moves exactly one seat, changes nothing when it fails, and — because it locks only the target — two opposite transfers in parallel both complete instead of waiting on each other in a cycle.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 133 lines and 7,633 bytes. Source hash: `48a58da2133d1b574c30ae117c4d67e145cb840e425ba767e409982419aa3d8f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;`
- Line 2: `using TutoringCentre.Domain.Common;`
- Line 3: `using TutoringCentre.Domain.Identity;`
- Line 4: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs#L13): `public sealed class EnrollmentTransferTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)`
- [Line 18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs#L18): `public async Task Transfer_MovesTheSeatFromTheSourceToTheTarget()`
- [Line 39](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs#L39): `public async Task Transfer_IntoAFullTarget_ReturnsGroupFullAndLeavesBothGroupsUnchanged()`
- [Line 56](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs#L56): `public async Task Transfer_WithAStaleVersion_ReturnsConcurrencyStaleAndChangesNothing()`
- [Line 72](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs#L72): `public async Task TwoOppositeTransfersInParallel_BothCompleteWithoutDeadlock_AcrossTwentyRepetitions()`
- [Line 105](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs#L105): `private async Task<Result<TransferEnrollmentResult>> TransferAsync(World world, Guid enrollmentId, Guid targetId)`
- [Line 114](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs#L114): `private Task<string> SnapshotAsync(World world) =>`
- [Line 121](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs#L121): `private async Task<World> ArrangeAsync(string tag, int targetCapacity = 20)`
- [Line 132](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs#L132): `private sealed record World(CreatedStaff Secretary, Guid SourceId, Guid TargetId, IReadOnlyList<Guid> Students);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 48.10: a transfer is one transaction that ends the old enrollment and starts a new one. It moves exactly
/// one seat, changes nothing when it fails, and — because it locks only the target — two opposite transfers in
/// parallel both complete instead of waiting on each other in a cycle.
/// </summary>
public sealed class EnrollmentTransferTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)
{
    private const int Repetitions = 20;

    [Fact]
    public async Task Transfer_MovesTheSeatFromTheSourceToTheTarget()
    {
        var world = await ArrangeAsync("tr1");
        var enrollmentId = await EnrollOkAsync(world.Secretary.Actor, world.SourceId, world.Students[0]);
        await EnrollOkAsync(world.Secretary.Actor, world.SourceId, world.Students[1]);

        var moved = await TransferAsync(world, enrollmentId, world.TargetId);

        Assert.True(moved.IsSuccess, moved.Error?.Message);
        Assert.Equal(1, (await GetGroupAsync(world.Secretary.Actor, world.SourceId)).EnrolledCount);
        Assert.Equal(1, (await GetGroupAsync(world.Secretary.Actor, world.TargetId)).EnrolledCount);
        Assert.Equal(
            $"ended|transferred|{world.SourceId}",
            await Fixture.ScalarAsync<string>($"select concat_ws('|', status, end_reason, group_id) from academics.enrollments where id = '{enrollmentId}'"));
        Assert.Equal(
            $"active|{world.TargetId}|{world.Students[0]}",
            await Fixture.ScalarAsync<string>(
                $"select concat_ws('|', status, group_id, student_id) from academics.enrollments where id = '{moved.Value.EnrollmentId}'"));
    }

    [Fact]
    public async Task Transfer_IntoAFullTarget_ReturnsGroupFullAndLeavesBothGroupsUnchanged()
    {
        var world = await ArrangeAsync("tr2", targetCapacity: 1);
        var enrollmentId = await EnrollOkAsync(world.Secretary.Actor, world.SourceId, world.Students[0]);
        await EnrollOkAsync(world.Secretary.Actor, world.TargetId, world.Students[1]);
        var before = await SnapshotAsync(world);

        var moved = await TransferAsync(world, enrollmentId, world.TargetId);

        Assert.Equal("group.full", moved.Error!.Code);
        Assert.Equal(before, await SnapshotAsync(world));
        Assert.Equal(
            "active",
            await Fixture.ScalarAsync<string>($"select status from academics.enrollments where id = '{enrollmentId}'"));
    }

    [Fact]
    public async Task Transfer_WithAStaleVersion_ReturnsConcurrencyStaleAndChangesNothing()
    {
        var world = await ArrangeAsync("tr3");
        var enrollmentId = await EnrollOkAsync(world.Secretary.Actor, world.SourceId, world.Students[0]);
        var version = (await GetStudentEnrollmentAsync(world.Secretary.Actor, world.Students[0], enrollmentId)).Version;
        var before = await SnapshotAsync(world);

        var moved = await Fixture.SendAsAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(
            world.Secretary.Actor, new TransferEnrollmentCommand(enrollmentId, world.TargetId, version + 1));

        // The new row was inserted in the same save that found the old one stale, so the rollback removes both.
        Assert.Equal("concurrency.stale", moved.Error!.Code);
        Assert.Equal(before, await SnapshotAsync(world));
    }

    [Fact]
    public async Task TwoOppositeTransfersInParallel_BothCompleteWithoutDeadlock_AcrossTwentyRepetitions()
    {
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-opposite@nile.test", StaffRole.Secretary);
        var subjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var teacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        for (var iteration = 0; iteration < Repetitions; iteration++)
        {
            var groupA = await Fixture.SeedGroupAsync(NileCentreId, $"Opposite A {iteration:D2}", subjectId, teacherId);
            var groupB = await Fixture.SeedGroupAsync(NileCentreId, $"Opposite B {iteration:D2}", subjectId, teacherId);
            var students = await SeedStudentsAsync(NileCentreId, 2, $"Opposite {iteration:D2}");
            var inA = await EnrollOkAsync(secretary.Actor, groupA, students[0]);
            var inB = await EnrollOkAsync(secretary.Actor, groupB, students[1]);
            var versionA = (await GetStudentEnrollmentAsync(secretary.Actor, students[0], inA)).Version;
            var versionB = (await GetStudentEnrollmentAsync(secretary.Actor, students[1], inB)).Version;

            var results = await RunTogetherAsync(new Func<Task<Result<TransferEnrollmentResult>>>[]
            {
                () => Fixture.SendAsAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(
```

The excerpt stops at line 90; use the original file for the remaining 43 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
