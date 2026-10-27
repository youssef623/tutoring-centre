# `src/TutoringCentre.Api/Auth/AntiforgeryTokenResponse.cs`

## Purpose

Source summary (intent, not independent execution proof): The request token only — the cookie half of the pair is set directly on the response, never in this body.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/AntiforgeryTokenResponse.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 4 lines and 228 bytes. Source hash: `68382a087d2aba15f0f753a1a9273290b227dcd6fe1a8ae1af7df0b360ab0c4c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../src/TutoringCentre.Api/Auth/AntiforgeryTokenResponse.cs#L4): `public sealed record AntiforgeryTokenResponse(string Token);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Api.Auth;

/// <summary>The request token only — the cookie half of the pair is set directly on the response, never in this body.</summary>
public sealed record AntiforgeryTokenResponse(string Token);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
