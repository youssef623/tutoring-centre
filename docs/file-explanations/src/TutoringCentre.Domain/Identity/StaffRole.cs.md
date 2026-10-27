# `src/TutoringCentre.Domain/Identity/StaffRole.cs`

## Purpose

Source summary (intent, not independent execution proof): A staff member's role within one centre (not global). Secretary is the product's "Assistant" role.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Identity/StaffRole.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 9 lines and 230 bytes. Source hash: `0b218ab802f92b2ca328d8b5e8777ba484ce08176d28ef1cdf3ff60df886d2f9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../src/TutoringCentre.Domain/Identity/StaffRole.cs#L4): `public enum StaffRole`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Domain.Identity;

/// <summary>A staff member's role within one centre (not global). Secretary is the product's "Assistant" role.</summary>
public enum StaffRole
{
    Owner,
    Teacher,
    Secretary,
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
