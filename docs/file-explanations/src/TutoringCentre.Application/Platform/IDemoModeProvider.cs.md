# `src/TutoringCentre.Application/Platform/IDemoModeProvider.cs`

## Purpose

Source summary (intent, not independent execution proof): Whether this deployment is a demo instance (Task 35.7). A port: the Application layer reads this flag without knowing it ultimately comes from configuration (Demo:Enabled) — the same options the Api's seed gate (Task 34.7) already reads, implemented in the Api project because that is where options binding lives, never by the Application layer reading configuration types itself.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Application/Platform/IDemoModeProvider.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 12 lines and 550 bytes. Source hash: `7bf40949e85de485b80c521961d9f6c90b2d831e7ec255fc2271da82077b70f8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../src/TutoringCentre.Application/Platform/IDemoModeProvider.cs#L9): `public interface IDemoModeProvider`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/DemoModeProvider.cs:7](../../../../../src/TutoringCentre.Api/Cli/DemoModeProvider.cs#L7)
- [src/TutoringCentre.Api/Program.cs:63](../../../../../src/TutoringCentre.Api/Program.cs#L63)
- [src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs:13](../../../../../src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs#L13)
- [tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs:70](../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs#L70)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:212](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L212)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Platform;

/// <summary>
/// Whether this deployment is a demo instance (Task 35.7). A port: the Application layer reads this flag
/// without knowing it ultimately comes from configuration (<c>Demo:Enabled</c>) — the same options the Api's
/// seed gate (Task 34.7) already reads, implemented in the Api project because that is where options binding
/// lives, never by the Application layer reading configuration types itself.
/// </summary>
public interface IDemoModeProvider
{
    bool IsEnabled { get; }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
