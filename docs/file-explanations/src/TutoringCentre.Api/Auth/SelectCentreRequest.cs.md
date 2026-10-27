# `src/TutoringCentre.Api/Auth/SelectCentreRequest.cs`

## Purpose

Source summary (intent, not independent execution proof): The centre ID here is only a request — nothing is written into the session before GetActiveMembershipQuery confirms the current user actually belongs to it.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/SelectCentreRequest.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 7 lines and 289 bytes. Source hash: `12520f5d106708dc0411cc668a72e20aa59e3c3ec9c3346d8609e659b45b2700`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Api/Auth/SelectCentreRequest.cs#L7): `public sealed record SelectCentreRequest(Guid CentreId);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Api.Auth;

/// <summary>
/// The centre ID here is only a request — nothing is written into the session before
/// GetActiveMembershipQuery confirms the current user actually belongs to it.
/// </summary>
public sealed record SelectCentreRequest(Guid CentreId);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
