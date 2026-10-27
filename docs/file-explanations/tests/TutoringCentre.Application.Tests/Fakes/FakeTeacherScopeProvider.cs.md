# `tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherScopeProvider.cs`

## Purpose

Source summary (intent, not independent execution proof): A settable stand-in for `ITeacherScopeProvider`; starts at None, failing closed like the real one.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherScopeProvider.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 463 bytes. Source hash: `ac32f39e8e7aa5c6bcbe94f9079105c60be3573075bba1ce9b4dd6576ba03799`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherScopeProvider.cs#L6): `public sealed class FakeTeacherScopeProvider : ITeacherScopeProvider`
- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherScopeProvider.cs#L10): `public Task<TeacherScope> GetAsync(CancellationToken ct) => Task.FromResult(Scope);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:155](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L155)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs:22](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs#L22)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:159](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L159)
- [tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs:18](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:149](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L149)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs:21](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs#L21)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>A settable stand-in for <see cref="ITeacherScopeProvider"/>; starts at None, failing closed like the real one.</summary>
public sealed class FakeTeacherScopeProvider : ITeacherScopeProvider
{
    public TeacherScope Scope { get; set; } = TeacherScope.None;

    public Task<TeacherScope> GetAsync(CancellationToken ct) => Task.FromResult(Scope);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
