# `src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to create a subject in the acting actor's own centre. Carries exactly the field its intent needs — the centre comes from the actor, never the caller.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 542 bytes. Source hash: `128518c95300edee741b1bec6ea7e87fc88f27181b850d5d3f67855811e1c937`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectCommand.cs#L7): `public sealed record CreateSubjectCommand(string Name) : ICommand<CreateSubjectResult>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:227](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L227)
- [src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs:82](../../../../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L82)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs:12](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs#L12)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectValidator.cs:7](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectValidator.cs#L7)
- [tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs:24](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs#L24)
- [tests/TutoringCentre.Application.Tests/Academics/CreateSubjectValidatorTests.cs:10](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateSubjectValidatorTests.cs#L10)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:36](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L36)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:33](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L33)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs:38](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L38)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs:26](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L26)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs:97](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L97)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRaceTests.cs:27](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRaceTests.cs#L27)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs:28](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs#L28)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs:30](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L30)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;

/// <summary>Intent to create a subject in the acting actor's own centre. Carries exactly the field its intent needs — the centre comes from the actor, never the caller.</summary>
public sealed record CreateSubjectCommand(string Name) : ICommand<CreateSubjectResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.SubjectsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
