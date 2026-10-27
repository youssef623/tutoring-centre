# `src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: scoped lookup of both aggregates → the Domain invariant (at most 4 parents, never the same parent twice). A parent in another centre and one that does not exist are indistinguishable: both parent.not_found — `IParentRepository.FindAsync` is itself tenant-scoped.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 33 lines and 1,584 bytes. Source hash: `eccde904713ea4e4af00cc5ef063625478a9dd371f0acd0d29df1c7b6e174b5c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Parents;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs#L14): `internal sealed class LinkParentHandler(IStudentRepository students, IParentRepository parents) : ICommandHandler<LinkParentCommand, Unit>`
- [Line 16](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs#L16): `public async Task<Result<Unit>> HandleAsync(LinkParentCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs:23](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L23)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:171](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L171)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Commands.LinkParent;

/// <summary>
/// Orchestrates: scoped lookup of both aggregates → the Domain invariant (at most 4 parents, never the same
/// parent twice). A parent in another centre and one that does not exist are indistinguishable: both
/// parent.not_found — <see cref="IParentRepository.FindAsync"/> is itself tenant-scoped.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class LinkParentHandler(IStudentRepository students, IParentRepository parents) : ICommandHandler<LinkParentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(LinkParentCommand command, CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(command.StudentId, command.Version, cancellationToken);
        if (student is null)
        {
            return Result<Unit>.Failure(StudentErrors.NotFound());
        }

        var parent = await parents.FindAsync(command.ParentId, cancellationToken);
        if (parent is null)
        {
            return Result<Unit>.Failure(ParentErrors.NotFound());
        }

        var added = student.AddParent(parent.Id, command.Relationship);
        return added.IsFailure ? Result<Unit>.Failure(added.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
