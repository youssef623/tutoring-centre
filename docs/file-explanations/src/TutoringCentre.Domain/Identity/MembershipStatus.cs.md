# `src/TutoringCentre.Domain/Identity/MembershipStatus.cs`

## Purpose

Source summary (intent, not independent execution proof): Whether a membership currently grants access to its centre.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Identity/MembershipStatus.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 8 lines and 185 bytes. Source hash: `0207f9dc3e97d290c51876f0c7f8972f2d82a6d2dd5ed7166ee513d35039daa3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../src/TutoringCentre.Domain/Identity/MembershipStatus.cs#L4): `public enum MembershipStatus`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Domain.Identity;

/// <summary>Whether a membership currently grants access to its centre.</summary>
public enum MembershipStatus
{
    Active,
    Inactive,
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
