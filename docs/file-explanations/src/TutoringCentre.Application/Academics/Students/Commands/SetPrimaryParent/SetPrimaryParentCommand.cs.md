# `src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to make a different already-linked parent the student's primary contact, guarded by the student's version.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 520 bytes. Source hash: `e177d02b448d57d2a95f51fe9f704df60d8267c98d79a6217fe953bb930882b3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentCommand.cs#L7): `public sealed record SetPrimaryParentCommand(Guid StudentId, Guid ParentId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:163](../../../../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L163)
- [src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentHandler.cs:9](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs:117](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L117)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:59](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L59)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;

/// <summary>Intent to make a different already-linked parent the student's primary contact, guarded by the student's version.</summary>
public sealed record SetPrimaryParentCommand(Guid StudentId, Guid ParentId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
