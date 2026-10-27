# `src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: load the actor's own centre at the expected version → Domain invariants. The handler never compares versions itself — the save detects a stale one. No audit code here: Day 31's interceptor records the name/defaultLocale change from the same SaveChanges this command's unit of work makes.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsHandler.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 28 lines and 1,584 bytes. Source hash: `de9f2bdccf19e3871193ae5c38303ba8adc9a47bdf7aca5b13c0348549ce0574`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../../src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsHandler.cs#L14): `internal sealed class UpdateCentreSettingsHandler(ICurrentActor currentActor, ICentreRepository centres)`
- [Line 17](../../../../../../../src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsHandler.cs#L17): `public async Task<Result<Unit>> HandleAsync(UpdateCentreSettingsCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs:99](../../../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L99)
- [tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs:18](../../../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs#L18)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;

/// <summary>
/// Orchestrates: load the actor's own centre at the expected version → Domain invariants. The handler never
/// compares versions itself — the save detects a stale one. No audit code here: Day 31's interceptor records
/// the name/defaultLocale change from the same SaveChanges this command's unit of work makes.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class UpdateCentreSettingsHandler(ICurrentActor currentActor, ICentreRepository centres)
    : ICommandHandler<UpdateCentreSettingsCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(UpdateCentreSettingsCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre (Day 22).
        var centreId = currentActor.Actor.CentreId!.Value;

        var centre = await centres.GetByIdAsync(centreId, command.Version, cancellationToken)
            ?? throw new InvalidOperationException("The acting actor's own centre could not be found.");

        var updated = centre.UpdateSettings(command.Name, command.DefaultLocale);
        return updated.IsFailure ? Result<Unit>.Failure(updated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
