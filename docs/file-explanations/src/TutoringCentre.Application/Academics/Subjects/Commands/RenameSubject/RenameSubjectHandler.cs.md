# `src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: scoped lookup → Domain invariants → set-based uniqueness, excluding the subject itself. A subject in another centre and one that does not exist are indistinguishable: both subject.not_found. The handler never compares versions itself — the save detects a stale one (Day 25).

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 1,930 bytes. Source hash: `e5956849c88e309164d646d2a1642d396707e5290072fa54545f180956547e94`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectHandler.cs#L13): `internal sealed class RenameSubjectHandler(ISubjectRepository subjects) : ICommandHandler<RenameSubjectCommand, Unit>`
- [Line 15](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectHandler.cs#L15): `public async Task<Result<Unit>> HandleAsync(RenameSubjectCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/RenameSubjectHandlerTests.cs:16](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:184](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L184)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:116](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L116)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;

/// <summary>
/// Orchestrates: scoped lookup → Domain invariants → set-based uniqueness, excluding the subject itself. A
/// subject in another centre and one that does not exist are indistinguishable: both subject.not_found. The
/// handler never compares versions itself — the save detects a stale one (Day 25).
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class RenameSubjectHandler(ISubjectRepository subjects) : ICommandHandler<RenameSubjectCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RenameSubjectCommand command, CancellationToken cancellationToken)
    {
        var subject = await subjects.GetByIdAsync(command.SubjectId, command.Version, cancellationToken);
        if (subject is null)
        {
            return Result<Unit>.Failure(Error.NotFound("subject.not_found", "This subject could not be found."));
        }

        var previousNormalizedName = subject.NormalizedName;

        var renamed = subject.Rename(command.Name);
        if (renamed.IsFailure)
        {
            return Result<Unit>.Failure(renamed.Error!);
        }

        if (subject.NormalizedName != previousNormalizedName
            && await subjects.NameExistsAsync(subject.NormalizedName, subject.Id, cancellationToken))
        {
            var fields = new Dictionary<string, string[]> { ["name"] = ["A subject with this name already exists."] };
            return Result<Unit>.Failure(
                new Error("subject.name_taken", "A subject with this name already exists.", ErrorKind.Conflict, fields));
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
