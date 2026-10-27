# `src/TutoringCentre.Application/Academics/Subjects/Commands/RestoreSubject/RestoreSubjectCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to bring an archived subject back into use, guarded by the version the client last read.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RestoreSubject/RestoreSubjectCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 483 bytes. Source hash: `96502fa83868288e7576397d9d015476698a6ac8789b274a5c3a72733a95393f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RestoreSubject/RestoreSubjectCommand.cs#L7): `public sealed record RestoreSubjectCommand(Guid SubjectId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs:100](../../../../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L100)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/RestoreSubject/RestoreSubjectHandler.cs:9](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RestoreSubject/RestoreSubjectHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/RestoreSubject/RestoreSubjectValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RestoreSubject/RestoreSubjectValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/RestoreSubjectHandlerTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreSubjectHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/RestoreSubjectValidatorTests.cs:10](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreSubjectValidatorTests.cs#L10)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:109](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L109)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:67](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L67)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs:73](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L73)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs:128](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L128)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;

/// <summary>Intent to bring an archived subject back into use, guarded by the version the client last read.</summary>
public sealed record RestoreSubjectCommand(Guid SubjectId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.SubjectsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
