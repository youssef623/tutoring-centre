# `src/TutoringCentre.Domain/Common/ErrorKind.cs`

## Purpose

Source summary (intent, not independent execution proof): Closed set of expected business-failure kinds. The Api maps each kind to exactly one HTTP status (Day 11), so adding a kind is an API-contract change.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Common/ErrorKind.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 15 lines and 345 bytes. Source hash: `a29b933e4bbc6793999a9ae9cca86cf437699d0324aae84b8de1c5df8bc283f9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Domain/Common/ErrorKind.cs#L7): `public enum ErrorKind`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Domain.Common;

/// <summary>
/// Closed set of expected business-failure kinds. The Api maps each kind to exactly one HTTP status (Day 11),
/// so adding a kind is an API-contract change.
/// </summary>
public enum ErrorKind
{
    Validation,
    NotFound,
    Conflict,
    Rule,
    Forbidden,
    Unauthenticated,
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
