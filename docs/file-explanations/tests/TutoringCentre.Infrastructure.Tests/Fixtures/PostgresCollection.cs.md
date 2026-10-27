# `tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresCollection.cs`

## Purpose

Source summary (intent, not independent execution proof): All real-database tests share one container (started once per run); tests in a collection run sequentially.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresCollection.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 602 bytes. Source hash: `4ca11a0148897a8fa8d6fb3d4fa6513f06d7881b4761a5b2173887412894f066`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresCollection.cs#L11): `public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs:4](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs#L4)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Infrastructure.Tests.Fixtures;

/// <summary>All real-database tests share one container (started once per run); tests in a collection run sequentially.</summary>
[SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "xUnit collection-definition convention; 'Collection' here names an xUnit concept, not .NET's ICollection.")]
[CollectionDefinition(Name)]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "postgres";
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
