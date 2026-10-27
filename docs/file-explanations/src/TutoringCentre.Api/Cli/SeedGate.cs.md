# `src/TutoringCentre.Api/Cli/SeedGate.cs`

## Purpose

Source summary (intent, not independent execution proof): Whether the seed command may run here (Task 34.7): Development and Testing always allow it (so local setup and the test suite are unaffected); Production allows it only for a demo instance that opted in explicitly. Checked by Program's CLI dispatch before `SeedCommand.RunAsync`, not inside it — the command itself keeps doing exactly what it always did, unconditionally, for every caller (including this project's own `SeedCommandTests`, which exercise it directly and never want it gated).

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Cli/SeedGate.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 32 lines and 1,461 bytes. Source hash: `23676f5ce000374aebb890605946ac87e0e271d5cc26d71d97efeddf89fb5939`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.Hosting;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../src/TutoringCentre.Api/Cli/SeedGate.cs#L12): `public static class SeedGate`
- [Line 14](../../../../../src/TutoringCentre.Api/Cli/SeedGate.cs#L14): `public static bool IsAllowed(IHostEnvironment environment, DemoOptions demoOptions)`
- [Line 26](../../../../../src/TutoringCentre.Api/Cli/SeedGate.cs#L26): `public static bool IncludesSetupCentre(IHostEnvironment environment)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:156](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L156)
- [src/TutoringCentre.Api/Program.cs:134](../../../../../src/TutoringCentre.Api/Program.cs#L134)
- [tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs:19](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L19)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.Hosting;

namespace TutoringCentre.Api.Cli;

/// <summary>
/// Whether the seed command may run here (Task 34.7): Development and Testing always allow it (so local setup
/// and the test suite are unaffected); Production allows it only for a demo instance that opted in explicitly.
/// Checked by Program's CLI dispatch before <see cref="SeedCommand.RunAsync"/>, not inside it — the command
/// itself keeps doing exactly what it always did, unconditionally, for every caller (including this project's
/// own `SeedCommandTests`, which exercise it directly and never want it gated).
/// </summary>
public static class SeedGate
{
    public static bool IsAllowed(IHostEnvironment environment, DemoOptions demoOptions)
    {
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(demoOptions);

        return environment.IsDevelopment() || environment.IsEnvironment("Testing") || demoOptions.Enabled;
    }

    /// <summary>
    /// Whether the seed also creates the empty setup centre and its owner (Task 54.2), which only the full-setup
    /// journey uses: Development and Testing only, never a demo instance, whatever Demo:Enabled says.
    /// </summary>
    public static bool IncludesSetupCentre(IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        return environment.IsDevelopment() || environment.IsEnvironment("Testing");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
