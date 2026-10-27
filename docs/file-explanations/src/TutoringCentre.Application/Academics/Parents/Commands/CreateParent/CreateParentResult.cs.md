# `src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentResult.cs`

## Purpose

Declares `CreateParentResult`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentResult.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 3 lines and 135 bytes. Source hash: `f2707d8acb879c77c71891ef245e3ed53d992cae48a040d6b6e1a36c81653b1b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentResult.cs#L3): `public sealed record CreateParentResult(Guid ParentId);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs:58](../../../../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L58)
- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentCommand.cs:7](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs:12](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs#L12)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:33](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L33)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs:24](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs#L24)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs:28](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L28)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentRaceTests.cs:36](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentRaceTests.cs#L36)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Academics.Parents.Commands.CreateParent;

public sealed record CreateParentResult(Guid ParentId);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
