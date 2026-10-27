# `src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to add a teacher record in the acting actor's own centre. No userId: linking an account is a separate, separately audited action (SetTeacherAccountCommand). Teacher names are not unique, so two teachers may share a name.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 648 bytes. Source hash: `dd29ec0e83534baedb8092f16ca02805c1f66404ce7a10ce5ed7e662281352ff`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherCommand.cs#L7): `/// Intent to add a teacher record in the acting actor's own centre. No userId: linking an account is a`
- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherCommand.cs#L11): `public sealed record CreateTeacherCommand(string FullName, string? Phone) : ICommand<CreateTeacherResult>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:467](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L467)
- [src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs:92](../../../../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L92)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs:12](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs#L12)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherValidator.cs:7](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherValidator.cs#L7)
- [tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs:21](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs#L21)
- [tests/TutoringCentre.Application.Tests/Academics/CreateTeacherValidatorTests.cs:10](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherValidatorTests.cs#L10)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:37](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L37)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs:33](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L33)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs:181](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L181)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs:28](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L28)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs:104](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L104)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;

/// <summary>
/// Intent to add a teacher record in the acting actor's own centre. No userId: linking an account is a
/// separate, separately audited action (SetTeacherAccountCommand). Teacher names are not unique, so two
/// teachers may share a name.
/// </summary>
public sealed record CreateTeacherCommand(string FullName, string? Phone) : ICommand<CreateTeacherResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
