# `tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs`

## Purpose

Declares `WithdrawEnrollmentHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 60 lines and 2,702 bytes. Source hash: `3b2d43dfb441a9f2c98bcf21a4791e5f2cc7166cbc887c65c23d827e51451c0e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs#L8): `public sealed class WithdrawEnrollmentHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs#L13): `public async Task HandleAsync_UnknownEnrollment_ReturnsEnrollmentNotFound()`
- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs#L25): `public async Task HandleAsync_AlreadyEnded_ReturnsAlreadyEndedAndKeepsTheFirstEnd()`
- [Line 44](../../../../../tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs#L44): `public async Task HandleAsync_ActiveEnrollment_EndsItAsWithdrawnAtTheClocksTime()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class WithdrawEnrollmentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownEnrollment_ReturnsEnrollmentNotFound()
    {
        var handler = new WithdrawEnrollmentHandler(new FakeClock(), new FakeEnrollmentRepository());

        var result = await handler.HandleAsync(new WithdrawEnrollmentCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("enrollment.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_AlreadyEnded_ReturnsAlreadyEndedAndKeepsTheFirstEnd()
    {
        var clock = new FakeClock();
        var enrollment = Enrollment.Start(NileCentreId, Guid.CreateVersion7(), Guid.CreateVersion7(), clock.UtcNow.AddDays(-2)).Value;
        var firstEnd = clock.UtcNow.AddDays(-1);
        enrollment.End(firstEnd, EnrollmentEndReason.Transferred);
        var repository = new FakeEnrollmentRepository();
        repository.Existing.Add(enrollment);
        var handler = new WithdrawEnrollmentHandler(clock, repository);

        var result = await handler.HandleAsync(new WithdrawEnrollmentCommand(enrollment.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("enrollment.already_ended", result.Error!.Code);
        Assert.Equal(firstEnd, enrollment.EndedAt);
        Assert.Equal(EnrollmentEndReason.Transferred, enrollment.EndReason);
    }

    [Fact]
    public async Task HandleAsync_ActiveEnrollment_EndsItAsWithdrawnAtTheClocksTime()
    {
        var clock = new FakeClock();
        var enrollment = Enrollment.Start(NileCentreId, Guid.CreateVersion7(), Guid.CreateVersion7(), clock.UtcNow.AddDays(-1)).Value;
        var repository = new FakeEnrollmentRepository();
        repository.Existing.Add(enrollment);
        var handler = new WithdrawEnrollmentHandler(clock, repository);

        var result = await handler.HandleAsync(new WithdrawEnrollmentCommand(enrollment.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(EnrollmentStatus.Ended, enrollment.Status);
        Assert.Equal(clock.UtcNow, enrollment.EndedAt);
        Assert.Equal(EnrollmentEndReason.Withdrawn, enrollment.EndReason);
        Assert.Empty(repository.Added);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
