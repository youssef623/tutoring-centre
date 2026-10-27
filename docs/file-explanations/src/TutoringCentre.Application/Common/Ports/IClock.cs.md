# `src/TutoringCentre.Application/Common/Ports/IClock.cs`

## Purpose

Source summary (intent, not independent execution proof): The only source of "now" and of local-time conversion. Implemented in Infrastructure (SystemClock).

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Ports/IClock.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 674 bytes. Source hash: `055d3cdd4a94f26a5779b7b17c2ad0d400f9c0ba62b7c12b850e5fde59b2bb4b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../../src/TutoringCentre.Application/Common/Ports/IClock.cs#L4): `public interface IClock`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:24](../../../../../onboarding-work/extend_backend_purposes.py#L24)
- [src/TutoringCentre.Api/Cli/SeedCommand.cs:581](../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L581)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:21](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L21)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs:22](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs#L22)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs:15](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs#L15)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs:18](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs#L18)
- [src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs:20](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs#L20)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:23](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L23)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs:15](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs#L15)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:20](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L20)
- [src/TutoringCentre.Application/Common/Time/CentreCalendar.cs:12](../../../../../../src/TutoringCentre.Application/Common/Time/CentreCalendar.cs#L12)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:57](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L57)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs:18](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs#L18)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs:16](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs#L16)
- [src/TutoringCentre.Infrastructure/Time/SystemClock.cs:6](../../../../../../src/TutoringCentre.Infrastructure/Time/SystemClock.cs#L6)
- [tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs:114](../../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs#L114)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:150](../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L150)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:143](../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L143)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeClock.cs:6](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClock.cs#L6)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Common.Ports;

/// <summary>The only source of "now" and of local-time conversion. Implemented in Infrastructure (SystemClock).</summary>
public interface IClock
{
    /// <summary>The current instant, in UTC.</summary>
    DateTimeOffset UtcNow { get; }

    /// <summary>Converts an instant to the wall-clock time in the given IANA time zone (DateTimeKind.Unspecified).</summary>
    DateTime ToLocal(DateTimeOffset utc, string timeZoneId);

    /// <summary>Converts a wall-clock time in the given IANA time zone to the instant it represents (offset zero).</summary>
    DateTimeOffset FromLocal(DateTime local, string timeZoneId);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
