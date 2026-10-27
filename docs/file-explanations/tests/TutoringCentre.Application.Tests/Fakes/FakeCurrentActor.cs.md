# `tests/TutoringCentre.Application.Tests/Fakes/FakeCurrentActor.cs`

## Purpose

Source summary (intent, not independent execution proof): A settable stand-in for `ICurrentActor`; starts anonymous like the real context does.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCurrentActor.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 9 lines and 341 bytes. Source hash: `d35377f058992beee36208dd30ff0b1e801ac513ef552c3b16a84e080fd984c7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCurrentActor.cs#L6): `public sealed class FakeCurrentActor : ICurrentActor`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs:151](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L151)
- [tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs:144](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L144)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs:127](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L127)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:149](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L149)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:154](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L154)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:152](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L152)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:142](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L142)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:150](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L150)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:166](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L166)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:106](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L106)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:213](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L213)
- [tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs:129](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L129)
- [tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs:56](../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs#L56)
- [tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs:92](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L92)
- [tests/TutoringCentre.Application.Tests/Centres/GetCentreSettingsHandlerTests.cs:19](../../../../../tests/TutoringCentre.Application.Tests/Centres/GetCentreSettingsHandlerTests.cs#L19)
- [tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs:17](../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs#L17)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:355](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L355)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:167](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L167)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>A settable stand-in for <see cref="ICurrentActor"/>; starts anonymous like the real context does.</summary>
public sealed class FakeCurrentActor : ICurrentActor
{
    public Actor Actor { get; set; } = new AnonymousActor();
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
