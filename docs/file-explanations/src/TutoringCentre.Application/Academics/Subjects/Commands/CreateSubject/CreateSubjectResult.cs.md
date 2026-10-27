# `src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectResult.cs`

## Purpose

Declares `CreateSubjectResult`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectResult.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 3 lines and 139 bytes. Source hash: `5b97a96cf108720ba21b21376f9811ed801af835b5d7553347734b62975e38d8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectResult.cs#L3): `public sealed record CreateSubjectResult(Guid SubjectId);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:227](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L227)
- [src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs:82](../../../../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L82)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectCommand.cs:7](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs:12](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs#L12)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:36](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L36)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:33](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L33)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs:38](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L38)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs:26](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L26)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs:97](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L97)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRaceTests.cs:23](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRaceTests.cs#L23)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs:28](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs#L28)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs:30](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L30)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;

public sealed record CreateSubjectResult(Guid SubjectId);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
