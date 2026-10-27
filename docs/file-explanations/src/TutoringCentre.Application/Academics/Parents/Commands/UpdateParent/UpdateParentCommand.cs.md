# `src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to change a parent's name or phone, guarded by the version the client last read. No centre: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 539 bytes. Source hash: `381587673704de210cd172903ad5b94ff9426bf1ca653b077fca3918a31b5f16`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentCommand.cs#L7): `public sealed record UpdateParentCommand(Guid ParentId, string FullName, string Phone, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs:71](../../../../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L71)
- [src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs:14](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentValidator.cs:7](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentValidator.cs#L7)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:45](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L45)
- [tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs:19](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs#L19)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs:51](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs#L51)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs:66](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L66)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;

/// <summary>Intent to change a parent's name or phone, guarded by the version the client last read. No centre: scope comes from the actor.</summary>
public sealed record UpdateParentCommand(Guid ParentId, string FullName, string Phone, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
