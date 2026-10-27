# `tests/TutoringCentre.Api.Tests/Fixtures/FixableTimeProvider.cs`

## Purpose

Source summary (intent, not independent execution proof): The application clock's time source in the Api tests (Day 53). Real time by default, so every earlier test runs exactly as before; a test that depends on "today" or "in the past" fixes it with `FixAt` and the fixture puts it back with `UseRealTime`. The collection runs its classes one at a time, so a fixed time never leaks into a concurrently running test.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/FixableTimeProvider.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 18 lines and 807 bytes. Source hash: `9baf1d25a76a0cc9775dc6a326d8a457d4e5253bffa0331ecbd9e94eb5f6d152`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/FixableTimeProvider.cs#L9): `public sealed class FixableTimeProvider : TimeProvider`
- [Line 13](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/FixableTimeProvider.cs#L13): `public void FixAt(DateTimeOffset utcNow) => _fixedUtcNow = utcNow.ToUniversalTime();`
- [Line 15](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/FixableTimeProvider.cs#L15): `public void UseRealTime() => _fixedUtcNow = null;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs:32](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs#L32)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>
/// The application clock's time source in the Api tests (Day 53). Real time by default, so every earlier test runs
/// exactly as before; a test that depends on "today" or "in the past" fixes it with <see cref="FixAt"/> and the
/// fixture puts it back with <see cref="UseRealTime"/>. The collection runs its classes one at a time, so a fixed
/// time never leaks into a concurrently running test.
/// </summary>
public sealed class FixableTimeProvider : TimeProvider
{
    private DateTimeOffset? _fixedUtcNow;

    public void FixAt(DateTimeOffset utcNow) => _fixedUtcNow = utcNow.ToUniversalTime();

    public void UseRealTime() => _fixedUtcNow = null;

    public override DateTimeOffset GetUtcNow() => _fixedUtcNow ?? System.GetUtcNow();
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
