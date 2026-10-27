# `src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Creates a centre (tenant). Orchestrates: authorize → uniqueness → Domain rules → add. The dispatcher saves and commits.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 42 lines and 1,939 bytes. Source hash: `d3bef3f8939ccc98f0a0bb2d9ca417a03ca1099497085765ca623f11713e8f1a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Domain.Centres;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs#L9): `internal sealed class CreateCentreHandler(ICurrentActor currentActor, ICentreRepository centres)`
- [Line 12](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs#L12): `public async Task<Result<CreateCentreResult>> HandleAsync(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs:18](../../../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs#L18)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Centres.Commands.CreateCentre;

/// <summary>Creates a centre (tenant). Orchestrates: authorize → uniqueness → Domain rules → add. The dispatcher saves and commits.</summary>
internal sealed class CreateCentreHandler(ICurrentActor currentActor, ICentreRepository centres)
    : ICommandHandler<CreateCentreCommand, CreateCentreResult>
{
    public async Task<Result<CreateCentreResult>> HandleAsync(
        CreateCentreCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Authorize BEFORE any database read: creating tenants is a platform operation, not a web-user operation.
        if (currentActor.Actor is not SystemActor)
        {
            return Result<CreateCentreResult>.Failure(
                Error.Forbidden("centre.create_forbidden", "Only the platform can create centres."));
        }

        // 2. Uniqueness (the unique index is the real guarantee under concurrency; see Day 10's race test).
        if (await centres.ExistsBySlugAsync(command.Slug, cancellationToken))
        {
            return Result<CreateCentreResult>.Failure(
                Error.Conflict("centre.slug_taken", "A centre with this slug already exists."));
        }

        // 3. The Domain decides validity.
        var created = Centre.Create(command.Name, command.Slug, command.TimeZoneId, command.DefaultLocale);
        if (created.IsFailure)
        {
            return Result<CreateCentreResult>.Failure(created.Error!);
        }

        // 4. Track the new entity; NO SaveChanges here — the dispatcher saves once and commits.
        centres.Add(created.Value);

        return Result<CreateCentreResult>.Success(new CreateCentreResult(created.Value.Id, created.Value.Slug));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
