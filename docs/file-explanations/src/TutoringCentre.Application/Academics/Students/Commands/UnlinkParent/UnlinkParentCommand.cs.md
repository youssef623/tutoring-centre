# `src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to remove a parent link from a student, guarded by the student's version. Student.RemoveParent refuses to remove the last parent or the primary parent.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 557 bytes. Source hash: `9c8596cd36fb6cf504c42e2ca0ff03b981e1f66c21bd7dbedaa37f3a82b5c57d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentCommand.cs#L7): `public sealed record UnlinkParentCommand(Guid StudentId, Guid ParentId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:156](../../../../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L156)
- [src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentHandler.cs:13](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentHandler.cs#L13)
- [src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs:72](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L72)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:56](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L56)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs:11](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs#L11)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;

/// <summary>Intent to remove a parent link from a student, guarded by the student's version. Student.RemoveParent refuses to remove the last parent or the primary parent.</summary>
public sealed record UnlinkParentCommand(Guid StudentId, Guid ParentId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
