# `src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to take a teacher out of use without deleting it, guarded by the version the client last read. The account link is kept.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 515 bytes. Source hash: `4d993d6858a7e9ce9df76b2cc2c5e34ecc666f71070181b35eb34530fd2d053a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherCommand.cs#L7): `public sealed record ArchiveTeacherCommand(Guid TeacherId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs:113](../../../../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L113)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherHandler.cs:10](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherHandler.cs#L10)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherValidatorTests.cs:10](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherValidatorTests.cs#L10)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:110](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L110)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs:98](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L98)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs:202](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L202)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs:66](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L66)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs:79](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L79)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;

/// <summary>Intent to take a teacher out of use without deleting it, guarded by the version the client last read. The account link is kept.</summary>
public sealed record ArchiveTeacherCommand(Guid TeacherId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
