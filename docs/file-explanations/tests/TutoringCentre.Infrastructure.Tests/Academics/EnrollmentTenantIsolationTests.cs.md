# `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 48.10: tenancy in both directions. Every ID in an enrollment command is resolved inside the actor's own centre, so another centre's group, student or enrollment answers the same not-found as an unknown ID — never a foreign-key 500 from the composite keys behind them — and the other centre's rows are unchanged.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 131 lines and 6,484 bytes. Source hash: `66613a499e728bacb665d95f9074a260c4bd5bfaf352dcd9472e3249c6c9805e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;`
- Line 2: `using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;`
- Line 3: `using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`
- Line 7: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L16): `public sealed class EnrollmentTenantIsolationTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)`
- [Line 22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L22): `public async Task Enroll_IntoTheOtherCentresGroup_ReturnsGroupNotFound(bool nileActsOnMaadi)`
- [Line 33](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L33): `public async Task Enroll_TheOtherCentresStudentIntoAnOwnGroup_ReturnsStudentNotFound(bool nileActsOnMaadi)`
- [Line 45](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L45): `public async Task Withdraw_TheOtherCentresEnrollment_ReturnsEnrollmentNotFound(bool nileActsOnMaadi)`
- [Line 58](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L58): `public async Task Transfer_TheOtherCentresEnrollment_ReturnsEnrollmentNotFound(bool nileActsOnMaadi)`
- [Line 70](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L70): `public async Task Transfer_AnOwnEnrollmentIntoTheOtherCentresGroup_ReturnsGroupNotFound(bool nileActsOnMaadi)`
- [Line 82](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L82): `private async Task AssertRefusedAsync(Result result, string code, CentreWorld foreign)`
- [Line 90](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L90): `private static (CentreWorld Own, CentreWorld Foreign) Pick((CentreWorld Nile, CentreWorld Maadi) world, bool nileActsOnMaadi) =>`
- [Line 94](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L94): `private Task<string> SnapshotAsync(Guid centreId) =>`
- [Line 112](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L112): `private async Task<CentreWorld> ArrangeCentreAsync(Guid centreId, string tag)`
- [Line 123](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L123): `private sealed record CentreWorld(`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;
using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 48.10: tenancy in both directions. Every ID in an enrollment command is resolved inside the actor's own
/// centre, so another centre's group, student or enrollment answers the same not-found as an unknown ID — never
/// a foreign-key 500 from the composite keys behind them — and the other centre's rows are unchanged.
/// </summary>
public sealed class EnrollmentTenantIsolationTests(PostgresFixture fixture) : EnrollmentTestBase(fixture)
{
    public static TheoryData<bool> Directions => new() { true, false };

    [Theory]
    [MemberData(nameof(Directions))]
    public async Task Enroll_IntoTheOtherCentresGroup_ReturnsGroupNotFound(bool nileActsOnMaadi)
    {
        var (own, foreign) = Pick(await ArrangeAsync(), nileActsOnMaadi);

        var result = await EnrollAsync(own.Secretary.Actor, foreign.GroupId, own.FreeStudentId);

        await AssertRefusedAsync(result, "group.not_found", foreign);
    }

    [Theory]
    [MemberData(nameof(Directions))]
    public async Task Enroll_TheOtherCentresStudentIntoAnOwnGroup_ReturnsStudentNotFound(bool nileActsOnMaadi)
    {
        var (own, foreign) = Pick(await ArrangeAsync(), nileActsOnMaadi);

        var result = await EnrollAsync(own.Secretary.Actor, own.GroupId, foreign.FreeStudentId);

        await AssertRefusedAsync(result, "student.not_found", foreign);
        Assert.Equal(0, await Fixture.ScalarAsync<long>($"select count(*) from academics.enrollments where student_id = '{foreign.FreeStudentId}'"));
    }

    [Theory]
    [MemberData(nameof(Directions))]
    public async Task Withdraw_TheOtherCentresEnrollment_ReturnsEnrollmentNotFound(bool nileActsOnMaadi)
    {
        var (own, foreign) = Pick(await ArrangeAsync(), nileActsOnMaadi);

        // Version 0 is deliberately meaningless: the lookup must refuse before any version comparison happens.
        var result = await Fixture.SendAsAsync<WithdrawEnrollmentCommand, Unit>(
            own.Secretary.Actor, new WithdrawEnrollmentCommand(foreign.EnrollmentId, 0));

        await AssertRefusedAsync(result, "enrollment.not_found", foreign);
    }

    [Theory]
    [MemberData(nameof(Directions))]
    public async Task Transfer_TheOtherCentresEnrollment_ReturnsEnrollmentNotFound(bool nileActsOnMaadi)
    {
        var (own, foreign) = Pick(await ArrangeAsync(), nileActsOnMaadi);

        var result = await Fixture.SendAsAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(
            own.Secretary.Actor, new TransferEnrollmentCommand(foreign.EnrollmentId, own.GroupId, 0));

        await AssertRefusedAsync(result, "enrollment.not_found", foreign);
    }

    [Theory]
    [MemberData(nameof(Directions))]
    public async Task Transfer_AnOwnEnrollmentIntoTheOtherCentresGroup_ReturnsGroupNotFound(bool nileActsOnMaadi)
    {
        var (own, foreign) = Pick(await ArrangeAsync(), nileActsOnMaadi);
        var version = (await GetStudentEnrollmentAsync(own.Secretary.Actor, own.EnrolledStudentId, own.EnrollmentId)).Version;

        var result = await Fixture.SendAsAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(
            own.Secretary.Actor, new TransferEnrollmentCommand(own.EnrollmentId, foreign.GroupId, version));

        await AssertRefusedAsync(result, "group.not_found", foreign);
        Assert.Equal("active", await Fixture.ScalarAsync<string>($"select status from academics.enrollments where id = '{own.EnrollmentId}'"));
    }

    private async Task AssertRefusedAsync(Result result, string code, CentreWorld foreign)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(code, result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
        Assert.Equal(foreign.Snapshot, await SnapshotAsync(foreign.CentreId));
    }

    private static (CentreWorld Own, CentreWorld Foreign) Pick((CentreWorld Nile, CentreWorld Maadi) world, bool nileActsOnMaadi) =>
```

The excerpt stops at line 90; use the original file for the remaining 41 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
