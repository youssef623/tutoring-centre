# `src/TutoringCentre.Api/Http/HttpContextKeys.cs`

## Purpose

Source summary (intent, not independent execution proof): Keys for per-request values stored in `HttpContext.Items`.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Http/HttpContextKeys.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 7 lines and 229 bytes. Source hash: `0e5bad6cb72997c738e791b997cabed19e9c05c5d165f3e4ad2dceb992ea2c6b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../src/TutoringCentre.Api/Http/HttpContextKeys.cs#L4): `internal static class HttpContextKeys`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs:26](../../../../../src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs#L26)
- [src/TutoringCentre.Api/Http/ProblemResult.cs:30](../../../../../src/TutoringCentre.Api/Http/ProblemResult.cs#L30)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Api.Http;

/// <summary>Keys for per-request values stored in <see cref="HttpContext.Items"/>.</summary>
internal static class HttpContextKeys
{
    public const string CorrelationId = "CorrelationId";
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
