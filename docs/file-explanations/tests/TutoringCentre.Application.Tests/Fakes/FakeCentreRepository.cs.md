# `tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory centre repository. `Existing` simulates stored rows; `Added` records Add calls.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 20 lines and 835 bytes. Source hash: `c2b433c6fcbc9319922cefda849ed06a2968a910ce7cdd9c608f60fca82186de`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Centres;`
- Line 2: `using TutoringCentre.Domain.Centres;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs#L7): `public sealed class FakeCentreRepository : ICentreRepository`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs#L13): `public Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct) =>`
- [Line 16](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs#L16): `public Task<Centre?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>`
- [Line 19](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs#L19): `public void Add(Centre centre) => Added.Add(centre);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs:97](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L97)
- [tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs:16](../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Centres;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory centre repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add calls.</summary>
public sealed class FakeCentreRepository : ICentreRepository
{
    public List<Centre> Existing { get; } = [];

    public List<Centre> Added { get; } = [];

    public Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct) =>
        Task.FromResult(Existing.Concat(Added).Any(centre => centre.Slug == slug));

    public Task<Centre?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.Concat(Added).SingleOrDefault(centre => centre.Id == id));

    public void Add(Centre centre) => Added.Add(centre);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
