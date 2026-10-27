# `src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to change a subject's name, guarded by the version the client last read. No centre: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 517 bytes. Source hash: `009cc400c0a7c9e836cc4bccecea29fb1dfd51f32a92dd2b14918924da6cd581`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectCommand.cs#L7): `public sealed record RenameSubjectCommand(Guid SubjectId, string Name, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;

/// <summary>Intent to change a subject's name, guarded by the version the client last read. No centre: scope comes from the actor.</summary>
public sealed record RenameSubjectCommand(Guid SubjectId, string Name, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.SubjectsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
