# `tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 48.10, the schedule's Week 8 demo: the capacity lock (ADR 0011) under real parallel transactions against real PostgreSQL, each attempt in its own scope and connection, repeated because a race test that runs once can pass by luck. Every count is read through the superuser connection, not inferred from the returned results. A loser is a valid request that lost the race, so it gets the Domain's own answer, never an exception.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 144 lines and 8,007 bytes. Source hash: `6d169c2677b49f11171dcea6f58bb474ea9c899aa6642dc53f1501727342ece9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;`
- Line 3: `using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`
- Line 7: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs#L17): `public sealed class LastSeatConcurrencyTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)`
- [Line 25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs#L25): `public void AppConnectionPool_IsNotSmallerThanTheLargestParallelism()`
- [Line 33](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs#L33): `public async Task TenParallelEnrollmentsIntoTheLastSeat_ExactlyOneSucceedsAndNineGetGroupFull_AcrossTwentyRepetitions()`
- [Line 62](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs#L62): `public async Task FiftyParallelEnrollmentsIntoASingleSeatGroup_ExactlyOneSucceeds()`
- [Line 83](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs#L83): `public async Task TheSameStudentEnrolledTwiceInParallel_OneSucceedsAndTheOtherGetsAlreadyEnrolled()`
- [Line 106](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs#L106): `public async Task EnrollAndShrinkInParallel_NeverLeaveMoreStudentsThanSeats()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Npgsql;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 48.10, the schedule's Week 8 demo: the capacity lock (ADR 0011) under real parallel transactions against
/// real PostgreSQL, each attempt in its own scope and connection, repeated because a race test that runs once can
/// pass by luck. Every count is read through the superuser connection, not inferred from the returned results.
/// A loser is a valid request that lost the race, so it gets the Domain's own answer, never an exception.
/// </summary>
public sealed class LastSeatConcurrencyTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)
{
    private const int Repetitions = 20;
    private const int LastSeatContenders = 10;
    private const int SingleSeatContenders = 50;
    private const int SingleSeatRepetitions = 5;

    [Fact]
    public void AppConnectionPool_IsNotSmallerThanTheLargestParallelism()
    {
        // A pool smaller than the parallelism would queue attempts in the client, not at the row lock, and the
        // races below would stop testing the database.
        Assert.True(new NpgsqlConnectionStringBuilder(Fixture.AppConnectionString).MaxPoolSize >= SingleSeatContenders);
    }

    [Fact]
    public async Task TenParallelEnrollmentsIntoTheLastSeat_ExactlyOneSucceedsAndNineGetGroupFull_AcrossTwentyRepetitions()
    {
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-last-seat@nile.test", StaffRole.Secretary);
        var subjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var teacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        for (var iteration = 0; iteration < Repetitions; iteration++)
        {
            // A fresh group per repetition, so no seat taken in one repetition counts toward the next.
            var groupId = await Fixture.SeedGroupAsync(NileCentreId, $"Last Seat {iteration:D2}", subjectId, teacherId, capacity: 20);
            await SeedEnrollmentsAsync(NileCentreId, groupId, await SeedStudentsAsync(NileCentreId, 19, $"Seated {iteration:D2}"));
            var contenders = await SeedStudentsAsync(NileCentreId, LastSeatContenders, $"Contender {iteration:D2}");

            var results = await RunTogetherAsync(contenders.Select(studentId =>
                (Func<Task<Result<EnrollStudentResult>>>)(() => EnrollAsync(secretary.Actor, groupId, studentId))));

            Assert.Equal(1, results.Count(result => result.IsSuccess));
            Assert.Equal(LastSeatContenders - 1, results.Count(result => result.IsFailure && result.Error!.Code == "group.full"));
            Assert.Equal(20, await ActiveRowsAsync(groupId));

            // The nine losers rolled back, audit rows included: only the winner's enrollment was audited.
            Assert.Equal(
                1,
                await Fixture.ScalarAsync<long>(
                    $"select count(*) from audit.audit_entries where entity_type = 'enrollment' and changes->'groupId'->>'after' = '{groupId}'"));
        }
    }

    [Fact]
    public async Task FiftyParallelEnrollmentsIntoASingleSeatGroup_ExactlyOneSucceeds()
    {
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-single-seat@nile.test", StaffRole.Secretary);
        var subjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var teacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        for (var iteration = 0; iteration < SingleSeatRepetitions; iteration++)
        {
            var groupId = await Fixture.SeedGroupAsync(NileCentreId, $"Single Seat {iteration:D2}", subjectId, teacherId, capacity: 1);
            var contenders = await SeedStudentsAsync(NileCentreId, SingleSeatContenders, $"Single {iteration:D2}");

            var results = await RunTogetherAsync(contenders.Select(studentId =>
                (Func<Task<Result<EnrollStudentResult>>>)(() => EnrollAsync(secretary.Actor, groupId, studentId))));

            Assert.Equal(1, results.Count(result => result.IsSuccess));
            Assert.All(results.Where(result => result.IsFailure), result => Assert.Equal("group.full", result.Error!.Code));
            Assert.Equal(1, await ActiveRowsAsync(groupId));
        }
    }

    [Fact]
    public async Task TheSameStudentEnrolledTwiceInParallel_OneSucceedsAndTheOtherGetsAlreadyEnrolled()
    {
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-same-student@nile.test", StaffRole.Secretary);
        var subjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var teacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        for (var iteration = 0; iteration < Repetitions; iteration++)
        {
```

The excerpt stops at line 90; use the original file for the remaining 54 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
