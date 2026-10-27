# `src/TutoringCentre.Api/Auth/SessionValidationOptions.cs`

## Purpose

Source summary (intent, not independent execution proof): Bound from configuration key SessionValidation:CacheDuration (e.g. "00:01:00"). Zero disables caching.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/SessionValidationOptions.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 9 lines and 369 bytes. Source hash: `7ed7bf9c90360a53dab10fd33ce354c60b64d72ffb2ecb19a0a538927ff761a7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../src/TutoringCentre.Api/Auth/SessionValidationOptions.cs#L4): `public sealed class SessionValidationOptions`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/AuthenticationSetup.cs:55](../../../../../src/TutoringCentre.Api/Auth/AuthenticationSetup.cs#L55)
- [src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs:42](../../../../../src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs#L42)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Api.Auth;

/// <summary>Bound from configuration key SessionValidation:CacheDuration (e.g. "00:01:00"). Zero disables caching.</summary>
public sealed class SessionValidationOptions
{
    public static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromSeconds(60);

    public TimeSpan CacheDuration { get; set; } = DefaultCacheDuration;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
