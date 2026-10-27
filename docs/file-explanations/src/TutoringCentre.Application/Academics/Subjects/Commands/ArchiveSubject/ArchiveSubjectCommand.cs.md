# `src/TutoringCentre.Application/Academics/Subjects/Commands/ArchiveSubject/ArchiveSubjectCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to take a subject out of use without deleting it, guarded by the version the client last read.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/ArchiveSubject/ArchiveSubjectCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 489 bytes. Source hash: `826891e09c146c1ee99b8ea7f1d1d46b2f61a653443522aaf77c2463d78dc4f2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/ArchiveSubject/ArchiveSubjectCommand.cs#L7): `public sealed record ArchiveSubjectCommand(Guid SubjectId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs:94](../../../../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L94)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/ArchiveSubject/ArchiveSubjectHandler.cs:10](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/ArchiveSubject/ArchiveSubjectHandler.cs#L10)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/ArchiveSubject/ArchiveSubjectValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/ArchiveSubject/ArchiveSubjectValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectValidatorTests.cs:10](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectValidatorTests.cs#L10)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:85](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L85)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:56](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L56)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs:105](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L105)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs:132](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L132)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs:59](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L59)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs:66](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs#L66)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs:94](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L94)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;

/// <summary>Intent to take a subject out of use without deleting it, guarded by the version the client last read.</summary>
public sealed record ArchiveSubjectCommand(Guid SubjectId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.SubjectsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
