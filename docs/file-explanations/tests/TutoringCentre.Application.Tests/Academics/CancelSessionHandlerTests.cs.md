# `tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs`

## Purpose

Declares `CancelSessionHandlerTests`, `Sut`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 72 lines and 2,767 bytes. Source hash: `d699107f27fab8459a346c60087875892a674b567f40eb912797d9f0dde5ca0e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`
- Line 3: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs#L9): `public sealed class CancelSessionHandlerTests`
- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs#L12): `public async Task HandleAsync_UnknownSession_ReturnsSessionNotFound()`
- [Line 24](../../../../../tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs#L24): `public async Task HandleAsync_FutureSession_IsCancelled()`
- [Line 35](../../../../../tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs#L35): `public async Task HandleAsync_PastSession_ReturnsSessionInPastAndStaysScheduled()`
- [Line 48](../../../../../tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs#L48): `public async Task HandleAsync_AlreadyCancelledSession_ReturnsSessionAlreadyCancelled()`
- [Line 59](../../../../../tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs#L59): `private static Sut CreateSut()`
- [Line 71](../../../../../tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs#L71): `private sealed record Sut(CancelSessionHandler Handler, ClassSession Past, ClassSession Future);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;
using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CancelSessionHandlerTests
{
    [Fact]
    public async Task HandleAsync_UnknownSession_ReturnsSessionNotFound()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new CancelSessionCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.session_not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_FutureSession_IsCancelled()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new CancelSessionCommand(sut.Future.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(ClassSessionStatus.Cancelled, sut.Future.Status);
    }

    [Fact]
    public async Task HandleAsync_PastSession_ReturnsSessionInPastAndStaysScheduled()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new CancelSessionCommand(sut.Past.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.session_in_past", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(ClassSessionStatus.Scheduled, sut.Past.Status);
    }

    [Fact]
    public async Task HandleAsync_AlreadyCancelledSession_ReturnsSessionAlreadyCancelled()
    {
        var sut = CreateSut();
        sut.Future.Cancel(Now);

        var result = await sut.Handler.HandleAsync(new CancelSessionCommand(sut.Future.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.session_already_cancelled", result.Error!.Code);
    }

    private static Sut CreateSut()
    {
        var rule = SaturdayRule(Guid.CreateVersion7(), Today.AddDays(-14));
        var generated = Sessions(rule, Guid.CreateVersion7(), Today.AddDays(-14), Today.AddDays(13));
        var sessions = new FakeClassSessionRepository();
        sessions.Existing.AddRange(generated);

        var past = generated.First(session => session.StartsAt < Now);
        var future = generated.First(session => session.StartsAt > Now);
        return new Sut(new CancelSessionHandler(Clock(), sessions), past, future);
    }

    private sealed record Sut(CancelSessionHandler Handler, ClassSession Past, ClassSession Future);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
