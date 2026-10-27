# `src/TutoringCentre.Api/Cli/DemoModeProvider.cs`

## Purpose

Source summary (intent, not independent execution proof): Adapts the bound `DemoOptions` to the Application layer's `IDemoModeProvider` port.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Cli/DemoModeProvider.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 390 bytes. Source hash: `8ad0e68e1805ca191ca282341455aad538f7b677ad8154ed2cee618b8f85b0ca`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.Options;`
- Line 2: `using TutoringCentre.Application.Platform;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Api/Cli/DemoModeProvider.cs#L7): `public sealed class DemoModeProvider(IOptions<DemoOptions> options) : IDemoModeProvider`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Program.cs:63](../../../../../src/TutoringCentre.Api/Program.cs#L63)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.Options;
using TutoringCentre.Application.Platform;

namespace TutoringCentre.Api.Cli;

/// <summary>Adapts the bound <see cref="DemoOptions"/> to the Application layer's <see cref="IDemoModeProvider"/> port.</summary>
public sealed class DemoModeProvider(IOptions<DemoOptions> options) : IDemoModeProvider
{
    public bool IsEnabled => options.Value.Enabled;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
