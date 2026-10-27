# `tests/TutoringCentre.Api.Tests/Fixtures/ConventionsCollection.cs`

## Purpose

Source summary (intent, not independent execution proof): Convention tests share one factory/container; they run sequentially.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionsCollection.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 561 bytes. Source hash: `5451f52d2304f5815b80cca86bee3fb3361d92b315755c3adfe4c1f2fa556efa`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionsCollection.cs#L11): `public sealed class ConventionsCollection : ICollectionFixture<ConventionsFactory>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs:23](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L23)
- [tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs:6](../../../../../tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs#L6)
- [tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs:6](../../../../../tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs#L6)
- [tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs:8](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L8)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>Convention tests share one factory/container; they run sequentially.</summary>
[SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "xUnit collection-definition convention; 'Collection' here names an xUnit concept, not .NET's ICollection.")]
[CollectionDefinition(Name)]
public sealed class ConventionsCollection : ICollectionFixture<ConventionsFactory>
{
    public const string Name = "conventions";
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
