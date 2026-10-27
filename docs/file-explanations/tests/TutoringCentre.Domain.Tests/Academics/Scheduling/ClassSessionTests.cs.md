# `tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 51.5. Every session comes from `SessionGenerator`: it is the only way to obtain one.

## Where it fits

Category: **test**. [Open current file](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 103 lines and 3,565 bytes. Source hash: `0775aac77e058283f7c0cd567e3503e72fc007f46f93c753806663ee31682216`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs#L7): `public sealed class ClassSessionTests`
- [Line 20](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs#L20): `public void Cancel_FutureSession_IsCancelled()`
- [Line 31](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs#L31): `public void Cancel_Twice_ReturnsSessionAlreadyCancelled()`
- [Line 43](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs#L43): `public void Cancel_AtTheStartInstant_ReturnsSessionInPastAndStaysScheduled()`
- [Line 54](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs#L54): `public void ReassignTeacher_FutureScheduledSession_ChangesTheTeacher()`
- [Line 65](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs#L65): `public void ReassignTeacher_StartedSession_SucceedsAndKeepsTheTeacher()`
- [Line 76](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs#L76): `public void ReassignTeacher_CancelledFutureSession_SucceedsAndKeepsTheTeacher()`
- [Line 87](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs#L87): `private static ClassSession NewSession()`
- [Line 97](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs#L97): `private static void AssertRuleFailure(Result result, string expectedCode)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics.Scheduling;

/// <summary>Task 51.5. Every session comes from <see cref="SessionGenerator"/>: it is the only way to obtain one.</summary>
public sealed class ClassSessionTests
{
    private static readonly TimeZoneInfo Cairo = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
    private static readonly Guid CentreId = Guid.NewGuid();
    private static readonly Guid GroupId = Guid.NewGuid();
    private static readonly Guid TeacherId = Guid.NewGuid();
    private static readonly Guid OtherTeacherId = Guid.NewGuid();

    // Saturday 12 December 2026, 18:00 Cairo = 16:00 UTC.
    private static readonly DateTimeOffset StartsAt = new(2026, 12, 12, 16, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset BeforeStart = StartsAt.AddMinutes(-1);

    [Fact]
    public void Cancel_FutureSession_IsCancelled()
    {
        var session = NewSession();

        var result = session.Cancel(BeforeStart);

        Assert.True(result.IsSuccess);
        Assert.Equal(ClassSessionStatus.Cancelled, session.Status);
    }

    [Fact]
    public void Cancel_Twice_ReturnsSessionAlreadyCancelled()
    {
        var session = NewSession();
        Assert.True(session.Cancel(BeforeStart).IsSuccess);

        var result = session.Cancel(BeforeStart);

        AssertRuleFailure(result, "schedule.session_already_cancelled");
        Assert.Equal(ClassSessionStatus.Cancelled, session.Status);
    }

    [Fact]
    public void Cancel_AtTheStartInstant_ReturnsSessionInPastAndStaysScheduled()
    {
        var session = NewSession();

        var result = session.Cancel(StartsAt);

        AssertRuleFailure(result, "schedule.session_in_past");
        Assert.Equal(ClassSessionStatus.Scheduled, session.Status);
    }

    [Fact]
    public void ReassignTeacher_FutureScheduledSession_ChangesTheTeacher()
    {
        var session = NewSession();

        var result = session.ReassignTeacher(OtherTeacherId, BeforeStart);

        Assert.True(result.IsSuccess);
        Assert.Equal(OtherTeacherId, session.TeacherId);
    }

    [Fact]
    public void ReassignTeacher_StartedSession_SucceedsAndKeepsTheTeacher()
    {
        var session = NewSession();

        var result = session.ReassignTeacher(OtherTeacherId, StartsAt);

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherId, session.TeacherId);
    }

    [Fact]
    public void ReassignTeacher_CancelledFutureSession_SucceedsAndKeepsTheTeacher()
    {
        var session = NewSession();
        Assert.True(session.Cancel(BeforeStart).IsSuccess);

        var result = session.ReassignTeacher(OtherTeacherId, BeforeStart);

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherId, session.TeacherId);
    }

    private static ClassSession NewSession()
    {
        var rule = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, new TimeOnly(18, 0), 90, "Room 2", new DateOnly(2026, 12, 5)).Value;
        var date = new DateOnly(2026, 12, 12);
```

The excerpt stops at line 90; use the original file for the remaining 13 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
