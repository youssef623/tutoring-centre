# `src/TutoringCentre.Application/Platform/SystemInfoDto.cs`

## Purpose

Source summary (intent, not independent execution proof): Non-sensitive status served anonymously. Deliberately excludes environment names, connection details and counts.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Application/Platform/SystemInfoDto.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 4 lines and 310 bytes. Source hash: `7fa6fe60a563db20f7268333dae780f92b8f18c6e55e1b548510fbe361853263`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../src/TutoringCentre.Application/Platform/SystemInfoDto.cs#L4): `public sealed record SystemInfoDto(string ApplicationVersion, string? LatestMigration, bool DatabaseUpToDate, bool DemoMode);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Platform;

/// <summary>Non-sensitive status served anonymously. Deliberately excludes environment names, connection details and counts.</summary>
public sealed record SystemInfoDto(string ApplicationVersion, string? LatestMigration, bool DatabaseUpToDate, bool DemoMode);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
