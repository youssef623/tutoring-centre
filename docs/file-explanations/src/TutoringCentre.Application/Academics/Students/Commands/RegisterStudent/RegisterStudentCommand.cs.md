# `src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to create a student with their first parent, in the acting actor's own centre, in one transaction. `Parent` carries exactly one of two forms: an existing parent's id, or a new parent's name and phone. The phone fields are raw input here; PhoneNumber.Create parses each once, in the handler.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 20 lines and 1,105 bytes. Source hash: `b66aa7b0d29af39d019b392d1b62670ade14f6d10ac9549ed5befb8ca04a684f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Academics;`
- Line 3: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentCommand.cs#L12): `public sealed record RegisterStudentCommand(`
- [Line 20](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentCommand.cs#L20): `public sealed record RegisterStudentParent(Guid? ExistingParentId, string? FullName, string? Phone);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:278](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L278)
- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:121](../../../../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L121)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentValidator.cs:33](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentValidator.cs#L33)
- [tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs:22](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L22)
- [tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs:11](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs#L11)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:41](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L41)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs:15](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L15)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs:9](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs#L9)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;

/// <summary>
/// Intent to create a student with their first parent, in the acting actor's own centre, in one transaction.
/// <see cref="Parent"/> carries exactly one of two forms: an existing parent's id, or a new parent's name and
/// phone. The phone fields are raw input here; PhoneNumber.Create parses each once, in the handler.
/// </summary>
public sealed record RegisterStudentCommand(
    string FullName, string? Phone, string? SchoolYear, RegisterStudentParent Parent, ParentRelationship Relationship)
    : ICommand<RegisterStudentResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}

/// <summary>Exactly one of <see cref="ExistingParentId"/> alone, or <see cref="FullName"/> and <see cref="Phone"/> together.</summary>
public sealed record RegisterStudentParent(Guid? ExistingParentId, string? FullName, string? Phone);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
