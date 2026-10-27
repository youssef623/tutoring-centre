# `src/TutoringCentre.Api/Cli/DemoOptions.cs`

## Purpose

Source summary (intent, not independent execution proof): Whether this deployment is a demo instance (Task 34.7). Defaults to false: a Production container is safe by default, and only an operator who deliberately wants demo data on a throwaway instance opts in. Changes nothing except the seed command's gate — the demo banner and reporting it in system info are Day 35.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Cli/DemoOptions.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 466 bytes. Source hash: `142a990b5b6bf6ddc9ef2f4d6fc1fb4a0ba386694e62a2dd77d858edb9579133`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../src/TutoringCentre.Api/Cli/DemoOptions.cs#L8): `public sealed class DemoOptions`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/DemoModeProvider.cs:7](../../../../../src/TutoringCentre.Api/Cli/DemoModeProvider.cs#L7)
- [src/TutoringCentre.Api/Cli/SeedGate.cs:14](../../../../../src/TutoringCentre.Api/Cli/SeedGate.cs#L14)
- [src/TutoringCentre.Api/Program.cs:61](../../../../../src/TutoringCentre.Api/Program.cs#L61)
- [tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs:19](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L19)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Api.Cli;

/// <summary>
/// Whether this deployment is a demo instance (Task 34.7). Defaults to false: a Production container is safe by
/// default, and only an operator who deliberately wants demo data on a throwaway instance opts in. Changes
/// nothing except the seed command's gate — the demo banner and reporting it in system info are Day 35.
/// </summary>
public sealed class DemoOptions
{
    public bool Enabled { get; set; }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
