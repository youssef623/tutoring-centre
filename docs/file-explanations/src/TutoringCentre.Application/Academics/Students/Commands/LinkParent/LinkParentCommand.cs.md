# `src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to link an existing parent to a student, guarded by the student's version. The parent must already exist in the acting centre.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 12 lines and 604 bytes. Source hash: `a3ff67dc434f203cf7ac8e24de9791a1b14ddde547d43cbb5607efe1f0ab937c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Academics;`
- Line 3: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentCommand.cs#L8): `public sealed record LinkParentCommand(Guid StudentId, Guid ParentId, ParentRelationship Relationship, uint Version)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:148](../../../../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L148)
- [src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs:14](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs:25](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L25)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:53](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L53)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs:163](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L163)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs:36](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs#L36)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.LinkParent;

/// <summary>Intent to link an existing parent to a student, guarded by the student's version. The parent must already exist in the acting centre.</summary>
public sealed record LinkParentCommand(Guid StudentId, Guid ParentId, ParentRelationship Relationship, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
