# `src/TutoringCentre.Application/Common/Security/CorrelationContext.cs`

## Purpose

Source summary (intent, not independent execution proof): Holds the correlation ID for one scope. Starts null; trusted edge code (Api middleware) calls `Set` at most once. Non-HTTP callers (seed, jobs, tests) never call it, so `CorrelationId` stays null.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Security/CorrelationContext.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 27 lines and 1,037 bytes. Source hash: `c4040500a120728192eebf5d8795369f583244e98fd5d4ae0a64247bdc4d2dca`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../src/TutoringCentre.Application/Common/Security/CorrelationContext.cs#L7): `public sealed class CorrelationContext : ICorrelationContext`
- [Line 15](../../../../../../src/TutoringCentre.Application/Common/Security/CorrelationContext.cs#L15): `public void Set(string correlationId)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs:19](../../../../../../src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs#L19)
- [src/TutoringCentre.Application/DependencyInjection.cs:19](../../../../../../src/TutoringCentre.Application/DependencyInjection.cs#L19)
- [tests/TutoringCentre.Application.Tests/Security/CorrelationContextTests.cs:10](../../../../../../tests/TutoringCentre.Application.Tests/Security/CorrelationContextTests.cs#L10)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs:30](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs#L30)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Common.Security;

/// <summary>
/// Holds the correlation ID for one scope. Starts null; trusted edge code (Api middleware) calls <see cref="Set"/>
/// at most once. Non-HTTP callers (seed, jobs, tests) never call it, so <see cref="CorrelationId"/> stays null.
/// </summary>
public sealed class CorrelationContext : ICorrelationContext
{
    private bool _isSet;

    public string? CorrelationId { get; private set; }

    /// <exception cref="ArgumentException"><paramref name="correlationId"/> is null or empty.</exception>
    /// <exception cref="InvalidOperationException">The correlation ID for this scope has already been set.</exception>
    public void Set(string correlationId)
    {
        ArgumentException.ThrowIfNullOrEmpty(correlationId);

        if (_isSet)
        {
            throw new InvalidOperationException("The correlation ID for this scope has already been set and cannot be replaced.");
        }

        CorrelationId = correlationId;
        _isSet = true;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
