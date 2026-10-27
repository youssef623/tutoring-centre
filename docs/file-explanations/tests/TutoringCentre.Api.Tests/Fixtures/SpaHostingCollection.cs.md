# `tests/TutoringCentre.Api.Tests/Fixtures/SpaHostingCollection.cs`

## Purpose

Source summary (intent, not independent execution proof): SPA hosting tests share one factory/container; its web root is fixed for the whole class.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SpaHostingCollection.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 580 bytes. Source hash: `14c9134df1edaa9d5f7c952bff0addec018c05dbac6e0954fb99108b1095ca48`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SpaHostingCollection.cs#L11): `public sealed class SpaHostingCollection : ICollectionFixture<SpaHostingFactory>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Http/SecurityHeadersTests.cs:5](../../../../../tests/TutoringCentre.Api.Tests/Http/SecurityHeadersTests.cs#L5)
- [tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs:6](../../../../../tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs#L6)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>SPA hosting tests share one factory/container; its web root is fixed for the whole class.</summary>
[SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "xUnit collection-definition convention; 'Collection' here names an xUnit concept, not .NET's ICollection.")]
[CollectionDefinition(Name)]
public sealed class SpaHostingCollection : ICollectionFixture<SpaHostingFactory>
{
    public const string Name = "spa-hosting";
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
