# `tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs`

## Purpose

Source summary (intent, not independent execution proof): An interface named I*Repository (for example ICentreRepository).

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 23 lines and 911 bytes. Source hash: `3b611178ff8736ef38fb7d0e84f815ce366d013a7b25748dc1d336f02a5865b3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using NetArchTest.Rules;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs#L5): `internal static class ArchitectureSupport`
- [Line 7](../../../../tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs#L7): `/// <summary>An interface named I*Repository (for example ICentreRepository).</summary>`
- [Line 8](../../../../tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs#L8): `public static bool IsRepositoryInterface(Type type) =>`
- [Line 14](../../../../tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs#L14): `/// <summary>A class named *Repository (for example CentreRepository).</summary>`
- [Line 15](../../../../tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs#L15): `public static bool IsRepositoryClass(Type type) =>`
- [Line 18](../../../../tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs#L18): `public static string Describe(TestResult result)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/components/shared/FilterBar.tsx:3](../../../../frontend/src/components/shared/FilterBar.tsx#L3)
- [tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs:15](../../../../tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs#L15)
- [tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs:50](../../../../tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs#L50)
- [tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs:57](../../../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs#L57)
- [tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs:27](../../../../tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs#L27)
- [tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs:99](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L99)
- [tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs:10](../../../../tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs#L10)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRowLevelSecurityCatalogueTests.cs:40](../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRowLevelSecurityCatalogueTests.cs#L40)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using NetArchTest.Rules;

namespace TutoringCentre.Architecture.Tests;

internal static class ArchitectureSupport
{
    /// <summary>An interface named I*Repository (for example ICentreRepository).</summary>
    public static bool IsRepositoryInterface(Type type) =>
        type.IsInterface
        && type.Name.Length > "IRepository".Length
        && type.Name.StartsWith('I')
        && type.Name.EndsWith("Repository", StringComparison.Ordinal);

    /// <summary>A class named *Repository (for example CentreRepository).</summary>
    public static bool IsRepositoryClass(Type type) =>
        type.IsClass && type.Name.EndsWith("Repository", StringComparison.Ordinal);

    public static string Describe(TestResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccessful ? "ok" : "Violating types: " + string.Join(", ", result.FailingTypeNames ?? []);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
