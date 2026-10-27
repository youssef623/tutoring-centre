# `src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to create a parent in the acting actor's own centre. The phone is raw input here; PhoneNumber.Create parses it once, in the handler.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 537 bytes. Source hash: `d0db6e61207a5420f81df3f3ad9e14db2ec3509e914eb952d322a1f9f3abd148`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentCommand.cs#L7): `public sealed record CreateParentCommand(string FullName, string Phone) : ICommand<CreateParentResult>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs:58](../../../../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L58)
- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs:12](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs#L12)
- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentValidator.cs:7](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentValidator.cs#L7)
- [tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs:22](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs#L22)
- [tests/TutoringCentre.Application.Tests/Academics/CreateParentValidatorTests.cs:10](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentValidatorTests.cs#L10)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:33](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L33)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs:24](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs#L24)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs:28](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L28)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentRaceTests.cs:40](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentRaceTests.cs#L40)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Parents.Commands.CreateParent;

/// <summary>Intent to create a parent in the acting actor's own centre. The phone is raw input here; PhoneNumber.Create parses it once, in the handler.</summary>
public sealed record CreateParentCommand(string FullName, string Phone) : ICommand<CreateParentResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
