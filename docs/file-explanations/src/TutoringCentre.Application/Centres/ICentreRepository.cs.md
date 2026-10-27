# `src/TutoringCentre.Application/Centres/ICentreRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): Write-side persistence port for the Centre aggregate. Methods are named by use; nothing here saves — the dispatcher does.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Application/Centres/ICentreRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 18 lines and 816 bytes. Source hash: `468ab2a05653e62319dd3faa68ee8e2cf4bfa743b92f38c0910a20e39eeb1f7f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Centres;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../src/TutoringCentre.Application/Centres/ICentreRepository.cs#L6): `public interface ICentreRepository`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs:9](../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs#L9)
- [src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsHandler.cs:14](../../../../../src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsHandler.cs#L14)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:124](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L124)
- [src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs:9](../../../../../src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs#L9)
- [tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs:97](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L97)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs:7](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs#L7)
- [tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs:66](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs#L66)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Centres;

/// <summary>Write-side persistence port for the Centre aggregate. Methods are named by use; nothing here saves — the dispatcher does.</summary>
public interface ICentreRepository
{
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct);

    /// <summary>
    /// Loads one centre for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version. Returns null when no centre with this id exists —
    /// callers that pass the acting actor's own centre id treat that as a bug, not a business outcome.
    /// </summary>
    Task<Centre?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    void Add(Centre centre);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
