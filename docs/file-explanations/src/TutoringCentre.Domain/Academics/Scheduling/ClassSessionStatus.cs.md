# `src/TutoringCentre.Domain/Academics/Scheduling/ClassSessionStatus.cs`

## Purpose

Declares `ClassSessionStatus`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ClassSessionStatus.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 7 lines and 120 bytes. Source hash: `9234aacc98fae28b6f3ddce3ba5bb76bf33689088b88ec511363c10195578e43`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ClassSessionStatus.cs#L3): `public enum ClassSessionStatus`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Domain.Academics.Scheduling;

public enum ClassSessionStatus
{
    Scheduled,
    Cancelled,
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
